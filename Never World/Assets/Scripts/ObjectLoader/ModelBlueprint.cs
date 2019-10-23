using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelBlueprint{
	
	public string format_version;
	public ModelGeometry[] model;

	public ModelDescription description;
	public BoneBlueprint[] bones;

	public ModelBlueprint(){}

	public ModelBlueprint(string format_version, List<ModelGeometry>model){
		this.format_version = format_version;
		this.model = model.ToArray();
	}
}

[System.Serializable]
public class ModelGeometry{
	public ModelDescription description;
	public BoneBlueprint[] bones;

	public ModelGeometry(){}

	public ModelGeometry(ModelDescription description, List<BoneBlueprint>bones){
		this.description = description;
		this.bones = bones.ToArray();
	}
}

[System.Serializable]
public class ModelDescription{
	public string identifier;
	public int texture_width;
	public int texture_height;
	public int visible_bounds_width;
	public int visible_bounds_height;
	public float[] visible_bounds_offset;

	public ModelDescription(){}

	public ModelDescription(string identifier, int texture_width,
		int texture_height, int visible_bounds_width, int visible_bounds_height,
		Vector3 visible_bounds_offset){
		this.identifier = identifier;
		this.texture_width = texture_width;
		this.texture_height = texture_height;
		this.visible_bounds_width = visible_bounds_width;
		this.visible_bounds_height = visible_bounds_height;
		this.visible_bounds_offset = new float[]{visible_bounds_offset.x,
			visible_bounds_offset.y,visible_bounds_offset.z};
	}

	public Vector3 VisibleBoundsOffset{
		get{
			return Utils.convertArrayToVector3(visible_bounds_offset);
		}
	}

	public Vector2 ImageSize{
		get{return new Vector2(texture_width,texture_height);}
	}
}

[System.Serializable]
public class BoneBlueprint{
	public string name;
	public string parent;
	public float[] pivot;
	public CubeBlueprint[] cubes;

	public BoneBlueprint(){}

	public BoneBlueprint(string name, string parent, Vector3 pivot, List<CubeBlueprint>cubes){
		this.name = name;
		this.parent = parent;
		this.pivot = new float[]{pivot.x,pivot.y,pivot.z};
		this.cubes = cubes.ToArray();
	}

	public BoneBlueprint(string name, string parent, float pivotX, float pivotY,
		float pivotZ, List<CubeBlueprint>cubes){
		this.name = name;
		this.parent = parent;
		this.pivot = new float[]{pivotX,pivotY,pivotZ};
		if(cubes != null)
		this.cubes = cubes.ToArray();
	}

	public Vector3 Pivot{get{return Utils.convertArrayToVector3(pivot);}}
}

[System.Serializable]
public class CubeBlueprint{
	public float[] origin;
	public float[] size;
	public float[] uv;

	public CubeBlueprint(){}

	public CubeBlueprint(Vector3 origin, Vector3 size, Vector2 uv){
		this.origin = new float[] {origin.x,origin.y,origin.z};
		this.size = new float[] {size.x,size.y,size.z};
		this.uv = new float[]{uv.x,uv.y};
	}

	public CubeBlueprint(float originX, float originY, float originZ,
		float sizeX, float sizeY, float sizeZ, float uvX, float uvY){
		this.origin = new float[]{originX,originY,originZ};
		this.size = new float[]{sizeX,sizeY,sizeZ};
		this.uv = new float[]{uvX,uvY};
	}

	public Vector3 Origin {
		get{return Utils.convertArrayToVector3(origin);}
	}

	public Vector3 Size {
		get{return Utils.convertArrayToVector3(size);}
	}

	public Vector2 UV {
		get{return Utils.convertArrayToVector2(uv);}
	}
}