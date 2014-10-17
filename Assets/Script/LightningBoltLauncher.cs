using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {

	//Prefabs to be assigned in Editor
	public GameObject BoltPrefab;
	
	//For pooling
	List<GameObject> activeBoltsObj;
	List<GameObject> inactiveBoltsObj;
	int maxBolts = 1000;	

	//Different modes for the demo
	enum Mode : byte
	{
		bolt,
		branch,
		moving,
		text,
		nodes,
		burst
	}
	
	//The current mode the demo is in
	Mode currentMode = Mode.bolt;
	
	//Will contain all of the pieces for the moving bolt
	List<GameObject> movingBolt = new List<GameObject>();
	
	//used for actually moving the moving bolt
	Vector2 lightningEnd = new Vector2(100, 100);
	Vector2 lightningVelocity = new Vector2(1, 0);
	
	//For handling mouse clicks
	public Vector2 pos1, pos2;
	
	//For storing all of the pixels that need to be drawn by the bolts 
	List<Vector2> textPoints = new List<Vector2>();
	
		
	void Start()
	{
		//Initialize lists
		activeBoltsObj = new List<GameObject>();
		inactiveBoltsObj = new List<GameObject>();
		
		//Grab the parent we'll be assigning to our bolt pool
		GameObject p = GameObject.Find("/Champ") as GameObject;
		
		//For however many bolts we've specified
		for(int i = 0; i < maxBolts; i++)
		{
			//create from our prefab
			GameObject bolt = (GameObject)Instantiate(BoltPrefab);
			
			//Assign parent
			bolt.transform.parent = p.transform;
			
			//Initialize our lightning with a preset number of max sexments
			bolt.GetComponent<LightningBolt>().Initialize(25);
			
			//Set inactive to start
			bolt.SetActive(false);
			
			//Store in our inactive list
			inactiveBoltsObj.Add(bolt);
		}
		GameObject boltObj;
		LightningBolt boltComponent;
		//Handle the current mode appropriately
		switch (currentMode)
		{
		case Mode.bolt:
			//create a (pooled) bolt from pos1 to pos2
			CreatePooledBolt(pos1,pos2, Color.white, 1f);
			break;
			
		case Mode.moving:
			//Prevent from getting a 0 magnitude (0 causes errors 
			if(Vector2.Distance(pos1, pos2) <= 0)
			{
				//Try a random position
				Vector2 adjust = Random.insideUnitCircle;
				
				//failsafe
				if(adjust.magnitude <= 0) adjust.x += .1f;
				
				//Adjust the end position
				pos2 += adjust;
			}
			
			//Clear out any old moving bolt (this is designed for one moving bolt at a time)
			for(int i = movingBolt.Count - 1; i >= 0; i--)
			{
				Destroy(movingBolt[i]);
				movingBolt.RemoveAt(i);
			}
			
			//get the "velocity" so we know what direction to send the bolt in after initial creation
			lightningVelocity = (pos2 - pos1).normalized;
			
			//instantiate from our bolt prefab
			boltObj = (GameObject)GameObject.Instantiate(BoltPrefab);
			
			//get the bolt component
			boltComponent = boltObj.GetComponent<LightningBolt>();
			
			//initialize it with 5 max segments
			boltComponent.Initialize(5);
			
			//activate the bolt using our position data
			boltComponent.ActivateBolt(pos1, pos2, Color.white, 1f);
			
			//add it to our list
			movingBolt.Add(boltObj);
			break;
		}
	}
	
	void Update()
	{
		//Declare variables for use later
		GameObject boltObj;
		LightningBolt boltComponent;
		
		//store off the count for effeciency
		int activeLineCount = activeBoltsObj.Count;
		
		//loop through active lines (backwards because we'll be removing from the list)
		for (int i = activeLineCount - 1; i >= 0; i--)
		{
			//pull GameObject
			boltObj = activeBoltsObj[i];
			
			//get the LightningBolt component
			boltComponent = boltObj.GetComponent<LightningBolt>();
			
			//if the bolt has faded out
			if(boltComponent.IsComplete)
			{
				//deactive the segments it contains
				boltComponent.DeactivateSegments();
				
				//set it inactive
				boltObj.SetActive(false);
				
				//move it to the inactive list
				activeBoltsObj.RemoveAt(i);
				inactiveBoltsObj.Add(boltObj);
			}
		}
				
		//loop through all of our bolts that make up the moving bolt
		for(int i = movingBolt.Count - 1; i >= 0; i--)
		{
			//get the bolt component
			boltComponent = movingBolt[i].GetComponent<LightningBolt>();
			
			//if the bolt has faded out
			if(boltComponent.IsComplete)
			{
				//destroy it
				Destroy(movingBolt[i]);
				
				//remove it from our list
				movingBolt.RemoveAt(i);
				
				//on to the next one, on on to the next one
				continue;
			}
			
			//update and draw bolt
			boltComponent.UpdateBolt();
			boltComponent.Draw();
		}
		
		//if our moving bolt is active
		if(movingBolt.Count > 0)
		{
			//calculate where it currently ends
			lightningEnd = movingBolt[movingBolt.Count-1].GetComponent<LightningBolt>().End;
			
			//if the end of the bolt is within 25 units of the camera
			if(Vector2.Distance(lightningEnd,(Vector2)Camera.main.transform.position) < 25)
			{
				//instantiate from our bolt prefab
				boltObj = (GameObject)GameObject.Instantiate(BoltPrefab);
				
				//get the bolt component
				boltComponent = boltObj.GetComponent<LightningBolt>();
				
				//initialize it with a maximum of 5 segments
				boltComponent.Initialize(5);
				
				//activate the bolt using our position data (from the current end of our moving bolt to the current end + velocity) 
				boltComponent.ActivateBolt(lightningEnd,lightningEnd + lightningVelocity, Color.white, 1f);
				
				//add it to our list
				movingBolt.Add(boltObj);
				
				//update and draw our new bolt
				boltComponent.UpdateBolt();
				boltComponent.Draw();
			}
		}

		//update and draw active bolts
		for(int i = 0; i < activeBoltsObj.Count; i++)
		{
			activeBoltsObj[i].GetComponent<LightningBolt>().UpdateBolt();
			activeBoltsObj[i].GetComponent<LightningBolt>().Draw();
		}
	}
	
	//calculate distance squared (no square root = performance boost)
	public float DistanceSquared(Vector2 a, Vector2 b)
	{
		return ((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y));
	}
	
	void CreatePooledBolt(Vector2 source, Vector2 dest, Color color, float thickness)
	{
		//if there is an inactive bolt to pull from the pool
		if(inactiveBoltsObj.Count > 0)
		{
			//pull the GameObject
			GameObject boltObj = inactiveBoltsObj[inactiveBoltsObj.Count - 1];
			
			//set it active
			boltObj.SetActive(true);
			
			//move it to the active list
			activeBoltsObj.Add(boltObj);
			inactiveBoltsObj.RemoveAt(inactiveBoltsObj.Count - 1);
			
			//get the bolt component
			LightningBolt boltComponent =  boltObj.GetComponent<LightningBolt>();
			
			//activate the bolt using the given position data
			boltComponent.ActivateBolt(source, dest, color, thickness);
		}
	}
	

}
