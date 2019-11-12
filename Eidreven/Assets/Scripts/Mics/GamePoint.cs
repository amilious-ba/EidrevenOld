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
    	return new GamePoint((int)Mathf.Round(vector3.x),(int)Mathf.Round(vector3.y),(int)Mathf.Round(vector3.z));
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

    public static GamePoint getParentIndex(Vector3 pos, int parentSize){
        GamePoint fixedPos = GamePoint.roundVector3(pos); GamePoint parentIndex = fixedPos;
        if(parentIndex.x<0)parentIndex.x = parentIndex.x-parentSize+1;
        if(parentIndex.y<0)parentIndex.y = parentIndex.y-parentSize+1;
        if(parentIndex.z<0)parentIndex.z = parentIndex.z-parentSize+1;
        parentIndex/=parentSize;
        return parentIndex;
    }

    public static GamePoint getParentIndex(GamePoint pos, int parentSize){
        if(pos.x<0)pos.x = pos.x-parentSize+1;
        if(pos.y<0)pos.y = pos.y-parentSize+1;
        if(pos.z<0)pos.z = pos.z-parentSize+1;
        pos/=parentSize; return pos;
    }

    public GamePoint getParentIndex(int parentSize){
        return getParentIndex(this,parentSize);
    }

    public static GamePoint getIndexInParent(Vector3 pos, int parentSize){
        GamePoint parentPos = GamePoint.getParentIndex(pos,parentSize);
        parentPos*=parentSize;
        GamePoint fixedPos = GamePoint.roundVector3(pos);
        fixedPos = parentPos - fixedPos;
        fixedPos.x = Mathf.Abs(fixedPos.x);        
        fixedPos.y = Mathf.Abs(fixedPos.y);
        fixedPos.z = Mathf.Abs(fixedPos.z);
        return fixedPos;
    }

    public GamePoint getIndexInParent(int parentSize){
        return GamePoint.getIndexInParent(this, parentSize);
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

    public static bool operator <(GamePoint a, int b){
        if(a.x<b&&a.y<b&&a.z<b)return true;
        return false;
    }

    public static bool operator <(GamePoint a, float b){
        if(a.x<(int)b&&a.y<(int)b&&a.z<(int)b)return true;
        return false;
    }

    public static bool operator <=(GamePoint a, int b){
        if(a.x<=b&&a.y<=b&&a.z<=b)return true;
        return false;
    }

    public static bool operator <=(GamePoint a, float b){
        if(a.x<=(int)b&&a.y<=(int)b&&a.z<=(int)b)return true;
        return false;
    }

    public static bool operator >(GamePoint a, int b){
        if(a.x>b&&a.y>b&&a.z>b)return true;
        return false;
    }

    public static bool operator >(GamePoint a, float b){
        if(a.x>(int)b&&a.y>(int)b&&a.z>(int)b)return true;
        return false;
    }

    public static bool operator >=(GamePoint a, int b){
        if(a.x>=b&&a.y>=b&&a.z>=b)return true;
        return false;
    }

    public static bool operator >=(GamePoint a, float b){
        if(a.x>=(int)b&&a.y>=(int)b&&a.z>=(int)b)return true;
        return false;
    }

    public static bool operator ==(GamePoint a, int b){
        if(a.x==b&&a.y==b&&a.z==b)return true;
        return false;
    }

    public static bool operator ==(GamePoint a, float b){
        if(a.x==(int)b&&a.y==(int)b&&a.z==(int)b)return true;
        return false;
    }

    public static bool operator !=(GamePoint a, int b){
        if(a.x!=b&&a.y!=b&&a.z!=b)return true;
        return false;
    }

    public static bool operator !=(GamePoint a, float b){
        if(a.x!=(int)b&&a.y!=(int)b&&a.z!=(int)b)return true;
        return false;
    }

    public static bool operator !=(GamePoint a, GamePoint b){
        if(a.x!=b.x||a.y!=b.y||a.z!=b.z)return false;
        return true;
    }

    public static bool operator ==(GamePoint a, GamePoint b){
        if(a.x==b.x&&a.y==b.y&&a.z==b.z)return true;
        return false;
    }

    public override string ToString(){
        return x +", "+y+", "+z;
    }

    public override bool Equals(object obj){
        //Check for null and compare run-time types.
        if((obj==null)||!this.GetType().Equals(obj.GetType()))return false;      
        else { 
            GamePoint p = (GamePoint) obj; 
            return (x == p.x) && (y == p.y) && (z == p.z);
        } 
    }

    public override int GetHashCode(){
        return base.GetHashCode();
    }

    public static implicit operator string(GamePoint a) => a.x +", "+a.y+", "+a.z;
    public static implicit operator Vector3(GamePoint a) => new Vector3(a.x,a.y,a.z);
    public static implicit operator GamePoint(Vector3 a) => new GamePoint((int)a.x,(int)a.y,(int)a.z);


    public int magnitude{get{
        int mag = 0;
        int magX = Mathf.Abs(x);if(magX>mag)mag=magX;
        int magY = Mathf.Abs(y);if(magY>mag)mag=magY;
        int magZ = Mathf.Abs(z);if(magZ>mag)mag=magZ;
        return mag;
    }}

}
