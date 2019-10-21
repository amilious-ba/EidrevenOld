using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModelFile{
	
	public string format_version;
	public ModelGeometry[] model;

	public ModelDescription description;
	public ModelBone[] bones;

	public ModelFile(){
		/*this.description = new ModelDescription();
		this.bones = new List<ModelBone>();*/
	}

	public ModelFile(string format_version, List<ModelGeometry>model){
		this.format_version = format_version;
		this.model = model.ToArray();
	}
}
