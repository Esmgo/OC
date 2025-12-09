using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private CameraFollow cameraFollow;
    private CameraShake cameraShake;

    public void Init()
    {
        cameraShake = Camera.main.transform.GetComponent<CameraShake>();
        cameraFollow = Camera.main.transform.parent.GetComponent<CameraFollow>();

        if (cameraShake == null) Debug.LogError("CameraShake component not found on the camera!");
        if (cameraFollow == null) Debug.LogError("CameraFollow component not found on the camera's parent!");
    }

    public void SetFollowTarget(Transform target)
    {
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(target);
        }
    }
    public void TriggerCameraShake(float duration = 0.1f, float strength = 0.5f, int vibrato = 10, float randomness = 90f)
    {
        if (cameraShake != null)
        {
            cameraShake.TriggerShake(duration, strength, vibrato, randomness);
        }
    }
}
