using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected ScriptableWeaponModel scriptableWeaponModel;
    [SerializeField] protected ScriptableWeapon weaponData;
    [SerializeField] GameObject BulletTracerStartingPostion;
    [SerializeField] Vector3 offset;
    [SerializeField] Camera camera;
    [SerializeField] PlayerMovement playerRef;
    protected AnimatorController controller;
    private object shootOutcome;
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Reload = Animator.StringToHash("Reload");
    public static readonly int Shoot = Animator.StringToHash("Shoot");
    public static readonly int Inspect = Animator.StringToHash("Inspect");
    public static readonly int Take = Animator.StringToHash("Take");

    private void Awake()
    {
        controller = scriptableWeaponModel.controller;
        scriptableWeaponModel.Spawn();
    }

    private void Start()
    {
        animator.CrossFade(Idle, 0, 0);
    }

    private void Update()
    {
       
    }

    public void PlayAnimation(int animationCode)
    {
        float speed;
        if (animationCode == Reload) speed =(scriptableWeaponModel.ReloadAnimationDuration / weaponData.reloadTime );
        else if (animationCode == Shoot) speed = scriptableWeaponModel.ShootAnimationDuration * (float)weaponData.fireRate;
        else speed = 1;
 
        animator.speed = speed;
        animator.CrossFade(animationCode, 0, 0);
    }

    public void BulletTracer(List<ShootOutcome> trailData, Vector3 ShootDirection)
    {
        // shootOutcome[shootOutcome.Count - 1]
        //ONLY FOR SHOOT ANIMATION

       // var _offset = BulletTracerStartingPostion.transform.position - playerRef.cameraHolder.transform.position;
       // var startingPoint = playerRef.cameraHolder.transform.position;

       // startingPoint += camera.transform.right;
        //startingPoint += camera.transform.up ;
        //startingPoint += camera.transform.forward;

        var startingPoint = BulletTracerStartingPostion.transform.position;

        
        // provare ad agganciare tracerstartposition al corpo
        float downwardOffset = 0.2f; 
        startingPoint += BulletTracerStartingPostion.transform.up * -downwardOffset;




        if (trailData.Count != 0)
        {
            StartCoroutine(scriptableWeaponModel.PlayTrail(
                            startingPoint,
                            trailData[0].hitPoint)
                            );
            Debug.DrawRay(startingPoint,
                            trailData[0].hitPoint - startingPoint, Color.magenta, 2f);
        }
        else
        {
            StartCoroutine(scriptableWeaponModel.PlayTrail(
                           startingPoint,
                           startingPoint + ShootDirection * scriptableWeaponModel.BulletTracerConfig.MissDistance)
                           );
            Debug.DrawRay(startingPoint,
                           ShootDirection * scriptableWeaponModel.BulletTracerConfig.MissDistance, Color.magenta, 2f);

        }

    }


}
