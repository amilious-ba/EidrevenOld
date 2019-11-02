using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonalForestBiome :  Biome{

	private static Color mapColor = new Color(73/255f, 100/255f, 35/255f, 1);
	public override Color getMapColor(){
		return mapColor;
	}
    
}

