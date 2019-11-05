using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class will be used to handle world gerneration
 */
public class WorldGenerator {

	static int maxHeight = 150;
	static float smooth = 0.01f;
	static int octaves = 4;
	static float persistence = 0.5f;


	public static Block[,,] generateChunkBlocksNew(Chunk chunk, int seed){
		//build block matrix
		Block[,,]blocks = new Block[Global.ChunkSize,Global.ChunkSize,Global.ChunkSize];
		//get the biome
		Biome biome = Biome.getBiome(Biome.getBiomeType(seed,chunk.Position.x,chunk.Position.z));
		

		//this will change in the future, for now I am trying to replicate the
		//old style

		//generate heightmaps
		int[,] height = GenerateHM(chunk.Position.x, chunk.Position.z, seed, 0, maxHeight, smooth, octaves, persistence, 2f);
		int[,] stoneHeight =  GenerateHM(chunk.Position.x, chunk.Position.z, seed, 0, maxHeight-7, smooth, octaves+1, persistence, 2f);

		for(int y=0;y<Global.ChunkSize;y++){
			for(int z=0;z<Global.ChunkSize;z++){
				for(int x=0;x<Global.ChunkSize;x++){

					//setPositions
					GamePoint cP = new GamePoint(x,y,z);
					GamePoint wP = chunk.getBlockPosition(cP);

					//generate bedrock
					if(wP.y == 0)blocks[x,y,z] = Block.CreateBlock(BlockType.BEDROCK, cP, chunk);
					//generate stone layer
					else if(wP.y<=stoneHeight[x,z]){
						blocks[x,y,z] = Block.CreateBlock(BlockType.STONE,cP,chunk);
					}
					//generate grass
					else if(wP.y == height[x,z]&&wP.y>=Global.SeaLevel-1){
						blocks[x,y,z] = Block.CreateBlock(BlockType.GRASS,cP,chunk);
					}
					//generate dirt
					else if(wP.y<=height[x,z])blocks[x,y,z] = Block.CreateBlock(BlockType.DIRT,cP,chunk);
					//generate sea and sky
					else{
						if(wP.y<Global.SeaLevel){//fill with water
							blocks[x,y,z] = Block.CreateBlock(BlockType.WATER, cP, chunk);
						}else{//fill with air
							blocks[x,y,z] = Block.CreateBlock(BlockType.AIR, cP, chunk);
						}
					}

				}
			}
		}

		return blocks;
	}

	public static int[,] GenerateHM(float cX, float cZ,int seed, int min, int max, float sm, int oc, float per, float lac){
		int[,] heightMap = new int[Global.ChunkSize,Global.ChunkSize];
		//first generate a noise map
		float[,] noiseMap = Utils.FBM_Map(cX, cZ, Global.ChunkSize, seed, sm, oc, per, lac);
		//convert the heightmap
		for(int x=0;x<Global.ChunkSize;x++)for(int z=0;z<Global.ChunkSize;z++){
			heightMap[x,z] = (int)(Utils.Map(min,max,0,1,noiseMap[x,z]));
		}
		//return the heightmap
		return heightMap;
	}


	public static Block[,,] generateChunkBlocks(Chunk chunk, int seed){
		Block[,,] blocks = new Block[Global.ChunkSize, Global.ChunkSize,Global.ChunkSize];

		//gerate heightmap

		//generate heatmap
		//generate mositure map
		


		for(int y = 0; y < Global.ChunkSize; y++){
			//check to see if past max generated height
			if(y>maxHeight){
				for(int z=0;z<Global.ChunkSize; z++)for(int x=0;x<Global.ChunkSize;x++){
					blocks[x,y,z] = Block.CreateBlock(BlockType.AIR, new GamePoint(x,y,z), chunk);
				}continue;
			}
			for(int z = 0; z < Global.ChunkSize; z++){
				for(int x = 0; x < Global.ChunkSize; x++){
					//position in chunk
					GamePoint cP = new GamePoint(x,y,z);
					//position in world
					GamePoint wP = chunk.getBlockPosition(cP);

					int surfaceHeight = GenerateHeight(wP.x,wP.z);
					
					
					if(wP.y == 0)
						blocks[x,y,z] = Block.CreateBlock(BlockType.BEDROCK, cP, chunk);
					else if(wP.y <= GenerateStoneHeight(wP.x,wP.z))
					{
						if(FBM3D(wP.x, wP.y, wP.z, 0.01f, 2) < 0.4f && wP.y < 40)
							blocks[x,y,z] = Block.CreateBlock(BlockType.DIAMONDORE, cP, chunk);
						else if(FBM3D(wP.x, wP.y, wP.z, 0.03f, 3) < 0.41f && wP.y < 20)
							blocks[x,y,z] = Block.CreateBlock(BlockType.REDSTONEORE, cP, chunk);
						else
							blocks[x,y,z] = Block.CreateBlock(BlockType.STONE, cP, chunk);
					}
					else if(wP.y == surfaceHeight&&wP.y>=Global.SeaLevel-1){ //grass
						blocks[x,y,z] = Block.CreateBlock(BlockType.GRASS, cP, chunk);
					}
					else if(wP.y<=surfaceHeight){ //dirt layer
						blocks[x,y,z] = Block.CreateBlock(BlockType.DIRT,cP,chunk);
					}
					else
					{
						if(wP.y<Global.SeaLevel){//fill with water
							blocks[x,y,z] = Block.CreateBlock(BlockType.WATER, cP, chunk);
						}else{//fill with air
							blocks[x,y,z] = Block.CreateBlock(BlockType.AIR, cP, chunk);
						}
					}
					//generate caves  change to not liquid
					if(FBM3D(wP.x, wP.y, wP.z, 0.1f, 3) < 0.42f&&wP.y>0 && blocks[x,y,z].bType!=BlockType.WATER){
						blocks[x,y,z] = Block.CreateBlock(BlockType.AIR, cP, chunk);
					}

				}
			}
		}
		return blocks;
	}



	public static int GenerateStoneHeight(float x, float z){
		float height = Map(0,maxHeight-7,0,1,FBM(x*smooth*2,z*smooth*2,octaves+1,persistence));
		return (int)height;
	}

	public static int GenerateHeight(float x, float z){
		float height = Map(0,maxHeight,0,1,FBM(x*smooth,z*smooth,octaves,persistence));
		return (int)height;
	}

	public static float Map(float newMin, float newMax, float oldMin, float oldMax, float value){
		return Mathf.Lerp(newMin,newMax,Mathf.InverseLerp(oldMin,oldMax, value));
	}

	public static float FBM(float x, float z, int oct, float pers){
		float total = 0;
		float freq = 1;
		float amp = 1;
		float maxVal = 0;
		float offset = 32000;
		for(int i=0;i<oct; i++){
			total+=Mathf.PerlinNoise((x+offset)*freq,(z+offset)*freq)*amp;
			maxVal+=amp;
			amp*= pers;
			freq*=2;
		}
		return total/maxVal;
	}

	public static float FBM3D(float x, float y, float z, float smooth, int oct){
		float XY = FBM(x*smooth, y*smooth,oct,0.5f);
		float YZ = FBM(y*smooth, z*smooth,oct,0.5f);
		float XZ = FBM(x*smooth, z*smooth,oct,0.5f);
		float YX = FBM(y*smooth, x*smooth,oct,0.5f);
		float ZY = FBM(z*smooth, y*smooth,oct,0.5f);
		float ZX = FBM(z*smooth, x*smooth,oct,0.5f);
		return (XY+YZ+XZ+YX+ZY+ZX)/6.0f;
	}

}
