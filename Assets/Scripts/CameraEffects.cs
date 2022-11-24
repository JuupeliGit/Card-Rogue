using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects instance;

    private Camera cam;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    public void CameraShake(float strength, float duration)
    {
        StartCoroutine(ShakeEffect(strength, duration));
    }

    private IEnumerator ShakeEffect(float strength, float duration)
    {
        float elapsed = 0f;

        while(elapsed < duration)
        {
            Vector2 shakePos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * strength;
            cam.transform.position = new Vector3(shakePos.x, shakePos.y, -10f);

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cam.transform.position = new Vector3(0f, 0f, -10f);
    }
}
