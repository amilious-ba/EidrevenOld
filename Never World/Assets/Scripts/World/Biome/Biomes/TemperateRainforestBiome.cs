using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperateRainforestBiome : Biome{

	private static Color mapColor = Color.red/*new Color(29/255f, 73/255f, 40/255f, 1)*/;

	public override Color getMapColor(){
		return mapColor;
	}
    
}

