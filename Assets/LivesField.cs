using UnityEngine;
using System.Collections;

public class LivesField : MonoBehaviour {
static int lives;
	// Use this for initialization
	void Start () {
		lives = 0;
	}
	
	public static void SetLives(int val)
	{
	    lives = val;
	}
	
	void OnGUI ()
	{
		guiText.text = lives.ToString();
	}
}
