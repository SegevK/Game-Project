using UnityEngine;
using System.Collections;

public class Respawn_On_Fall : MonoBehaviour
{
   
    public GameObject objToTP;
    public Transform tpLoc;
    

    void OnTriggerStay(Collider other)
    {
     
        if (other.gameObject.tag == "Player") 
        {
            objToTP.transform.position = tpLoc.transform.position;
        }
    }
   
}