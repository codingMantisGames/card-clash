using UnityEngine;
using UnityEngine.Events;

namespace CodingMantisGames.UtilityAI
{
    public class Action : MonoBehaviour
    {
        #region VARIABLES
        public Consideration[] considerations;
        public UnityEvent OnActionComplete;
        [HideInInspector] public float score;
        #endregion

        #region FUNCTIONS
        public void PerformAction()
        {
            OnActionComplete.Invoke();
        }    
        #endregion
    }
}
