using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerManager))] //so we will always get a valid component when calling the PM//

// This script is Setting up the player, and Adding/Removing him To/From the network

public class PlayerSetup : NetworkBehaviour 
{
	
	[SerializeField] Behaviour[] ComponentsToDisable;
	[SerializeField] GameObject[] GameObjectsToDisable;
	

	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;
		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, newLayer);
		}
		return;
	}


	void Start()    
	{

		if (!isLocalPlayer)
		{
			for (int i = 0; i < ComponentsToDisable.Length; i++)
				ComponentsToDisable[i].enabled = false;
			
			for (int j = 0; j < GameObjectsToDisable.Length; j++)
				SetLayerRecursively(GameObjectsToDisable[j], 0);

		}

		else   //Only the local player enters the else statment
		{
			//after finished all the start script,we are ready to call the Setup method in PlayerManager Script
			GetComponent<PlayerManager>().Setup();    
		}
		
	}


	// This Function is called every time a player is SetUp localy //
	//
	public override void OnStartClient()
	{
		base.OnStartClient();
		string NetId = GetComponent<NetworkIdentity>().netId.ToString();
		PlayerManager playerManager = GetComponent<PlayerManager>();

		GameManager.RegisterPlayer(NetId, playerManager);
	}


	void OnDisable()
	{
		if (isLocalPlayer)
		{
			GameManager.instance.SetLobbyCameraActive(true);
		}

		//Enables the lobby camera on local player death
		// every time a player is disconecting Or Destroyed(died),**need to seperate to 2 cases later//
		
		GameManager.UnRegisterPlayer (transform.name); 
	}

}
