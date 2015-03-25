using UnityEngine;
using System.Collections;

//directive to enforce that our parent Game Object has a Character Controller
[RequireComponent(typeof(CharacterController))]

public class Avatar : MonoBehaviour
{
	//The Character Controller on my parent GameObject
	CharacterController characterController;

	// The linear gravity factor. Made available in the Editor.
	//public float gravity = 100.0f;
		
	// mass of vehicle
	public float mass = 1.0f;

	// The initial orientation.
	private Quaternion initialOrientation;
	public bool velocityGreater = false;
	// The cummulative rotation about the y-Axis.
	private float cummulativeRotation;

	// The rotation factor, this will control the speed we rotate at.
	public float rotationSensitvity = 500.0f;

	// The scout is used to mark the future position of the vehicle.
	// It is made visible as a debugging aid, but the point it is placed at is
	// used in alligment and in keeping the vehicle from leaving the terrain.
	public GameObject scout;
	public GameObject containedObject;

	//variables used to align the vehicle with the terrain surface 
	public float lookAheadDist = 2.0f; // How far ahead the scout is place
	private Vector3 hitNormal; // Normal to the terrain under the vehicle
	private Vector3 lookAtPt; // used to align the vehicle; marked by scout
	private Vector3 rayOrigin; // point from which ray is cast to locate scout
	private RaycastHit rayInfo; // struct to hold information returned by raycast

	//movement variables - exposed in inspector panel
	public float maxSpeed = 50.0f; //maximum speed of vehicle
	public float maxForce = 15.0f; // maximimum force allowed
	public float friction = 0.997f; // multiplier decreases speed
	
	//movement variables - updated by this component
	private float speed = 0.0f;  //current speed of vehicle
	private Vector3 moveDirection;

	private Vector3 steeringForce; // force that accelerates the vehicle
	private Vector3 velocity; //change in position per second
	



	// Use this for initialization
	void Start ()
	{
		//Use GetComponent to save a reference to the Character Controller. This 
		//generic method is avalable from the parent Game Object. The class in the  
		// angle brackets <  > is the type of the component we need a reference to.
		
		characterController = gameObject.GetComponent<CharacterController> ();
		
		//save the quaternion representing our initial orientation from the transform
		initialOrientation = transform.rotation;
		
		//set the cummulativeRotation to zero.
		cummulativeRotation = 0.0f;
		
		//half the height of vehicle bounding box
		//halfHeight = renderer.bounds.extents.y;
	}

	// Update is called once per frame
	void Update ()
	{
		//transform.position = new Vector3(transform.position.x, 10, transform.position.z);
		// We will get our orientation before we move: rotate before translation	
		// We are using the left or right movement of the Mouse to steer our vehicle. 
		SteerWithMouse ();
		CalcForces ();
		//calculate steering - forces that change velocity
		ClampForces ();
		//forces must not exceed maxForce
		CalcVelocity ();
		//orient vehicle transform toward velocity
		if (velocity != Vector3.zero) {
			transform.forward = velocity;
			velocityGreater = true;
			//characterController.Move (transform.forward * Time.deltaTime);
	
			//transform.position.y = 0;
			//MoveAndAlign ();
		}	
		transform.position += (transform.forward * Time.deltaTime)*speed;
			//transform.position 
		
	}
	

	//-----------------------------------steer with mouse------------------------------------		
	// In mouse steering, we keep track of the cumulative rotation on the y-axis which we can combine
	// with our initial orientation to get our current heading. We are keeping our transform level so that
	// right and left turning remains predictable even if our vehicle banks and climbs.	
	void SteerWithMouse ()
	{
		//Get the left/right Input from the Mouse and use time along with a scaling factor 
		// to add a controlled amount to our cummulative rotation about the y-Axis.
		cummulativeRotation += Input.GetAxis ("Mouse X") * Time.deltaTime * rotationSensitvity;
		
		//Create a Quaternion representing our current cummulative rotation around the y-axis. 
		Quaternion currentRotation = Quaternion.Euler (0.0f, cummulativeRotation, 0.0f);
		
		//Use the quaternion to update the transform of our vehicle of the vehicles Game Object based on initial orientation 
		//and the currently applied rotation from the original orientation. 
		transform.rotation = initialOrientation * currentRotation;
	}

	//----------------------------Accelerate with Arrow or WASD keys------------------------------------		
	// If the user is pressing the up-arrow or W key we will return a force to accelerate the vehicle
	// along its z-axis which is to say in the foward direction.
	private Vector3 KeyboardAcceleration ()
	{
		//Move 'forward' based on player input
		Vector3 force;
		Vector3 dv = Vector3.zero;
		//dv is desired velocity
		dv.z = Input.GetAxis ("Vertical");
		//forward is positive z 
		//Take the moveDirection from the vehicle's local space to world space 
		//using the transform of the Game Object this script is attached to.
		dv = transform.TransformDirection (dv);
		dv *= maxSpeed;
		force = dv - transform.forward * speed;
		//Debug.DrawLine(transform.position, transform.position + dv);
		return force;
	}
	
	// Calculate the forces that alter velocity
	private void CalcForces ()
	{
		steeringForce = Vector3.zero;
		steeringForce += KeyboardAcceleration ();
	}

	// if steering forces exceed maxForce they are set to maxForce
	private void ClampForces ()
	{
		if (steeringForce.magnitude > maxForce) {
			steeringForce.Normalize ();
			steeringForce *= maxForce;
		}
	}
	
	// acceleration and velocity are calculated
	void CalcVelocity ()
	{
		Vector3 moveDirection = transform.forward;
		//Debug.DrawLine (transform.position, transform.position+moveDirection, Color.green);
		// move in forward direction
		speed *= friction;
		// speed is reduced to simulate friction
		velocity = moveDirection * speed;
		// movedirection is scaled to get velocity
		Vector3 acceleration = steeringForce / mass;
		// acceleration is force/mass
		velocity += acceleration * Time.deltaTime;
		// add acceleration to velocity
		speed = velocity.magnitude;
		// speed is altered by acceleration		
		if (speed > maxSpeed) {
			// clamp speed & velocity to maxspeed
			speed = maxSpeed;
			velocity = moveDirection * speed;
		}
	}

	// The hitNormal will give us a normal to the terrain under our vehicle
	// which we can use to align the vehicle with the terrain. It will be
	// called repeatedly when the collider on the character controller
	// of our vehicle contacts the collider on the terrain
	void OnControllerColliderHit (ControllerColliderHit hit)
	{	
		hitNormal = hit.normal;
	}
}
