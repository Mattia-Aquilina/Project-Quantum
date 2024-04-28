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
    [SerializeField] RangeBoard board;

    [Header("Range Settings")]
    [SerializeField] int NumberOfTries = 20;
    [SerializeField] float SpawnDuration = 1f;
    
    [SerializeField] GameObject TopLeftCorner;
    [SerializeField] GameObject TopRightCorner;
    [SerializeField] GameObject BottomLeftCorner;
    [SerializeField] GameObject BottomRightCorner;

    [Header("Practice Data")]
    private int currentKills = 0;

    public static Action<int> PracticeStarted;
    public static Action PracticeEnded;
    public static Action<int> MannequinKilled;
    public static Action<int> MannequinSpawned;

    public bool ShootingTestStarted = false;
    private Mannequin instance;
    public void StartTest()
    {
        PracticeStarted.Invoke(NumberOfTries);
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
            MannequinSpawned.Invoke(spawns);
            instance.Die += OnMannequinKilled;

            yield return new WaitForSeconds(SpawnDuration);
            if(instance.gameObject != null) Destroy(instance.gameObject);
        }
        ShootingTestStarted = false;
        PracticeEnded.Invoke();
    }

    private Vector3 GetRandomPostion()
    {
        var position = new Vector3();

        var bottomRandPostion = Vector3.Lerp(BottomLeftCorner.transform.position, BottomRightCorner.transform.position, UnityEngine.Random.Range(0f, 1f));
        var TopRandPostion = Vector3.Lerp(TopLeftCorner.transform.position, TopRightCorner.transform.position, UnityEngine.Random.Range(0f, 1f));


        position = Vector3.Lerp(bottomRandPostion, TopRandPostion, UnityEngine.Random.Range(0f, 1f));

        return position;
    }

    private void OnMannequinKilled()
    {
        currentKills++;
        MannequinKilled.Invoke(currentKills);
    }
}
