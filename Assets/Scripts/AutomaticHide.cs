using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticHide : MonoBehaviour
{
    #region VARIABLES

    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        Invoke("Hide", 3);
    }
    void Update()
    {
	
    }
    #endregion

    #region FUNCTIONS
    void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
