using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PlayerManager : NetworkBehaviour
{
    private bool didSetup = false;   //traks if the local player has already gone through the setup method atleast once

    [SerializeField]   //used for hp in ui
    Text Hp;

    [SerializeField]
    private GameObject[] disableGameObjOnDeath;
    
    [SerializeField]  
    private GameObject Crosshair;    //used for disabling crossahir on local player death
    
    
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled; //array that stores all the components that got disabled when the Localplayer died


    [SerializeField]
    private int maxHealth = 100;

    [SyncVar] //syncvar means every time the value is changed,it will change for all the players//
    private int currentHealth;

    [SyncVar]
    private bool dead = false;
    public bool IsDead
    {
        get { return dead; }
        protected set { dead = value; } //protected  means that only classes that derive from the playerManager class are able to change this variable
    }


    //we call this method when the playerSetup script is ready,from that script
    //becuse need to check if a component is enabled only after its enabled/disabled in the PlayerSetup Script
    public void Setup() 
    {
        if (isLocalPlayer)
        {
            //Switching Back the cameras On Local Respawn - to Player Camera
            GameManager.instance.SetLobbyCameraActive(false);
        }

        CmdOnNewPlayerSetup();
    }
    

    [Command]
    private void CmdOnNewPlayerSetup()    //telling the server to broadcast the player setup every time for all of the clients
    {
        RpcSetupPlayerOnAll();
    }


    [ClientRpc]     //Add Code To switch Cameras + disable lobby camera on respawn
    private void RpcSetupPlayerOnAll()
    {
        if (!didSetup)  // This if is for Initializing the wasEnabled array only one time-on the first setup
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
                wasEnabled[i] = disableOnDeath[i].enabled;
            
            didSetup = true;
        }
        

        SetDefaultStats();  //the SetDefaultStats function can be used for more pourpeses so we change the current health in it,and NOT in the awake fucntion - can be used later//
    }

    public void SetDefaultStats()
    {
        dead = false;

        currentHealth = maxHealth;

        //Re-Enabling all the Components that were disabled on LocalPlayer Death(scripts,etc...)
        for (int i = 0; i < disableOnDeath.Length; i++)  
            disableOnDeath[i].enabled = wasEnabled[i];
        
        // Re-Enabling all the GameObjects that were disabled on LocalPlayer death(Camera,Gun camera,etc...)
        for (int i = 0; i < disableGameObjOnDeath.Length; i++)
            disableGameObjOnDeath[i].SetActive(true);

        // special case for colliders,because it isnt possible to put them in the array with other components
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

    }

    
    [ClientRpc]  // makes sure it will execute the function for all the players
    public void RpcTakeDamage(int amount)
    {
        if (dead)
            return;
        
        currentHealth -= amount;
        if (currentHealth <= 0)
            KillPlayer();
        else
            Debug.Log(transform.name + "has " + currentHealth + "hp");

    }

    
    private void Update()
    {
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))   //**FOR TESTING** pressing k will automaticly kill the player
            RpcTakeDamage(999);
        if (gameObject.transform.position.y < -10)     //kills the player if he falls off the map   --- (TRY TO PUT THIS IN A DIFFRENT SCRIPT THAT KILLS THE PLAYER IF HE COLLIDES WITH A COLLIDER THAT IS BELLOW THE MAP)
            RpcTakeDamage(999);
        Hp.text = currentHealth.ToString();
    }
    

    private void KillPlayer()
    {
        dead = true;
        Crosshair.SetActive(false);
        //disables all Components on LocalPlayer Death
        for (int i = 0; i < disableOnDeath.Length; i++)
            disableOnDeath[i].enabled = false;

        //Disable GameObjects On LocalPlayer Death
        for (int i = 0; i < disableGameObjOnDeath.Length; i++)
            disableGameObjOnDeath[i].SetActive(false);


        //** Consider Adding all the mesh renderers to GameObjectsToDisable[] array to disable all of them on local death
        //Disables the collider of the local player
        Collider col = GetComponent<Collider>();
        if (col != null)   //disables the collider on local player death,and makes him "dissapear" by sending him 100 units downward
        {
            col.enabled = false;
            Transform PlayerPos = GetComponent<Transform>();
            PlayerPos.position += Vector3.down * 100f;
        }

        //***ADD a death effect when the player dies***

       
        //Switching the cameras On Local Death - to Lobby Camera
        //Remember That the camera Should be Activated AND Switched to
        if (isLocalPlayer)  
            GameManager.instance.SetLobbyCameraActive(true);
        
        Debug.Log(transform.name + "Died");
        StartCoroutine(Respawn());   //this is how to call an IEnumerator method
        
    }


    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.RespawnTimer);
        
        //Respawning the player in a Spawn point(Teleporting him there)
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.05f);  //waiting 0.05 sec so the player will spawn first,and then go to setup fucntion
        Crosshair.SetActive(true);
        Setup();
    }

}
