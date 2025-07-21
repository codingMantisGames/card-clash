using UnityEngine;

namespace CodingMantisGames.SimpleAI 
{
    [CreateAssetMenu(menuName = "Simple AI/Score Calculator/Normal Action Score Calculator", fileName = "Normal Action Score Calculator")]
    public class NormalActionScoreCalculator : ScoreCalculator
    {
        #region VARIABLES
        [SerializeField, Range(0, 1)] private float noAllyCharatersScore = 0.25f;
        [SerializeField, Range(0, 1)] private float noEnemyNearScore = 0.25f;
        [SerializeField, Range(0, 1)] private float towerHealthScore = 0.25f;
        #endregion

        #region FUNCTIONS
        public override void CalculateScore(AIBrain ai)
        {
            score = 0;

            if (ai.allyCharacters.Count == 0) score += noAllyCharatersScore;

            //check any enemy near any ally if no add score
            foreach (var character in ai.allyCharacters)
            {
                int count = character.GetEnemyNearByCount();
                if (count == 0) score += noAllyCharatersScore;
            }

            //check tower health
            if (ai.allyTower.life > 3.5) score += towerHealthScore;
        }
        #endregion
    }
}