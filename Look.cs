using UnityEngine;
using UnityEngine.InputSystem;


public class Look : MonoBehaviour
{

    InputMaster inputControls;

    public CharacterController myCC;
    private float cameraAngle = 0;
    
    private float horizontalSensitivity = 3f;
    private float verticalSensitivity = 3f;

   
    //vertical look varivables:
    [SerializeField] private Transform cameraTransform;
    private float minCamAngle = -89f;
    private float maxCamAngle = 89f;
   

    private void Awake()
    {
        inputControls = new InputMaster();
        Cursor.lockState = CursorLockMode.Locked;
        inputControls.Player.MouseX.performed += RotationAxisY;
        inputControls.Player.MouseY.performed += CameraRoationAxisX;
        myCC = GetComponent<CharacterController>();
    }

    private void RotationAxisY(InputAction.CallbackContext context)
    {
        //Reads input value from delta x of the mouse movement
        float yRotate = context.ReadValue<float>();
        //Rotates this transform in the y-axis base of the input value
        transform.Rotate(new Vector3(0, yRotate, 0) * horizontalSensitivity * Time.deltaTime);
    }

    private void CameraRoationAxisX(InputAction.CallbackContext context)
    {
        //Adding or subtracting to the current x rotation of the camera transform
        cameraAngle += context.ReadValue<float>() * verticalSensitivity * Time.deltaTime;
        //Restricting the angle of rotation
        cameraAngle = Mathf.Clamp(cameraAngle, minCamAngle, maxCamAngle);
        //Applying the new rotation to the camera transform
        cameraTransform.localEulerAngles = new Vector3(-cameraAngle, 0, 0);
    }

    private void OnEnable()
    {
        //Regestering to action 
        inputControls.Enable();

    }

    private void OnDisable()
    {
        //Unregistering from action events
        inputControls.Disable();
    }

  
}