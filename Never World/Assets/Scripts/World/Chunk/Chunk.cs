using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Rendering;

public enum ChunkStatus{DRAW, DONE, KEEP}

public class Chunk {

	public Material cubeMaterial;
	public Material fluidMaterial;
	public Block[,,] chunkBlocks;
	private GameObject solidGameObject;
	private GameObject fluidGameObject;
	public ChunkStatus status;
	public ChunkMB mb;
	public Vector3 position;
	SaveData saveData;
	public bool changed = false;

	public GameObject getSolids(){
		return this.solidGameObject;
	}

	public GameObject getFluids(){
		return this.fluidGameObject;
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

	void BuildChunk(){
		if(Load()) chunkBlocks = saveData.loadBlocks(this);
		else chunkBlocks = WorldGenerator.generateChunkBlocks(this);
		status = ChunkStatus.DRAW;
	}

	public void Redraw()
	{
		GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshFilter>());
		GameObject.DestroyImmediate(solidGameObject.GetComponent<MeshRenderer>());
		GameObject.DestroyImmediate(solidGameObject.GetComponent<Collider>());
		GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshFilter>());
		GameObject.DestroyImmediate(fluidGameObject.GetComponent<MeshRenderer>());
		DrawChunk();
	}

	public void DrawChunk()
	{
		for(int z = 0; z < Global.ChunkSize; z++)
			for(int y = 0; y < Global.ChunkSize; y++)
				for(int x = 0; x < Global.ChunkSize; x++)
				{
					chunkBlocks[x,y,z].Draw();
				}
		CombineQuads(solidGameObject.gameObject, ShadowCastingMode.On, cubeMaterial);
		MeshCollider collider = solidGameObject.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = solidGameObject.transform.GetComponent<MeshFilter>().mesh;

		CombineQuads(fluidGameObject.gameObject, ShadowCastingMode.Off, fluidMaterial);
		status = ChunkStatus.DONE;
	}

	public Chunk(){}
	// Use this for initialization
	public Chunk (Vector3 position, Material c, Material t) {
		
		solidGameObject = new GameObject(World.BuildChunkName(position));
		solidGameObject.transform.position = position;
		fluidGameObject = new GameObject(World.BuildChunkName(position)+"_F");
		fluidGameObject.transform.position = position;
		this.position = position;
		mb = solidGameObject.AddComponent<ChunkMB>();
		mb.SetOwner(this);
		cubeMaterial = c;
		fluidMaterial = t;
		BuildChunk();
	}

	public void Destroy(){
		GameObject.Destroy(solidGameObject);
		GameObject.Destroy(fluidGameObject);
		Save();
	}

	public Chunk getNeighbor(Direction direction){
		string neighborName = getNeighborName(direction);
		Chunk c = null;
		World.loadedChunks.TryGetValue(neighborName, out c);
		return c;
	}

	public string getNeighborName(Direction direction){
		switch(direction){
			case Direction.WEST:
				return World.BuildChunkName(new Vector3(position.x-Global.ChunkSize,position.y,position.z));
			case Direction.EAST:
				return World.BuildChunkName(new Vector3(position.x+Global.ChunkSize,position.y,position.z));
			case Direction.UP:
				return World.BuildChunkName(new Vector3(position.x,position.y+Global.ChunkSize,position.z));
			case Direction.DOWN:
				return World.BuildChunkName(new Vector3(position.x,position.y-Global.ChunkSize,position.z));
			case Direction.NORTH:
				return World.BuildChunkName(new Vector3(position.x,position.y,position.z+Global.ChunkSize));
			case Direction.SOUTH:
				return World.BuildChunkName(new Vector3(position.x,position.y,position.z-Global.ChunkSize));
			default:
				return "unknown chunk";
		}
	}
	
	public void CombineQuads(GameObject o, UnityEngine.Rendering.ShadowCastingMode shadow, Material m)
	{
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

}
