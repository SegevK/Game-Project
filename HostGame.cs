using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    private NetworkManager networkManager;

    public uint LobbySize = 6;   //uint  is a diffrent version of int and holds only positive values,6 is the size of the lobby in the services tab
    
    private string LobbyName;

    private void Start()    //starts the unity matchmaker
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();       

    }

    public void SetLobbyName(string LName)
    {
        LobbyName = LName;
    }

    public void CreateLobby()
    {
        if (LobbyName != "" && LobbyName != null)
        {
            Debug.Log("Creating a room" + LobbyName + "with size of " + LobbySize);

            //the bool sent is match advertised = can you see the match when searching
            networkManager.matchMaker.CreateMatch(LobbyName, LobbySize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);   //IF THE PROBLEM WITH


        }
    }


}
