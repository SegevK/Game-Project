using UnityEngine;
using System.Collections;

// FIX THE ROTATION PROBLEMS!!!!! \\


public class LadderClimb : MonoBehaviour
{

    //do we want the character to climb or not
    bool Climbing = false;
    //the speed we want the character to travel at
    public float speed = 5.0f;
    //the character who is climbing this ladder    
    Transform target;


    private void OnTriggerStay(Collider other)
    {
        //gets the person who touches the ladder
        //if the person hits enter start the climbing
        //if we are climbing stop climbing
        if (Input.GetKey(KeyCode.Return) && !Climbing)
        {
            target = other.transform;
            Climbing = true;
            return;
            // any code you need to turn back on control of your playercontroller goes here
            // this by default is setup for the character motor
            // its turn the control,gravity and the stepoffset on
        }

        
        if (Climbing)
        {
            target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
            //locks the players x,z axis to the ladders since we only need to move on the y axis

            //this allows the player to move up and down the ladder
            if (Input.GetKey(KeyCode.W))
            {
                target.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                target.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
            }

            else if (Input.GetKey(KeyCode.Return))
            {
                Climbing = false;
                target.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
                target.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
                target = null; 
                return;
            }

        }

    }

    // any code you need to turn back on control of your playercontroller should go here
    // this by default is setup for the character motor
    // it turn the control,gravity and the stepoffset on
    // also pushes the character forward so in case the character is on top of the ladder
    private void OnTriggerExit(Collider other)
    {
        if (target)
        {
            Climbing = false;
            target.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
            target.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
            target.GetComponent<Rigidbody>().AddForce(Vector3.up * 500 + target.forward * 300);
            target = null;
            return;
        }
    }
}