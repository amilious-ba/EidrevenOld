using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TundraBiome : Biome{

	private static Color mapColor = new Color(96/255f, 131/255f, 112/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}

