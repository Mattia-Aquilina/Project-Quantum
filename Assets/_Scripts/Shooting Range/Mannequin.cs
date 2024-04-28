using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.UI;


public class Mannequin : MonoBehaviour,IDamageable
{
    [Header("Mannequin Settings")]
    [SerializeField] private bool canDie  = true;
    [SerializeField] private bool showDamage = false;
    public float healt { get; private set; } = 150f;

    [Header("Components")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip destroyAudio;
    [SerializeField] GameObject mannequinBody;

    [Header("Text pop ups")]
    [SerializeField] ShootingText textPrefab;
    [SerializeField] GameObject textSpawnPoint;


    public Action Die;


    // Update is called once per frame

    public void Damage(float Damage)
    {
        healt -= Damage;

        //Display a text with the damage;
        if (showDamage)
            Instantiate(textPrefab, textSpawnPoint.transform.position, Quaternion.identity).Enable(Damage.ToString());


        if (healt < 0)
        {
            healt = 0;
            OnDie();
        }
            
    }

    private void OnDie()
    {
        Die?.Invoke();
        if (!canDie) return;

        mannequinBody.SetActive(false);
        audioSource.PlayOneShot(destroyAudio);


        Invoke(nameof(destroy), destroyAudio.length + 0.1f);
        //logic for the mannequin on die
    }

    private void destroy() => Destroy(gameObject);
}
