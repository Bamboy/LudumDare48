using UnityEngine;
using System.Collections;

//////////////////////////////////////
////// By: Stephan "Bamboy" Ennen ////
////// Last Updated: 4/10/18      ////
//////////////////////////////////////
///
///This script just holds general functions we might want to use in our other scripts.
///Generally they have to do with vector manipulation.
public class VectorExtras
{
	/// <summary>
	/// Remap 'val' from range A to output range B. 'A' is the input range, and 'B' is the output range.
	/// </summary>
	public static float Remap( float minA, float maxA, float minB, float maxB, float val )
	{

		float percentage = ReverseLerp( val, minA, maxA ); //Percentage of minimum range

		return Mathf.Lerp( minB, maxB, percentage );

	}



	/// Tests 'val'. If at or below 'min', return 0. If at or above 'max', return 1. Otherwise returns the percentage between min and max. Useful for determining progress.
	public static float ReverseLerp( float val, float min, float max )
	{
		if( val <= min ) { return 0.0f; }
		else if( val >= max ) { return 1.0f; }
		else
		{
			float offset;
			float offsetMax;
			float offsetVal;
			if( min == 0.0f )
			{
				offset = 0.0f;
			}
			else
			{
				offset = -min;
			}
			offsetMax = offset + max;
			offsetVal = offset + val;
			return offsetVal / offsetMax;
		}
	}

	/// Tests 'val'. If at or below 'min', return 0. If at or above 'max', return 1. Otherwise returns the percentage between min and max. Useful for determining progress.
	public static float ReverseLerp( int val, int min, int max )
	{
		if( val <= min ) { return 0.0f; }
		else if( val >= max ) { return 1.0f; }
		else
		{
			float offset;
			float offsetMax;
			float offsetVal;
			if( min == 0 )
			{
				offset = 0.0f;
			}
			else
			{
				offset = -(float)min;
			}
			offsetMax = offset + (float)max;
			offsetVal = offset + (float)val;
			return offsetVal / offsetMax;
		}
	}
		
	/// Average the specified values.
	public static float Average( params float[] values )
	{
		if( values == null || values.Length == 0f )
			return 0f;
		else if( values.Length == 1f )
			return values[0];
		else
		{
			float total = 0f;
			foreach (float n in values) 
				total += n;
			
			return total / (float)values.Length;
		}
	}
	/// Average the specified values.
	public static Vector2 Average( params Vector2[] values )
	{
		if( values == null || values.Length == 0 )
			return Vector2.zero;
		else if( values.Length == 1 )
			return values[0];
		else
		{
			Vector2 total = Vector2.zero;
			foreach (Vector2 v in values) 
				total += v;
			
			return total / (float)values.Length;
		}
	}
	/// Average the specified values.
	public static Vector3 Average( params Vector3[] values )
	{
		if( values == null || values.Length == 0 )
			return Vector3.zero;
		else if( values.Length == 1 )
			return values[0];
		else
		{
			Vector3 total = Vector3.zero;
			foreach (Vector3 v in values) 
				total += v;

			return total / (float)values.Length;
		}
	}


	///Mirrors the value between min and max
	public static float MirrorValue( float val, float min, float max )
	{
		return min + (max - val);
	}

	///Returns true or false with a 50% chance.
	public static bool SplitChance()
	{
		return Random.Range(0, 2) == 0 ? true : false;
	}
	
	///A helper function for getting the speed multiplier needed to make 'defaultLength' equal 'duration'.
	/// 
	///Mainly intended for animations as you can only change the playback speed by using multipliers and not the time in seconds.
	public static float GetDurationMultiplier( float defaultLength, float duration )
	{
		return defaultLength / duration;
	}
	
	///Rounds 'val' to the nearest step of 'snapValue'. Example: RoundTo( 1.2, 0.25 ) would return 1.25.
	public static float RoundTo( float val, float snapValue )
	{
		snapValue = Mathf.Abs(snapValue);
		if( snapValue != 0.0f )
			return snapValue * Mathf.Round( val / snapValue );
		else
			return 0.0f;
	}
	public static float FloorTo( float val, float snapValue )
	{
		snapValue = Mathf.Abs(snapValue);
		if( snapValue != 0.0f )
			return snapValue * Mathf.Floor( val / snapValue );
		else
			return 0.0f;
	}
	
