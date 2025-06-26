using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/Number of Cards in Hand", fileName = "Number of Cards in Hand Consideration")]
    public class NumberOfCardsInHandConsiseration : Consideration
    {
        #region VARIABLES

        #endregion

        #region FUNCTIONS
        public override float Score(Agent agent)
        {
            float nor = agent.cardInHand.Count / 3.0f;

            return response.Evaluate(nor);
        }
        #endregion

    }
}
