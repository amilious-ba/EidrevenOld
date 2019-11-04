using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebugScript : MonoBehaviour {

    public Canvas hudCanvas;
    public GameObject panel;
    public Text debugText;

    //passed unmodified values
    private static Vector3 lookingAt;
    private static Vector3 playerPosition;

	private static string playerPositionString = "";
	private static string playerLookingAt = "";
    private static string timerValue = "";
	private static bool debug = true;
	private static bool updated = true;
    private static bool timer = false;
    private static DateTime startTime;
    private static int loadedChunks=0;
    private static int counter=0;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        counter++;if(counter<100){return;}counter = 0;
        if(!updated||!debug||debugText == null)return;
        updated = false;
        debugText.text = "Position:\t\t"+playerPositionString;
        debugText.text += "\nLooking at:\t"+playerLookingAt;
        debugText.text += "\nLoaded Chunks:\t"+loadedChunks;
        debugText.text += "\n"+timerValue;
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

    public static void setLoadedChunks(int lc){
        if(lc==loadedChunks)return;
        loadedChunks = lc;
        updated=true;
    }

    public static void setDebugging(bool debug){
    	debug = debug;
    }

    public static void startTimer(){
        timer = true;
        startTime = DateTime.Now;
    }

    public static void stopTimer(string name){
        if(!timer)return;
        TimeSpan elapsedTime = DateTime.Now - startTime;
        timerValue = name+": "+elapsedTime.Ticks;
        timer = false;
        updated = true;
    }

    public static void stopTimer(){
        if(!timer)return;
        TimeSpan elapsedTime = DateTime.Now - startTime;
        timerValue = "Timer:\t\t\t"+elapsedTime.Ticks;
        timer = false;
        updated = true;
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
