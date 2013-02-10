using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {
int sensitivity;
float leftedge_x;
float rightedge_x;
	// Use this for initialization
	void Start () 
	{
		name = "Platform";
		sensitivity = 300;
		leftedge_x = -75;
		rightedge_x = 75;
		renderer.material.color = Color.yellow;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey("left") || Input.GetKey("a"))
		{	
			if (transform.position.x > leftedge_x)
				transform.Translate(Vector3.left * Time.deltaTime * sensitivity);
			//Debug.Log (transform.position.x);
		}
		if (Input.GetKey("right") || Input.GetKey("d"))
		{	
			if (transform.position.x < rightedge_x)
				transform.Translate(Vector3.right * Time.deltaTime * sensitivity);
			//Debug.Log (transform.position.x);
		}
	}
}
