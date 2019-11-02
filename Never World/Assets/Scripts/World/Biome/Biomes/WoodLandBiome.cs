using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLandBiome : Biome{

	private static Color mapColor = new Color(139/255f, 175/255f, 90/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}

