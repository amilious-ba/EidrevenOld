﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour {

    public Canvas hudCanvas;
    public GameObject panel;
    public Text debugText;

    //passed unmodified values
    private static Vector3 lookingAt;
    private static Vector3 playerPosition;

	private static string playerPositionString = "";
	private static string playerLookingAt = "";
	private static bool debug = true;
	private static bool updated = true;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        if(!updated||!debug||debugText == null)return;
        updated = false;
        debugText.text = "Position:\t\t"+playerPositionString;
        debugText.text += "\nLooking at:\t"+playerLookingAt;
    }

    public static void setPlayerPos(Vector3 pos){
        if(playerPosition!=null&&pos==playerPosition)return;
        playerPosition = pos;
    	playerPositionString = "("+getChunkString(pos)+") "+(int)pos.x+", "+(int)pos.y+", "+(int)pos.z;
        updated = true;
    }

    public static void setPlayerLookingAt(Vector3 pos){
        if(lookingAt!=null&&pos==lookingAt)return;
        lookingAt = pos;
    	playerLookingAt = "("+getChunkString(pos)+") "+(int)pos.x+", "+(int)pos.y+", "+(int)pos.z;
        updated = true;
    }

    public static void setPlayerLookingAt(string str){
        if(str.Equals(playerLookingAt))return;
        lookingAt = Vector3.zero;
        playerLookingAt = str;
        updated = true;
    }

    public static void setDebugging(bool debug){
    	debug = debug;
    }

    private static string getChunkString(Vector3 pos){
        int x = (int)(pos.x/Global.ChunkSize);
        int y = (int)(pos.y/Global.ChunkSize);
        int z = (int)(pos.z/Global.ChunkSize);
        int cx = (int)(pos.x%Global.ChunkSize);        
        int cy = (int)(pos.y%Global.ChunkSize);
        int cz = (int)(pos.z%Global.ChunkSize);
        return x+", "+y+", "+z+" ||"+cx+", "+cy+", "+cz;
    }
}