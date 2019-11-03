using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour{


    //variables
    [Header("Map Settings")]
    public int chunkSize;
    public string seed;   
    public int seedValue;
    public Vector2 offset;
    public bool autoUpdate;
    public DrawType draw;

    [Space(10)]
    [Header("HeatMap")]
    public int heatOctaves;
    [Range(0,1)]
    public float heatPersistance;
    [Range(0,1)]
    public float heatSmooth;
    public float heatLacunarity;
    public Color heatColorA;
    public Color heatColorB;

    [Space(10)]
    [Header("HumidMap")]
    public int humidOctaves;
    [Range(0,1)]
    public float humidPersistance;
    [Range(0,1)]
    public float humidSmooth;
    public float humidLacunarity;    
    public Color humidColorA;
    public Color humidColorB;


    public void GenerateMap(){
    	seedValue = Utils.getSeedValue(seed);
    	float[,] heatMap = Utils.FBM_Map((int)offset.x, (int)offset.y, chunkSize, seedValue, heatSmooth, heatOctaves, heatPersistance, heatLacunarity);
    	float[,] humidMap = Utils.FBM_Map((int)offset.x, (int)offset.y, chunkSize, seedValue+1, humidSmooth, humidOctaves, humidPersistance, humidLacunarity);

    	MapDisplay display = FindObjectOfType<MapDisplay>();
    	switch(draw){
    		case DrawType.HeatMap:
    			display.DrawNoiseMap(heatMap,heatColorA,heatColorB);
    			break;
    		case DrawType.HumidMap:    		
    			display.DrawNoiseMap(humidMap,humidColorA,humidColorB);
    			break;
    		case DrawType.BiomeMap:
    			Color[] colorMap = new Color[chunkSize*chunkSize];
    			for(int y=0;y<chunkSize;y++){
    				int yoff = y*chunkSize;
    				for(int x=0;x<chunkSize;x++){
						colorMap[yoff+x] = Biome.getBiome(Biome.getBiomeType(humidMap[x,y], heatMap[x,y])).getMapColor();
					}
    			}
    			display.DrawColorMap(colorMap,chunkSize);
    			break;
    	}
    }

    public void onValidate(){
    	if(chunkSize<1)chunkSize=1;
    	if(heatLacunarity<1)heatLacunarity=1;
    	if(humidLacunarity<1)humidLacunarity=1;
    	if(heatOctaves<0)heatOctaves=0;
    	if(humidOctaves<0)humidOctaves=0;

    }

    public class GenerationArgs{

    }

    public enum DrawType {HeatMap, HumidMap, BiomeMap}
}
