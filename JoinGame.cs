using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


//dealing with the join game interface

public class JoinGame : MonoBehaviour
{
    List<GameObject> ServerBrowser = new List<GameObject>();
    private NetworkManager networkManager;

    [SerializeField]
    private Text status;

    //prefab button of scroll list
    [SerializeField]
    private GameObject ServerBrowserItemPrefab;

    //scrolllist  
    [SerializeField]
    private Transform ServerBrowserParent;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matches == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshServerBrowser();
    }


   
    public void RefreshServerBrowser()
    {
        ClearServerBrowser();
        //(pages numbers, elements per list, filter, callback method)
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
        if (matchList == null)
        {
            status.text = "Couldn't get Server Browser.";
            return;
        }


        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject ServerBrowserItemGO = Instantiate(ServerBrowserItemPrefab);
            //parent the element to the list
            ServerBrowserItemGO.transform.SetParent(ServerBrowserParent);

            ServerBrowserItem serverBrowserItme = ServerBrowserItemGO.GetComponent<ServerBrowserItem>();
            if (serverBrowserItme != null)
            {
                serverBrowserItme.Setup(match, JoinRoom);
            }

            ServerBrowser.Add(ServerBrowserItemGO);
        }

        if (ServerBrowser.Count == 0)
        {
            status.text = "No room available.";
        }
    }


    private void ClearServerBrowser()     //clears the server browser after joining a game(to display joining text),or after a room is closed)
    {
        for (int i = 0; i < ServerBrowser.Count; i++)
        {
            Destroy(ServerBrowser[i]);
        }

        
        ServerBrowser.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)     
    {
        // the arguments passed are-(netId,password,callback)
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearServerBrowser();
        status.text = "Joining " + _match.name + "...";
    }
}
