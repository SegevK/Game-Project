using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using TMPro;
using UnityEngine.UI;


public class UserDataManager : MonoBehaviour
{
	private const string API = "https://finalproject-3ec77.firebaseio.com/players/";

	public static string userName;
	string password;

	public static UserDataManager instance;
	
	public float timeOutLength = 5f;   //Time Spent trying to connect to a game, until it times out

	public SaveData playerData;

	[Space]
	public InputField user;
	public InputField pass;
	public TextMeshProUGUI error;
	public TextMeshProUGUI greeting;
	[Space]
	public GameObject menu;
	public GameObject signInMenu;


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		
		else
		{
			Destroy(this.gameObject);
		}
	}

	public void onStart()
	{
		getMenuUI();
		if (userName != null && userName != "")
		{
			signInValid();
		}
		else
		{
			menu.SetActive(false);
			signInMenu.SetActive(true);
			error.text = "";
		}
	}
	

	public void getMenuUI()
	{
		user = GameObject.Find("Name").GetComponent<InputField>();
		pass = GameObject.Find("Password").GetComponent<InputField>();
		menu = GameObject.Find("Lobby");
		signInMenu = GameObject.Find("Account Menu");
		error = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();
		greeting = GameObject.FindGameObjectWithTag("NameTag").GetComponent<TextMeshProUGUI>();
	}

	public void signIn()
	{
		userName = user.text;
		password = pass.text;
		getUserData();
	}

	void checkForOtherNames()
	{
		error.text = "Checking name...";
		StartCoroutine(waitForTimeOutRegister());
		RestClient.Get<SaveData>(API + userName + ".json").Then(response => {
			StopAllCoroutines();

			if (response.password == "" || response.password == null  || response.password == password)
			{
				error.text = "There is a user with this name";
			}

			else
			{
				noOtherPlayers();
			}
		});
	}

	public void register()
	{
		userName = user.text;
		password = pass.text;
		if (user && pass)
		{
			if (checkValidInput())
			{
				checkForOtherNames();    
			}

			else
			{
				user.text = "";
				pass.text = "";
				error.text = "This Name Is Taken";
			}
		}
		else
		{
			Debug.LogError("An error occured");
		}
	}


	public bool checkValidInput()   //set restrictions on name and password that are chosen in this method
	{
		string us = userName;
		string p = password;

		return !(string.IsNullOrWhiteSpace(us) || us.Trim().Length == 0 || string.IsNullOrEmpty(us) || string.IsNullOrWhiteSpace(p) || p.Trim().Length == 0 || string.IsNullOrEmpty(p) || us.Length <= 2 || us.Length > 20 || p.Length <= 2 || p.Length > 20);
	}


	void noOtherPlayers()
	{
		SaveData newData = new SaveData();
		newData.userName = userName;
		newData.password = password;
		RestClient.Put(API + userName + ".json", newData);
		playerData = newData;
		signInValid();
	}



	void checkPassword()
	{
		if(password == playerData.password)
		{
			signInValid();	
		}
		else
		{
			signInInvalid();
		}
	}


	void signInValid()   //after a user signs in to his account
	{
		greeting.text = "Playing As " +userName;
		signInMenu.SetActive(false);
		menu.SetActive(true);
	}


	void signInInvalid()    //if the details entered to sign in/up are invalid
	{
		user.text = "";
		pass.text = "";
		error.text = "Invalid sign-in info";
	}


	void getUserData()
	{
		StartCoroutine(waitForTimeOut());
		RestClient.Get<SaveData>(API + userName + ".json").Then(response => {	
		playerData = response;
		StopAllCoroutines();
		checkPassword();
				
		});
		
	}


	public void saveUserData()   // Saves the user name and password in the data base Under an Account called the same as the username entered
		
	{
		SaveData.instance.userName = userName;
		SaveData.instance.password= password;
		RestClient.Put(API + userName + ".json", SaveData.instance);     //the savedata.instance contains the username and password
	}


	IEnumerator waitForTimeOut()
	{
		yield return new WaitForSeconds(timeOutLength);
		if(playerData.password == null || playerData.userName == "")
		{
			error.text = "Invalid Player Name";
			Debug.LogError("Invalid player name");
		}
	}


	IEnumerator waitForTimeOutRegister()
	{
		yield return new WaitForSeconds(timeOutLength/2f);
		noOtherPlayers();
	}


	


	public void signOut()
	{
		saveUserData();       //WHEN THIS IS ACTIVE THE USER name and data are set to "" IN THE DATABASE AFTER LOGGING OUT,BUT WHEN DISABLED A USER REGISTER OVERIDES AN EXISTING USER!!
		
		user.text = "";
		pass.text = "";
		error.text = "";
		menu.SetActive(false);
		signInMenu.SetActive(true);
		
		
	}
}



[System.Serializable]
public struct SaveData
{
	public string userName;
	public string password;
	public static SaveData instance;
}
