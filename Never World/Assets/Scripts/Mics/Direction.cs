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
	public static Direction getDirection(Vector3 from, Vector3 to){
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
	public static Direction getDirection(Vector3 p){ 
		if(p.y==0&&p.x==0&&p.z==0)return Direction.NONE;
		if(p.z==0){//no north or south
			if(p.x==0){//no east or west
				if(p.z>0){//up
					return Direction.UP;
				}else{//down
					return Direction.DOWN;
				}
			}else if(p.x>0){//east
				if(p.z==0){//no up down
					return Direction.EAST;
				}else if(p.z>0){//up
					return Direction.UP_EAST;
				}else{//down
					return Direction.DOWN_EAST;
				}
			}else{//west
				if(p.z==0){//no up down
					return Direction.WEST;
				}else if(p.z>0){//up
					return Direction.UP_WEST;
				}else{//down
					return Direction.DOWN_WEST;
				}
			}
		}else if(p.z>0){//north
			if(p.x==0){//no east or west
				if(p.z==0){//no up down
					return Direction.NORTH;
				}else if(p.z>0){//up
					return Direction.UP_NORTH;
				}else{//down
					return Direction.DOWN_NORTH;
				}
			}else if(p.x>0){//east
				if(p.z==0){//no up down
					return Direction.NORTH_EAST;
				}else if(p.z>0){//up
					return Direction.UP_NORTH_EAST;
				}else{//down
					return Direction.DOWN_NORTH_EAST;
				}
			}else{//west
				if(p.z==0){//no up down
					return Direction.NORTH_WEST;
				}else if(p.z>0){//up
					return Direction.UP_NORTH_WEST;
				}else{//down
					return Direction.DOWN_NORTH_WEST;
				}
			}
		}else{//south
			if(p.x==0){//no east or west
				if(p.z==0){//no up down
					return Direction.SOUTH;
				}else if(p.z>0){//up
					return Direction.UP_SOUTH;
				}else{//down
					return Direction.DOWN_SOUTH;
				}
			}else if(p.x>0){//east
				if(p.z==0){//no up down
					return Direction.SOUTH_EAST;
				}else if(p.z>0){//up
					return Direction.UP_SOUTH_EAST;
				}else{//down
					return Direction.DOWN_SOUTH_EAST;
				}
			}else{//west
				if(p.z==0){//no up down
					return Direction.SOUTH_WEST;
				}else if(p.z>0){//up
					return Direction.UP_SOUTH_WEST;
				}else{//down
					return Direction.DOWN_SOUTH_WEST;
				}
			}
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