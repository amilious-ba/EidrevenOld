using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBlock : Block{
   
    public AirBlock(Vector3 position, Chunk chunk)
		:base(BlockType.AIR, BlockMatterState.GAS,-1,position,chunk){
	}

	public override int getMetaData(){
		return 0;
	}

	public override void setMetaData(int metaData){
		return;
		//ther is no metadata for grass
	}

	/**
	 * This method is used to get the uvs of this block when it
	 * is drawn.
	 * @param  BlockSide side The side of the block that is being
	 * drawn.
	 * @param  int       position      The uv number for the side
	 * that is being drawn.				
	 * @return Vector2           Contains the uvs for the give side
	 * and position.
	 */
	public override Vector2 getBlockUvs(Direction side, int position){	
		return new Vector2(0,0);
	}
}
