using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavannaBiome : Biome{

	private static Color mapColor = new Color(177/255f, 209/255f, 110/255f, 1);

	public override Color getMapColor(){
		return mapColor;
	}
    
}

