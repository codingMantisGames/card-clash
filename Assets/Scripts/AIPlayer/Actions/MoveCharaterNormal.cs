using UnityEngine;

namespace CodingMantisGames.UtilityAI 
{
    public class MoveCharaterNormal : Action
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
        public override float GetValidatedScore(AIBrain brain, float score)
        {
            return score;
        }

        public override void PerformAction(AIBrain brain)
        {
            brain.HandleOnMoveCharacterChoosed_NormalAction();
        }
        #endregion
    }
}