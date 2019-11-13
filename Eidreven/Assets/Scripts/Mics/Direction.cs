using UnityEngine;

/**
 * In this corrd system I used the following
 * x-1 = WEST
 * x+1 = EAST
 * y+1 = UP
 * y-1 = DOWN
 * z+1 = NORTH
 * z-1 = SOUTH
 */
public enum Direction {
	NONE,
	NORTH, SOUTH, EAST, WEST, UP, DOWN,
	NORTH_EAST, NORTH_WEST, SOUTH_EAST, SOUTH_WEST,
	UP_NORTH, UP_SOUTH, UP_EAST, UP_WEST,
	DOWN_NORTH, DOWN_SOUTH, DOWN_EAST, DOWN_WEST,
	UP_NORTH_EAST, UP_NORTH_WEST, UP_SOUTH_EAST, UP_SOUTH_WEST,
	DOWN_NORTH_EAST, DOWN_NORTH_WEST, DOWN_SOUTH_EAST, DOWN_SOUTH_WEST
}

/**
 * This class is for functions that use the direction enum
 */
public class Directions{
	
	/**
	 * This method is used to get the direction one object
	 * is from another.
	 * @param  Vector3 from the point you want to get the
	 * direction from.
	 * @param  Vector3 to the point you want to get
	 * the direction to.
	 * @return Direction        Returns the direction (to) is
	 * from (from)
	 */
	public static Direction getDirection(GamePoint from, GamePoint to){
		return getDirection(to-from);
	}

	/**
	 * This method is used to get the direction the position
	 * is from the origin.
	 * @param  Vector3 p             The position you want to 
	 * get the direction from the origin for.
	 * @return Direction	returns the direction the position
	 * is from the origin.
	 */
	public static Direction getDirection(GamePoint p){ 
		if(p.y==0&&p.x==0&&p.z==0)return Direction.NONE;
		if(p.z==0){//no north or south
			if(p.x==0){//no east or west
				if(p.y>0){//up
					return Direction.UP;
				}else{//down
					return Direction.DOWN;
				}
			}else if(p.x>0){//east
				if(p.y==0){//no up down
					return Direction.EAST;
				}else if(p.y>0){//up
					return Direction.UP_EAST;
				}else{//down
					return Direction.DOWN_EAST;
				}
			}else{//west
				if(p.y==0){//no up down
					return Direction.WEST;
				}else if(p.y>0){//up
					return Direction.UP_WEST;
				}else{//down
					return Direction.DOWN_WEST;
				}
			}
		}else if(p.z>0){//north
			if(p.x==0){//no east or west
				if(p.y==0){//no up down
					return Direction.NORTH;
				}else if(p.y>0){//up
					return Direction.UP_NORTH;
				}else{//down
					return Direction.DOWN_NORTH;
				}
			}else if(p.x>0){//east
				if(p.y==0){//no up down
					return Direction.NORTH_EAST;
				}else if(p.y>0){//up
					return Direction.UP_NORTH_EAST;
				}else{//down
					return Direction.DOWN_NORTH_EAST;
				}
			}else{//west
				if(p.y==0){//no up down
					return Direction.NORTH_WEST;
				}else if(p.y>0){//up
					return Direction.UP_NORTH_WEST;
				}else{//down
					return Direction.DOWN_NORTH_WEST;
				}
			}
		}else{//south
			if(p.x==0){//no east or west
				if(p.y==0){//no up down
					return Direction.SOUTH;
				}else if(p.y>0){//up
					return Direction.UP_SOUTH;
				}else{//down
					return Direction.DOWN_SOUTH;
				}
			}else if(p.x>0){//east
				if(p.y==0){//no up down
					return Direction.SOUTH_EAST;
				}else if(p.y>0){//up
					return Direction.UP_SOUTH_EAST;
				}else{//down
					return Direction.DOWN_SOUTH_EAST;
				}
			}else{//west
				if(p.y==0){//no up down
					return Direction.SOUTH_WEST;
				}else if(p.y>0){//up
					return Direction.UP_SOUTH_WEST;
				}else{//down
					return Direction.DOWN_SOUTH_WEST;
				}
			}
		}
	}



	public static Direction getLookingDirection(int x, int y){
		Vector3 looking = new Vector3(0,0,0);
		bool invert = false;x+=90; if(x>=360){x-=360;}
		//get up/ down
		if(x>180){invert=true; x-=180;}
		if(x<60){looking.y=1;}else if(x<120){looking.y=0;}
		else {looking.y = -1;}
		//y is for nsew 0 to 360 n e s w
		if((y>-1&&y<23)||(y<361&&y>337)){looking.z = 1;}
		else if(y>22&&y<68){looking.z=1;looking.x=1;}
		else if(y>67&&y<113){looking.x=1;}
		else if(y>112&&y<158){looking.z=-1; looking.x=1;}
		else if(y>157&&y<203){looking.z=-1;}
		else if(y>202&&y<248){looking.z=-1; looking.x=-1;}
		else if(y>247&&y<293){looking.x=-1;}
		else if(y>292&&y<338){looking.x=-1;looking.z=1;}
		//invert if needed
		if(invert){looking.z*=-1;looking.x*=-1;}
		//return direction
		return getDirection(looking);
	}

