using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelCube{
	public float[] origin;
	public float[] size;
	public float[] uv;

	public ModelCube(){
		/*this.origin = new Vector3(0,0,0);
		this.size = new Vector3(0,0,0);
		this.uv = new Vector2(0,0);*/
	}

	public ModelCube(Vector3 origin, Vector3 size, Vector2 uv){
		this.origin = new float[] {origin.x,origin.y,origin.z};
		this.size = new float[] {size.x,size.y,size.z};
		this.uv = new float[]{uv.x,uv.y};
	}

	public ModelCube(float originX, float originY, float originZ,
		float sizeX, float sizeY, float sizeZ, float uvX, float uvY){
		this.origin = new float[]{originX,originY,originZ};
		this.size = new float[]{sizeX,sizeY,sizeZ};
		this.uv = new float[]{uvX,uvY};
	}

	public Vector3 Origin {
		get{
			return Utils.convertArrayToVector3(origin);
		}
	}

	public Vector3 Size {
		get{
			return Utils.convertArrayToVector3(size);
		}
	}

	public Vector2 UV {
		get{
			return Utils.convertArrayToVector2(uv);
		}
	}
}
