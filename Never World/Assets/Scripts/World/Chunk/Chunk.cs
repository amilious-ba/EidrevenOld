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

	private GamePoint position;
	private GamePoint index;
	public ChunkMB mb;
	
	private Block[,,] blocks;
	public ChunkStatus status = ChunkStatus.NOTBUILT;
	SaveData saveData;
	private bool changed = false;


	// Use this for initialization
	public Chunk (GamePoint index, World world) {
		this.world = world;
		this.index = index;
		this.position = index * Global.ChunkSize;	
	}	

	public void Build(){
		if(built)return;
		built = true;
		if(status==ChunkStatus.DEAD)return;
		//set up		
		solidGameObject = new GameObject(NameFromIndex(index));
		solidGameObject.transform.position = position;
		solidGameObject.transform.parent = world.transform;
		fluidGameObject = new GameObject(NameFromIndex(index)+"_F");
		fluidGameObject.transform.position = position;
		fluidGameObject.transform.parent = world.transform;
		mb = solidGameObject.AddComponent<ChunkMB>();
		mb.SetOwner(this);	
		//build
		status = ChunkStatus.BUILDING;
		if(Load()) blocks = saveData.loadBlocks(this);
		else blocks = WorldGenerator.generateChunkBlocksNew(this,world.Seed);
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
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshFilter>());}catch(MissingReferenceException){}
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshRenderer>());}catch(MissingReferenceException){}
				try{GameObject.DestroyImmediate(solidGameObject.GetComponent<Collider>());}catch(MissingReferenceException){}
				try{GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshFilter>());}catch(MissingReferenceException){}
				try{GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshRenderer>());}catch(MissingReferenceException){}
			}
			for(int z = 0; z < Global.ChunkSize; z++)for(int y = 0; y < Global.ChunkSize; y++)
			for(int x = 0; x < Global.ChunkSize; x++)blocks[x,y,z].Draw();
			CombineQuads(solidGameObject.gameObject, ShadowCastingMode.On, world.BlockMaterial);
			MeshCollider collider = solidGameObject.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			collider.sharedMesh = solidGameObject.transform.GetComponent<MeshFilter>().mesh;

			CombineQuads(fluidGameObject.gameObject, ShadowCastingMode.Off, world.FluidMaterial);
			status = ChunkStatus.GOOD;
			drawn = true;		
 		}catch(MissingReferenceException){return false;}
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
		changed = false;
		string chunkFile = getFileName(position);
		
		if(!File.Exists(chunkFile))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
		saveData = new SaveData(blocks);
		bf.Serialize(file, saveData);
		file.Close();
		//Debug.Log("Saving chunk from file: " + chunkFile);
	}	

	public Chunk getNeighbor(Direction direction){
		string neighborName = getNeighborName(direction);Chunk c = null;
		world.getLoadedChunk(neighborName, out c);return c;
	}

	public string getNeighborName(Direction direction){
		return NameFromIndex(index.moveDirection(direction,1));
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

	public Block getBlock(GamePoint index){
		try{ return blocks[index.x,index.y,index.z];}
		catch(IndexOutOfRangeException) {return null;}
	}

	public bool setBlock(GamePoint index, Block block){
		if(index<0||index>Global.ChunkSize)return false;
		blocks[index.x,index.y,index.z] = block;
		changed = true; return true;
	}

	public Vector3 getBlockPosition(int x, int y, int z){
		return getBlockPosition(new GamePoint(x,y,z));
	}

	public GamePoint getBlockPosition(GamePoint blockIndex){
		return position + blockIndex;
	}
	
	public static string NameFromIndex(GamePoint index){
		return index.x+"_"+index.y+"_"+index.z;
	}

	public static string NameFromPosition(GamePoint position){
		return NameFromIndex(IndexAtPosition(position));
	}

	public static GamePoint IndexAtPosition(GamePoint position){
		return position.getParentIndex(Global.ChunkSize);
	}

	public static GamePoint PositionFromIndex(GamePoint index){
		return index*Global.ChunkSize;
	}

	public GamePoint Position{get{return position;}}
	public GamePoint Index{get{return index;}}
	public ChunkStatus Status{get{return status;}set{status=value;}}
	public bool Changed{get{return changed;}}
	public GameObject Solids{get{return this.solidGameObject;}}
	public GameObject Fluids{get{return this.fluidGameObject;}}

}