﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TundraBiome : Biome{

	private static Color mapColor = new Color(96/255f, 131/255f, 112/255f, 1);
	private static GenValues heightSettings = new GenValues(0.01f,4,0.5f,2f);
	private static GenValues stoneSettings = new GenValues(0.01f,5,0.5f,2f);

	public override Color getMapColor(){
		return mapColor;
	}

	public override int getStoneHeight(int seed, int x, int z){
		return Utils.getHeight(x, z, seed, 0, Global.MaxGeneratedHeight, stoneSettings);
	}

	public override int getHeight(int seed, int x, int z){
		return Utils.getHeight(x, z, seed, 0, Global.MaxGeneratedHeight, heightSettings);
	}
    
}

