using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Assets/Weapon", fileName = "weaponSample")]
public class ScriptableWeapon : ScriptableObject
{
    [field: SerializeField] public FiringMode firingMode {get; private set; }
    [field: SerializeField] public WeaponType weaponType { get; private set; }
    [field: SerializeField] public WeaponPenetration weaponPenetration { get; private set; }
    [field: SerializeField] public int fireRate { get; private set; }
    [field: SerializeField] public int magSize { get; private set; }
    [field: SerializeField] public float reloadTime { get; private set; }
    [field: SerializeField] public int NumberOfMags { get; private set; }
    [field: SerializeField] public int[] RecoilValues { get; private set; }
    [field: SerializeField] public float heatDecay { get; private set; } = 0.1f;
    [field: SerializeField] public float heatWaitDecayTime { get; private set; } 
    [field: SerializeField] public int heatTimeCoefficient { get; private set; }
    [field: SerializeField] public FallofReduction[] fallofs { get; private set; }
}
public enum WeaponType
{
    pistol,
    smg,
    sniper,
    shotgun,
    rifle
}

public enum FiringMode
{
    boltAction,
    semiAuto,
    auto,
    burst
}

[System.Serializable]
public struct FallofReduction
{
    public int distance;
    public float damageReduction;
}

public enum WeaponPenetration { 
    soft, normal, hard
}


