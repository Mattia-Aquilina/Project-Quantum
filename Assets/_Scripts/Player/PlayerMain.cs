using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{

    [Header("Camera Control")]
    [SerializeField] float sensX = 800;
    [SerializeField] float sensY = 800;

    [Header("Movement")]
    [SerializeField] float movSpeed = 10f;
    [SerializeField] float groundDrag;

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
    [SerializeField] GameObject cameraHolder;
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
        if(weaponsModel)
            weaponsModel.parent = camera.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Ground check

        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, playerHeight * .5f + .2f, groundMask);


        Inputs();

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
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && grounded && canJump)
            Jump();
    }

    private void CameraMovement()
    {
        //Center Camera in the current position
        camera.transform.position = cameraHolder.transform.position;
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
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
            rigidbody.AddForce(moveDir * movSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rigidbody.AddForce(moveDir * movSpeed * 10f * AirMovFactor, ForceMode.Force);
    }

    private void SpeedControl()
    {
        var vel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if(vel.magnitude > movSpeed)
        {
            var limitedVelocity = vel.normalized * movSpeed;
            rigidbody.velocity = new Vector3(limitedVelocity.x,rigidbody.velocity.y, limitedVelocity.z );
        } 
    }

    private void Jump()
    {
        canJump = false;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump() => canJump = true;


}
