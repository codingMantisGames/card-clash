using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class Action : MonoBehaviour
    {
        #region VARIABLES
        public Consideration[] considerations;
        [HideInInspector] public float score;
        #endregion

        #region FUNCTIONS
        public abstract float GetValidatedScore(Agent agent, float score);
        public abstract void PerformAction(Agent agent);
        #endregion
    }
}
