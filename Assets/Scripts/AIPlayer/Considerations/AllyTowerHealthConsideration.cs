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
            if (agent.allyTower == null) return 0;

            float nor = agent.allyTower.life / 7;

            return response.Evaluate(nor);
        }
        #endregion
    }
}