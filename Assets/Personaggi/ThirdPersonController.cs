using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    
    private Vector3 moveDirection;
    private ThirdPersonController controller;

    private Animator animator;

    private bool isWalkingArmaPressed = false;

    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            isWalkingArmaPressed = true;
            animator.SetBool("corsa", isWalkingArmaPressed);
        }
        // Se il tasto W è rilasciato, imposta isWalkingArmaPressed a false
        else if (Input.GetKeyUp(KeyCode.W))
        {
            isWalkingArmaPressed = false;
            animator.SetBool("corsa", isWalkingArmaPressed);
        }
        //denis gay
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            animator.SetBool("ricarica", true);
        }
        
        else if (Input.GetKeyUp(KeyCode.R))
        {
            
            animator.SetBool("ricarica", false);
        }

    }
}
