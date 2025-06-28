using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Treat Level Consideration", fileName = "Treat Level Consideration")]
    public class TreatLevelConsideration : Consideration
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
            float treatLevel = 0;
            foreach (var item in brain.enemyCharacters)
            {
                if (item.AnyTargetInAttackRange(RoundStage.ATTACK))
                {
                    treatLevel++;
                }
            }

            float nor = treatLevel / brain.enemyCharacters.Count;
            return response.Evaluate(nor);  
        }
        #endregion
    }
}