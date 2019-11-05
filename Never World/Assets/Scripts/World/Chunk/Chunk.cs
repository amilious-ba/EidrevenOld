using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Rendering;

public enum ChunkStatus{DRAW, DRAWING, BUILDING, NOTBUILT, GOOD, DEAD}

public class Chunk {

	private World world;
	private GameObject solidGameObject;
	private GameObject fluidGameObject;
	private bool drawn = false;
	private bool built = false;

	public Vector3 position;
	public ChunkMB mb;
	
	public Block[,,] chunkBlocks;
	public ChunkStatus status = ChunkStatus.NOTBUILT;
	SaveData saveData;
	public bool changed = false;

	public ChunkStatus Status{get{return status;}set{status=value;}}
	public GameObject Solids{get{return this.solidGameObject;}}
	public GameObject Fluids{get{return this.fluidGameObject;}}


	// Use this for initialization
	public Chunk (Vector3 position, World world) {
		this.world = world;
		this.position = position;	
	}	

	public void Build(){
		if(built)return;
		built = true;
		if(status==ChunkStatus.DEAD)return;
		//set up		
		solidGameObject = new GameObject(BuildName(position));
		solidGameObject.transform.position = position;
		solidGameObject.transform.parent = world.transform;
		fluidGameObject = new GameObject(BuildName(position)+"_F");
		fluidGameObject.transform.position = position;
		fluidGameObject.transform.parent = world.transform;
		mb = solidGameObject.AddComponent<ChunkMB>();
		mb.SetOwner(this);	
		//build
		status = ChunkStatus.BUILDING;
		if(Load()) chunkBlocks = saveData.loadBlocks(this);
		else chunkBlocks = WorldGenerator.generateChunkBlocksNew(this,world.getSeed());
		status = ChunkStatus.DRAW;
		if(!world.FirstLoad){for(int i=1;i<7;i++){
			Chunk neighbor = getNeighbor((Direction)i);
			if(neighbor!=null)neighbor.Status = ChunkStatus.DRAW;}
		}
	}

	public bool Draw()	{
		if(status==ChunkStatus.NOTBUILT||status==ChunkStatus.DRAWING||status==ChunkStatus.DEAD)return false;
		status = ChunkStatus.DRAWING;
		try{
			if(drawn){
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshFilter>());}catch(MissingReferenceException mre){}
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshRenderer>());}catch(MissingReferenceException mre){}
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<Collider>());}catch(MissingReferenceException mre){}
				try{GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshFilter>());}catch(MissingReferenceException mre){}
				try{GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshRenderer>());}catch(MissingReferenceException mre){}
			}
			for(int z = 0; z < Global.ChunkSize; z++)for(int y = 0; y < Global.ChunkSize; y++)
			for(int x = 0; x < Global.ChunkSize; x++)chunkBlocks[x,y,z].Draw();
			CombineQuads(solidGameObject.gameObject, ShadowCastingMode.On, world.getBlockMaterial());
			MeshCollider collider = solidGameObject.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			collider.sharedMesh = solidGameObject.transform.GetComponent<MeshFilter>().mesh;

			CombineQuads(fluidGameObject.gameObject, ShadowCastingMode.Off, world.getFluidMaterial());
			status = ChunkStatus.GOOD;
			drawn = true;		
 		}catch(MissingReferenceException mre){return false;}
 		return true;
	}

	public void Destroy(){
		if(status ==ChunkStatus.DEAD)return;
		status = ChunkStatus.DEAD;
		GameObject.Destroy(solidGameObject);
		GameObject.Destroy(fluidGameObject);
		Save();
	}

	public string getFileName(Vector3 v){
		return Application.persistentDataPath + "/savedata/Chunk_" + 
				(int)v.x+"_"+(int)v.y+"_"+(int)v.z+"_"+Global.ChunkSize +
				"_" + Global.ChunksTall +".dat";
	}

	public bool Load() {
		if(!Global.AllowLoad)return false;
		string chunkFile = getFileName(position);
		if(File.Exists(chunkFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(chunkFile, FileMode.Open);
			saveData = new SaveData();
			saveData = (SaveData) bf.Deserialize(file);
			file.Close();
			//Debug.Log("Loading chunk from file: " + chunkFile);
			return true;
		} return false;
	}

	public void Save(){
		if(!Global.AllowSave)return;
		string chunkFile = getFileName(position);
		
		if(!File.Exists(chunkFile))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
		saveData = new SaveData(chunkBlocks);
		bf.Serialize(file, saveData);
		file.Close();
		//Debug.Log("Saving chunk from file: " + chunkFile);
	}	

	public Chunk getNeighbor(Direction direction){
		string neighborName = getNeighborName(direction);Chunk c = null;
		world.getLoadedChunk(neighborName, out c);return c;
	}

	public string getNeighborName(Direction direction){
		return BuildName(Directions.moveDirection(position,direction,Global.ChunkSize));
	}
	
	public void CombineQuads(GameObject o, UnityEngine.Rendering.ShadowCastingMode shadow, Material m){		
		//1. Combine all children meshes
		MeshFilter[] meshFilters = o.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter) o.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
		MeshRenderer renderer = o.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = m;
		renderer.shadowCastingMode = shadow;

		//5. Delete all uncombined children
		foreach (Transform quad in o.transform) {
     		GameObject.Destroy(quad.gameObject);
 		}

	}

	public Block getBlock(Vector3 index){
		try{
			return chunkBlocks[(int)index.x,(int)index.y,(int)index.z];
		}catch(IndexOutOfRangeException){
			return null;
		}
	}

	public Vector3 getPosition(){
		return position;
	}

	public Vector3 getChunkBlocksWorldPosition(int x, int y, int z){
		return getChunkBlocksWorldPosition(new Vector3(x,y,z));
	}

	public Vector3 getChunkBlocksWorldPosition(Vector3 blocksChunkPosition){
		return position + blocksChunkPosition;
	}

	/**
	 * This method is used to get the name of a chunk
	 * based on its position
	 * @param Vector3 chunkPosition the chunks position in game.
	 */
	public static string BuildName(Vector3 chunkPosition){
		return (int)chunkPosition.x +"_"+chunkPosition.y+"_"+chunkPosition.z;
	}



	//below here is the new chunk position and name code

	
	public static string ChunkNameFromIndex(Vector3 index){
		return (int)index.x+"_"+(int)index.y+"_"+(int)index.z;
	}

	public static string ChunkNameFromPosition(Vector3 position){
		return ChunkNameFromIndex(ChunkIndexAtPosition(position));
	}

	public static Vector3 ChunkIndexAtPosition(Vector3 position){
		int x = (int)(position.x/Global.ChunkSize);
		int y = (int)(position.y/Global.ChunkSize);
		int z = (int)(position.z/Global.ChunkSize);
		return new Vector3(x,y,z);
	}

	public static Vector3 ChunkPositionAtIndex(Vector3 index){
		return new Vector3(index.x*Global.ChunkSize,index.y*Global.ChunkSize,index.z*Global.ChunkSize);
	}

	public static Vector3 ChunkIndexAtPosition(Vector3 position, out Vector3 blockPosition){
		int x = (int)(position.x/Global.ChunkSize);
		int y = (int)(position.y/Global.ChunkSize);
		int z = (int)(position.z/Global.ChunkSize);
		int bx = (int)(position.x%Global.ChunkSize);
		int by = (int)(position.y%Global.ChunkSize);
		int bz = (int)(position.z%Global.ChunkSize);
		blockPosition = new Vector3(bx,by,bz);
		return new Vector3(x,y,z);
	}

}
