using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/Ally Tower Health", fileName = "Ally Tower Health Consideration")]
    public class AllyTowerHealthConsideration : Consideration
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
        public override float Score(Agent agent)
        {
            float nor = agent.allyTower.life / 7.0f;

            return response.Evaluate(nor);
        }
        #endregion
    }
}