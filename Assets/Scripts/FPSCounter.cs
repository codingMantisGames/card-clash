using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    #region VARIABLES
    [Header("Settings")]
    public float updateInterval = 0.5f; 

    [Header("References")]
    public TMP_Text fpsText;

    private float elapsedTime = 0f;
    private int frames = 0;
    private float fps = 0f; 
    #endregion

    #region UNITY FUNCTIONS
    void Update()
    {
        frames++;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= updateInterval)
        {
            fps = frames / elapsedTime;

            if (fpsText != null)
            {
                fpsText.text = $"FPS: {fps:F1}"; 
            }

            frames = 0;
            elapsedTime = 0f;
        }
    }
    #endregion

    #region FUNCTIONS

    #endregion
}
