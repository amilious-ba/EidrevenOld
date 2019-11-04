using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {

	//general settings
	public static bool AllowSave = false;
	public static bool AllowLoad = false;
	public static float version = 0.01f;
	

	//World Generation Settings
	public static float BlockSize = 1;				//this value changes the base block size
	public static int ChunkSize = 16;				//this value sets the chunk dimentions
	public static int ChunksTall = 16;				//this is the number of chunks tall the world is
	public static int SeaLevel = 70;					//This sets the worlds sea level.
	public static int MaxGeneratedHeight = 150;		//This is the max height that the terian will generate
	public static GenValues HumiditySettings = new GenValues(0.038f,2,0.5f,2f);
	public static GenValues HeatSettings = new GenValues(0.027f,2,0.5f,2f);


	//Preformance Settings
	public static uint MaxCoroutines = 2000;			//This value sets the max number of coroutines
	public static bool BuildOnChunkChange = true;	//If true the chunk building will be locked until you change chunks
	public static int BuildWhenMovedXChunks = 1;		//This value sets the distance moved before chunks are redrawn
	public static int LoadRadius = 5;			//This is the distance chunks will be loaded
	

	//methods
	public static float getHalfBlockSize(){return BlockSize/2f;}
}