	///Switch between types of vectors.
	public static Vector3 V3FromV2( Vector2 V2, float val )
	{
		return new Vector3( V2.x, V2.y, val );
	}
	///Switch between types of vectors.
	public static Vector2 V2FromV3( Vector3 V3 )
	{
		return new Vector2( V3.x, V3.y );
	}

	public static Vector2 CreateVector2( float v )
	{
		return new Vector2( v, v );
	}
	public static Vector2 CreateVector3( float v )
	{
		return new Vector3( v, v, v );
	}



	public static Vector3 ClampMagnitude( Vector3 vector, float minMagnitude, float maxMagnitdue )
	{
		float mag = vector.magnitude;
		if( mag > minMagnitude && mag < maxMagnitdue )
			return vector;
		else
		{
			if( mag > minMagnitude )
				return vector.normalized * minMagnitude;
			else
				return vector.normalized * maxMagnitdue;
		}
	}
	
	//===========================================
	//=========== Directions ====================
	//===========================================
	
	/// Returns the direction from point 'origin' to point 'target', with values between 0 and 1.
	public static Vector3 Direction( Vector3 origin, Vector3 target )
	{
		return ( target - origin ).normalized;
	}
	/// Returns the direction from point 'origin' to point 'target', with values between 0 and 1.
	public static Vector2 Direction( Vector2 origin, Vector2 target )
	{
		return ( target - origin ).normalized;
	}
	/// Returns the perpendicular vector.
	public static Vector2 Perpendicular( Vector2 toDirection )
	{
		return new Vector2( -toDirection.y, toDirection.x );
	}
	
	//===========================================
	//=========== Position Manipulation =========
	//===========================================
	
	/// Move point 'origin' in the direction of point 'target' by 'distance'. Useful for all sorts of things.
	public static Vector3 OffsetPosInPointDirection( Vector3 origin, Vector3 target, float distance )
	{
		return origin + (Direction( origin, target ) * distance);
	}
	/// Move point 'origin' in the direction of point 'target' by 'distance'. Useful for all sorts of things.
	public static Vector2 OffsetPosInPointDirection( Vector2 origin, Vector2 target, float distance )
	{
		return origin + (Direction( origin, target ) * distance);
	}
	
	/// Move point 'origin' in 'direction' by 'distance'. Return the new position.
	public static Vector3 OffsetPosInDirection( Vector3 origin, Vector3 direction, float distance )
	{
		return origin + (direction * distance);
	}
	/// Move point 'origin' in 'direction' by 'distance'. Return the new position.
	public static Vector2 OffsetPosInDirection( Vector2 origin, Vector2 direction, float distance )
	{
		return origin + (direction * distance);
	}
	
	/// Tries to move the point 'origin' to 'target' while staying within 'maxDistance' of 'origin'.
	/// Returns the closest point to 'target' while staying within 'maxDistance' of 'origin'.
	public static Vector2 AnchoredMovePosTowardTarget( Vector2 origin, Vector2 target, float maxDistance )
	{
		float distanceToTarget = Vector2.Distance( origin, target );
		if( distanceToTarget > maxDistance )
			return OffsetPosInPointDirection( origin, target, maxDistance );
		else
			return target;
	}
	public static Vector3 AnchoredMovePosTowardTarget( Vector3 origin, Vector3 target, float maxDistance )
	{
		float distanceToTarget = Vector3.Distance( origin, target );
		if( distanceToTarget > maxDistance )
			return OffsetPosInPointDirection( origin, target, maxDistance );
		else
			return target;
	}
	
	/// Same as above but also has a minimum value.
	public static Vector2 MinAnchoredMovePosTowardTarget( Vector2 origin, Vector2 target, float minDistance, float maxDistance )
	{
		float distanceToTarget = Vector2.Distance( origin, target );
		if( distanceToTarget > maxDistance )
			return OffsetPosInPointDirection( origin, target, maxDistance );
		else if( distanceToTarget < minDistance )
			return origin;
		else
			return target;
	}
	public static Vector3 MinAnchoredMovePosTowardTarget( Vector3 origin, Vector3 target, float minDistance, float maxDistance )
	{
		float distanceToTarget = Vector3.Distance( origin, target );
		if( distanceToTarget > maxDistance )
			return OffsetPosInPointDirection( origin, target, maxDistance );
		else if( distanceToTarget < minDistance )
			return origin;
		else
			return target;
	}
	
	//===========================================
	//=========== Randomizing Directions ========
	//===========================================
	
