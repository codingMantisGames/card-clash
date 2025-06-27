using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Number of Cards in Hand Consideration", fileName = "Number of Cards in Hand Consideration")]
    public class NumberOfCardInHandConsideration : Consideration
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
        public override float Score(AIBrain brain)
        {
            float nor = brain.cardManager.cardInHand.Count / 3;

            return response.Evaluate(nor);
        }
        #endregion
    }
}