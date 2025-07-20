using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Score Calculator/Attack Enemy Tower Action Score Calculator", fileName = "Attack Enemy Tower Action Score Calculator")]
    public class AttackEnemyTowerActionScoreCalculator : ScoreCalculator
    {
        #region VARIABLES
        [SerializeField] private float distanceFromTower;
        [SerializeField] private int minCount = 3;
        [SerializeField] private float scoreMul = 0.1f;
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
            int count = 0;
            foreach (var item in ai.allyCharacters)
            {
                if (Vector3.Distance(ai.enemyTower.transform.position, item.transform.position) <= distanceFromTower)
                {
                    count++;
                }
            }


            if (count > minCount)
            {
                count -= minCount;
                score = scoreMul * Mathf.Clamp(count, 1, 100);
            }
        }
        #endregion
    }
}