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

	public static Vector3 convertArrayToVector3(float[] array){
		if(array==null||array.Length<3)return new Vector3(0,0,0);
		return new Vector3(array[0],array[1],array[2]);
	}

	public static Vector2 convertArrayToVector2(float[] array){
		if(array==null||array.Length<2)return new Vector2(0,0);
		return new Vector2(array[0],array[1]);
	}
}
