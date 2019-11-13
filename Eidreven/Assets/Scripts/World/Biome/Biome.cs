using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType{
    Desert,
    Savanna,
    TropicalRainforest,
    Grassland,
    Woodland,
    SeasonalForest,
    TemperateRainforest,
    BorealForest,
    Tundra,
    Ice
}

public abstract class Biome {

	public abstract Color getMapColor();
	public abstract int getHeight(int seed, int x, int z);
	public abstract int getStoneHeight(int seed, int x, int z);
	public abstract BiomeType getType();

    protected static BiomeType[,] BiomeTable = new BiomeType[6,6] {   
	    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
	    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
	};

	protected static DesertBiome desert = new DesertBiome();
	protected static SavannaBiome savanna = new SavannaBiome();
	protected static TropicalRainforestBiome tropicalRainforest = new TropicalRainforestBiome();
	protected static GrasslandBiome grassland = new GrasslandBiome();
	protected static WoodLandBiome woodland = new WoodLandBiome();
	protected static SeasonalForestBiome seasonalForest = new SeasonalForestBiome();
	protected static TemperateRainforestBiome temperateRainforest = new TemperateRainforestBiome();
	protected static BorealForestBiome borealForest = new BorealForestBiome();
	protected static TundraBiome tundra = new TundraBiome();
	protected static IceBiome ice = new IceBiome();

	public static BiomeType getBiomeType(int moistureLevel, int heatLevel){
		return BiomeTable[moistureLevel,heatLevel];
	}

	public static Biome getBiome(BiomeType type){
		switch(type){
			case BiomeType.Desert: return desert;
			case BiomeType.Savanna: return savanna;
			case BiomeType.TropicalRainforest: return tropicalRainforest;
			case BiomeType.Grassland: return grassland;
			case BiomeType.Woodland: return woodland;
			case BiomeType.SeasonalForest: return seasonalForest;
			case BiomeType.TemperateRainforest: return temperateRainforest;
			case BiomeType.BorealForest: return borealForest;
			case BiomeType.Tundra: return tundra;
			case BiomeType.Ice: return ice;
			default: return grassland;
		}
	}

	public static int getHeight(BiomeType biome, int seed, int x, int z){
		return getBiome(biome).getHeight(seed, x, z);
	}

	public static int getStoneHeight(BiomeType biome, int seed, int x, int z){
		return getBiome(biome).getStoneHeight(seed, x, z);
	}

	public static int getTopHeight(BiomeType biome, int seed, int x, int z){
		int height = getBiome(biome).getHeight(seed, x, z);
		int stoneHeight = getBiome(biome).getStoneHeight(seed, x, z);
		return (height>=stoneHeight)?height:stoneHeight;
	}

	public static BiomeType getBiomeType( int seed, int x, int z){
		float heat = Utils.FBM(x, z, seed, Global.HeatSettings);
		float humid = Utils.FBM(x,z,seed,Global.HumiditySettings);
		return getBiomeType(getMoistureLevel(seed, x, z), getHeatLevel(seed, x, z));
	} 

	public static int getMoistureLevel(int seed, int x, int z){
		int moisture = (int)Utils.Map(0,BiomeTable.GetLength(0),0,1,Utils.FBM(x,z,seed,Global.HumiditySettings));
		if(moisture==BiomeTable.GetLength(0))moisture=BiomeTable.GetLength(0)-1;
		return moisture;
	}

	public static int getMoistureLevel(float value){
		int moisture = (int)Utils.Map(0,BiomeTable.GetLength(0),0,1,value);
		if(moisture==BiomeTable.GetLength(0))moisture=BiomeTable.GetLength(0)-1;
		return moisture;
	}

	public static int getHeatLevel(int seed, int x, int z){
		int heat = (int)Utils.Map(0,BiomeTable.GetLength(1),0,1,Utils.FBM(x,z,seed,Global.HeatSettings));
		if(heat==BiomeTable.GetLength(1))heat=BiomeTable.GetLength(1)-1;
		return heat;
	}

	public static int getHeatLevel(float value){
		int heat = (int)Utils.Map(0,BiomeTable.GetLength(1),0,1,value);
		if(heat==BiomeTable.GetLength(1))heat=BiomeTable.GetLength(1)-1;
		return heat;
	}

}
