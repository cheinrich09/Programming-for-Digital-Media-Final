using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class FlockManager : MonoBehaviour
{
	// weight parameters are set in editor and used by all flockers 
	// if they are initialized here, the editor will override settings	 
	// weights used to arbitrate btweeen concurrent steering forces 
	public float alignmentWt;
	public float separationWt;
	public float cohesionWt;
	public float avoidWt;
	public float inBoundsWt;
	public float wanderWt; 
	public float unAlignedWt;
	public float containWt;
	public float eHeight;
	public float flowFieldWt;
	
	// these distances modify the respective steering behaviors
	public float avoidDist;
	public float separationDist;
	public float containDist;
	
	
	
	
	// wander vals
	public float distanceToCircleCenter;
	public float radiusOfCircle;
	public float angleChange;
	
	//unaligned vals
	public float unAlignedDist;

	// set in editor to promote reusability.
	public int numberOfFlockers;
	public Object flockerPrefab;
	public Object obstaclePrefab;
	public Terrain flockerTerrain;

	//values used by all flockers that are calculated by controller on update
	private Vector3 flockDirection;
	private Vector3 centroid;
	
	//flow Field Object

	public Object ffContainer;
	private GameObject[] ffPoints;
	public GameObject[] FFPoints { get{return ffPoints;}}
	
	//accessors
	private static FlockManager instance;
	public static FlockManager Instance { get { return instance; } }

	public Vector3 FlockDirection {
		get { return flockDirection; }
	}
	
	public Vector3 Centroid { get { return centroid; } }
	public GameObject centroidContainer;
	public GameObject avatarBoat;
		
	
		
	// list of flockers with accessor
	private List<GameObject> flockers = new List<GameObject>();
	public List<GameObject> Flockers {get{return flockers;}}

	// array of obstacles with accessor
	private GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}
	
	// this is a 2-dimensional array for distances between flockers
	// it is recalculated each frame on update
	private float[,] distances;

		
		//construct our 2d array based on the value set in
	public void Start ()
	{
		instance = this;
		//construct our 2d array based on the value set in the editor
		distances = new float[numberOfFlockers, numberOfFlockers];
		//reference to Vehicle script component for each flocker
		Flocking flocker; // reference to flocker scripts
	
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		ffPoints = GameObject.FindGameObjectsWithTag("FlowField Point");
		
		for (int i = 0; i < numberOfFlockers; i++) {
			//Instantiate a flocker prefab, catch the reference, cast it to a GameObject
			//and add it to our list all in one line.
			flockers.Add ((GameObject)Instantiate (flockerPrefab, 
				new Vector3 (75 + 25 * i, eHeight, 75+25*i), Quaternion.identity));
			//grab a component reference
			flocker = flockers [i].GetComponent<Flocking> ();
			//set values in the Vehicle script
			flocker.Index = i;
		}
		centroidContainer.transform.position = new Vector3 (320, 20, 100);
	}
	public void Update( )
	{
		calcCentroid( );//find average position of each flocker 
		calcFlockDirection( );//find average "forward" for each flocker
		calcDistances( );
	}
	
	
	void calcDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfFlockers; i++)
		{
			for( int j = i+1; j < numberOfFlockers; j++)
			{
				dist = Vector3.Distance(flockers[i].transform.position, flockers[j].transform.position);
				distances[i, j] = dist;
				distances[j, i] = dist;
			}
		}
	}
	
	public float getDistance(int i, int j)
	{
		return distances[i, j];
	}
	
	
		
	private void calcCentroid ()
	{
		// calculate the current centroid of the flock
		// use transform.position
		//centroidContainer.transform.position = flockers [4].transform.position;
		for (int i = 0; i< numberOfFlockers; i++)
		{
			if(i == 0)
			{
				centroidContainer.transform.position = flockers[i].transform.position;	
			}
			else
			{
				centroidContainer.transform.position += flockers[i].transform.position;	
			}
		}
		centroidContainer.transform.position = centroidContainer.transform.position/numberOfFlockers;
		centroid = centroidContainer.transform.position;
	}
	
	private void calcFlockDirection ()
	{
		// calculate the average heading of the flock
		// use transform.
		for (int i = 0; i< numberOfFlockers; i++)
		{
			if(i == 0)
			{
				flockDirection = flockers[i].transform.forward;	
			}
			else
			{
				flockDirection += flockers[i].transform.forward;	
			}
		}
		flockDirection = flockDirection/numberOfFlockers;
		centroidContainer.transform.forward = flockDirection;
	}
	
}