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
    private GameObject player = null;
	[Space(10)]

	[Header("Texture Maps")]
	[SerializeField]
	private Material blockTextures = null;
	[SerializeField]
	private Material fluidTextures = null;
	[Space(10)]

	[Header("Hud Objects")]
	public Slider loadingProgress;
	public Text loadingText;
	public Camera menuCamera;
	public Canvas menuCanvas;
	public Canvas hudCanvas;	
	
	//private variables
	private bool firstLoad = true;
	private bool building = false;
	private int buildStep;
	private int buildSteps;
	private ConcurrentDictionary<string, Chunk> loadedChunks;
	private GamePoint lastbuildPos;
	private CoroutineQueue processQueue;
	private LockFreeQueue<String> unloadChunks = new LockFreeQueue<string>();

	private static int counter=0;


	/**
	 * This method is called to initialize the world
	 * object.
	 */
	void Start () {	
		hudCanvas.gameObject.SetActive(false);		
		player.SetActive(false);
		loadingProgress.gameObject.SetActive(false);
		loadingText.gameObject.SetActive(false);
		BlockInteration.setWorld(this);
		processQueue = new CoroutineQueue(Global.MaxCoroutines, StartCoroutine);
		firstLoad = true;
		loadedChunks = new ConcurrentDictionary<string, Chunk>();

		//generate world seed
		seedVal = Utils.getSeedValue(seed);
		this.clickPlay();		
	}
	
	/**
	 * This method is called once per frame.
	 */
	void Update () {
		counter++;if(counter<100){return;}counter = 0;
		if(building)return;
		if(firstLoad){
			if(processQueue==null)return;
			processQueue.Run(DrawChunks());
			return;
		}
		GamePoint playerPosition = GamePoint.getParentIndex(player.transform.position,Global.ChunkSize);
		GamePoint movement = lastbuildPos - playerPosition;
		if(Global.BuildOnChunkChange){
			if(movement.magnitude > Global.BuildWhenMovedXChunks){
				lastbuildPos = playerPosition;
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
	public Block getBlock(GamePoint pos){
		//get the chunk
		string chunkName = Chunk.NameFromPosition(pos);
		Chunk chunk = null; if(loadedChunks.TryGetValue(chunkName, out chunk))
			return chunk.getBlock(Block.IndexInChunk(pos));
		else return null;
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

	
	private bool BuildChunk(GamePoint ChunkIndex){
		if(ChunkIndex.y<0||ChunkIndex.y>Global.ChunksTall)return false;
		//get chunk name
		string chunkName = Chunk.NameFromIndex(ChunkIndex);
		Chunk chunk = null;
		//if chunk does not exist create it
		if(!loadedChunks.TryGetValue(chunkName,out chunk)){
			chunk = new Chunk(ChunkIndex, this);
			//try to add chunk
			if(loadedChunks.TryAdd(chunkName,chunk)){
				chunk.Build(); return true;
			}else chunk.Destroy();
		}return false; //the chunk exists
	}

	IEnumerator BuildWorld(GamePoint index,Direction direction, int rad){
		if(rad<=0){yield break;}rad--;
		//first build in the passed direction
		GamePoint buildPos = index.moveDirection(direction,1);
		if(BuildChunk(buildPos)){updatefirstLoad("building "+buildPos);}
		processQueue.Run(BuildWorld(buildPos, direction, rad));
		yield return null;
		//build down
		GamePoint down = index.moveDirection(Direction.DOWN,1);
		if(BuildChunk(down)){updatefirstLoad("building "+down);}
		processQueue.Run(BuildWorld(down, direction, rad));
		yield return null;
		
		//then build to the left
		GamePoint left = index.moveDirection(Directions.rotateNWSE(direction),1);
		if(BuildChunk(left)){updatefirstLoad("building "+left);}
		processQueue.Run(BuildWorld(left, direction, rad));
		yield return null;

		//then build to the right
		GamePoint right = index.moveDirection(Directions.rotateNESW(direction),1);
		if(BuildChunk(right)){updatefirstLoad("building "+right);}
		processQueue.Run(BuildWorld(right, direction, rad));
		yield return null;

		//build up
		GamePoint up = index.moveDirection(Direction.UP,1);
		if(BuildChunk(up)){updatefirstLoad("building "+up);}
		processQueue.Run(BuildWorld(up, direction, rad));
		yield return null;

		//then build behind
		GamePoint invert = index.moveDirection(Directions.invertDirection(direction),1);
		if(BuildChunk(invert)){updatefirstLoad("building "+invert);}
		processQueue.Run(BuildWorld(invert, direction, rad));
		yield return null;
	}

	public bool isInRange(GamePoint position1, GamePoint position2, int radius){
    	radius++;radius*=radius;
    	int dxdx = (int)Mathf.Abs(position1.x-position2.x);dxdx*=dxdx;
    	int dydy = (int)Mathf.Abs(position1.y-position2.y);dydy*=dydy;
    	int dzdz = (int)Mathf.Abs(position1.z-position2.z);dzdz*=dzdz;
    	if(dxdx+dydy+dzdz>=radius)return false;
    	return true;
    }	

	/*IEnumerator BuildRecursiveWorld(GamePoint index, int rad){
		if(rad<=0){yield break;}rad--;
		if(index.y>=0&&index.y<=Global.ChunksTall+1){
			
			//build chunk forward
			GamePoint forward = new GamePoint(index.x,index.y,index.z+1);
			if(BuildChunk(forward)){updatefirstLoad("building "+forward);}
			processQueue.Run(BuildRecursiveWorld(forward, rad));
			yield return null;
			
			//build chunk backward
			GamePoint back = new GamePoint(index.x,index.y,index.z-1);
			if(BuildChunk(back)){updatefirstLoad("building "+back);}
			processQueue.Run(BuildRecursiveWorld(back, rad));
			yield return null;
			
			//build chunk left
			GamePoint left = new GamePoint(index.x-1,index.y,index.z);
			if(BuildChunk(left)){updatefirstLoad("building "+left);}
			processQueue.Run(BuildRecursiveWorld(left, rad));
			yield return null;
			
			//build chunk right
			GamePoint right = new GamePoint(index.x+1,index.y,index.z);
			if(BuildChunk(right)){updatefirstLoad("building "+right);}
			processQueue.Run(BuildRecursiveWorld(right, rad));
			yield return null;
		}
		//build chunk up
		if(index.y+1<Global.ChunksTall+1){
			GamePoint up = new GamePoint(index.x,index.y+1,index.z);
			if(BuildChunk(up)){updatefirstLoad("building "+up);}
			processQueue.Run(BuildRecursiveWorld(up, rad));
			yield return null;
		}
		//build chunk down
		if(index.y-1>=0){
			GamePoint down = new GamePoint(index.x,index.y-1,index.z);
			if(BuildChunk(down)){updatefirstLoad("building "+down);}
			processQueue.Run(BuildRecursiveWorld(down, rad));
			yield return null;
		}
	}*/

	IEnumerator DrawChunks(){
		foreach(KeyValuePair<string, Chunk> value in loadedChunks){
			Chunk chunk = value.Value;
			if(Vector3.Distance(player.transform.position,
				chunk.Position)>Global.LoadRadius*Global.ChunkSize){
				unloadChunks.Enqueue(value.Key);
			}
			else if(chunk.Status == ChunkStatus.DRAW){
				//try{
					chunk.Draw();
				//}catch(MissingReferenceException mre){}
				updatefirstLoad("drawing "+(chunk.Index));
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
		GamePoint playerPosition = GamePoint.roundVector3(player.transform.position);
		playerPosition.y = WorldGenerator.GenerateHeight(playerPosition.x,playerPosition.y)+1;
		DebugScript.startTimer();
		//set the player above the ground
		player.transform.position = playerPosition;
		lastbuildPos = Chunk.IndexAtPosition(playerPosition);

		//set up the world
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
		//caculate build steps
		this.buildSteps = calculateSteps(lastbuildPos.y,Global.LoadRadius);
		this.loadingProgress.maxValue = buildSteps;
		//build and draw the starting chunk
		building = true;
		BuildChunk(lastbuildPos);
		//processQueue.Run(DrawChunks());
		//Vector3 chunkIndex = new Vector3((int)(ppos.x/Global.ChunkSize),(int)(ppos.y/Global.ChunkSize),(int)(ppos.z/Global.ChunkSize));
		processQueue.Run(BuildWorld(lastbuildPos,getLookingDirection(),Global.LoadRadius));
	}

	private Direction getLookingDirection(){
		int looking = (int)player.transform.rotation.eulerAngles.y;
		looking = (looking+45); if(looking>360)looking-=360;looking/=90;
		switch(looking){
			case 0: return Direction.NORTH;
			case 1: return Direction.EAST;
			case 2: return Direction.SOUTH;
			case 3: return Direction.WEST;
			default: return Direction.NORTH;
		}
	}

	public void BuildNearPlayer(){
		StopCoroutine("BuildRecursiveWorld");
		processQueue.Run(BuildWorld(
			lastbuildPos,
			getLookingDirection(),
			Global.LoadRadius));
	}

	public void updatefirstLoad(string status){
		DebugScript.setLoadedChunks(loadedChunks.Count);
		if(!firstLoad)return;
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
			firstLoad=false;
			//activate player
			DebugScript.stopTimer("start Time");
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

	 public int Seed{get{return seedVal;}}
	 public Material FluidMaterial{get{return this.fluidTextures;}}
	 public Material BlockMaterial{get{return this.blockTextures;}}
	 public bool FirstLoad{get{return firstLoad;}}
}