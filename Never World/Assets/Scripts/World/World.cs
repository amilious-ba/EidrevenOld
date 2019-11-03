using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/**
 * This is the main class used to interact with the world.
 * @type {[type]}
 */
public class World : MonoBehaviour {


    [Header("World Seed")]
    [SerializeField]
    private string seed = "0";
    private string lastSeed = "0";
    [SerializeField]
    private int seedVal = 0;
    private int lastSeedVal = 0;
    [SerializeField]
    private GameObject player;
	[Space(10)]

	[Header("Texture Maps")]
	public Material blockTextures;
	public Material fluidTextures;
	[Space(10)]

	[Header("Hud Objects")]
	public Slider loadingProgress;
	public Text loadingText;
	public Camera menuCamera;
	public Canvas menuCanvas;
	public Canvas hudCanvas;	
	
	//private variables
	private bool firstbuild = true;
	private bool building = false;
	private int buildStep;
	private int buildSteps;
	private static ConcurrentDictionary<string, Chunk> loadedChunks;
	private Vector3 lastbuildPos;
	public static CoroutineQueue processQueue;
	public static LockFreeQueue<String> unloadChunks = new LockFreeQueue<string>();


	/**
	 * Project notes
	 *
	 * only use position when talking about the world coords
	 * other wise use index. index is the location of the object
	 * in another object. position or location is the location in the world
	 */



	/**
	 * This method is called to initialize the world
	 * object.
	 */
	void Start () {	
		hudCanvas.gameObject.SetActive(false);		
		player.SetActive(false);
		loadingProgress.gameObject.SetActive(false);
		loadingText.gameObject.SetActive(false);
				
		processQueue = new CoroutineQueue(Global.MaxCoroutines, StartCoroutine);
		firstbuild = true;
		loadedChunks = new ConcurrentDictionary<string, Chunk>();

		//generate world seed
		seedVal = Utils.getSeedValue(seed);
		this.clickPlay();		
	}
	
	/**
	 * This method is called once per frame.
	 */
	void Update () {
		if(building)return;
		if(firstbuild){
			if(processQueue==null)return;
			processQueue.Run(DrawChunks());
			return;
		}
		Vector3 movement = lastbuildPos - player.transform.position;
		if(Global.BuildOnChunkChange){
			if(movement.magnitude > Global.ChunkSize * Global.BuildWhenMovedXChunks){
				lastbuildPos = player.transform.position;
				BuildNearPlayer();
			}
		}else{
			lastbuildPos = player.transform.position;
			BuildNearPlayer();
		}
		//Draw Chunks
		processQueue.Run(DrawChunks());
		//remove unneeded chunks
		processQueue.Run(RemoveOldChunks());
	}

	/**
	 * This method is called when the game object
	 * is edited in the editor
	 */
	public void OnValidate(){
	 	if(!seed.Equals(lastSeed)||seedVal!=lastSeedVal){
	 		lastSeed = seed;
	 		seedVal = lastSeedVal = Utils.getSeedValue(seed);
	 	}
	 }

	/**
	 * This method returns the block at the given world position.
	 * @param Vector3 pos The position in the world.
	 * @returns Vector3 containing the block at the given
	 * position, otherwise returns null.
	 */
	public static Block getBlock(Vector3 pos){
		Vector3 bIndex;Chunk chunk = null;
		Vector3 cIndex = Chunk.ChunkIndexAtPosition(pos, out bIndex);
		//Debug.Log(cIndex+" "+bIndex);
		if(loadedChunks.TryGetValue(Chunk.ChunkNameFromIndex(cIndex),out chunk))
			return chunk.chunkBlocks[(int)bIndex.x,(int)bIndex.y,(int)bIndex.z];
		else return null;
	}

