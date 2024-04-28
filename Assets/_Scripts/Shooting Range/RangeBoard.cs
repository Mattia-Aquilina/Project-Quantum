using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBoard : MonoBehaviour
{
    [SerializeField] TextMesh KillCount;
    [SerializeField] TextMesh RemainingCount;

    private int numberOfMan = 0;    
    private void Awake()
    {
        InitBoard();
        ShootingRangeManager.PracticeStarted += OnPracticeStart;
        ShootingRangeManager.PracticeEnded += OnPracticeEnd;
        ShootingRangeManager.MannequinKilled += OnMannequinKilled;
        ShootingRangeManager.MannequinSpawned += OnMannequinSpawn;

    }

    private void OnPracticeStart(int NumberOfTries)
    {
        RemainingCount.text = NumberOfTries.ToString();
        numberOfMan = NumberOfTries; 
    }

    private void OnPracticeEnd()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        KillCount.text = "0";
        RemainingCount.text = "0";
    }

    private void OnMannequinKilled(int currentKills)
    {
        KillCount.text= currentKills.ToString(); 
    }
    private void OnMannequinSpawn(int current)
    {
        RemainingCount.text =  (numberOfMan - current).ToString();
    }
}
