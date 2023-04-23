using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Shared { get; set; }
    private const float DISTORTION_RANGE = 0.5f;

    private void Awake()
    {
        Shared = this;
    }

    public IEnumerator StartShake(float timeOfShake, float magnitude)
    {
        Vector3 originalCameraPos = transform.localPosition;
        float activeTime = 0;
        while (activeTime <= timeOfShake)
        {
            float xDistortion = Random.Range(-DISTORTION_RANGE, DISTORTION_RANGE)*magnitude;
            float yDistortion = Random.Range(-DISTORTION_RANGE, DISTORTION_RANGE)*magnitude;
            transform.localPosition = new Vector3(xDistortion, yDistortion, originalCameraPos.z);
            activeTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalCameraPos;
    }
    
}