	/**
	 * This method takes a world position and gives you the containing
	 * chunks position.
	 * @param Vector3 worldPos The position in the world.
	 * @returns Vector3 The position of the containing chunk.
	 */
	public static Vector3 WorldToChunkWorldPosistion(Vector3 worldPos){
		int cx, cy, cz;		
		if(worldPos.x < 0)cx = (int) ((Mathf.Round(worldPos.x-Global.ChunkSize)+1)/(float)Global.ChunkSize) * Global.ChunkSize;
		else cx = (int) (Mathf.Round(worldPos.x)/(float)Global.ChunkSize) * Global.ChunkSize;		
		if(worldPos.y < 0) cy = (int) ((Mathf.Round(worldPos.y-Global.ChunkSize)+1)/(float)Global.ChunkSize) * Global.ChunkSize;
		else cy = (int) (Mathf.Round(worldPos.y)/(float)Global.ChunkSize) * Global.ChunkSize;		
		if(worldPos.z < 0) cz = (int) ((Mathf.Round(worldPos.z-Global.ChunkSize)+1)/(float)Global.ChunkSize) * Global.ChunkSize;
		else cz = (int) (Mathf.Round(worldPos.z)/(float)Global.ChunkSize) * Global.ChunkSize;
		return new Vector3(cx,cy,cz);
	}

	/**
	 * This is a wrapper function used to get loaded chunks.
	 * @param  string name The name of the chunk you want to try
	 * get.
	 * @param  out    Chunk	chunk This variable will be set to the
	 * desired chunk if it is found, otherwise it will be set to
	 * null.		
	 * @return boolean returns true if the chunk was found in the
	 * loaded chunks, otherwise returns false.
	 */
	public bool getLoadedChunk(string name, out Chunk chunk){
		if(loadedChunks.TryGetValue(name, out chunk))return true;
		else{chunk = null;return false;}
	}


	/**
	 * This method takes a world position and gives you the blocks
	 * position within the chunk.
	 * @param Vector3 worldPos The position in the world
	 * @returns Vector3 the position within its chunk.
	 */
	public static Vector3 WorldToChunkPosition(Vector3 worldPos){
		Vector3 chunkPos = WorldToChunkWorldPosistion(worldPos);
		//get location in chunk
		int x = (int) Mathf.Abs((float)Math.Round(worldPos.x) - chunkPos.x);
		int y = (int) Mathf.Abs((float)Math.Round(worldPos.y) - chunkPos.y);
		int z = (int) Mathf.Abs((float)Math.Round(worldPos.z) - chunkPos.z);
		return new Vector3(x,y,z);
	}

	private bool BuildChunk(Vector3 chunkIndex){
		Chunk chunk = null;
		if(!loadedChunks.TryGetValue(Chunk.ChunkNameFromIndex(chunkIndex),out chunk)){
			chunk = new Chunk(chunkIndex,this);
			if(loadedChunks.TryAdd(Chunk.ChunkNameFromIndex(chunkIndex),chunk)){
				return true;
			}else{
				Debug.Log("Could not add chunk, because it already exists.");
				return false;
			}
		}return false;
	}

	IEnumerator BuildRecursiveWorld(Vector3 chunkIndex,int rad){
		if(rad<=0){yield break;}rad--;
		if(chunkIndex.y>=0||chunkIndex.y<=Global.ChunksTall){
			//build this chunk
			if(BuildChunk(chunkIndex))updateFirstBuild("building "+chunkIndex);
			yield return null;
		}
		//start recursion
		for(int i =1; i<7;i++){
			processQueue.Run(BuildRecursiveWorld(
				Directions.moveDirection(chunkIndex,(Direction)i,1f),rad));
			yield return null;
		}
	}

	IEnumerator DrawChunks(){
		foreach(KeyValuePair<string, Chunk> chunk in loadedChunks){

			if(chunk.Value.getSolids() && Vector3.Distance(player.transform.position,
				chunk.Value.getSolids().transform.position)>Global.LoadRadius*Global.ChunkSize){
				unloadChunks.Enqueue(chunk.Key);
			}
			else if(chunk.Value.status == ChunkStatus.DRAW){
				chunk.Value.DrawChunk(); if(!firstbuild)continue;
				updateFirstBuild("drawing x:"+(chunk.Value.position.x/Global.ChunkSize)+" y:"+(chunk.Value.position.y/Global.ChunkSize)+" z:"+(chunk.Value.position.z/Global.ChunkSize));
			}

			yield return null;
		}
	}

	IEnumerator RemoveOldChunks(){
		string chunkName = "";
		while(unloadChunks.Dequeue(out chunkName)){
			processQueue.Run(RemoveChunk(chunkName));
		}yield return null;
	}

