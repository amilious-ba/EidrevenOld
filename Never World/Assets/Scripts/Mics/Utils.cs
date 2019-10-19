using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

	/**
	 * This method is used to multiply all the vector3s values
	 * by the passed scale.
	 * @param Vector3 vector The vector3 you want to scale.
	 * @param float   scale  The number you want to multiply
	 * all the vector3s points by.
	 * @returns Vector3 a scaled vector3
	 */
	public static Vector3 ScaleVector3(Vector3 vector, float scale){
		return new Vector3(vector.x*scale,vector.y*scale,vector.z*scale);
	}

	/**
	 * This method is used to create a Vector3 where all the
	 * values are the same.
	 * @param  float value         The value for the x, y, and
	 * z values in the Vector3.
	 * @return Vector3       returns a Vector3 with all values
	 * set to the given value.
	 */
	public static Vector3 fillVector3(float value){
		return new Vector3(value, value, value);
	}
}
