using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBiome  : Biome{

	private static Color mapColor = new Color(238/255f, 218/255f, 130/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}
