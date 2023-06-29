using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine shake;
    private Vector3 originalPos;

    public float shakeMagnitude = 0.5f;
    public float shakeDuration = 0.5f;

    private void Awake()
    {
        originalPos = transform.position;
    }

    public void Shake(float shakeDuration=0.5f, float shakeMagnitude=0.5f)
    {
        if (shake != null)
        {
            StopCoroutine(shake);
            transform.position = originalPos;
        }
        shake = StartCoroutine(ShakeRoutine(shakeDuration, shakeMagnitude));
    }



    private IEnumerator ShakeRoutine(float shakeDuration, float shakeMagnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            if (!GameManager.isPaused)
            {
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;

                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return new WaitForSecondsRealtime(0.01f);
            }
            else
            {
                yield return null;
            }
            
        }

        transform.position = originalPos;
    }
}
