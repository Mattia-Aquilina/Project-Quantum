using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script principale che gestisce lo stato del giocatore
/// </summary>
public class PlayerMain : MonoBehaviour
{
    // Game Data
    public int money { get; private set; } = 800;
    public int skillPoints { get; private set; } = 1;
    public int health { get; private set; } = 100;
    public int shield { get; private set; } = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

