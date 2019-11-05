using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GamePoint{
    public int x, y, z;
    
    public GamePoint(int x, int y, int z){
    	this.x=x;
    	this.y=y;
    	this.z=z;
    }

    public GamePoint(int v){
    	this.x=v;
    	this.y=v;
    	this.z=v;
    }

    public GamePoint(Vector3 vector3){
    	this.x = (int)vector3.x;
    	this.y = (int)vector3.y;
    	this.z = (int)vector3.z;
    }

    public static GamePoint roundVector3(Vector3 vector3){
    	return new GamePoint((int)Mathf.Round(vector3.x),(int)Mathf.Round(vector3.y),(int)Mathf.Round(vector3.y));
    }

    public GamePoint moveDirection(Direction direction, int distance){
    	switch(direction){
			case Direction.NORTH: return this + new GamePoint(0,0,distance);
			case Direction.SOUTH: return this + new GamePoint(0,0,-distance);
			case Direction.EAST: return this + new GamePoint(distance,0,0);
			case Direction.WEST: return this + new GamePoint(-distance,0,0);
			case Direction.UP: return this + new GamePoint(0,distance,0);
			case Direction.DOWN: return this + new GamePoint(0,-distance,0);
			case Direction.NORTH_EAST: return this + new GamePoint(distance,0,distance);
			case Direction.NORTH_WEST: return this + new GamePoint(-distance,0,distance);
			case Direction.SOUTH_EAST: return this + new GamePoint(distance,0,-distance);
			case Direction.SOUTH_WEST: return this + new GamePoint(-distance,0,-distance);
			case Direction.UP_NORTH: return this + new GamePoint(0,distance,distance);
			case Direction.UP_SOUTH: return this + new GamePoint(0,distance,-distance);
			case Direction.UP_EAST:  return this + new GamePoint(distance,distance,0);
			case Direction.UP_WEST:  return this + new GamePoint(-distance,distance,0);
			case Direction.DOWN_NORTH: return this + new GamePoint(0,-distance,distance);
			case Direction.DOWN_SOUTH: return this + new GamePoint(0,-distance,-distance);
			case Direction.DOWN_EAST:  return this + new GamePoint(distance,-distance,0);
			case Direction.DOWN_WEST:  return this + new GamePoint(-distance,-distance,0);
			case Direction.UP_NORTH_EAST: return this + new GamePoint(distance,distance,distance);
			case Direction.UP_NORTH_WEST: return this + new GamePoint(-distance,distance,distance);
			case Direction.UP_SOUTH_EAST: return this + new GamePoint(distance,distance,-distance);
			case Direction.UP_SOUTH_WEST: return this + new GamePoint(-distance,distance,-distance);
			case Direction.DOWN_NORTH_EAST: return this + new GamePoint(distance,-distance,distance);
			case Direction.DOWN_NORTH_WEST: return this + new GamePoint(-distance,-distance,distance);
			case Direction.DOWN_SOUTH_EAST: return this + new GamePoint(distance,-distance,-distance);
			case Direction.DOWN_SOUTH_WEST: return this + new GamePoint(-distance,-distance,-distance);
			default: return this;
		}
    }

    public static GamePoint operator +(GamePoint a, GamePoint b){
    	return new GamePoint(a.x+b.x,a.y+b.y,a.z+b.z);
    }

    public static GamePoint operator +(GamePoint a, int b){
    	return new GamePoint(a.x+b,a.y+b,a.z+b);
    }

    public static GamePoint operator +(GamePoint a, float b){
    	return new GamePoint(a.x+(int)b,a.y+(int)b,a.z+(int)b);
    }

    public static GamePoint operator -(GamePoint a, GamePoint b){
    	return new GamePoint(a.x-b.x,a.y-b.y,a.z-b.z);
    }

    public static GamePoint operator -(GamePoint a, int b){
    	return new GamePoint(a.x-b,a.y-b,a.z-b);
    }

    public static GamePoint operator -(GamePoint a, float b){
    	return new GamePoint(a.x-(int)b,a.y-(int)b,a.z-(int)b);
    }

    public static GamePoint operator *(GamePoint a, GamePoint b){
    	return new GamePoint(a.x*b.x,a.y*b.y,a.z*b.z);
    }

    public static GamePoint operator *(GamePoint a, int b){
    	return new GamePoint(a.x*b,a.y*b,a.z*b);
    }

    public static GamePoint operator *(GamePoint a, float b){
    	return new GamePoint(a.x*(int)b,a.y*(int)b,a.z*(int)b);
    }

    public static GamePoint operator /(GamePoint a, GamePoint b){
    	return new GamePoint(a.x/b.x,a.y/b.y,a.z/b.z);
    }

    public static GamePoint operator /(GamePoint a, int b){
    	return new GamePoint(a.x/b,a.y/b,a.z/b);
    }

    public static GamePoint operator /(GamePoint a, float b){
    	return new GamePoint(a.x/(int)b,a.y/(int)b,a.z/(int)b);
    }    

    public static GamePoint operator %(GamePoint a, GamePoint b){
    	return new GamePoint(a.x%b.x,a.y%b.y,a.z%b.z);
    }

    public static GamePoint operator %(GamePoint a, int b){
    	return new GamePoint(a.x%b,a.y%b,a.z%b);
    }

    public static GamePoint operator %(GamePoint a, float b){
    	return new GamePoint(a.x%(int)b,a.y%(int)b,a.z%(int)b);
    }

    public static bool operator !=(GamePoint a, GamePoint b){
        if(a.x!=b.x||a.y!=b.y||a.z!=b.z)return false;
        return true;
    }

    public static bool operator ==(GamePoint a, GamePoint b){
        if(a.x==b.x&&a.y==b.y&&a.z==b.z)return true;
        return false;
    }

    public static implicit operator Vector3(GamePoint a) => new Vector3(a.x,a.y,a.z);
    public static implicit operator GamePoint(Vector3 a) => new GamePoint((int)a.x,(int)a.y,(int)a.z);

}