	public static Direction rotateNESW(Direction direction){
		switch(direction){
			case Direction.NORTH: return Direction.EAST;
			case Direction.EAST: return Direction.SOUTH;
			case Direction.SOUTH: return Direction.WEST;
			case Direction.WEST: return Direction.NORTH;
			default: return direction;
		}
	}

	public static Direction rotateNWSE(Direction direction){
		switch(direction){
			case Direction.NORTH: return Direction.WEST;
			case Direction.WEST: return Direction.SOUTH;
			case Direction.SOUTH: return Direction.EAST;
			case Direction.EAST: return Direction.NORTH;
			default: return direction;
		}
	}

	/**
	 * This method is used to invert a direction.
	 * @param  Direction direction     The direction you
	 * want to invert.
	 * @return Direction returns the invert of the given
	 * direction.
	 */
	public static Direction invertDirection(Direction direction){
		switch(direction){
			case Direction.NORTH: return Direction.SOUTH;
			case Direction.SOUTH: return Direction.NORTH;
			case Direction.EAST: return Direction.WEST;
			case Direction.WEST: return Direction.EAST;
			case Direction.UP: return Direction.DOWN;
			case Direction.DOWN: return Direction.UP;
			case Direction.NORTH_EAST: return Direction.SOUTH_WEST;
			case Direction.NORTH_WEST: return Direction.SOUTH_EAST;
			case Direction.SOUTH_EAST: return Direction.NORTH_WEST;
			case Direction.SOUTH_WEST: return Direction.NORTH_EAST;
			case Direction.UP_NORTH: return Direction.DOWN_SOUTH;
			case Direction.UP_SOUTH: return Direction.DOWN_NORTH;
			case Direction.UP_EAST: return Direction.DOWN_WEST;
			case Direction.UP_WEST: return Direction.DOWN_EAST;
			case Direction.DOWN_NORTH: return Direction.UP_SOUTH;
			case Direction.DOWN_SOUTH: return Direction.UP_NORTH;
			case Direction.DOWN_EAST: return Direction.UP_WEST;
			case Direction.DOWN_WEST: return Direction.UP_EAST;
			case Direction.UP_NORTH_EAST: return Direction.DOWN_SOUTH_WEST;
			case Direction.UP_NORTH_WEST: return Direction.DOWN_SOUTH_EAST;
			case Direction.UP_SOUTH_EAST: return Direction.DOWN_NORTH_WEST;
			case Direction.UP_SOUTH_WEST: return Direction.DOWN_NORTH_EAST;
			case Direction.DOWN_NORTH_EAST: return Direction.UP_SOUTH_WEST;
			case Direction.DOWN_NORTH_WEST: return Direction.UP_SOUTH_EAST;
			case Direction.DOWN_SOUTH_EAST: return Direction.UP_NORTH_WEST;
			case Direction.DOWN_SOUTH_WEST: return Direction.UP_NORTH_EAST;
			default: return Direction.NONE;
		}
	}

