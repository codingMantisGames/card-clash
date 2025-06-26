using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class SpawnNewCharacterAction : Action
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
        public override float GetValidatedScore(Agent agent, float score)
        {
            return score;
        }

        public override void PerformAction(Agent agent)
        {
            //have to add logic to find which chacter to spawn and where to spawn
        }
        #endregion
    }
}