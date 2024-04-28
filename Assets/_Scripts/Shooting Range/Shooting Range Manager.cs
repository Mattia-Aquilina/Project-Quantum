using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShootingRangeManager : Singleton<ShootingRangeManager>
{
    [Header("Prefabs")]
    [SerializeField] Mannequin mannequinPrefab;
    [SerializeField] ShootingText shootingTextPrefab;

    [Header("Range Settings")]
    [SerializeField] int NumberOfTries = 20;
    [SerializeField] float SpawnDuration = 1f;
    [SerializeField] GameObject TopLeftCorner;
    [SerializeField] GameObject TopRightCorner;
    [SerializeField] GameObject BottomLeftCorner;
    [SerializeField] GameObject BottomRightCorner;

    public bool ShootingTestStarted = false;
    private Mannequin instance;
    public void StartTest()
    {
        if (!ShootingTestStarted)
        {
            ShootingTestStarted = true;
            StartCoroutine(Test());
        }
    }

    private IEnumerator Test(){
        var spawns = 0;
        while (spawns < NumberOfTries){
            yield return new WaitForSeconds(SpawnDuration);

            spawns++;
            instance = Instantiate(mannequinPrefab, GetRandomPostion(), Quaternion.identity);

            yield return new WaitForSeconds(SpawnDuration);
            if(instance.gameObject != null) Destroy(instance.gameObject);
        }
        ShootingTestStarted = false;
    }

    private Vector3 GetRandomPostion()
    {
        var position = new Vector3();

        var bottomRandPostion = Vector3.Lerp(BottomLeftCorner.transform.position, BottomRightCorner.transform.position, UnityEngine.Random.Range(0f, 1f));
        var TopRandPostion = Vector3.Lerp(TopLeftCorner.transform.position, TopRightCorner.transform.position, UnityEngine.Random.Range(0f, 1f));


        position = Vector3.Lerp(bottomRandPostion, TopRandPostion, UnityEngine.Random.Range(0f, 1f));

        return position;
    }
}
