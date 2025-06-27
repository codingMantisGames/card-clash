using UnityEngine;
using UnityEngine.Events;

namespace CodingMantisGames.UtilityAI
{
    public abstract class Action : MonoBehaviour
    {
        #region VARIABLES
        public Consideration[] considerations;
        public UnityEvent OnActionComplete;
        [HideInInspector] public float score;
        #endregion

        #region FUNCTIONS
        public abstract float GetValidatedScore(AIBrain brain, float score);
        public abstract void PerformAction(AIBrain brain);
        #endregion
    }
}
