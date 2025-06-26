using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class DrawNewCardAction : Action
    {
        #region VARIABLES

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
        public override void PerformAction(Agent agent)
        {
            agent.GetNewCard();
            agent.HandlePostAction();
        }
        public override float GetValidatedScore(Agent agent, float tempScore)
        {
            if (agent.cardInHand.Count >= 3 || agent.cardCounter <= 0)
                return 0f;
            return tempScore;
        }
        #endregion
    }
}
