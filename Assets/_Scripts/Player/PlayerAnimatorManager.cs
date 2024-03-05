using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe che si occupa di gestire le animazioni del player
/// </summary>
public class PlayerAnimatorManager : MonoBehaviour
{
    //ANIMATIONS CODE
    [Header("Animator Components")]
    [SerializeField] private Animator animator;
    //Animations
    public static readonly int Idle = Animator.StringToHash("Armature|Idle");
    public static readonly int Reload = Animator.StringToHash("Armature|Reload");
    public static readonly int Shoot = Animator.StringToHash("Armature|Shoot");


    // Start is called before the first frame update
    void Start()
    {
        animator.CrossFade(Idle, 0, 0);
    }

    public void AskForAnimation(int Animation) => animator.CrossFade(Animation, 0, 0);
    
}
