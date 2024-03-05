using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Debug ONLY")]
    [SerializeField] TextMeshProUGUI text;
    private string[] StateEncode = { "slowWalking", "walking", "crouching", "air", "airCrouching", "idle" };

    [Header("Camera Control")]
    [SerializeField] float sensX = 800;
    [SerializeField] float sensY = 800;

    [Header("Movement")]
    float movSpeed = 10f;
    [SerializeField] float slowWalkSpeed = 3f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float groundDrag;

    [Header("Crouching")]
    [SerializeField] private GameObject scalableObjectsContainer;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;

    [Header("Keybindings")]
    [SerializeField] KeyCode ShiftKey = KeyCode.LeftShift;
    [SerializeField] KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Slope Control")]
    [SerializeField] float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask; 
    [SerializeField] float playerHeight;
    bool grounded;

    [Header("Air Controll")]
    [SerializeField] float AirMovFactor = 0.4f;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpCooldown = 0.2f;
    bool canJump = true;

    //Object components
    [Header("Camera Controls")]
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] GameObject cameraHolder;


    private PlayerMovementState movState = PlayerMovementState.idle;
    [SerializeField] Transform orientation;
    [SerializeField] Transform weaponsModel;
    [SerializeField] Camera camera;
    [SerializeField] Rigidbody rigidbody;
   

    //local variables

    //rotation holders
    float xRotation = 0;
    float yRotation = 0;

    //keyboard input
    float horizontalInput;
    float verticalInput;
    Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        //FORSE DOVREBBE ESSERE SPOSTATO IN UN ALTRO SCRIPT
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startYScale = scalableObjectsContainer.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUG
        if(text != null) text.text  = "Speed: " + rigidbody.velocity.magnitude + " State: " + StateEncode[((int)movState)];
        //Ground check
        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, playerHeight * .5f + .2f, groundMask);

       
        Inputs();
        StateHandler();


        if (grounded)
            rigidbody.drag = groundDrag;
        else
            rigidbody.drag = 0;

        ///CAMERA MOVEMENT
        CameraMovement();
    }
    private void FixedUpdate()
    {
        //physics needs always to be computed in fixedUpdate
        MovePlayer();
        SpeedControl();
    }

    private void StateHandler()
    {
        switch (movState)
        {
            case PlayerMovementState.slowWalking:
                movSpeed = slowWalkSpeed;
                break;
            case PlayerMovementState.walking:
                movSpeed = walkSpeed;
                break;
            case PlayerMovementState.crouching:
                movSpeed = slowWalkSpeed;
                break;
            case PlayerMovementState.air:
                break;
            case PlayerMovementState.idle:
                break;
            default:
                break;
        }
    }

    private void Inputs()
    {
        var shifting = Input.GetKey(ShiftKey);
        var crouching = Input.GetKey(CrouchKey);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //state settings

        if (!grounded && crouching) movState = PlayerMovementState.airCrouching;
        else if (!grounded) movState = PlayerMovementState.air;
        else if (horizontalInput == 0 && verticalInput == 0)
            movState = PlayerMovementState.idle;
        else if (crouching)
            movState = PlayerMovementState.crouching;
        else if (shifting)
            movState = PlayerMovementState.slowWalking;
        else
            movState = PlayerMovementState.walking;


        //input management

        if (Input.GetKeyDown(CrouchKey))
        {
            scalableObjectsContainer.transform.localScale = new Vector3(1, crouchYScale, 1);
            rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
            
        if (Input.GetKeyUp(CrouchKey))
            scalableObjectsContainer.transform.localScale = new Vector3(1, startYScale, 1);
        
        if (Input.GetButtonDown("Jump") && grounded && canJump)
            Jump();
    }

    private void CameraMovement()
    {
        //Center Camera in the current position
        //weaponsModel.transform.position = cameraHolder.transform.position;
        camera.transform.position = cameraHolder.transform.position + cameraOffset;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void MovePlayer()
    {
        moveDir = orientation.forward.normalized * verticalInput + orientation.right.normalized * horizontalInput;

        if(OnSlope())
            rigidbody.AddForce(GetSlopeMoveDirection() * movSpeed * 10f, ForceMode.Force);

        if (grounded)
            rigidbody.AddForce(moveDir * movSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rigidbody.AddForce(moveDir * movSpeed * 10f * AirMovFactor , ForceMode.Force);

        //rigidbody.useGravity = !OnSlope() || exitingSlope;
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rigidbody.velocity.magnitude > movSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * movSpeed;
        }
        else
        {
            var vel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

            if(vel.magnitude > movSpeed)
            {
                var limitedVelocity = vel.normalized * movSpeed;
                
                rigidbody.velocity = new Vector3(limitedVelocity.x,rigidbody.velocity.y, limitedVelocity.z);
            }
        }
    }

    private void Jump()
    {
        canJump = false;
        exitingSlope = true;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    private void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
    }

    /// <summary>
    /// Check if the player is standing on a slope
    /// </summary>
    /// <returns>true or false whether the player is on the slope or not.</returns>
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); //normal è la normale alla superficie colpita
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection() => Vector3.ProjectOnPlane(moveDir.normalized, slopeHit.normal);
}


public enum PlayerMovementState { 
    slowWalking,
    walking,
    crouching,
    air,
    airCrouching,
    idle,
}



