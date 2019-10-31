using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjectLoader : MonoBehaviour{

	[SerializeField] ModelGeometry model;

	public string blueprintPath;
	public string skinPath;
	public string animationPath;
	public bool loadedInUnity = false;

	private Dictionary<string, GameObject> objects;

    // Start is called before the first frame update
    void Start(){
    	
    	
    }

    private void OnValidate(){
    	if(loadedInUnity)return;
    	if(objects!=null){
    		foreach(KeyValuePair<string, GameObject> entry in objects){
    			StartCoroutine(DestroyGO(entry.Value));
			}
			objects.Clear();
    	}
    	load();
    	loadedInUnity = true;
    }

    IEnumerator DestroyGO(GameObject obj){
    	yield return new WaitForEndOfFrame();
    	DestroyImmediate(obj);
    }

    public void load(){
    	//check to see if all the required assests exist    	
    	if(blueprintPath==null||
    		!File.Exists(Application.streamingAssetsPath + "/"+blueprintPath))return;
    	//create the material
    	Material loadedMaterial;
    	if(createMaterial(Application.streamingAssetsPath + "/"+skinPath,
    	 Shader.Find("Standard"), out loadedMaterial)){

    	}
    	
    	//build the moddel
    	loadModel(blueprintPath,out model);
    	buildModel(model,loadedMaterial);
    	//load animations
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public bool createMaterial(string path, Shader shader, out Material material){
    	Material loadedMaterial = new Material(shader);
    	//check to see if the file exists
    	if(!File.Exists(path)){
    		material = null; return false;
    	}
    	var bytes = System.IO.File.ReadAllBytes(path);
    	var texture = new Texture2D(1,1);
    	texture.filterMode = FilterMode.Point;
    	texture.LoadImage(bytes);
    	loadedMaterial.mainTexture = texture;
    	//need to modify the material here
    	loadedMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
 		loadedMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
    	//return the material
    	material = loadedMaterial;return true;
    }

    /**
     * This method is used to load and decode a model blueprint from a 
     * json file.
     * @param  string path          path to the jason file.
     * @param  out    ModelGeometry model         Will be set to the 
     * loaded ModelGeometry
     * @return bool        Returns true if the object was loaded,
     * otherwise returns false.
     */
    public bool loadModel(string path, out ModelGeometry model){
    	string jsonString = File.ReadAllText(Application.streamingAssetsPath + "/"+path);
    	jsonString = jsonString.Replace("minecraft:geometry", "model");
    	ModelBlueprint modelBlueprint = JsonUtility.FromJson<ModelBlueprint>(jsonString);
    	if(modelBlueprint.model!=null){
    		model = modelBlueprint.model[0];
    		return true;
    	}else{
    		model = null;
    		return false;
    	} 
    }

    /**
     * When called this method builds a model from the passed
     * modelGeometry.
     * @param  ModelGeometry model         The blueprint of the
     * model.
     */
    public void buildModel(ModelGeometry model, Material material){
		objects  = new Dictionary<string,GameObject>();
    	//create the cubes
    	for(int boneId=0;boneId<model.bones.Length;boneId++){
    		GameObject bone = createBone(model.bones[boneId],model.description.ImageSize,material);
    		objects.Add(model.bones[boneId].name, bone);
    	}
    	for(int boneId=0;boneId<model.bones.Length;boneId++){
    		BoneBlueprint bone = model.bones[boneId];
    		if(bone.name!=null&&bone.parent!=null&&bone.name!=""&&bone.parent!=""&&
    			objects[bone.name]!=null&&objects[bone.parent]!=null){
    			objects[bone.name].transform.parent = objects[bone.parent].transform;
    		}
    	}
    }

    /**
     * This method is called to create a bone for the model.
     * @param  ModelBone modelBone     The blueprint for the
     * bone.
     * @param  Vector2   imageSize     Contains the with (x) and
     * the height (y) of the objects skin file.
     */
    public GameObject createBone(BoneBlueprint boneBlueprint,Vector2 imageSize, Material material){
    	GameObject bone = new GameObject();
    	bone.name = boneBlueprint.name;
    	bone.transform.position = boneBlueprint.Pivot;
    	bone.transform.parent = this.transform;
    	//check if the bone contains cubes
    	if(boneBlueprint.cubes == null||boneBlueprint.cubes.Length == 0)return bone;
    	//the bone has cubes so create them
    	for(int i=0;i<boneBlueprint.cubes.Length;i++){
    		createCube("cube_"+i,bone,boneBlueprint.cubes[i].Origin, boneBlueprint.cubes[i].Size,
    			boneBlueprint.cubes[i].UV,imageSize,material);
    	}
    	return bone;
    }

    private void createCube(string name, GameObject parent, Vector3 origin, Vector3 size, Vector2 uv, Vector2 imageSize, Material material){
    	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    	cube.gameObject.name = name;
    	cube.gameObject.transform.parent = parent.transform;
    	//set the material
    	cube.GetComponent<Renderer>().material = material;
    	//set cube position
    	cube.transform.localScale = size;
    	cube.transform.position = this.transform.position + origin + new Vector3(size.x/2,size.y/2,size.z/2);
    	//set the uvs for the material
    	Mesh mesh;
    	#if UNITY_EDITOR
    		MeshFilter mf = cube.GetComponent<MeshFilter>();
    		Mesh meshCopy = Mesh.Instantiate(mf.sharedMesh) as Mesh;
    		mesh = mf.mesh = meshCopy;
    	#else
    		mesh = cube.GetComponent<MeshFilter>().mesh as Mesh;
    	#endif 
    	mesh.uv = mapUvs(uv, size, imageSize);
    }

    private Vector2[] mapUvs(Vector2 muv, Vector3 size, Vector2 imageSize){
    	Vector2[] uvs = new Vector2[24];
    	//front
    	uvs[10] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[6] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y));
    	uvs[7] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y)); 	
    	uvs[11] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	//back
    	uvs[3] = new Vector2((muv.x+size.z+size.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[1] = new Vector2((muv.x+size.z+size.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y));
    	uvs[0] = new Vector2((muv.x+size.z+size.x+size.z)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y)); 	
    	uvs[2] = new Vector2((muv.x+size.z+size.x+size.z)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	//left
    	uvs[18] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[19] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y));
    	uvs[16] = new Vector2((muv.x)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y)); 	
    	uvs[17] = new Vector2((muv.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));    	    	
    	//right
    	uvs[22] = new Vector2((muv.x+size.z+size.x+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[23] = new Vector2((muv.x+size.z+size.x+size.x)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y));
    	uvs[20] = new Vector2((muv.x+size.x+size.x)/imageSize.x,1-((muv.y+size.z+size.y)/imageSize.y)); 	
    	uvs[21] = new Vector2((muv.x+size.x+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	//top
    	uvs[8] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y)/imageSize.y));
    	uvs[4] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[5] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y+size.z)/imageSize.y)); 	
    	uvs[9] = new Vector2((muv.x+size.z)/imageSize.x,1-((muv.y)/imageSize.y));
    	//bottom
    	uvs[13] = new Vector2((muv.x+size.z+size.x+size.x)/imageSize.x,1-((muv.y)/imageSize.y));
    	uvs[12] = new Vector2((muv.x+size.z+size.x+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y));
    	uvs[15] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y+size.z)/imageSize.y)); 	
    	uvs[14] = new Vector2((muv.x+size.z+size.x)/imageSize.x,1-((muv.y)/imageSize.y));
    	return uvs;
    }
}
