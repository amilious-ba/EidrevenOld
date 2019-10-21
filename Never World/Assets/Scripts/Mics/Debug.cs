using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuger : MonoBehaviour {

	private static string playerPosition = "";
	private static string playerLookingAt = "";
	private static bool debug = true;
	private static bool updated = true;

	public static Canvas hudCanvas;
	public static GameObject panel;
	public static Text debugText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        if(!updated||!debug||debugText == null)return;
        updated = false;
        debugText.text = "Position: "+playerPosition;
        debugText.text += "\n Looking at: "+playerLookingAt;
    }

    public static void setPlayerPos(Vector3 pos){
    	updated = true;
    	playerPosition = (int)pos.x+", "+(int)pos.y+", "+(int)pos.z;
    }

    public static void setPlayerLookingAt(Vector3 pos){
    	updated = true;
    	playerLookingAt = (int)pos.x+", "+(int)pos.y+", "+(int)pos.z;
    }

    public static void setPlayerPos(string pos){
    	updated = true;
    	playerPosition = pos;
    }

    public static void setPlayerLookingAt(string pos){
    	updated = true;
    	playerLookingAt = pos;
    }

    public static void setDebugging(bool debug){
    	if(debug!=Debuger.debug)return;
    	Debuger.debug = debug;
    	panel.gameObject.SetActive(debug);
    }
}
