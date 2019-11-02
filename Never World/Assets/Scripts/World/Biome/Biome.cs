using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
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

	public static BiomeType getBiomeType(float moisture, float temperature){
		int m = (int)Utils.Map(0,BiomeTable.GetLength(0),0,1,moisture);
		int t = (int)Utils.Map(0,BiomeTable.GetLength(1),0,1,temperature);
		if(m==BiomeTable.GetLength(0))m=BiomeTable.GetLength(0)-1;
		if(t==BiomeTable.GetLength(1))t=BiomeTable.GetLength(1)-1;
		return BiomeTable[m,t];

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

	public abstract Color getMapColor();
	//public abstract Block[,,] generateChunkBlocks(Vector2 position, int seed, int maxHeight);

}
