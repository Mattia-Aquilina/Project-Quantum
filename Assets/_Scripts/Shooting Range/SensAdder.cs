using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensAdder : MonoBehaviour, IDamageable 
{
    //dovrebbe essere assegnata dinamicamente
    [SerializeField] PlayerMovement playerRef;
    [SerializeField] TextMesh currentSens;
    [SerializeField] int SensValue;
    [SerializeField] TextMesh mesh;
    public void Damage(float Damage)
    {
        var newSens = playerRef.GetSense() + SensValue;
        playerRef.SetSense(newSens);
        currentSens.text = playerRef.GetSense().ToString();
    }

    // Start is called before the first frame update
    void Awake()
    {
        mesh.text = SensValue.ToString();
    }
}
