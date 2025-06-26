using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class DecisionMaker : MonoBehaviour
    {
        #region FUNCTIONS
        public abstract void Init(AIBrain brain);
        public abstract Action DecideAction(Action[] actions);
        public abstract ActionPlan DecideActionPlan(ActionPlan[] actionPlans);
        #endregion
    }
}