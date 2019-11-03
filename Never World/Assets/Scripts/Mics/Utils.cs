using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

public class Utils {

	/**
	 * This method is used to multiply all the vector3s values
	 * by the passed scale.
	 * @param Vector3 vector The vector3 you want to scale.
	 * @param float   scale  The number you want to multiply
	 * all the vector3s points by.
	 * @returns Vector3 a scaled vector3
	 */
	public static Vector3 ScaleVector3(Vector3 vector, float scale){
		return new Vector3(vector.x*scale,vector.y*scale,vector.z*scale);
	}

	/**
	 * This method is used to create a Vector3 where all the
	 * values are the same.
	 * @param  float value         The value for the x, y, and
	 * z values in the Vector3.
	 * @return Vector3       returns a Vector3 with all values
	 * set to the given value.
	 */
	public static Vector3 fillVector3(float value){
		return new Vector3(value, value, value);
	}

	public static Vector3 convertArrayToVector3(float[] array){
		if(array==null||array.Length<3)return new Vector3(0,0,0);
		return new Vector3(array[0],array[1],array[2]);
	}

	public static Vector2 convertArrayToVector2(float[] array){
		if(array==null||array.Length<2)return new Vector2(0,0);
		return new Vector2(array[0],array[1]);
	}

	/**
	 * This method lets us get a seed value from the given string.
	 * @param  string seed          The seed you want to make an
	 * integer value for.
	 * @return int        Contains the seed from the given value.
	 */
	public static int getSeedValue(string seed){
		int val=0;
		if(int.TryParse(seed, out val)){
			return val;
		}else{
			MD5 md5Hasher = MD5.Create();
			var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(seed));
			return (int)BitConverter.ToInt32(hashed,0);
		}
	}

	public static float FBM(float x, float z, int seed, GenValues genValues){
		return FBM(x,z,seed,genValues.Smooth,genValues.Octaves,genValues.Persistance,genValues.Lacunarity);
	}

	public static float FBM(float x, float z, int seed, float smooth, int octaves, float persistance, float lacunarity){
		float total = 0;
		float freq = 1;
		float amp = 1;
		float maxVal = 0;

		System.Random psodRandom = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for(int i=0;i<octaves;i++){
			float offsetX = psodRandom.Next(-32000,32000);
			float offsetY = psodRandom.Next(-32000,32000);
			octaveOffsets[i] = new Vector2(offsetX,offsetY);
		}

		for(int i=0;i<octaves; i++){
			total+=Mathf.PerlinNoise((x+octaveOffsets[i].x)*smooth*freq,(z+octaveOffsets[i].y)*smooth*freq)*amp;
			maxVal+=amp;
			amp*= persistance;
			freq*=lacunarity;
		}
		return total/maxVal;
	}

	public static float[,] FBM_Map(float x, float z, int chunkSize, int seed, GenValues genValues){
		return FBM_Map(x,z,chunkSize,seed,genValues.Smooth,genValues.Octaves,genValues.Persistance,genValues.Lacunarity);
	}

	/**
	 * This method should be used when generating a complete chunk.
	 * @param {[type]} float x           the chunk x position
	 * @param {[type]} float z           the chunk y position
	 * @param {[type]} int   size		 the chunk size
	 * @param {[type]} int   seed        the map seed
	 * @param {[type]} float smooth      the scale value for the noise
	 * generation.  This should be a number greater than 0 and less than 1.
	 * @param {[type]} int   octaves     the number of octaves.
	 * @param {[type]} float persistance the persistance in amplitude
	 * @param {[type]} float lacunarity  the lacunarity in the frequesncy
	 */
	public static float[,] FBM_Map(float x, float z, int chunkSize, int seed, float smooth, int octaves, float persistance, float lacunarity){
		float[,] noiseMap = new float[chunkSize,chunkSize];
		System.Random psodRandom = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for(int i=0;i<octaves;i++){
			float offsetX = psodRandom.Next(-32000,32000);
			float offsetY = psodRandom.Next(-32000,32000);
			octaveOffsets[i] = new Vector2(offsetX,offsetY);
		}
		for(int mY=0;mY<chunkSize;mY++)for(int mX=0;mX<chunkSize;mX++){
			float total = 0;
			float freq = 1;
			float amp = 1;
			float maxVal = 0;
			for(int i=0;i<octaves; i++){
				total+=Mathf.PerlinNoise((x+octaveOffsets[i].x+mX)*smooth*freq,(z+octaveOffsets[i].y+mY)*smooth*freq)*amp;
				maxVal+=amp;
				amp*= persistance;
				freq*=lacunarity;
			}
			noiseMap[mX,mY] =  total/maxVal;
		}
		return noiseMap;
	}

	public static float FBM3D(float x, float y,float z,int seed,GenValues genValues){
		return FBM3D(x,y,z,seed,genValues.Smooth,genValues.Octaves,genValues.Persistance,genValues.Lacunarity);
	}

	public static float FBM3D(float x, float y, float z, int seed, float smooth, int octaves, float persistance, float lacunarity){
		float XY = FBM(x, y,seed,smooth,octaves,persistance,lacunarity);
		float YZ = FBM(y, z,seed,smooth,octaves,persistance,lacunarity);
		float XZ = FBM(x, z,seed,smooth,octaves,persistance,lacunarity);
		float YX = FBM(y, x,seed,smooth,octaves,persistance,lacunarity);
		float ZY = FBM(z, y,seed,smooth,octaves,persistance,lacunarity);
		float ZX = FBM(z, x,seed,smooth,octaves,persistance,lacunarity);
		return (XY+YZ+XZ+YX+ZY+ZX)/6.0f;
	}

	public static float[,] FBM3D_Map(float x, float y, float z, int size, int seed, GenValues genValues){
		return FBM3D_Map(x,y,z,size,seed,genValues.Smooth,genValues.Octaves,genValues.Persistance,genValues.Lacunarity);
	}

	public static float[,] FBM3D_Map(float x, float y, float z, int chunkSize, int seed, float smooth, int octaves, float persistance, float lacunarity){
		float[,] XY = FBM_Map(x, y,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] YZ = FBM_Map(y, z,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] XZ = FBM_Map(x, z,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] YX = FBM_Map(y, x,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] ZY = FBM_Map(z, y,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] ZX = FBM_Map(z, x,chunkSize,seed,smooth,octaves,persistance,lacunarity);
		float[,] fbm3d = new float[chunkSize,chunkSize];
		for(int mY=0;mY<chunkSize;mY++)for(int mX=0;mX<chunkSize;mX++){
			fbm3d[mX,mY] = (XY[mX,mY]+YZ[mX,mY]+XZ[mX,mY]+YX[mX,mY]+ZY[mX,mY]+ZX[mX,mY])/6.0f;
		}
		return fbm3d;
	}

	public static float Map(float newMin, float newMax, float oldMin, float oldMax, float value){
		return Mathf.Lerp(newMin,newMax,Mathf.InverseLerp(oldMin,oldMax, value));
	}

	public static int getHeight(float x, float z, int seed, int min, int max, GenValues genValues){
		float rawHeight = FBM(x, z, seed, genValues);
		return (int)Map(min,max,0,1,rawHeight);
	}

	public static int[,] GenerateHM(float x, float z, int size, int seed, int min, int max, GenValues genValues){
		int[,] heightMap = new int[size,size];
		//first generate a noise map
		float[,] noiseMap = Utils.FBM_Map(x, z, size, seed, genValues);
		//convert the heightmap
		for(int cx=0;cx<size;cx++)for(int cz=0;cz<size;cz++){
			heightMap[cx,cz] = (int)(Map(min,max,0,1,noiseMap[cx,cz]));
		}
		//return the heightmap
		return heightMap;
	}


}
