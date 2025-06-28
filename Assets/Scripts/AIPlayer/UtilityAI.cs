using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class UtilityAI : DecisionMaker
    {
        #region VARIABLES
        private AIBrain brain;
        #endregion

        #region UNITY FUNCTIONS
        #endregion

        #region FUNCTIONS
        public override void Init(AIBrain brain)
        {
            this.brain = brain;
        }
        public override Action DecideAction(Action[] actions)
        {

            //Loop through all actions and score them.
            //Score is the average score of all considerations.
            foreach (Action action in actions)
            {
                float score = 0;
                foreach (Consideration c in action.considerations)
                {
                    score += c.Score(brain);
                }

                score /= action.considerations.Length;
                Debug.LogWarning("📈 Score for " + action.gameObject.name + " is " + score);
                action.score = score;
            }

            //Find the action with best score
            float bestScore = 0;
            Action bestAction = null;
            foreach (Action action in actions)
            {
                if (action.score > bestScore)
                {
                    bestAction = action;
                    bestScore = action.score;
                }
            }

            return bestAction;
        }

        public override ActionPlan DecideActionPlan(ActionPlan[] actionPlans)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}