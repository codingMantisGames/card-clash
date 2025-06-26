using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class ActionPlanChooser : DecisionMaker
    {
        #region VARIABLES
        private AIBrain brain;
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

        public override ActionPlan DecideActionPlan(ActionPlan[] actionPlans)
        {
            foreach (ActionPlan plan in actionPlans)
            {
                float score = 0;
                foreach (Consideration c in plan.considerations)
                {
                    score += c.Score(brain);
                }

                score /= plan.considerations.Length;

                plan.score = score;
            }

            float bestScore = 0;
            ActionPlan bestActionPlan = null;
            foreach (ActionPlan plan in actionPlans)
            {
                if(plan.score > bestScore)
                {
                    bestScore = plan.score;
                    bestActionPlan = plan;
                }
            }

            return bestActionPlan;
        }

        public override void Init(AIBrain brain)
        {
            this.brain = brain;
        }
        #endregion
    }
}
