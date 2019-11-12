using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeExplosion : MonoBehaviour
{

	public float cubeSize = 0.2f;
	public int cubesInRow = 5;
	public int explosionForce = 50;
	public int explosionRadius = 4;
	public float explosionUpward = 0.4f;

	float cubesPivotDistance;
	Vector3 cubesPivot;

    // Start is called before the first frame update
    void Start(){
        //calcuate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //calculate pivot vector
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
    	if(other.gameObject.name == "Floor"){
    		explode();
    	}
    }

    public void explode(){
    	//make the object disapear
    	gameObject.SetActive(false);

    	//create the sub cubes
    	for(int x=0;x<cubesInRow;x++)for(int y=0;y<cubesInRow;y++)
    		for(int z=0;z<cubesInRow;z++){
    			createPiece(x, y, z);
    	}

    	//get explosion position
    	Vector3 explosionPos = transform.position;
    	//get colliders in that position radius
    	Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
    	//add explosion force to all colliders
    	foreach(Collider hit in colliders){
    		//get rigidbody from collider object
    		Rigidbody rb = hit.GetComponent<Rigidbody>();
    		if(rb != null){
    			//add explosion force to this body
    			rb.AddExplosionForce(explosionForce, transform.position, explosionUpward);
    		}
    	}
    }

    private void createPiece(int x, int y, int z){
    	//create piece
    	GameObject piece;
    	piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

    	//set piece position
    	piece.transform.position = transform.position + new Vector3(cubeSize*x,cubeSize*y,cubeSize*z) - cubesPivot;
    	piece.transform.localScale = new Vector3(cubeSize,cubeSize,cubeSize);

    	//add rigidbody and set mass
    	piece.AddComponent<Rigidbody>();
    	piece.GetComponent<Rigidbody>().mass = 0.2f;
    }
}
