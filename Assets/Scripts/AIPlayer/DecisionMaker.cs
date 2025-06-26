using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public abstract class DecisionMaker : MonoBehaviour
    {
        #region FUNCTIONS
        public abstract void Init(Agent agent);
        public abstract Action DecideAction(Action[] actions);
        public abstract Action DecideActionPlan(ActionPlan[] actionPlans);
        #endregion
    }
}