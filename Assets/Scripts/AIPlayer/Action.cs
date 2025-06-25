using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class Action : MonoBehaviour
    {
        #region VARIABLES
        public Consideration[] considerations;
        public float score;
        #endregion

        #region FUNCTIONS
        public abstract void PerformAction(Agent agent);
        #endregion
    }
}
