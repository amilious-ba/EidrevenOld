using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB : MonoBehaviour {

	Chunk chunk;
	public ChunkMB(){}
	public void SetOwner(Chunk chunk){
		this.chunk = chunk;
		InvokeRepeating("SaveProgress",10,1000);
	}

	public IEnumerator HealBlock(GamePoint index){
		yield return new WaitForSeconds(3);
		if(chunk.getBlock(index).bType != BlockType.AIR)
			chunk.getBlock(index).Reset();
	}

	public IEnumerator Flow(Block b, BlockType bt, int strength, int maxsize){
		//reduce strenght of the fluid block
		/*if(maxsize<=0)yield break;
		if(b==null)yield break;
		if(strength<=0)yield break;
		b.SetType(bt);
		b.currentHealth = strength;
		b.chunk.Redraw();
		yield return new WaitForSeconds(1);

		//flow down if air block beneath
		Block below = b.GetBlock(Direction.DOWN);
		if(below != null && below.bType == BlockType.AIR){
			StartCoroutine(Flow(below, bt, strength,--maxsize));
			yield break;
		}else{*/
			/*--strength;
			--maxsize;
			StartCoroutine(Flow(b.GetBlock(Direction.NORTH),bt,strength,maxsize));
			yield return new WaitForSeconds(1);
			StartCoroutine(Flow(b.GetBlock(Direction.SOUTH),bt,strength,maxsize));
			yield return new WaitForSeconds(1);
			StartCoroutine(Flow(b.GetBlock(Direction.EAST),bt,strength,maxsize));
			yield return new WaitForSeconds(1);
			StartCoroutine(Flow(b.GetBlock(Direction.WEST),bt,strength,maxsize));
			yield return new WaitForSeconds(1);*/
		//}
		yield return null;
	}

	/*public IEnumerator Drop(){
		
	}*/

	void SaveProgress(){
		if(chunk.Changed){
			chunk.Save();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