	IEnumerator RemoveChunk(string name){
		Chunk chunk;
		if(loadedChunks.TryGetValue(name,out chunk)){
			chunk.Destroy();
			loadedChunks.TryRemove(name, out chunk);
			yield return null;
		}
	}

	public void clickPlay(){
		//playButton.gameObject.SetActive(false);
		loadingProgress.gameObject.SetActive(true);
		loadingText.gameObject.SetActive(true);	
		Vector3 ppos = player.transform.position;
		/*
		Vector3 ppos = player.transform.position;
		Vector3 cpos = WorldToChunkWorldPosistion(player.transform.position);
		//set the player above the ground
		Biome biome = Biome.getBiome(Biome.getBiomeType(seedVal, (int)cpos.x, (int)cpos.z));
		player.transform.position = new Vector3(ppos.x,biome.getHeight(seedVal,(int)ppos.x,(int)ppos.z)+1,ppos.z);*/
		//set the player above the ground
		player.transform.position = new Vector3(ppos.x,WorldGenerator.GenerateHeight(ppos.x,ppos.z)+1,ppos.z);
		//player.transform.position = new Vector3(ppos.x,150,ppos.z);
		ppos = player.transform.position;
		lastbuildPos = ppos;		
		//set up the world
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
		//caculate build steps
		//this.buildSteps = calculateSteps((int)(ppos.y/Global.ChunkSize),Global.LoadRadius);
		this.buildSteps = 129;
		this.loadingProgress.maxValue = buildSteps;
		//build and draw the starting chunk
		building = true;
		//BuildChunkAt((int)(ppos.x/Global.ChunkSize),(int)(ppos.y/Global.ChunkSize),(int)(ppos.z/Global.ChunkSize));
		//processQueue.Run(DrawChunks());
		//processQueue.Run(BuildRecursiveWorld((int)(ppos.x/Global.ChunkSize),(int)(ppos.y/Global.ChunkSize),(int)(ppos.z/Global.ChunkSize),Global.LoadRadius));
		processQueue.Run(BuildRecursiveWorld(Chunk.ChunkIndexAtPosition(ppos), Global.LoadRadius));
	}

	public void BuildNearPlayer(){
		Vector3 ppos = player.transform.position;
		StopCoroutine("BuildRecursiveWorld");
		processQueue.Run(BuildRecursiveWorld(Chunk.ChunkIndexAtPosition(ppos), Global.LoadRadius));
	}

	public void updateFirstBuild(string status){
		if(!firstbuild)return;
		buildStep++;
		//this.loadingText.text = status;
		this.loadingText.text = buildStep+"/"+buildSteps+" "+status;
		this.loadingProgress.value = buildStep;
		if(building&&buildStep==buildSteps){
			building = false;
			buildStep = 0;
			buildSteps-=(buildSteps/15);
			loadingProgress.maxValue = buildSteps;
		}else if(buildStep==buildSteps){
			firstbuild=false;
			//activate player
			player.SetActive(true);
			menuCanvas.gameObject.SetActive(false);
			menuCamera.gameObject.SetActive(false);
			hudCanvas.gameObject.SetActive(true);
		}
	}

	public int calculateSteps(int y, int radius){
		int chunkCount = calculateLayerSteps(radius);
		//calculate up
		int up = y+radius; int upRad = radius-1;
		if(up>Global.ChunksTall)up=Global.ChunksTall;
		for(int i=y+1;i<up;i++)
			chunkCount += calculateLayerSteps(upRad--);
		//calculate down
		int down = y-radius;  int downRad = radius-1;
		if(down<0)down=0;
		for(int i=y-1;i>=down;i--)
			chunkCount+= calculateLayerSteps(downRad--);
		return chunkCount;
	}


	public int calculateLayerSteps(int radius){
		int calc = 0;
		for(int i=(radius*2)-1;i>0;i-=2)calc+=i;
		calc*=2;calc+=((radius*2)+1);
		return calc;
	}

	



	 public int getSeed(){return seedVal;}
	 public Material getFluidMaterial(){return this.fluidTextures;}
	 public Material getBlockMaterial(){return this.blockTextures;}

}
