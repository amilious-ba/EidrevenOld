using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperateRainforestBiome : Biome{

	private static Color mapColor = Color.red/*new Color(29/255f, 73/255f, 40/255f, 1)*/;
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

	public override BiomeType getType(){return BiomeType.TemperateRainforest;}
    
}

