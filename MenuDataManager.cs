using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDataManager : MonoBehaviour
{
    public UserDataManager manager;

    void Start()
    {
        manager = GameObject.Find("UserDataManager").GetComponent<UserDataManager>();
        manager.onStart();    
    }

    public void signIn()
    {
        manager.signIn();
    }

    public void register()
    {
        manager.register();
    }

    public void signOut()
    {
        manager.signOut();
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
