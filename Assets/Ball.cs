using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

public static float level_velocity_multiplier;
float time_multiplication_coefficient;
Vector3 velocity_vector;

	// Use this for initialization
	void Start () 
	{
		name = "Ball";
		time_multiplication_coefficient = 40.0f;
		float x_offset = 0;
		while (Mathf.Abs(x_offset) < 0.1f)
			x_offset = Random.Range(-0.7f, 0.7f);
		velocity_vector = new Vector3(x_offset, -1, 0);
		velocity_vector.Normalize();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 corrected_position = transform.position;
		corrected_position.z = 0;
		transform.position = corrected_position;
		transform.Translate(level_velocity_multiplier * velocity_vector * Time.deltaTime * time_multiplication_coefficient);
		//Debug.Log ("transofrm.position = " + transform.position);
	}
	
	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.name.Equals("BottomBorder"))
		{
			GameManager.DecreasePlayerLives();
		}
		velocity_vector = Vector3.Reflect(velocity_vector, collision.contacts[0].normal);	
		velocity_vector.z = 0;
		//Debug.Log("velocity_vector = " + velocity_vector.ToString());
	}
	
	public static void SetVelocityMultiplier(float multiplier)
	{
		level_velocity_multiplier = multiplier;
	}
}
