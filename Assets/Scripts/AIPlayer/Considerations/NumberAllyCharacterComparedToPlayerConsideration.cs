using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Number of Ally Character Compared To Player Consideration", fileName = "Number of Ally Character Compared To Player")]
    public class NumberAllyCharacterComparedToPlayerConsideration : Consideration
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
            float allyCount = Mathf.Clamp(brain.allyCharacters.Count, 0, brain.enemyCharacters.Count);

            float norm = allyCount / (float)brain.enemyCharacters.Count;

            return response.Evaluate(norm);
        }
        #endregion
    }
}