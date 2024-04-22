using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Trail Confing", menuName ="Weapon/Trail Config")]
public class BulletTracerConfig : ScriptableObject
{
    [field: SerializeField] public Material material { get; private set; }

    [field: SerializeField] public float Duration { get; private set; } = 0.5f;
    [field: SerializeField] public float MinVertexDistance { get; private set; } = .1f;
    [field: SerializeField] public Gradient color { get; private set; }
    [field: SerializeField] public AnimationCurve curve { get; private set; }

    [field: SerializeField] public float MissDistance { get; private set; } = 100f;
    [field: SerializeField] public float SimulationSpeed { get; private set; } = 100f;
}
