using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GenValues{
    
    public int Octaves;
    public float Smooth, Persistance, Lacunarity;

    public GenValues(float Smooth, int Octaves, float Persistance, float Lacunarity){
    	this.Smooth = Smooth;
    	this.Octaves = Octaves;
    	this.Persistance = Persistance;
    	this.Lacunarity = Lacunarity;
    }
}
