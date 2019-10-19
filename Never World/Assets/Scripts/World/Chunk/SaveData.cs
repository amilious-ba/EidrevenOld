using UnityEngine;
using System;

/**
 * This class is used serialize chunk data so
 * that it can be read or written to a file.
 */
[Serializable]
class SaveData{

	//all varables should be public
	public BlockType[,,] blockType;
	public int[,,] metaData;
	
	/**
	 * This is the default constructor which must be defined.
	 * This constructor is used when the data is loaded
	 */
	public SaveData(){}

	/**
	 * This constructor should be used when saving data.
	 * @param  Block[,,] contains the chunks blocks.
	 */
	public SaveData(Block[,,] b){
		blockType = new BlockType[Global.ChunkSize,Global.ChunkSize,Global.ChunkSize];
		metaData = new int[Global.ChunkSize,Global.ChunkSize,Global.ChunkSize];
		for(int z=0; z<Global.ChunkSize; z++)for(int y=0; y< Global.ChunkSize; y++)
			for(int x=0; x<Global.ChunkSize; x++){
				blockType[x,y,z] = b[x,y,z].bType;
				metaData[x,y,z] = b[x,y,z].getMetaData();
		}
	}

	/**
	 * This method is used by the chunk to load the loaded data.
	 * @param  Chunk chunk	The chunk that is being loaded.	
	 * @return Block[,,]	Contains all the block data for the
	 * chunk.
	 */
	public Block[,,] loadBlocks(Chunk chunk){
		Block[,,] blocks = new Block[Global.ChunkSize,Global.ChunkSize,
			Global.ChunkSize];
		for(int z=0; z<Global.ChunkSize; z++)for(int y=0; y<Global.ChunkSize; y++)
			for(int x = 0; x < Global.ChunkSize; x++){
				blocks[x,y,z] = Block.CreateBlock(blockType[x,y,z],new Vector3(x,y,z), chunk);
				blocks[x,y,z].setMetaData(metaData[x,y,z]);			
		}
		return blocks;
	}
}