using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic class for all the weapons
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{

    //Dati dell'arma
    public ScriptableWeapon ScriptableWeapon { get; private set; }

    protected FiringMode firingMode;
    protected WeaponType weaponType;
    protected readonly int fireRate;
    protected readonly int magSize;
    protected readonly float reloadTime;
    protected readonly int NumberOfMags;
    protected readonly int[] RecoilValues = {};

    //Get them from the keybindings
    [SerializeField] protected KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode reloadKey = KeyCode.R;

    private bool canShoot = true;
    protected int currentMag;
    protected int totalBullets;
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
        if (isShooting() && currentMag > 0) Shoot();
        else if (isShooting() && currentMag <= 0) Reload();
        else if (Input.GetKeyDown(reloadKey)) Reload();
    }

    protected void Shoot()
    {
        //In teoria andrebbe riprodotto un suono adeguato
        if (currentMag <= 0) return;

        currentMag--;
        
        //spara con l'arma, utilizza il ray cast, riproduci l'animazione sul modello
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

        if (shooting) Lockshoot();

        return shooting;
    }

    protected virtual void Lockshoot()
    {
        canShoot = false;
        Invoke(nameof(ResetShoot), 1 / ScriptableWeapon.fireRate);   
    }

    protected virtual void ResetShoot() => canShoot = true;


    protected virtual void Reload()
    {
        canShoot = false;
        Invoke(nameof(ResetShoot), reloadTime);
    }
}



