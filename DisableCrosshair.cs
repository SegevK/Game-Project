using UnityEngine.UI;
using UnityEngine;

//this script disables the crosshair on the lobby camera

public class DisableCrosshair : MonoBehaviour
{

    //Camera LobbyCamera;
    
    private Image crosshair;

    void Start()
    {
        //LobbyCamera = Camera.main;
        crosshair = GetComponent<Image>();
        crosshair.enabled = false;
    }

 /*
    void Update()  // MAKE THE METHOD RUN ONLY UNTIL THE PLAYER JOINS THE GAME
    {
        if (LobbyCamera.gameObject.activeSelf)
            crosshair.enabled = false;
    }
    

    public void disableCrosshair()
    {
        crosshair.enabled = false;
    }

    public void enableCrosshair()
    {
        crosshair.enabled = true;
    }

    */

}
