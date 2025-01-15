using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleStateEvent: StateMachineBehaviour
{
    #region VARIABLES
    #endregion

    #region UNITY FUNCTIONS

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlaceableItem item = animator.GetComponentInParent<PlaceableItem>();
        if (item && item.itemToDisable)
            item.itemToDisable.SetActive(true);
            
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlaceableItem item = animator.GetComponentInParent<PlaceableItem>();
        if (item && item.itemToDisable)
            item.itemToDisable.SetActive(false);
    }
    #endregion

    #region FUNCTIONS

    #endregion
}
