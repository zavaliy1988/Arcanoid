using UnityEngine;
using System.Collections;

public class DurableBrick : MonoBehaviour {
int lives;
int points_for_destroy = 100;
	// Use this for initialization
	void Start () 
	{
		lives = 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.Equals("Ball"))
		{
			lives--;
			if (lives == 0)	
			{
				GameManager.AddScore(points_for_destroy);
				Destroy(gameObject);
			}			
		}
	}
}
