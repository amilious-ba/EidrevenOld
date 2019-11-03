using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor{
    
    public override void OnInspectorGUI(){
    	MapGenerator mapGen = (MapGenerator)target;
    	if(DrawDefaultInspector()&&mapGen.autoUpdate){
    		mapGen.GenerateMap();
    	}
    	if(GUILayout.Button("Load From Global")){
    		//load heat settings
    		mapGen.heatSmooth = Global.HeatSettings.Smooth;
    		mapGen.heatOctaves = Global.HeatSettings.Octaves;
    		mapGen.heatPersistance = Global.HeatSettings.Persistance;
    		mapGen.heatLacunarity = Global.HeatSettings.Lacunarity;
    		//load humidity settings
    		mapGen.humidSmooth = Global.HumiditySettings.Smooth;
    		mapGen.humidOctaves = Global.HumiditySettings.Octaves;
    		mapGen.humidPersistance = Global.HumiditySettings.Persistance;
    		mapGen.humidLacunarity = Global.HumiditySettings.Lacunarity;
    		//generate the new map
    		mapGen.GenerateMap();
    	}
    	if(GUILayout.Button("Generate")){
    		mapGen.GenerateMap();
    	}
    }
}
