using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    #region VARIABLES
    public static CameraShake instance;

    private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeDuration;
    private float shakeTimer;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
        virtualCamera = GetComponent<CinemachineCamera>();
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Start()
    {

    }
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0)
            {
                noise.AmplitudeGain = 0f;
            }
            else
            {
                noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, 0f, Time.deltaTime / shakeDuration);
            }
        }
    }
    #endregion

    #region FUNCTIONS
    public void ShakeCamera(float intensity, float duration)
    {
        if (noise == null) return;

        noise.AmplitudeGain = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }
    #endregion
}
