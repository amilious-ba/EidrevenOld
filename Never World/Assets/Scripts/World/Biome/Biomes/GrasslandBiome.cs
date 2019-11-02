using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasslandBiome : Biome{

	private static Color mapColor = new Color(164/255f, 225/255f, 99/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}

