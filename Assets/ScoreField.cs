using UnityEngine;
using System.Collections;

public class ScoreField : MonoBehaviour {
private static int score;
	// Use this for initialization
	void Start () 
	{
		score = 0;
	}
	
 	public static void SetScore(int val)
	{
	    score = val;
	}
	
	void OnGUI ()
	{
		guiText.text = score.ToString();
	}
}
