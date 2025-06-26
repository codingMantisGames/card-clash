using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class ActionPlanChooser : DecisionMaker
    {
        #region VARIABLES
        private Agent agent;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region FUNCTIONS
        public override Action DecideAction(Action[] actions)
        {
            throw new System.NotImplementedException();
        }

        public override Action DecideActionPlan(ActionPlan[] actionPlans)
        {
            throw new System.NotImplementedException();
        }

        public override void Init(Agent agent)
        {
            this.agent = agent;
        }
        #endregion
    }
}
