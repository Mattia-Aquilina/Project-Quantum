using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

/// <summary>
/// Basic class for all the weapons
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{

    [Header("Debug ONLY")]
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] TextMeshProUGUI magText;
    [SerializeField] Camera camera;
    private string[] StateEncode = { "idle", "reloading", "shooting", "inspecting" };
    //Dati dell'arma
    [field: SerializeField]public ScriptableWeapon ScriptableWeapon { get; private set; }

    protected FiringMode firingMode;
    protected WeaponType weaponType;



    //Get them from the keybindings
    [Header("Key bindings")]
    [SerializeField] protected KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode reloadKey = KeyCode.R;

    protected WeaponStates weaponState = WeaponStates.idle;
    protected bool canShoot = true;
    protected bool reloading = false;
    protected float shootLockedTill;
    protected int currentMag;
    protected int totalBullets;
    protected float currentHeat = 0;
    protected float heatLockedTill = 0f;
    // Start is called before the first frame update

    protected virtual void Awake()
    {
        currentMag = ScriptableWeapon.magSize;
        totalBullets = ScriptableWeapon.NumberOfMags * ScriptableWeapon.magSize;
    }

    protected void OnEnable()
    {
        canShoot = true;
    }

    protected virtual void Update()
    {
        heatText.text = "heat:" + currentHeat.ToString() + "\nstate: " + StateEncode[(int)weaponState];
        magText.text = currentMag.ToString() + "/" + ScriptableWeapon.magSize;

        //control the heat
        if (currentHeat >= 0 && Time.time >= heatLockedTill)
            currentHeat= currentHeat - ScriptableWeapon.heatDecay >0 ? currentHeat - ScriptableWeapon.heatDecay: 0f;


        //controllo l'input
        var _isShooting = isShooting();

        if (_isShooting && currentMag > 0 && canShoot) Shoot();
        else if (_isShooting && currentMag <= 0 && !reloading) Reload();
        else if (Input.GetKeyDown(reloadKey) && !reloading) Reload();

        //compute the state
        if (Input.GetKey(shootKey)) weaponState = WeaponStates.shooting;
        else if (reloading) weaponState = WeaponStates.reloading;
        else weaponState = WeaponStates.idle;

    }

    protected void Shoot()
    {
        //In teoria andrebbe riprodotto un suono adeguato
        if (currentMag <= 0) return;

        //aggiorna il numero di colpi nel caricatore
        currentMag--;
        currentHeat++;
        heatLockedTill = Time.time + ScriptableWeapon.heatWaitDecayTime;

        Lockshoot();
        //spara con l'arma, utilizza il ray cast, riproduci l'animazione sul modello
        RaycastHit[] hits;
        //var weaponeRecoil = ScriptableWeapon.RecoilValues[Mathf.RoundToInt(currentHeat)];
        hits = Physics.RaycastAll(camera.transform.position, Vector3.forward, 100f);

        Array.Sort(hits, (a, b) => Vector3.Distance(a.point, camera.transform.position).CompareTo(Vector3.Distance(b.point, camera.transform.position)));

        for (int i = hits.Length - 1; i < 0; i--)
        {
            Ray ray = new(hits[i].point, hits[i - 1].point);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Vector3.Distance(hits[i].point, hits[i - 1].point) + .5f);


        }
    }

    /// <summary>
    /// Ritorna true solo se è stato inviando il comando di sparare
    /// </summary>
    /// <returns></returns>
    protected bool isShooting()
    {
        if (!canShoot) return false;
        var shooting = false;

        if (ScriptableWeapon.firingMode == FiringMode.auto)
            shooting = Input.GetKey(shootKey); //Solo per le armi automatiche controlliamo se il si sta tenendo premuto il pulsante
        else
            shooting = Input.GetKeyDown(shootKey);

        return shooting;
    }

    protected virtual void Lockshoot()
    {
        canShoot = false;
        Invoke(nameof(ResetShoot), 1f / ScriptableWeapon.fireRate);   
    }

    protected virtual void ResetShoot() => canShoot = true;


    protected virtual void Reload()
    {
        canShoot = false;
        reloading = true;
        Invoke(nameof(ReloadCompleted), ScriptableWeapon.reloadTime);
    }

    protected virtual void ReloadCompleted()
    {
        canShoot = true;
        reloading = false;
        currentMag = ScriptableWeapon.magSize;
    }
}

public enum WeaponStates
{
    idle, reloading, shooting, inspecting
}


