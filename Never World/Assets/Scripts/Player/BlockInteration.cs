using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockInteration : MonoBehaviour {

	public GameObject cam;
	public Text debugText;
	public GameObject displayCube;
	private Animator animator;
	private Mesh mesh;
	BlockType buildType = BlockType.STONE;

	enum HitType{IN,OUT}

	// Use this for initialization
	void Start () {
		StartCoroutine(updateDisplayCube(BlockType.STONE));
	}

	public IEnumerator updateDisplayCube(BlockType bType){
		Mesh mesh = ((MeshFilter)displayCube.gameObject.GetComponent("MeshFilter")).mesh;		
		Animator animator = (Animator)displayCube.gameObject.GetComponent("Animator");
		if(animator!=null)animator.Play("CubeFastSpin");
		buildType = bType;
		yield return new WaitForSeconds(1);
		Vector2[] uvs = mesh.uv;
		Block block = Block.CreateBlock(bType,new Vector3(0,0,0),null);
		//front
		uvs[0] = block.getBlockUvs(Direction.NORTH,0);
		uvs[1] = block.getBlockUvs(Direction.NORTH,1);
		uvs[2] = block.getBlockUvs(Direction.NORTH,2);
		uvs[3] = block.getBlockUvs(Direction.NORTH,3);
		//top
		uvs[4] = block.getBlockUvs(Direction.UP,0);
		uvs[5] = block.getBlockUvs(Direction.UP,1);
		uvs[8] = block.getBlockUvs(Direction.UP,2);
		uvs[9] = block.getBlockUvs(Direction.UP,3);
		//back
		uvs[6] = block.getBlockUvs(Direction.SOUTH,0);
		uvs[7] = block.getBlockUvs(Direction.SOUTH,1);
		uvs[10] = block.getBlockUvs(Direction.SOUTH,2);
		uvs[11] = block.getBlockUvs(Direction.SOUTH,3);
		//bottom
		uvs[12] = block.getBlockUvs(Direction.DOWN,0);
		uvs[13] = block.getBlockUvs(Direction.DOWN,1);
		uvs[14] = block.getBlockUvs(Direction.DOWN,2);
		uvs[15] = block.getBlockUvs(Direction.DOWN,3);
		//left
		uvs[16] = block.getBlockUvs(Direction.WEST,0);
		uvs[17] = block.getBlockUvs(Direction.WEST,1);
		uvs[18] = block.getBlockUvs(Direction.WEST,2);
		uvs[19] = block.getBlockUvs(Direction.WEST,3);
		//right
		uvs[23] = block.getBlockUvs(Direction.EAST,0);
		uvs[20] = block.getBlockUvs(Direction.EAST,1);
		uvs[21] = block.getBlockUvs(Direction.EAST,2);
		uvs[22] = block.getBlockUvs(Direction.EAST,3);
		//add mesh
		mesh.uv = uvs;
		block = null;
	}
	
	// Update is called once per frame
	void Update () {
		DebugScript.setPlayerPos(this.transform.position);
		RaycastHit looking;
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out looking,10)){
			Vector3 lookBlock = looking.point - looking.normal/2.0f;

				int lX = (int)(Mathf.Round(lookBlock.x)-looking.collider.gameObject.transform.position.x);
				int lY = (int)(Mathf.Round(lookBlock.y)-looking.collider.gameObject.transform.position.y);
				int lZ = (int)(Mathf.Round(lookBlock.z)-looking.collider.gameObject.transform.position.z);
				DebugScript.setPlayerLookingAt(lookBlock);
				//debugText.text = "looking at x:"+Mathf.Round(lookBlock.x)+" y:"+Mathf.Round(lookBlock.y)+" z:"+Mathf.Round(lookBlock.z);
				Vector3 chunkPos = new Vector3(Mathf.Round(lookBlock.x/Global.ChunkSize),Mathf.Round(lookBlock.y/Global.ChunkSize),Mathf.Round(lookBlock.z/Global.ChunkSize));
				//debugText.text=debugText.text+"\nChunk Name: "+Chunk.BuildName(chunkPos);
		}else{
			DebugScript.setPlayerLookingAt("-");
			//debugText.text = "";
		}

		if(Input.GetKeyDown("1"))
			StartCoroutine(updateDisplayCube(BlockType.SAND));
		if(Input.GetKeyDown("2"))
			StartCoroutine(updateDisplayCube(BlockType.STONE));
		if(Input.GetKeyDown("3"))
			StartCoroutine(updateDisplayCube(BlockType.DIAMONDORE));
		if(Input.GetKeyDown("4"))
			StartCoroutine(updateDisplayCube(BlockType.REDSTONEORE));
		if(Input.GetKeyDown("5"))
			StartCoroutine(updateDisplayCube(BlockType.WATER));
		if(Input.GetKeyDown("6")){			
			StartCoroutine(updateDisplayCube(BlockType.GRASS));
		}

		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10)){
				Vector3 hitBlock = hit.point - hit.normal/2.0f;
				Block block = World.getBlock(hitBlock);
				if(block!=null){
					block.HitBlock();
					Debug.Log(block.chunk.position);
				}
			}
		}
		if(Input.GetMouseButtonDown(1)){
			RaycastHit hit2;
			if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2, 10)){
				Vector3 hitBlock2 = hit2.point + hit2.normal/2.0f;			
				Block block = World.getBlock(hitBlock2);
				//do not build block if standing on it
					
				int px = (int)this.transform.position.x;
				int py = (int)this.transform.position.y;
				int pz = (int)this.transform.position.z;
				//first check y
								
				if(block.worldPosition.y>py-1&&((int)block.worldPosition.x)==px&&((int)block.worldPosition.z)==pz){
					return;
				}
				if(block!=null){
					block.setBlock(buildType);
				}
			}
		}

	}

	private bool canBuild(){
		return false;
	}
}
