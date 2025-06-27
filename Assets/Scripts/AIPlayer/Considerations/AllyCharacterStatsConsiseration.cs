using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Ally Stats Consideration", fileName = "Ally Stats Consideration")]
    public class ALlyCharacterStatsConsiseration : Consideration
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
            int strongCharatersCount = 0;

            foreach (var item in brain.allyCharacters)
            {
                if (item.life > 3.5f || item.attackValue > 2.5f)
                {
                    strongCharatersCount++;
                    item.isFlaggedCharacter = true;
                }
            }

            return response.Evaluate((float)strongCharatersCount / (float)brain.allyCharacters.Count);
        }
        #endregion
    }
}