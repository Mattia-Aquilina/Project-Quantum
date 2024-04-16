using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Weapon", fileName = "WeaponModelSample")]
public class ScriptableWeaponModel : ScriptableObject
{
    [field: SerializeField]public AnimatorController controller { get; private set; }
    [field: SerializeField]public int ShootAnimationDuration { get; private set; }
    [field: SerializeField]public int ReloadAnimationDuration { get; private set; }
    [field: SerializeField]public int TakeAnimationDuration { get; private set; }
    [field: SerializeField]public int WatchAnimationDuration { get; private set; }
}
