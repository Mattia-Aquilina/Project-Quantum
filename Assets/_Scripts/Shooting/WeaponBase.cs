using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// Basic class for all the weapons
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{

    [Header("Debug ONLY")]
    [SerializeField] LayerMask HardWalls;
    [SerializeField] LayerMask MediumWalls;
    [SerializeField] LayerMask SoftWalls;
    [SerializeField] LayerMask hittable;
    [SerializeField] LayerMask notWallbangable;
    [SerializeField] LayerMask canHitObjects;
    private List<ShootOutcome> outcome = new();

    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] TextMeshProUGUI magText;
    [SerializeField] Camera camera;

    [SerializeField] PlayerMovement playerRef;
    [Header("Animations")]
    [SerializeField] protected WeaponModel model;

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

        Debug.DrawRay(camera.transform.position, camera.transform.forward * 100f, Color.green);

        heatText.text = "heat:" + currentHeat.ToString() + "\nstate: " + StateEncode[(int)weaponState];
        magText.text = currentMag.ToString() + "/" + ScriptableWeapon.magSize;

        //control the heat
        if (currentHeat >= 0 && Time.time >= heatLockedTill)
            currentHeat = currentHeat - ScriptableWeapon.heatDecay > 0 ? currentHeat - ScriptableWeapon.heatDecay: 0f;


        //controllo l'input
        var _isShooting = isShooting();

        if (_isShooting && currentMag > 0 && canShoot &&!reloading) Shoot();

        else if (_isShooting && currentMag <= 0 && !reloading) Reload();
        else if (Input.GetKeyDown(reloadKey) && !reloading) Reload();

        //compute the state
        if (reloading) weaponState = WeaponStates.reloading;
        else if (Input.GetKey(shootKey)) weaponState = WeaponStates.shooting;
        else weaponState = WeaponStates.idle;



        switch (weaponState)
        {
            case WeaponStates.idle:
                model.PlayAnimation(WeaponModel.Idle);
                break;
            case WeaponStates.reloading:
                model.PlayAnimation(WeaponModel.Reload);
                break;
            case WeaponStates.shooting:
                
                break;
            case WeaponStates.inspecting:
                model.PlayAnimation(WeaponModel.Inspect);
                break;
            default:
                break;
        }
    }

    protected void Shoot()
    {
        Lockshoot();
        
        //In teoria andrebbe riprodotto un suono adeguato
        if (currentMag <= 0) return;

        //aggiorna il numero di colpi nel caricatore
        currentMag--;
        currentHeat++;
        heatLockedTill = Time.time + ScriptableWeapon.heatWaitDecayTime;

        
        //spara con l'arma, utilizza il ray cast, riproduci l'animazione sul modello

        RaycastHit[] hits;
        var shootDirection = camera.transform.forward + ScriptableWeapon.RecoilValues[(int)currentHeat];
        if (playerRef.rigidbody.velocity.magnitude >= ScriptableWeapon.movementSpeedThreshold)
            shootDirection += new Vector3(UnityEngine.Random.Range(-ScriptableWeapon.movementInaccuracy, +ScriptableWeapon.movementInaccuracy), UnityEngine.Random.Range(-ScriptableWeapon.movementInaccuracy, +ScriptableWeapon.movementInaccuracy),0);


        //Compute weapon recoil using  var weaponeRecoil = ScriptableWeapon.RecoilValues[Mathf.RoundToInt(currentHeat)];


        hits = Physics.RaycastAll(camera.transform.position, shootDirection, 100f, canHitObjects);
       
        //ordiniamo gli hit in base alla distanza (l'ordine non è garantito da RaycastAll
        Array.Sort(hits, (a, b) => Vector3.Distance(a.point, camera.transform.position).CompareTo(Vector3.Distance(b.point, camera.transform.position)));

        var targetHit = false;

        List<ShootOutcome> shootOutcome = new();

        for (int i = 0; i < hits.Length; i++)
        {
            LayerMask hitLayer = hits[i].collider.gameObject.layer;
            //se in ogni momento incotriamo un muro non wallbangabile ci fermiamo
            if (notWallbangable == (notWallbangable | (1 << hitLayer)))
            {
                shootOutcome.Add(new ShootOutcome(hits[i].point));
                break;
            }


            //se l'arma è soft controllo solo il primo hit, visto che non puo wallbangare
            if (i == 0 && hittable == (hittable | (1 << hitLayer)) && ScriptableWeapon.weaponPenetration == WeaponPenetration.soft)
            {
                shootOutcome.Add(new PlayerHit(hits[i].collider.tag, hits[i].collider.gameObject, hits[i].point));
                break;
            }
            else if (i == 0 && ScriptableWeapon.weaponPenetration == WeaponPenetration.soft)
            {
                shootOutcome.Add(new ShootOutcome(hits[i].point));
                break;
            }


            //gestiamo in maniera specifica l'ultimo elemento dell'array
            if (i == hits.Length - 1 && !(hittable == (hittable | (1 << hitLayer))))
            {
                shootOutcome.Add(new ShootOutcome(hits[i].point));
                break;
            }
            else if (i == hits.Length - 1 && (hittable == (hittable | (1 << hitLayer))))
            {
                //the last element is a player
                shootOutcome.Add(new PlayerHit(hits[i].collider.tag, hits[i].collider.gameObject, hits[i].point));
                break;
            }

            //gestiamo il caso in cui abbiamo colpito un giocatore
            if(hittable == (hittable | (1 << hitLayer))) {
                shootOutcome.Add(new PlayerHit(hits[i].collider.tag, hits[i].collider.gameObject, hits[i].point));
            }
            else { 
                //abbiamo colpito un muro
                //controlliamo in base alla penetrazione dell'arma
                if (ScriptableWeapon.weaponPenetration == WeaponPenetration.hard)
                    //abbiamo un arma hard, misuriamo in ogni caso lo spessore del muro colpito
                    shootOutcome.Add(computeHitObjectDepth(hits[i].point, hits[i + 1].point, hitLayer));          
                else if (ScriptableWeapon.weaponPenetration == WeaponPenetration.normal && !(HardWalls == (HardWalls | (1 << hitLayer))))
                    shootOutcome.Add(computeHitObjectDepth(hits[i].point, hits[i + 1].point, hitLayer));  
            }
        }

        this.outcome = shootOutcome;

        model.PlayAnimation(WeaponModel.Shoot);
        model.BulletTracer(shootOutcome, shootDirection);
    }
    private ShootOutcome computeHitObjectDepth(Vector3 pointA, Vector3 pointB, LayerMask hitLayer)
    {
        Ray ray = new(pointB, -camera.transform.forward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Vector3.Distance(pointA, pointB), canHitObjects);


        var wallDepth = Vector3.Distance(hit.point, pointA);
        return new WallHit(hitLayer, wallDepth, pointA, hit.point);
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

    /// <summary>
    /// in questo modo non si puo interrompere la ricarica
    /// </summary>

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
    
    protected virtual void OnWeaponSwap()
    {
        //interrompiamo eventualmente la ricarica dell'arma
        CancelInvoke();
    }


    //DEBUG ONLY

    public void OnDrawGizmos()
    {
        if (outcome.Count != 0)
        {
            foreach (var item in outcome)
            {
                if(item.GetType() == typeof(WallHit))
                {
                    var wall = (WallHit)item;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(wall.entryPoint, .2f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(wall.exitPoint, .2f);
                }

            }
        }
    }
}

public enum WeaponStates
{
    idle, reloading, shooting, inspecting
}


public class ShootOutcome {
    public Vector3 hitPoint;

    public ShootOutcome(Vector3 hitPoint) { this.hitPoint = hitPoint; }
}
public class WallHit : ShootOutcome {
    public LayerMask wallHit;
    public float wallDepth;

    public Vector3 entryPoint;
    public Vector3 exitPoint;
    public WallHit(LayerMask wall,  float wallDepth, Vector3 entryPoint, Vector3 exitPoint) : base(entryPoint)
    {
        this.wallHit = wall;    
        this.wallDepth = wallDepth;
        this.entryPoint = entryPoint;
        this.exitPoint = exitPoint;
    }

    public override string ToString()
    {
        return "you have hit a " +  wallHit.ToString() + " of depth " + wallDepth;
    }
}

public class PlayerHit : ShootOutcome
{
    public BodyPart bodyPart;
    public GameObject playerHit;
    private String[] bodyPartEncode = { "legs", "body", "head" };
    public PlayerHit(string bodyPart, GameObject playerHit, Vector3 hitPoint) : base(hitPoint)
    { 
        if (bodyPart == "legs")
            this.bodyPart = BodyPart.legs;
        else if (bodyPart == "body")
            this.bodyPart = BodyPart.body;
        else if (bodyPart == "head")
            this.bodyPart = BodyPart.head;

        this.playerHit = playerHit;

    }

    public override string ToString()
    {
        return "you have hit a player at " + bodyPartEncode[(int)bodyPart];
    }
}

/// <summary>
/// DA SPOSTARE IN UN FILE APPOSITO!!!!!!!!!!!!!!!!!!!!!! **TO DO**
/// </summary>
public enum BodyPart { 
    legs, body, head
}






