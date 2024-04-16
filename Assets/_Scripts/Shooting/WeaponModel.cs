using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected ScriptableWeaponModel scriptableWeaponModel;
    [SerializeField] protected ScriptableWeapon weaponData;
    protected AnimatorController controller;

    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Reload = Animator.StringToHash("Reload");
    public static readonly int Shoot = Animator.StringToHash("Shoot");
    public static readonly int Inspect = Animator.StringToHash("Inspect");
    public static readonly int Take = Animator.StringToHash("Take");

    [Header("Debug Only")]
    [SerializeField] AnimatorController _controller;
    private void Awake()
    {
        controller = scriptableWeaponModel.controller;

    }

    private void Start()
    {
        animator.CrossFade(Idle, 0, 0);
    }

    public void PlayAnimation(int animationCode)
    {
        if (animationCode == Reload) animator.speed = weaponData.reloadTime/scriptableWeaponModel.ReloadAnimationDuration ;
        if (animationCode == Shoot) animator.speed = (1/weaponData.fireRate) / scriptableWeaponModel.ReloadAnimationDuration;
        else animator.speed = 1;

        animator.CrossFade(animationCode, 0, 0);
    }


}