	// Randomize a direction starting at point 'origin', traveling in 'direction', by a scale of 'radius'. (Radius should never exceed 10.0f)
	// Return the new directional vector. This is useful for simulating bullet spread.
	// Basically it generates a random point inside a circle or sphere that is located in front of origin. (In the direction)
	// The returned direction is the direction from origin to the randomized point. Smaller values of radius will result in smaller spreads.
	/*public static Vector2 DirectionalCone( Vector2 origin, Vector2 direction, float radius )
	{

		Vector2 circlePoint = OffsetPosInDirection( origin, direction.normalized, 10.0f ) + 
			(Asteroidians.Core.RandomUtility.singleton.PointInsideCircle() * Mathf.Clamp(radius, 0.0f, 10.0f));
		
		// Comment or Uncomment these three lines for debug drawing. (Cyan is the actual bullet travel, yellow is where you were aiming)
		//Vector2 circleCenter = OffsetPosInDirection( origin, direction, 10.0f ); 
		//Debug.DrawLine( V3FromV2( origin, 0.0f ), V3FromV2( circleCenter, 0.0f ), Color.yellow, 1.0f );
		//Debug.DrawLine( V3FromV2( origin, 0.0f ), V3FromV2( circlePoint, 0.0f ), Color.cyan, 3.0f );
		
		return Direction( origin, circlePoint );
	}*/
	public static Vector3 DirectionalCone( Vector3 origin, Vector3 direction, float radius )
	{
		// ORIGIN IS A WORLD POS, DIRECTION IS A DIRECTIONAL VECTOR
		Vector3 spherePoint = OffsetPosInDirection( origin, direction.normalized, 10.0f ) + (Random.insideUnitSphere * Mathf.Clamp(radius, 0.0f, 10.0f));
		
		// Comment or Uncomment these three lines for debug drawing. (Cyan is the actual bullet travel, yellow is where you were aiming)
		//Vector3 sphereCenter = OffsetPosInDirection( origin, direction, 10.0f ); 
		//Debug.DrawLine( origin, sphereCenter, Color.yellow, 1.0f );
		//Debug.DrawLine( origin, spherePoint, Color.cyan, 3.0f );
		
		return Direction( origin, spherePoint );
	}
	/*
	// Same idea as above but it uses two world points instead.
	public static Vector2 DirectionalConeTowardPoint( Vector2 origin, Vector2 target, float radius )
	{
		// THIS TAKES TWO WORLD POSITIONS
		Vector2 circlePoint = OffsetPosInPointDirection(origin, target, 10.0f) + (Asteroidians.Core.RandomUtility.singleton.PointInsideCircle() * Mathf.Clamp(radius, 0.0f, 10.0f));
		return Direction( origin, circlePoint );
	} */
	public static Vector3 DirectionalConeTowardPoint( Vector3 origin, Vector3 target, float radius )
	{
		// THIS TAKES TWO WORLD POSITIONS
		Vector3 spherePoint = OffsetPosInPointDirection(origin, target, 10.0f) + (Random.insideUnitSphere * Mathf.Clamp(radius, 0.0f, 10.0f));
		return Direction( origin, spherePoint );
	}
	
	//===========================================
	//=========== Degrees And Vectors ===========
	//===========================================
	
