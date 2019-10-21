using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelBone{
	public string name;
	public string parent;
	public float[] pivot;
	public ModelCube[] cubes;

	public ModelBone(){
		/*this.name = "";
		this.parent = "";
		this.pivot = new Vector3(0,0,0);
		this.cubes = new List<ModelCube>();*/
	}

	public ModelBone(string name, string parent, Vector3 pivot, List<ModelCube>cubes){
		this.name = name;
		this.parent = parent;
		this.pivot = new float[]{pivot.x,pivot.y,pivot.z};
		this.cubes = cubes.ToArray();
	}

	public ModelBone(string name, string parent, float pivotX, float pivotY,
		float pivotZ, List<ModelCube>cubes){
		this.name = name;
		this.parent = parent;
		this.pivot = new float[]{pivotX,pivotY,pivotZ};
		if(cubes != null)
		this.cubes = cubes.ToArray();
	}

	public Vector3 Pivot{get{return Utils.convertArrayToVector3(pivot);}}
}
