using UnityEngine;

namespace CodingMantisGames.SimpleAI 
{
    [CreateAssetMenu(menuName = "Simple AI/Score Calculator/Attack Enemy Action Score Calculator", fileName = "Attack Enemy Action Score Calculator")]
    public class AttackEnemyActionScoreCalculator : ScoreCalculator
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
        public override void CalculateScore(AIBrain ai)
        {
            score = 100;
        }
        #endregion
    }
}