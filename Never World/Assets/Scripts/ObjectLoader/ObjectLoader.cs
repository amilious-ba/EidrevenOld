using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjectLoader : MonoBehaviour{

	[SerializeField] ModelGeometry model;
	private Dictionary<string, GameObject> objects;

	public string path;
	public Material material;
	private string jsonString;

    // Start is called before the first frame update
    void Start(){
    	objects = new Dictionary<string,GameObject>();
    	if(path==null)return;
    	jsonString = File.ReadAllText(Application.streamingAssetsPath + "/"+path);
    	jsonString = jsonString.Replace("minecraft:geometry", "model");
    	ModelFile modelFile = JsonUtility.FromJson<ModelFile>(jsonString);
    	if(modelFile.model!=null)
    	buildModel(modelFile.model[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate(){

    }

    public void buildModel(ModelGeometry model){
    	//create the cubes
    	for(int boneId=0;boneId<model.bones.Length;boneId++){
    		createBone(model.bones[boneId],model.description.ImageSize);
    	}
    	for(int boneId=0;boneId<model.bones.Length;boneId++){
    		ModelBone bone = model.bones[boneId];
    		if(bone.name!=null&&bone.parent!=null&&bone.name!=""&&bone.parent!=""){
    			objects[bone.name].transform.parent = objects[bone.parent].transform;
    		}
    	}
    }

    public void createBone(ModelBone modelBone,Vector2 imageSize){
    	GameObject bone = new GameObject();
    	this.objects.Add(modelBone.name,bone);
    	bone.name = modelBone.name;
    	bone.transform.position = modelBone.Pivot;
    	bone.transform.parent = this.transform;
    	//check if the bone contains cubes
    	if(modelBone.cubes == null||modelBone.cubes.Length == 0)return;
    	//the bone has cubes so create them
    	for(int i=0;i<modelBone.cubes.Length;i++){
    		createCube("cube_"+i,bone,modelBone.cubes[i].Origin, modelBone.cubes[i].Size,
    			modelBone.cubes[i].UV,imageSize);
    	}
    }

    private void createCube(string name, GameObject parent, Vector3 origin, Vector3 size, Vector2 uv, Vector2 imageSize){
    	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    	cube.gameObject.name = name;
    	cube.gameObject.transform.parent = parent.transform;
    	//set the material
    	cube.GetComponent<Renderer>().material = this.material;
    	//set cube position
    	cube.transform.localScale = size;
    	cube.transform.position = this.transform.position + origin + new Vector3(size.x/2,size.y/2,size.z/2);
    	//set the uvs for the material
    	Mesh mesh = cube.GetComponent<MeshFilter>().mesh as Mesh;
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

    //this method would be replaced by the code that will
    //load the values from a json file
    public ModelGeometry loadModel(){
    	//set up object
    	ModelDescription description = new ModelDescription("geomery.unknown",128,128,3,3,new Vector3(0,1.5f,0));
    	List<ModelBone>bones = new List<ModelBone>();
    	//create root bone
    	ModelBone root = new ModelBone("root","",3,17,-4,null);
    	bones.Add(root);
    	//create upper body
    	ModelCube ub1 = new ModelCube(-3,40,0,6,4,4,56,11);
    	ModelCube ub2 = new ModelCube(-8,31,-1,16,9,6,0,20);
    	ModelCube ub3 = new ModelCube(-6,26,-1, 12,5,6,36,36);
    	List<ModelCube>ub_cubes = new List<ModelCube>();
    	ub_cubes.Add(ub1);ub_cubes.Add(ub2);ub_cubes.Add(ub3);
    	ModelBone upperBody = new ModelBone("Upper_Body","root",0,26,2,ub_cubes);
    	bones.Add(upperBody);
    	//create head
    	ModelCube h1 = new ModelCube(-5,42,-3,10,10,10,0,0);
    	List<ModelCube>h_cubes = new List<ModelCube>();
    	h_cubes.Add(h1);
    	ModelBone head = new ModelBone("Head","Upper_Body",0,42,2,h_cubes);
    	bones.Add(head);
    	//create Arm_Left
    	ModelCube al1 = new ModelCube(7,29,0,4,9,4,16,61);
    	ModelCube al2 = new ModelCube(7,19,0,4,10,4,36,60);
    	//build object and return
    	ModelGeometry model = new ModelGeometry(description,bones);
    	return model;
    }
}
