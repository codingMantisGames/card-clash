using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/New Action Plan", fileName = "Action Plan")]
    public class ActionPlan : ScriptableObject
    {
        #region VARIABLES
        public Consideration[] considerations;
        public ActionPlanType planType;
        public float score;
        #endregion
    }

    public enum ActionPlanType { NORMAL, PROTECT_TOWER, PROTECT_ALLY, ATTACK_ENEMY };
}
