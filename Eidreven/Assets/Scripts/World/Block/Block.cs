﻿using System.Collections.Generic;
using UnityEngine;

public enum CubeSide {BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK}
public enum BlockType {GRASS, DIRT, WATER, STONE, SAND, GOLDORE, BEDROCK, REDSTONEORE, DIAMONDORE, AIR};
public enum CrackType {NOCRACK, CRACK1, CRACK2, CRACK3, CRACK4};
public enum BlockMatterState {GAS, LIQUID, SOLID};

public abstract class Block {

	//variables
	private static float blockSize = 1f;
	private static float halfBlock = blockSize/2f;

	public BlockType bType;
	public CrackType cType;
	public int currentHealth;
	public int maxHealth;
	int[]blockHealthMax = {3,3,8,4,2,3,-1,4,4,0};

	public GameObject parent;
	protected GamePoint index;
	protected GamePoint position;
	public Chunk chunk;
	public BlockMatterState matterState;
	public bool solid;
	public bool isLiquid;

	public GamePoint Index{get{return index;}}
	public GamePoint Position{get{return position;}}

	public static Block CreateBlock(BlockType blockType, Vector3 position, Chunk chunk){
		//https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.enumbuilder?view=netframework-4.8
		Block block;
		switch(blockType){
			case BlockType.GRASS: block = new GrassBlock(position,chunk);break;
			case BlockType.DIRT: block = new DirtBlock(position,chunk);break;
			case BlockType.STONE: block = new StoneBlock(position,chunk);break;
			case BlockType.SAND: block = new SandBlock(position,chunk);break;
			case BlockType.WATER: block = new WaterBlock(position,chunk);break;
			case BlockType.BEDROCK: block = new BedrockBlock(position, chunk);break;
			case BlockType.GOLDORE: block = new GoldOreBlock(position, chunk);break;
			case BlockType.REDSTONEORE: block = new RedstoneOreBlock(position, chunk);break;
			case BlockType.DIAMONDORE: block = new DiamondOreBlock(position,chunk);break;
			default:block = new AirBlock(position,chunk);break;
		}
		return block;
	}

	//Block abstract methods
	public abstract Vector2 getBlockUvs(Direction side,int position);
	//this method is used to store the meta data remember you can store multiple values in one int
	public abstract int getMetaData();
	//This method is used to set the metaData when rebuilding the chunk
	public abstract void setMetaData(int metaData);


	Vector2[,] crackUvs = {
		/*NOCRACK*/			{new Vector2( 0.6875f, 0f ), new Vector2( 0.75f, 0f),
								new Vector2( 0.6875f, 0.0625f ),new Vector2( 0.75f, 0.0625f )},
		/*CRACK1*/			{ new Vector2(0f,0f),  new Vector2(0.0625f,0f),
 								new Vector2(0f,0.0625f), new Vector2(0.0625f,0.0625f)},
 		/*CRACK2*/			{ new Vector2(0.0625f,0f),  new Vector2(0.125f,0f),
 								new Vector2(0.0625f,0.0625f), new Vector2(0.125f,0.0625f)},
 		/*CRACK3*/			{ new Vector2(0.125f,0f),  new Vector2(0.1875f,0f),
 								new Vector2(0.125f,0.0625f), new Vector2(0.1875f,0.0625f)},
 		/*CRACK4*/			{ new Vector2(0.1875f,0f),  new Vector2(0.25f,0f),
 								new Vector2(0.1875f,0.0625f), new Vector2(0.25f,0.0625f)}
	};

	public Block(BlockType blockType, BlockMatterState matterState, int maxHealth, GamePoint index, Chunk chunk){

		this.bType = blockType;
		if(chunk==null)return;
		this.matterState = matterState;
		if(matterState == BlockMatterState.LIQUID){
			this.parent = chunk.Fluids.gameObject;
			isLiquid = true;
		}else{
			this.parent = chunk.Solids.gameObject;
			if(blockType!=BlockType.AIR)solid=true;
		}
		this.index = index;
		this.position = chunk.Position+Index;
		this.chunk = chunk;
		this.currentHealth = maxHealth;
		this.maxHealth = maxHealth;
		cType = CrackType.NOCRACK;
	}

	public bool HitBlock(){
		if(currentHealth == -1)return false;
		currentHealth--;
		//crappy heal that should be removed
		if(currentHealth == (blockHealthMax[(int)bType]-1)){
			chunk.mb.StartCoroutine(chunk.mb.HealBlock(index));
		}
		if(currentHealth<=0){
			setBlock(BlockType.AIR);
			return true;
		}else{
			float maxHealth = blockHealthMax[(int)bType];
			cType = (CrackType)((int)(4-(currentHealth / maxHealth * 4)));
		}
		chunk.Draw();
		return false;
	}

	public void UpdateNeighborChunks(){
		List<Chunk> updates = new List<Chunk>();
		if(index.x==0)updates.Add(chunk.getNeighbor(Direction.WEST));
		if(index.x==Global.ChunkSize-1)updates.Add(chunk.getNeighbor(Direction.EAST));
		if(index.y==0)updates.Add(chunk.getNeighbor(Direction.DOWN));
		if(index.y==Global.ChunkSize-1)updates.Add(chunk.getNeighbor(Direction.UP));
		if(index.z==0)updates.Add(chunk.getNeighbor(Direction.SOUTH));
		if(index.z==Global.ChunkSize-1)updates.Add(chunk.getNeighbor(Direction.NORTH));
		foreach(Chunk uChunk in updates){
			if(uChunk==null)continue;
			uChunk.Draw();
		}
	}

