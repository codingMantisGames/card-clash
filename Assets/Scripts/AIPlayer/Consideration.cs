using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        #region VARIABLES
        [SerializeField] protected AnimationCurve response;
        #endregion

        #region FUNCTIONS
        public abstract float Score(Agent agent);
        #endregion
    }
}