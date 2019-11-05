using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        generateCirleMethed1(new Vector3(-7,0,0),7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateCirleMethed1(Vector3 center, int radius){
    	GameObject loadedChunks = new GameObject("Loaded Chunks");
    	radius++;
    	int rr = radius*radius;
    	for(int x=-radius;x<=radius;x++){
    		int xx = x*x;
    		for(int y=-radius;y<=radius;y++){
    			int yy = y*y;
    			for(int z=-radius;z<=radius;z++){
    				if(xx+yy+z*z>=rr)continue;    				
    				//draw the cube
    				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    				cube.transform.position = new Vector3(center.x+x,center.y+y,center.z+z);
    				cube.transform.parent = loadedChunks.transform;
    			}
    		}
		}
    }

   /* public void generateCircleMethod2(Vector3 center, int radius){

    }

    public void generateCircleMethod2b(Vector3 center, int rr, int x, int xx, int y, int yy, int z, int zz){
    	if(xx+yy+zz>=rr)return;
    	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    	cube.transform.position = new Vector3(center.x+x,center.y+y,center.z+z);
    	//pause and move in directions
    	generateCircleMethod2b(center,rr,x,xx,yy,z,zz);
    	generateCircleMethod2b(center,rr,x,xx,yy,z,zz);
    	generateCircleMethod2b(center,rr,x,xx,yy,z,zz);
    	generateCircleMethod2b(center,rr,x,xx,yy,z,zz);
    }*/

    public bool isInRange(Vector3 position1, Vector3 position2, int radius){
    	radius*=radius;
    	int dxdx = (int)Mathf.Abs(position1.x-position2.x);dxdx*=dxdx;
    	int dydy = (int)Mathf.Abs(position1.y-position2.y);dydy*=dydy;
    	int dzdz = (int)Mathf.Abs(position1.z-position2.z);dzdz*=dzdz;
    	if(dxdx+dydy+dzdz>=radius)return false;
    	return true;
    }

    public int countChunks(int radius){
    	radius++;int count=0;
    	int rr = radius*radius;
    	for(int x=-radius;x<=radius;x++){
    		int xx = x*x;
    		for(int y=-radius;y<=radius;y++){
    			int yy = y*y;
    			for(int z=-radius;z<=radius;z++){
    				int zz=z*z;
    				if(xx+yy+zz>=rr)continue;
    				count++;
    			}
    		}
		}return count;
    }



}
