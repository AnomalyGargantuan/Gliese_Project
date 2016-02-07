using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController_old : RaycastController
{
	public LayerMask passengerMask;

	[Range (0,2)]
	public float easeAmount;
	public float speed;
	public float waitTime;
	public bool cyclic;

	public Vector3[] localWaypoints;

	Vector3[] globalWaypoints;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform, PlayerPhysicsImproved> passengerDictionary	= new Dictionary<Transform, PlayerPhysicsImproved> ();

	public override void Start()
	{
		base.Start();
		globalWaypoints = new Vector3[localWaypoints.Length];
		for(int i = 0; i < localWaypoints.Length; i++)
		{
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	void Update()
	{
		UpdateRaycastOrigins();

		Vector3 velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);
		
		MovePassenger(true);
		transform.Translate(velocity);
		MovePassenger(false);
	}

	//Function that makes a smooth movement for platforms. Better suited to run with a from 1 to 3
	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
	}

	Vector3 CalculatePlatformMovement()
	{
		if(Time.time < nextMoveTime)
		{
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

		if(percentBetweenWaypoints >= 1)
		{
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;

			if(!cyclic)
			{
				if(fromWaypointIndex >= globalWaypoints.Length-1)
				{
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}

			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	void MovePassenger(bool beforeMovePlatform)
	{
		foreach(PassengerMovement passenger in passengerMovement)
		{
			if(!passengerDictionary.ContainsKey(passenger.transform))
			{
				passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<PlayerPhysicsImproved> ());
			}
			
			if(passenger.moveBeforePlatform == beforeMovePlatform) 
			{
				passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}
	//TODO - BUG: Passenger Falling
	void CalculatePassengerMovement(Vector3 velocity)
	{
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();
		
		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);
		
		//Vertically moving platform
		if(velocity.y != 0)
		{	
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;
			
			for(int i = 0; i < verticalRayCount; i++)
			{
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);

				RaycastHit hit;
				Ray ray = new Ray(rayOrigin, Vector2.up * directionY);
				//RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);//TODO change the type of raycast

				if(Physics.Raycast(ray, out hit, rayLength, passengerMask) && hit.distance != 0)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
						
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), (directionY == 1), true));
					}
					
				}
			}
		}
		
		//Horizontally moving Platform
		if(velocity.x != 0)
		{
			float rayLenght = Mathf.Abs(velocity.x) + skinWidth;
			
			for(int i = 0; i < horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);

				RaycastHit hit;
				Ray ray = new Ray(rayOrigin, Vector2.right * directionX);
				//RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, passengerMask); //TODO change the type of raycast
				
				if(Physics.Raycast(ray, out hit, rayLenght, passengerMask) && hit.distance != 0)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;
						
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
					}
				}
			}
		}
		
		//Passenger on horizontally or downward moving platform
		if(directionY == -1 || velocity.y == 0 && velocity.x != 0)
		{
			float rayLength = skinWidth * 2;
			
			for(int i = 0; i < verticalRayCount; i++)
			{
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

				RaycastHit hit;
				Ray ray = new Ray(rayOrigin, Vector2.up);
				//RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
				
				if(Physics.Raycast(ray, out hit, rayLength, passengerMask) && hit.distance != 0)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);
						
						float pushX = velocity.x;
						float pushY = velocity.y;
						
						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
					}
					
				}
			}
		}
	}
	
	struct PassengerMovement
	{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;
		
		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
		{
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}
	
	void OnDrawGizmos()
	{
		if(localWaypoints != null)
		{
			Gizmos.color = Color.green;
			float size = .3f;
			
			for (int i = 0; i < localWaypoints.Length; i++)
			{
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}
