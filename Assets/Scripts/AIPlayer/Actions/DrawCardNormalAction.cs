using UnityEngine;

namespace CodingMantisGames.UtilityAI 
{
    public class DrawCardNormalAction : Action
    {
        #region VARIABLES
        [SerializeField] private BotCardManager cardManager;
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
        public override float GetValidatedScore(AIBrain brain, float score)
        {
            return score;
        }

        public override void PerformAction(AIBrain brain)
        {
            cardManager.GetNewCard();

            OnActionComplete.Invoke();
        }
        #endregion
    }
}