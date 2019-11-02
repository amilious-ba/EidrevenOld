using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBiome : Biome{

	private static Color mapColor = Color.white;

	public override Color getMapColor(){
		return mapColor;
	}
    
}
