using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Number of Ally Characters Consideration", fileName = "Number of Ally Characters Consideration")]
    public class NumberOfAllyCharactersConsideration : Consideration
    {
        #region VARIABLES

        #endregion

        #region FUNCTIONS
        public override float Score(AIBrain brain)
        {
            return 1;
        }
        #endregion
    }
}