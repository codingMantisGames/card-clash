using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullScreenSetup : MonoBehaviour
{
    #region VARIABLES
    private Button button;
    private TMP_Text txt;
    public Resolution originalResolution;
    private bool isFullscreen = false;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        originalResolution = new Resolution
        {
            width = 1280,
            height = 720
        };

        button = GetComponent<Button>();
        txt = button.GetComponentInChildren<TMP_Text>();

        button.onClick.AddListener(ToggleFullScreen);
        txt.text = Screen.fullScreen ? "Windowed" : "Fullscreen";

        isFullscreen = Screen.fullScreen;
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ToggleFullScreen()
    {
        if (isFullscreen)
        {
            // Switch to windowed mode with the original resolution
            Screen.SetResolution(originalResolution.width, originalResolution.height, false);
        }
        else
        {
            // Switch to fullscreen using the native resolution
            Resolution nativeResolution = Screen.currentResolution;
            Screen.SetResolution(nativeResolution.width, nativeResolution.height, true);
        }

        // Toggle the fullscreen state
        isFullscreen = !isFullscreen;

        txt.text = isFullscreen ? "Windowed" : "Fullscreen";
    }
    #endregion
}
