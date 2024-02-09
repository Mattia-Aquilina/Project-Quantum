using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    //EXTERNAL PARAMETERS
    [SerializeField] float sensX = 800;
    [SerializeField] float sensY = 800;
    [SerializeField] float movSpeed = 10f;
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
        Inputs();
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
    }

    private void CameraMovement()
    {
        //Center Camera in the current position
        camera.transform.position = cameraHolder.transform.position;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        Debug.Log(mouseX + " " + mouseY);

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rigidbody.AddForce(moveDir * movSpeed * 10f);
    }
}