	/**
	 * This method is used to move a position in the given direction
	 * by the given distance.
	 * @param  Vector3   position      The original position.
	 * @param  Direction direction     The direction you want to move.
	 * @param  float     distance      The distace you want to move in
	 * the given direction.
	 * @return Vector3           Returns a position that has been moved
	 * in the given direction by the given amount.
	 */
	public static Vector3 moveDirection(Vector3 position, Direction direction, float distance){
		switch(direction){
			case Direction.NORTH: return position + new Vector3(0,0,distance);
			case Direction.SOUTH: return position + new Vector3(0,0,-distance);
			case Direction.EAST: return position + new Vector3(distance,0,0);
			case Direction.WEST: return position + new Vector3(-distance,0,0);
			case Direction.UP: return position + new Vector3(0,distance,0);
			case Direction.DOWN: return position + new Vector3(0,-distance,0);
			case Direction.NORTH_EAST: return position + new Vector3(distance,0,distance);
			case Direction.NORTH_WEST: return position + new Vector3(-distance,0,distance);
			case Direction.SOUTH_EAST: return position + new Vector3(distance,0,-distance);
			case Direction.SOUTH_WEST: return position + new Vector3(-distance,0,-distance);
			case Direction.UP_NORTH: return position + new Vector3(0,distance,distance);
			case Direction.UP_SOUTH: return position + new Vector3(0,distance,-distance);
			case Direction.UP_EAST:  return position + new Vector3(distance,distance,0);
			case Direction.UP_WEST:  return position + new Vector3(-distance,distance,0);
			case Direction.DOWN_NORTH: return position + new Vector3(0,-distance,distance);
			case Direction.DOWN_SOUTH: return position + new Vector3(0,-distance,-distance);
			case Direction.DOWN_EAST:  return position + new Vector3(distance,-distance,0);
			case Direction.DOWN_WEST:  return position + new Vector3(-distance,-distance,0);
			case Direction.UP_NORTH_EAST: return position + new Vector3(distance,distance,distance);
			case Direction.UP_NORTH_WEST: return position + new Vector3(-distance,distance,distance);
			case Direction.UP_SOUTH_EAST: return position + new Vector3(distance,distance,-distance);
			case Direction.UP_SOUTH_WEST: return position + new Vector3(-distance,distance,-distance);
			case Direction.DOWN_NORTH_EAST: return position + new Vector3(distance,-distance,distance);
			case Direction.DOWN_NORTH_WEST: return position + new Vector3(-distance,-distance,distance);
			case Direction.DOWN_SOUTH_EAST: return position + new Vector3(distance,-distance,-distance);
			case Direction.DOWN_SOUTH_WEST: return position + new Vector3(-distance,-distance,-distance);
			default: return position;
		}
	}

	public static GamePoint convertToGamePoint(Direction direction){
		GamePoint point = new GamePoint(0,0,0);
		//north-south
		switch(direction){
			case Direction.NORTH: case Direction.NORTH_EAST: case Direction.NORTH_WEST:
			case Direction.UP_NORTH: case Direction.UP_NORTH_EAST: case Direction.UP_NORTH_WEST:
			case Direction.DOWN_NORTH: case Direction.DOWN_NORTH_EAST: case Direction.DOWN_NORTH_WEST:
				//north
				point.z = 1;
				break;
			case Direction.SOUTH: case Direction.SOUTH_EAST: case Direction.SOUTH_WEST:
			case Direction.UP_SOUTH: case Direction.UP_SOUTH_EAST: case Direction.UP_SOUTH_WEST:
			case Direction.DOWN_SOUTH: case Direction.DOWN_SOUTH_EAST: case Direction.DOWN_SOUTH_WEST:
				//south
				point.z=-1;
				break;
			default:
				//not north or south
				break;
		}

		//east - west
		switch(direction){
			case Direction.EAST: case Direction.NORTH_EAST: case Direction.SOUTH_EAST:
			case Direction.UP_EAST: case Direction.UP_NORTH_EAST: case Direction.UP_SOUTH_EAST:
			case Direction.DOWN_EAST: case Direction.DOWN_NORTH_EAST: case Direction.DOWN_SOUTH_EAST:
				//east
				point.x=1;
				break;
			case Direction.WEST: case Direction.NORTH_WEST: case Direction.SOUTH_WEST:
			case Direction.UP_WEST: case Direction.UP_NORTH_WEST: case Direction.UP_SOUTH_WEST:
			case Direction.DOWN_WEST: case Direction.DOWN_NORTH_WEST: case Direction.DOWN_SOUTH_WEST:
				//east
				point.x=-1;
				break;
			default: //not east or west
				break;			
		}

		//up - down
		switch(direction){
			case Direction.UP: case Direction.UP_EAST: case Direction.UP_WEST:
			case Direction.UP_NORTH: case Direction.UP_SOUTH: case Direction.UP_NORTH_EAST:
			case Direction.UP_NORTH_WEST: case Direction.UP_SOUTH_EAST: case Direction.UP_SOUTH_WEST:
				//up
				point.y=1;
				break;
			case Direction.DOWN: case Direction.DOWN_EAST: case Direction.DOWN_WEST:
			case Direction.DOWN_NORTH: case Direction.DOWN_SOUTH: case Direction.DOWN_NORTH_EAST:
			case Direction.DOWN_NORTH_WEST: case Direction.DOWN_SOUTH_EAST: case Direction.DOWN_SOUTH_WEST:
				//up
				point.y=-1;
				break;
			default: break; //no up down
		}
		return point;
	}

	public static Vector3 getCubeNormal(Direction direction){
		switch(direction){
			case Direction.NORTH: return Vector3.forward;
			case Direction.SOUTH: return Vector3.back;
			case Direction.UP: return Vector3.up;
			case Direction.DOWN: return Vector3.down;
			case Direction.WEST: return Vector3.left;
			case Direction.EAST: return Vector3.right;
			default: return Vector3.forward;
		}
	}
}