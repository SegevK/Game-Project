using UnityEngine;
using UnityEngine.Networking;
 
public class PlayerSetup : NetworkBehaviour {
	[SerializeField] Behaviour[] ComponentsToDisable;
	Camera LobbyCamera;
	void Start()    //*try to use Awake instead of start //
	{
		if (!isLocalPlayer)
		{
			for (int i = 0; i < ComponentsToDisable.Length; i++)
			{
				ComponentsToDisable[i].enabled = false;
			}
		}
		else
		{
			LobbyCamera = Camera.main;
			if (LobbyCamera != null)
			{
				Camera.main.gameObject.SetActive(false);
			}
			
		}
	
	}
	void OnDisable()
	{
		if (LobbyCamera != null)
		{
			LobbyCamera.gameObject.SetActive(true);
		}
	}

}
