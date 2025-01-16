using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    #region VARIABLES
    public UnityEvent OnAttack;
    #endregion

    #region UNITY FUNCTIONS
    #endregion

    #region FUNCTIONS
    public void Attack()
    {
        OnAttack.Invoke();
    }
    #endregion
}
