using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class ActionPlan : ScriptableObject
    {
        #region VARIABLES
        public Consideration[] considerations;
        public ActionPlanType planType;
        public float score;
        #endregion
    }
}
