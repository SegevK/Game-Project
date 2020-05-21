using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class ServerBrowserItem : MonoBehaviour
{

    //callback function
    //delegate allows to make a reference to functions
    //these funcions will be executed when JoinGame is called
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallback;

    [SerializeField]
    private Text LobbyNameText;
    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback)
    {
        match = _match;
        joinRoomCallback = _joinRoomCallback;
        LobbyNameText.text = _match.name + " (" + match.currentSize + "/" + match.maxSize + ") ";
    }

    public void JoinRoom()   //leave as JoinRoom because Join Game Dosent work
    {
        joinRoomCallback.Invoke(match);
    }
}
