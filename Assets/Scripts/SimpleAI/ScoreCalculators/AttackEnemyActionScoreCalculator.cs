using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Score Calculator/Attack Enemy Action Score Calculator", fileName = "Attack Enemy Action Score Calculator")]
    public class AttackEnemyActionScoreCalculator : ScoreCalculator
    {
        #region VARIABLES
        [SerializeField] private int minAllyNeeded = 4;
        [SerializeField]
        int c;
        [SerializeField] private int minEnemyNeeded = 4;
        [SerializeField] private float scorePerExtraAlly = 0.2f;
        [SerializeField] private float scorePerEnemy = 0.2f;
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
            score = 0;
            score += scorePerEnemy * ai.enemyCharacters.Count;


            if (ai.allyCharacters.Count == 0)
            {
                c = ai.allyCharacters.Count;

                if (c > minAllyNeeded)
                    c -= minAllyNeeded;

                score += scorePerExtraAlly * c;
            }
        }
        #endregion
    }
}