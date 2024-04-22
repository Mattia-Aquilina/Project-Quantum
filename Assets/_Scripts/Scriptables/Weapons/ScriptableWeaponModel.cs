using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Assets/Weapon/Model", fileName = "WeaponModelSample")]
public class ScriptableWeaponModel : ScriptableObject
{
    [field: SerializeField]public AnimatorController controller { get; private set; }
    [field: SerializeField]public float ShootAnimationDuration { get; private set; }
    [field: SerializeField]public float ReloadAnimationDuration { get; private set; }
    [field: SerializeField]public float TakeAnimationDuration { get; private set; }
    [field: SerializeField]public float WatchAnimationDuration { get; private set; }

    [field: SerializeField] public BulletTracerConfig BulletTracerConfig { get; private set; }

    private ObjectPool<TrailRenderer> TrailPool;


    public void Spawn()
    {
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        Debug.Log("trail pool inited");
    }
    public TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("bullet trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = BulletTracerConfig.color;
        trail.material = BulletTracerConfig.material;
        trail.widthCurve = BulletTracerConfig.curve;
        trail.time = BulletTracerConfig.Duration;
        trail.minVertexDistance = BulletTracerConfig.MinVertexDistance;

        trail.emitting = true;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public IEnumerator PlayTrail(Vector3 TrailStartPosition, Vector3 EndPoint)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = TrailStartPosition;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(TrailStartPosition, EndPoint);
        float remainingDistance = distance;

        while(remainingDistance > 0)
        {
            
            instance.transform.position = Vector3.Lerp(TrailStartPosition, EndPoint, Mathf.Clamp01(1 - (remainingDistance/distance)) );
            remainingDistance -= BulletTracerConfig.SimulationSpeed *Time.deltaTime;
            yield return null;

        }
        yield return new WaitForSeconds(BulletTracerConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);

    }
}

