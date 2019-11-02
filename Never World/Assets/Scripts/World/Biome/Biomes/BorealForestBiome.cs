using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorealForestBiome : Biome{

	private static Color mapColor = new Color(95/255f, 115/255f, 62/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}
