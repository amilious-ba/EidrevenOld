﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlock  : Block{

   public StoneBlock(Vector3 position, Chunk chunk)
		:base(BlockType.STONE, BlockMatterState.SOLID,4,position,chunk){
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
		if(position==0) return new Vector2( 0, 0.875f );
		if(position==1) return new Vector2( 0.0625f, 0.875f);
		if(position==2) return new Vector2( 0, 0.9375f );
		if(position==3) return new Vector2( 0.0625f, 0.9375f );
		return new Vector2(0,0);
	}
}