using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingText : MonoBehaviour
{
    // Start is called before the first frame update
    private bool active = false;
    [SerializeField] float duration;
    [SerializeField] float movement = 0.1f;
    [SerializeField] TextMesh textMesh;

    private void Awake()
    {
        textMesh.text = "";

    }
    private void Update()
    {
        if (active)
            transform.position += Vector3.up * movement;

    }
    private void LateUpdate()
    {
        if(active)
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void Enable(string text)
    {
        active = true;
        textMesh.text = text;
        Invoke(nameof(Destroy), duration);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