	public static Vector2 DegreesToVector( float angle ) //This does not normalize!
	{
		return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
	}
	public static float VectorToDegrees( Vector2 vector )
	{
		return Mathf.Atan2( vector.y, vector.x ) * Mathf.Rad2Deg;
	}
	public static Vector2 RadiansToVector( float angle )
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}
	public static float VectorToRadians( Vector2 vector )
	{
		return Mathf.Atan2( vector.y, vector.x );
	}

	
	public static Vector2 NormalFromLine( Vector2 point1, Vector2 point2 ) //Possibly doesnt work?
	{
		Vector2 normal = Vector2.zero;
		normal.x = point2.x - point1.x;
		normal.y = point2.y - point1.y;
		return new Vector2( -normal.y, normal.x );
	}

	#region Better Signs
	//================================================
	//=========== Better Sign ========================
	//================================================

	/// Returns the sign of 'val'.
	/// Output will be -1 if 'val' is negative, 0 if 0, or 1 if positive.
	public static int Sign01( float val )
	{
		if( val == 0f ) 
			return 0;
		else if( val > 0f )
			return 1;
		else
			return -1;
	}
	/// Returns the sign of 'val'.
	/// Output will be -1 if 'val' is negative, 0 if 0, or 1 if positive.
	public static int Sign01( int val )
	{
		if( val == 0 ) 
			return 0;
		else if( val > 0 )
			return 1;
		else
			return -1;
	}
	/// Returns the sign of 'val'.
	/// Output will be -1 if 'val' is negative, 0 if 0, or 1 if positive.
	public static Vector2Int Sign01( Vector2 val )
	{
		return new Vector2Int( Sign01(val.x), Sign01(val.y) );
	}
	/// Returns the sign of 'val'.
	/// Output will be -1 if 'val' is negative, 0 if 0, or 1 if positive.
	public static Vector3Int Sign01( Vector3 val )
	{
		return new Vector3Int( Sign01(val.x), Sign01(val.y), Sign01(val.z) );
	}


	///Mathf.Sign on Vector2
	public static Vector2 Sign( Vector2 v2 )
	{
		return new Vector2( Mathf.Sign( v2.x ), Mathf.Sign(v2.y) );
	}
	///Mathf.Sign on Vector3
	public static Vector3 Sign( Vector3 v3 )
	{
		return new Vector3( Mathf.Sign( v3.x ), Mathf.Sign(v3.y), Mathf.Sign(v3.z) );
	}
	#endregion
	
	//================================================
	//=========== Array Utilities ====================
	//================================================
	
	public static bool Contains<T>( object[] array, object val )
	{
		for(int i = 0; i < array.Length; i++)
		{
			if(val == array[i])
				return true;
		}
		return false;
	}
	

	//===========================================
	//=========== Geometrics ====================
	//===========================================
	
	///Function taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
	/// 
	///Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
	///The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
	///the function outputs true, otherwise false.
	public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
	{
		
		linePoint = Vector3.zero;
		lineVec = Vector3.zero;
		
		//We can get the direction of the line of intersection of the two planes by calculating the 
		//cross product of the normals of the two planes. Note that this is just a direction and the line
		//is not fixed in space yet. We need a point for that to go with the line vector.
		lineVec = Vector3.Cross(plane1Normal, plane2Normal);
		
		//Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
		//the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
		//errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
		//the cross product of the normal of plane2 and the lineDirection.		
		Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);		
		
		float denominator = Vector3.Dot(plane1Normal, ldir);
		
		//Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
		if(Mathf.Abs(denominator) > 0.006f){
			
			Vector3 plane1ToPlane2 = plane1Position - plane2Position;
			float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
			linePoint = plane2Position + t * ldir;
			
			return true;
		}
		
		//output not valid
		else{
			return false;
		}
	}

	///Function taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
	///This function returns a point which is a projection from a point to a line.
	///The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
	public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{		

		//get vector from point on line to point in space
		Vector3 linePointToPoint = point - linePoint;

		float t = Vector3.Dot(linePointToPoint, lineVec);

		return linePoint + lineVec * t;
	}

	///Function taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
	///Two non-parallel lines which may or may not touch each other have a point on each line which are closest
	///to each other. This function finds those two points. If the lines are not parallel, the function 
	///outputs true, otherwise false.
	public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){

		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;

		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);

		float d = a*e - b*b;

		//lines are not parallel
		if(d != 0.0f){

			Vector3 r = linePoint1 - linePoint2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);

			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;

			closestPointLine1 = linePoint1 + lineVec1 * s;
			closestPointLine2 = linePoint2 + lineVec2 * t;

			return true;
		}

		else{
			return false;
		}
	}	

	///Function taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
	///Get the intersection between a line and a plane. 
	///If the line and plane are not parallel, the function outputs true, otherwise false.
	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{

		float length;
		float dotNumerator;
		float dotDenominator;
		Vector3 vector;
		intersection = Vector3.zero;

		//calculate the distance between the linePoint and the line-plane intersection point
		dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
		dotDenominator = Vector3.Dot(lineVec, planeNormal);

		//line and plane are not parallel
		if(dotDenominator != 0.0f){
			length =  dotNumerator / dotDenominator;

			//create a vector from the linePoint to the intersection point
			vector = SetVectorLength(lineVec, length);

			//get the coordinates of the line-plane intersection point
			intersection = linePoint + vector;	

			return true;	
		}

		//output not valid
		else{
			return false;
		}
	}

	//create a vector of direction "vector" with length "size"
	private static Vector3 SetVectorLength(Vector3 vector, float size){

		//normalize the vector
		Vector3 vectorNormalized = Vector3.Normalize(vector);

		//scale the vector
		return vectorNormalized *= size;
	}
}
