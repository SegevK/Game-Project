using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public MatchSettings matchSettings;
	public static GameManager instance;    

	[SerializeField]
	private GameObject LobbyCamera;

	void Awake()
	{
		if (instance != null)  //if instance isnt null it means that there is another game manager in the scene
			Debug.LogError("There is more than one Game Manager object in the scene");
		
		else
			instance = this;
	}

	//Activates The lobby Camera Object For a dead local player-with its components(Audio listener,etc...)
	public void SetLobbyCameraActive(bool IsActive)
	{
		if (LobbyCamera == null)
			return;


		LobbyCamera.SetActive(IsActive);
	}

	//#region = Enabales the option to collapse the script from the first # to the second one in the bottom
	#region Player Data Tracking   


	private const string PLAYER_ID_PREFIX = "Player ";
	private static Dictionary<string, PlayerManager> Players = new Dictionary<string, PlayerManager>();

	// returns the player component from the list with the name that is sent// 
	public static PlayerManager GetPlayer(string playerID)
	{
		return Players[playerID];
	}
	
	// players get their id based on how many players have joined before them - the seconed player to join the game has the id 2//
	public static void RegisterPlayer(string NetId, PlayerManager playerManeger)
	{
		string PlayerId = PLAYER_ID_PREFIX + NetId;
		Players.Add(PlayerId, playerManeger);
		playerManeger.transform.name = PlayerId;
	}


	public static void UnRegisterPlayer(string playerId)
	{
		Players.Remove(playerId); //removes the player from the dictionary
	}


	/*   Add a On hold key(tab) to enable this gui for scoreboard
	//Creats a list of all players currently connected to the lobby on the screen
	void OnGUI () 
	{
		GUILayout.BeginArea(new Rect(300, 300, 300, 600));
		GUILayout.BeginVertical();
		
		foreach (string playerID in Players.Keys)
		{
			GUILayout.Label (playerID + "  -  " + Players[playerID].transform.name);
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	*/

	#endregion

}
