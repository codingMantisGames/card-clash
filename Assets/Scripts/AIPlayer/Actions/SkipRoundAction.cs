using System.Collections;
using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class SkipRoundAction : Action
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
        public override void PerformAction(Agent agent)
        {
            StartCoroutine(SkipAfterTime(agent));
        }
        IEnumerator SkipAfterTime(Agent agent)
        {
            yield return new WaitForSeconds(3);

            agent.SkipRound();
            agent.HandlePostAction();
        }
        #endregion
    }
}