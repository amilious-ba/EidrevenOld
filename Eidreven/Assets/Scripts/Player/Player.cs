using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

	private Transform camera = null;
	public Transform displayCube = null;
	private Animator animator = null;

    // Start is called before the first frame update
    void Start(){
    	//get the attached camera
    	camera = this.GetComponentInChildren<Camera>().transform;
    	displayCube = camera.gameObject.transform.GetChild(0);
    	animator = displayCube.gameObject.GetComponentInChildren<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this method is used to get the direction the player is looking
    public Direction LookingDirection{
    	get{
    		return Directions.getLookingDirection(
    			(int)camera.rotation.eulerAngles.x,
    			(int)this.transform.rotation.eulerAngles.y
    		);
    	}
    }
    
}
