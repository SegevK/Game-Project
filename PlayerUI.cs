using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	/*    LEAVE FOR LATER
	[SerializeField]
	RectTransform thrusterFuelFill;

	[SerializeField]
	RectTransform healthBarFill;

	[SerializeField]
	Text ammoText;
	
	[SerializeField]
	GameObject scoreboard;
    */
	
	[SerializeField]
	GameObject pauseMenu;

	
	// ***THREE CASES TO DEAL WITH: 1.On Player Join Game OR spawn/3.ON PLAYER DEATH*** 
	
	
	[SerializeField]
	Image Crosshair;
	
	
	/*
	private PlayerManager player;
	private PlayerController controller;
	private WeaponManager weaponManager;
	

	public void SetPlayer (PlayerManager _player)
	{
		
		player = _player;
		
		controller = player.GetComponent<PlayerController>();
		weaponManager = player.GetComponent<WeaponManager>();
	   
	}

	*/

	void Start ()
	{
		PauseMenu.IsOn = false;
		Crosshair.enabled = true;
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			TogglePauseMenu();

		if (PauseMenu.IsOn)
			Crosshair.enabled = false;

		else
		{
			Crosshair.enabled = true;
			pauseMenu.SetActive(false);
			if (Input.GetButton("Fire2"))
				Crosshair.enabled = false;
		}
		

		/*    USE FOR SCOREBOARD -- NOT YET IMPLEMENTED
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			scoreboard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
		{
			scoreboard.SetActive(false);
        }
		*/
	}

	public void TogglePauseMenu ()    //toggles the pause menue And disables/enables the mouse cursur      FIX PROBLEM IN BUILD THAT CUSRSUR ISNT VISIBLE
	{
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.IsOn = pauseMenu.activeSelf;
		
		if (PauseMenu.IsOn)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
			
	}
}
