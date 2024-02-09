using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class char_controller : MonoBehaviour
{
    
    private Vector3 moveDirection;
    private char_controller controller;

    private Animator animator;

    private bool isWalkingArmaPressed = false;

    void Start()
    {
        controller = GetComponent<char_controller>();
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            isWalkingArmaPressed = true;
        }
        // Se il tasto W è rilasciato, imposta isWalkingArmaPressed a false
        else if (Input.GetKeyUp(KeyCode.W))
        {
            isWalkingArmaPressed = false;
        }

        // Imposta il parametro dell'animator solo se il tasto è attualmente premuto
        animator.SetBool("corsa", isWalkingArmaPressed);

        

    }
}
