using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Make a damageable object take damage
    /// </summary>
    /// <param name="Damage">Damage to infert</param>
    public void Damage(float Damage);
}
