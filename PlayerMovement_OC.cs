using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement_OC : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    
    public LayerMask groundMask;
    
    public float dashTime = 0f;
    public float dashSpeed = 25;
    public float jumpHeight = 7f;
    public float speed = 17f;
    public float gravity = -45;
    public float groundDistance = 0.4f; 

    bool isGrounded;
    bool canDoubleJump;
    bool isDash = false;

    //InputActions
    InputMaster controls;

    // Move
    Vector2 movementInput;
    Vector3 velocity;


    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        
    }

    void Update()
    {
        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            canDoubleJump = true;
            isDash = false;
        }
        

        float x = movementInput.x;
        float z = movementInput.y;

        Vector3 move = transform.right * x + transform.forward * z;
      
        controller.Move(move * speed * Time.deltaTime);

        if (kb.spaceKey.wasPressedThisFrame && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        // the seconed jump//
        if (kb.spaceKey.wasPressedThisFrame && !isGrounded && canDoubleJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity * 0.6f);
            canDoubleJump = false;
        }

        // leaping forward //
        if (kb.leftShiftKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity * 0.3f);
            isDash = true;
            StartCoroutine(dash(dashTime, dashSpeed));
            
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

   
    IEnumerator dash(float time, float dashSpeed)
    {
        for (int i = 0; i < 1000000; i++)
        {
            yield return new WaitForSeconds(time / 100);
            this.transform.position += this.transform.forward * dashSpeed / 100;
            if(!isDash)
            {
                break;
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

}

// STUFF TO FIX

//1.
//If an object is above the character when they jump, the character will still jump 
//and continue to move against the object.
//--*try to use collision flags.

//2.
// Trying to jump onto something can cause a stutter/jitter effect because the 
// slope limit (it should be adjusted when you jump and then readjusted when you land)


//3.
//the movement vector isn't normalized, resulting in an increased speed if 
//moving at a diagonal angle.

//4.
//add a cooldonwn timer for the dash.
//Find a way to dash forward(the y axis works, just need to make the player lunch 
//forward, towards where he is looking/aiming, AND with increased speed.(*maybe change the dash to direction of the pressed key-'d' and shift = dash right,and so on)