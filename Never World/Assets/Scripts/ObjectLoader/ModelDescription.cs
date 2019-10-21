using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelDescription{
	public string identifier;
	public int texture_width;
	public int texture_height;
	public int visible_bounds_width;
	public int visible_bounds_height;
	public float[] visible_bounds_offset;

	public ModelDescription(){
		/*this.identifier = "";
		this.textureWidth = 0;
		this.textureHeight = 0;
		this.visibleBoundsWidth = 0;
		this.visibleBoundsHeight = 0;
		this.visibleBoundsOffset = new Vector3(0,0,0);*/
	}

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
