using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameManager : MonoBehaviour {

//---------------------prefabs-----------------------
public UnityEngine.Object fragile_brick;
public UnityEngine.Object durable_brick;
public UnityEngine.Object undestroyable_brick;
public UnityEngine.Object ball;
public UnityEngine.Object platform;
public UnityEngine.Object record_table_cell;

//I know, that it's very bad to create duplicate variables
//but i don't know how to assign prefabs to static variable in Inspector
static UnityEngine.Object static_fragile_brick;
static UnityEngine.Object static_durable_brick;
static UnityEngine.Object static_undestroyable_brick;
static UnityEngine.Object static_ball;
static UnityEngine.Object static_platform;
static UnityEngine.Object static_record_table_cell;
	
	
//--------------------game process-------------------	
static List <GameObject> list_of_destroyable_bricks;
static List <GameObject> list_of_undestroyable_bricks;	
static int player_lives;	
static int player_score;
static float level_velocity_multiplier;
static float increaseballspeed_completion_time;
static float decreaseballspeed_completion_time;

static bool show_save_result_window;
static bool show_best_results;
static List<KeyValuePair<string,int>> sorted_results;
static GameObject [,] records_table;
// Use this for initialization
	void Start () 
	{
		static_fragile_brick = fragile_brick;
		static_durable_brick = durable_brick;
		static_undestroyable_brick = undestroyable_brick;
		static_platform = platform;
		static_ball = ball;
		static_record_table_cell = record_table_cell;
		
		player_lives = 5;
		player_score = 0;
		level_velocity_multiplier = 1.0f;
		increaseballspeed_completion_time = 0.0f;
		decreaseballspeed_completion_time = 0.0f;
		show_save_result_window = false;
		show_best_results = false;
		list_of_destroyable_bricks = new List<GameObject>();
		list_of_undestroyable_bricks = new List<GameObject>();
		sorted_results = new List<KeyValuePair<string,int>>();
		
		LivesField.SetLives(player_lives);
		ScoreField.SetScore(player_score);
		CreateBricks(3,5);
		CreatePlatform();
		CreateBall(level_velocity_multiplier);
		CreateBestResultsTable();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//disable ball speed bonuse, if time of bonuses is over
		if ((increaseballspeed_completion_time < Time.time) && (decreaseballspeed_completion_time < Time.time))
			Ball.SetVelocityMultiplier(level_velocity_multiplier);
	}
	
	public static void AddScore (int val)
	{
		//set score
		player_score += val;
		ScoreField.SetScore(player_score);
		
		//check - if level is completed
		int amount_of_null_bricks = 1;
		foreach(GameObject brick in list_of_destroyable_bricks)
		{				
			if (brick == null) amount_of_null_bricks++;
		}
		if (amount_of_null_bricks == list_of_destroyable_bricks.Count)
			{
				level_velocity_multiplier *= 2.0f;
				//Debug.Log ("LEVEL VELOCITY MULTIPLIER = " + level_velocity_multiplier.ToString());
				CreateBricks(3,5);
				CreatePlatform();
				CreateBall(level_velocity_multiplier);
				return;
			}

		//if game contunies
		GenerateBonus();
	}
	
	public static void DecreasePlayerLives ()
	{
		player_lives--;
		LivesField.SetLives(player_lives);
		if (player_lives == 0)
		{
			EditorUtility.DisplayDialog("You lose", "You score is " + player_score.ToString() + " points", "Ok");
			foreach(GameObject brick in list_of_destroyable_bricks) GameObject.Destroy(brick);
			foreach(GameObject brick in list_of_undestroyable_bricks) GameObject.Destroy(brick);
			Destroy(GameObject.Find("Ball"));
			show_save_result_window = true;
			//CreateBricks(3,5);
			//CreatePlatform();
			//CreateBall(level_velocity_multiplier);
			//player_lives = 5;
			//player_score = 0;
		}
	}

//----------------------------Objects Creation Block------------------------------------------------------------------------------------------	
//--------------------------------------------------------------------------------------------------------------------------------------------	
	static void CreateBricks (int rows, int columns)
	{
		foreach(GameObject brick in list_of_destroyable_bricks) GameObject.Destroy(brick);
		foreach(GameObject brick in list_of_undestroyable_bricks) GameObject.Destroy(brick);
		list_of_destroyable_bricks.Clear();
		list_of_undestroyable_bricks.Clear();
		
		int first_brick_pos_x = -80;
		int first_brick_pos_y = 40;
		int distance_between_bricks_x = 40;
		int distance_between_bricks_y = 15;
		for(int i = 0; i < rows; i++)
		{
			int current_brick_pos_x = first_brick_pos_x;
			for(int j = 0; j < columns; j++)
				{
				    GameObject brick = null;
					if ((i == 1 && j == 1) || (i == 4 && j == 1)) 
						{
							brick = Instantiate(static_undestroyable_brick, new Vector3(current_brick_pos_x, first_brick_pos_y, 0), Quaternion.identity) as GameObject;
							list_of_undestroyable_bricks.Add(brick);	
						}
					else if ((i == 0) && (j == 2) || (i == 1) && (j == 3) || (i == 2) && (j == 4))
						{	
							brick = Instantiate(static_durable_brick, new Vector3(current_brick_pos_x, first_brick_pos_y, 0), Quaternion.identity) as GameObject;
							list_of_destroyable_bricks.Add(brick);
						}
					else 
						{
							brick = Instantiate(static_fragile_brick, new Vector3(current_brick_pos_x, first_brick_pos_y, 0), Quaternion.identity) as GameObject;
							list_of_destroyable_bricks.Add(brick);
						}
					current_brick_pos_x += distance_between_bricks_x;	
				}
				
			first_brick_pos_y -= distance_between_bricks_y;
		}
	}
	
	static void CreatePlatform ()
	{
		GameObject platform = GameObject.Find("Platform");
		if (platform != null) GameObject.Destroy(platform);
		platform = Instantiate(static_platform, new Vector3(0,-48, 0), Quaternion.identity) as GameObject;
	}
	
	static void CreateBall (float velocity_multiplier)
	{
		GameObject ball = GameObject.Find("Ball");
		if (ball != null) GameObject.Destroy(ball);
		ball = Instantiate(static_ball, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		Ball.SetVelocityMultiplier(level_velocity_multiplier);
	}
	
	static void CreateBestResultsTable ()
	{
		const int rows_count = 10;
		records_table = new GameObject[10,2];
		float first_cell_pos_x = 0.40f;
		float first_cell_pos_y = 0.9f;
		float distance_between_cells_x = 0.16f;
		float distance_between_cells_y = 0.08f;
		for(int i = 0; i < rows_count; i++)
		{
			for(int j = 0; j < 2; j++)
				records_table[i,j] = Instantiate(static_record_table_cell,  new Vector3(first_cell_pos_x + j * distance_between_cells_x, first_cell_pos_y,0), Quaternion.identity) as GameObject;
			first_cell_pos_y -= distance_between_cells_y;
		}
	}

//----------------------------Bonuses Block---------------------------------------------------------------------------------------------------	
//--------------------------------------------------------------------------------------------------------------------------------------------	
	static void GenerateBonus ()
	{
		int bonus_chance = UnityEngine.Random.Range(0,10);
		if (bonus_chance > 1)
			{
				int bonus_type = UnityEngine.Random.Range(0,4);
				if (bonus_type == 0) IncreaseLivesBonus();
				if (bonus_type == 1) IncreasePointsBonus();
				if (bonus_type == 2) IncreaseBallSpeedBonus();
				if (bonus_type == 3) DecreaseBallSpeedBonus();
			}
	}
	
	static void IncreaseLivesBonus ()
	{
		const int max_player_lives = 8;
		if (player_lives < max_player_lives) player_lives++;	
		LivesField.SetLives(player_lives);
	}
	
	static void IncreasePointsBonus ()
	{
		const int bonus_points = 100;
		player_score += bonus_points;
		ScoreField.SetScore(player_score);
	}
	
	//turn off performs in Update()
	static void IncreaseBallSpeedBonus ()
	{
		const float bonus_duration_seconds = 10.0f;
		const float speed_multiplier = 2.0f;
		if (increaseballspeed_completion_time < Time.time)
			{
				increaseballspeed_completion_time = Time.time + bonus_duration_seconds;
				float bonus_velocity = level_velocity_multiplier * speed_multiplier;
				Ball.SetVelocityMultiplier(bonus_velocity);
			}
	}
	
	//turn off performs in Update()
	static void DecreaseBallSpeedBonus ()
	{
		const float bonus_duration_seconds = 10.0f;
		const float speed_multiplier = 0.5f;
		if (decreaseballspeed_completion_time < Time.time)
			{
				decreaseballspeed_completion_time = Time.time + bonus_duration_seconds;
				float bonus_velocity = level_velocity_multiplier * speed_multiplier;
				Ball.SetVelocityMultiplier(bonus_velocity);
			}
	}
	

//----------------------------Save Records Block----------------------------------------------------------------------------------------------	
//--------------------------------------------------------------------------------------------------------------------------------------------	
	void OnGUI ()
	{
		if (show_save_result_window == true) ShowSaveResultWindow();
		if (show_best_results == true) ShowBestResults();

	}
	
	
	static string string_to_edit = "";
	void ShowSaveResultWindow()
	{
		int window_size_x = 200;
		int window_size_y = 100;
		GUILayout.BeginArea(new Rect(Screen.width/2 - window_size_x/2, Screen.height/2 - window_size_y/2, window_size_x, window_size_y));
		GUILayout.BeginVertical ("box");
		GUILayout.Label("Enter your name");
   		string_to_edit = GUILayout.TextField(string_to_edit); 
		if (GUILayout.Button("Save result"))
		{
			show_save_result_window = false;
			SavePlayerResult(string_to_edit);
			GetBestResults();
			return;
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void ShowBestResults()
	{
		const int table_lenght = 10;
		const int max_length_of_displayed_name = 15;
		
		int j = 0;
		foreach(KeyValuePair<string,int> pair in sorted_results)
			{
				string result_key;
				if (pair.Key.Length > max_length_of_displayed_name) result_key = pair.Key.Remove(max_length_of_displayed_name);
				else result_key = pair.Key;
				records_table[j,0].guiText.text = result_key;
				records_table[j,1].guiText.text = pair.Value.ToString();
				j++;
				if (j == table_lenght) break;
			}
	}
	
	void SavePlayerResult (string player_name)
	{
		System.IO.StreamWriter file = new System.IO.StreamWriter("Assets/My_Resources/Records.txt", true);
		if (player_name.Length == 0) player_name = "Unknown_Player";
		//space will be separator, so - replace spaces in player_name to other symbol
		string corrected_player_name = player_name.Replace(' ', '_');
		file.WriteLine(corrected_player_name + " " + player_score.ToString());
		file.Close();
	}
	
	void GetBestResults ()
	{
		//Get all results from file
		sorted_results.Clear();
		System.IO.StreamReader file = new System.IO.StreamReader("Assets/My_Resources/Records.txt");
		string next_line = null;
		while((next_line = file.ReadLine()) != null)
		{
			string [] data_lines = next_line.Split(new System.Char [] {' '});
			if (data_lines.Count() == 2) sorted_results.Add(new KeyValuePair<string,int>(data_lines[0], System.Int32.Parse(data_lines[1])));
			//Debug.Log("New Line = " + next_line + "Parsed = " + System.Int32.Parse(data_lines[1]));
		}
		file.Close();
		
		//sort results
		sorted_results.Sort(CompareKeyPairValues);
		show_best_results = true;
	}
	
	//Comparer for list of player results
	static int CompareKeyPairValues(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
    {
		return b.Value.CompareTo(a.Value);
    }
	
}
