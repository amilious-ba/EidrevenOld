using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelGeometry{
	public ModelDescription description;
	public ModelBone[] bones;

	public ModelGeometry(){
		/*this.description = new ModelDescription();
		this.bones = new List<ModelBone>();*/
	}

	public ModelGeometry(ModelDescription description, List<ModelBone>bones){
		this.description = description;
		this.bones = bones.ToArray();
	}
}