	public bool isSolid(){
		return this.solid;
	}

	public bool setBlock(BlockType b){
		chunk.setBlock(index,Block.CreateBlock(b,index,chunk));
		chunk.Draw();
		UpdateNeighborChunks();
		return true;
	}

	public void Reset(){
		cType = CrackType.NOCRACK;
		currentHealth = blockHealthMax[(int)bType];
		chunk.Draw();
	}

	public BlockMatterState getMatterState(){
		return this.matterState;
	}

	/**
	 * This method is used to create the sides of the
	 * blocks.
	 * @param Direction side The side of the block you
	 * want to create.
	 * @return returns false if not able to create the quad,
	 * otherwise returns true.
	 */
	private bool CreateQuad(Direction side){
		Mesh mesh = new Mesh();
		mesh.name = "ScriptedMesh";
		Vector3[] verts = new Vector3[4];
		List<Vector2> suvs = new List<Vector2>();

		//get uvs from block file
		Vector2[] uvs = new Vector2[] {
			this.getBlockUvs(side, 3),	//uv11
			this.getBlockUvs(side, 2), 	//uv01
			this.getBlockUvs(side, 0), 	//uv00
			this.getBlockUvs(side, 1) 	//uv10
		};
		//set block effects
		suvs.Add(crackUvs[(int)(cType),3]);
		suvs.Add(crackUvs[(int)(cType),2]);
		suvs.Add(crackUvs[(int)(cType),0]);
		suvs.Add(crackUvs[(int)(cType),1]);

		//all possible verticies
		Vector3 p0 = new Vector3(-halfBlock, -halfBlock,  halfBlock);
		Vector3 p1 = new Vector3( halfBlock, -halfBlock,  halfBlock);
		Vector3 p2 = new Vector3( halfBlock, -halfBlock, -halfBlock);
		Vector3 p3 = new Vector3(-halfBlock, -halfBlock, -halfBlock);
		Vector3 p4 = new Vector3(-halfBlock,  halfBlock,  halfBlock);
		Vector3 p5 = new Vector3( halfBlock,  halfBlock,  halfBlock);
		Vector3 p6 = new Vector3( halfBlock,  halfBlock, -halfBlock);
		Vector3 p7 = new Vector3(-halfBlock,  halfBlock, -halfBlock);

		switch(side){
			case Direction.DOWN: verts = new Vector3[]{p0,p1,p2,p3};break;
			case Direction.UP: verts = new Vector3[]{p7,p6,p5,p4};break;
			case Direction.WEST: verts = new Vector3[]{p7,p4,p0,p3};break;
			case Direction.EAST: verts = new Vector3[]{p5,p6,p2,p1};break;
			case Direction.SOUTH: verts = new Vector3[]{p6,p7,p3,p2};break;
			default: verts = new Vector3[]{p4,p5,p1,p0};break;
		} Vector3 norms = Directions.getCubeNormal(side);

		mesh.vertices = verts;
		mesh.normals = new Vector3[] {norms,norms,norms,norms};
		mesh.uv = uvs;
		mesh.SetUVs(1,suvs);
		mesh.triangles = new int[] {3,1,0,3,2,1};
		mesh.RecalculateBounds();
		GameObject quad = new GameObject("Quad");
		try{
			quad.transform.parent = parent.gameObject.transform;
		}catch(MissingReferenceException){
			GameObject.DestroyImmediate(quad);
			return false;
		}
		if(blockSize==1){
			quad.transform.position = index;
		}else{			
			quad.transform.position = Utils.ScaleVector3(index, blockSize);
		}
		MeshFilter meshFilter = (MeshFilter) quad.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = mesh;
		return true;
	}

	public Block getNeighbor(Direction direction){
		//get the neighbors position in this chunk
		Vector3 pic = Directions.moveDirection(index,direction,1);
		//check to see if in different chunk
		if(pic.x<0){pic.x += Global.ChunkSize;
		}else if(pic.x>=Global.ChunkSize){pic.x -= Global.ChunkSize;
		}else if(pic.y<0){pic.y += Global.ChunkSize;
		}else if(pic.y>=Global.ChunkSize){pic.y -= Global.ChunkSize;
		}else if(pic.z<0){pic.z += Global.ChunkSize;
		}else if(pic.z>=Global.ChunkSize){pic.z -= Global.ChunkSize;			
		}else{//the block is in this chunk
			return chunk.getBlock(pic);
		}//the block is in another chunk
		Chunk neighbor = chunk.getNeighbor(direction);
		if(neighbor==null)return null;
		return neighbor.getBlock(pic);
	}

	protected bool shouldRenderQuad(Direction direction){
		//this should check for all matter states
		Block b = getNeighbor(direction);
		if(b!=null){
			if(b.isSolid())return false;
			return (this.matterState!=b.getMatterState());
		}else return false; 			
	}

	public void Draw(){
		if(bType==BlockType.AIR) return;
		for(int i=1;i<=6;i++){
			Direction dir = (Direction)i;
			if(shouldRenderQuad(dir)){
				if(!CreateQuad(dir))return;
			}
		}
	}

	public static GamePoint IndexInChunk(GamePoint position){
		return position.getIndexInParent(Global.ChunkSize);
	}
}
