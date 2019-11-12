using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour{
    
	public Renderer textureRenderer;

	public void DrawNoiseMap(float[,] noiseMap, Color a, Color b){
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);
		Color[] colorMap = new Color[width*height];
		for(int y=0; y<height;y++)for(int x=0;x<width;x++){
			colorMap[y*width+x] = Color.Lerp(a, b, noiseMap[x,y]);			
		}
		DrawColorMap(colorMap, width);
	}

	public void DrawColorMap(Color[] colorMap,int chunkSize){
		int width = chunkSize;
		int height = chunkSize;

		Texture2D texture = new Texture2D(width,height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();
		textureRenderer.sharedMaterial.mainTexture = texture;
	}

}
