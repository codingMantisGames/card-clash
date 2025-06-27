using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Number of Ally Characters Consideration", fileName = "Number of Ally Characters Consideration")]
    public class NumberOfAllyCharactersConsideration : Consideration
    {
        #region VARIABLES
        [SerializeField] private int minmunNumberOfAllyCharactersNeeded = 3;
        #endregion

        #region FUNCTIONS
        public override float Score(AIBrain brain)
        {
            float norm = Mathf.Clamp(brain.allyCharacters.Count, 0, minmunNumberOfAllyCharactersNeeded) / minmunNumberOfAllyCharactersNeeded;

            return response.Evaluate(norm);
        }
        #endregion
    }
}