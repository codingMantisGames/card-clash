using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class UtilityAI : DecisionMaker
    {
        #region VARIABLES
        private Agent agent;
        #endregion

        #region UNITY FUNCTIONS
        #endregion

        #region FUNCTIONS
        public override void Init(Agent agent)
        {
            this.agent = agent;
        }
        public override Action DecideAction(Action[] actions)
        {
            float score = 0;

            //Loop through all actions and score them.
            //Score is the average score of all considerations.
            foreach (Action action in actions)
            {
                foreach (Consideration c in action.considerations)
                {
                    score += c.Score(agent);
                }

                score = action.GetValidatedScore(agent,score);

                score /= action.considerations.Length;
                action.score = score;
            }

            //Find the action with best score
            float bestScore = 0;
            Action bestAction = null;
            foreach (Action action in actions)
            {
                if(action.score >  bestScore)
                {
                    bestAction = action;
                    bestScore = action.score;
                }
            }

            return bestAction;
        }

        public override Action DecideActionPlan(ActionPlan[] actionPlans)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}