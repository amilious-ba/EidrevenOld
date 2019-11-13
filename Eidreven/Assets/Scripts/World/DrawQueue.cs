using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawQueue{
    
    //private variables
    private LockFreeQueue<GamePoint>[] standardQueue;
    private LockFreeQueue<GamePoint>[] priorityQueue;

    public DrawQueue(){
    	standardQueue = new LockFreeQueue<GamePoint>[24];
    	priorityQueue = new LockFreeQueue<GamePoint>[24];
    }

    public void Enqueue(Direction direction, Chunk chunk){
    	//If chunk is topmost add to priority queue
    	if(chunk.TopMost){priorityQueue[(int)direction].Enqueue(chunk.Index);}
    	//otherwise add to the normal queue
    	else{standardQueue[(int)direction].Enqueue(chunk.Index);}    	
    }

    public bool Dequeue(Direction direction, out GamePoint chunkIndex){
    	//modify direction
    	GamePoint dir = Directions.convertToGamePoint(direction); dir.y=0;
    	direction = Directions.getDirection(dir);
    	//try deque from the given direction
    	GamePoint dequeue = new GamePoint(0,0,0);
    	if(dequeuePriorityQueue(direction, out dequeue)){
    		chunkIndex = dequeue; return true;
    	}else if(dequeueStandardQueue(direction, out dequeue)){
    		chunkIndex = dequeue; return true;
    	}
    	chunkIndex = dequeue;
    	return false;	
    }

    private bool dequeuePriorityQueue(Direction direction, out GamePoint chunkIndex){
    	for(int i=0;i<24;i++){
    		Direction dir = getClosestDirection(direction,i);
    		if(priorityQueue[(int)dir].Dequeue(out chunkIndex))return true;
    	}chunkIndex = new GamePoint(0,0,0); return false; 
    }

    private bool dequeueStandardQueue(Direction direction, out GamePoint chunkIndex){
    	for(int i=0;i<24;i++){
    		Direction dir = getClosestDirection(direction,i);
    		if(standardQueue[(int)dir].Dequeue(out chunkIndex))return true;
    	}chunkIndex = new GamePoint(0,0,0);	return false; 
    }

	private Direction getClosestDirection(Direction direction, int position){
		if(position>23||position<0)return Direction.NONE; GamePoint p = new GamePoint(0,0,0);
		switch(position){
			case 0:  return Direction.DOWN;						//DOWN
			case 1:  return direction;							//NORTH
			case 2:  return rotateDirectionLeft(direction,1);	//NORTH_WEST
			case 3:  return rotateDirectionRight(direction,1);	//NORTH_EAST
			case 4:	 p = Directions.convertToGamePoint(rotateDirectionLeft(direction,1));
				p.y=-1; return Directions.getDirection(p);		//DOWN_NORTH_WEST
			case 5:	 p = Directions.convertToGamePoint(rotateDirectionRight(direction,1));
				p.y=-1; return Directions.getDirection(p);		//DOWN_NORTH_EAST
			case 6:	 p = Directions.convertToGamePoint(rotateDirectionLeft(direction,1));
				p.y=1; return Directions.getDirection(p);		//UP_NORTH_WEST
			case 7:	 p = Directions.convertToGamePoint(rotateDirectionRight(direction,1));
				p.y=1; return Directions.getDirection(p);		//UP_NORTH_EAST
			case 8:  return Direction.UP;						//UP
			case 9:	 return rotateDirectionLeft(direction,2);	//WEST
			case 10: return rotateDirectionRight(direction,2);	//EAST
			case 11: p = Directions.convertToGamePoint(rotateDirectionLeft(direction,2));
				p.y=-1; return Directions.getDirection(p);		//DOWN_WEST
			case 12: p = Directions.convertToGamePoint(rotateDirectionRight(direction,2));
				p.y=-1; return Directions.getDirection(p);		//DOWN_EAST
			case 13: p = Directions.convertToGamePoint(rotateDirectionLeft(direction,2));
				p.y=1; return Directions.getDirection(p);		//UP_WEST
			case 14: p = Directions.convertToGamePoint(rotateDirectionRight(direction,2));
				p.y=1; return Directions.getDirection(p);		//UP_EAST
			case 15: return rotateDirectionLeft(direction,3);	//SOUTH_WEST
			case 16: return rotateDirectionRight(direction,3);	//SOUTH_EAST
			case 17: p = Directions.convertToGamePoint(rotateDirectionLeft(direction,3));
				p.y=-1; return Directions.getDirection(p);		//DOWN_SOUTH_WEST
			case 18: p = Directions.convertToGamePoint(rotateDirectionRight(direction,3));
				p.y=-1; return Directions.getDirection(p);		//DOWN_SOUTH_EAST
			case 19: p = Directions.convertToGamePoint(rotateDirectionLeft(direction,3));
				p.y=1; return Directions.getDirection(p);		//UP_SOUTH_WEST
			case 20: p = Directions.convertToGamePoint(rotateDirectionRight(direction,3));
				p.y=1; return Directions.getDirection(p);		//UP_SOUTH_EAST
			case 21: return rotateDirectionRight(direction,4);	//SOUTH
			case 22: p = Directions.convertToGamePoint(rotateDirectionRight(direction,4));
				p.y=-1; return Directions.getDirection(p);		//DOWN_SOUTH
			case 23: p = Directions.convertToGamePoint(rotateDirectionRight(direction,4));
				p.y=1; return Directions.getDirection(p);		//UP_SOUTH
			default: return direction;
		}
	}

	private Direction rotateDirectionLeft(Direction direction, int places){
		if(places<=0)return direction; switch(direction){
			case Direction.NORTH:		direction=Direction.NORTH_WEST;	break;
			case Direction.NORTH_WEST:	direction=Direction.WEST;		break;
			case Direction.WEST:		direction=Direction.SOUTH_WEST;	break;
			case Direction.SOUTH_WEST:	direction=Direction.SOUTH;		break;
			case Direction.SOUTH:		direction=Direction.SOUTH_EAST;	break;
			case Direction.SOUTH_EAST:	direction=Direction.EAST;		break;
			case Direction.EAST:		direction=Direction.NORTH_EAST;	break;
			case Direction.NORTH_EAST:	direction=Direction.NORTH;		break;
		}places--; return rotateDirectionLeft(direction,places);
	}

	private Direction rotateDirectionRight(Direction direction, int places){
		if(places<=0)return direction; switch(direction){
			case Direction.NORTH:		direction=Direction.NORTH_EAST;	break;
			case Direction.NORTH_EAST:	direction=Direction.EAST;		break;
			case Direction.EAST:		direction=Direction.SOUTH_EAST;	break;
			case Direction.SOUTH_EAST:	direction=Direction.SOUTH;		break;
			case Direction.SOUTH:		direction=Direction.SOUTH_WEST;	break;
			case Direction.SOUTH_WEST:	direction=Direction.WEST;		break;
			case Direction.WEST:		direction=Direction.NORTH_WEST;	break;
			case Direction.NORTH_WEST:	direction=Direction.NORTH;		break;
		}places--; return rotateDirectionRight(direction, places);
	}

}
