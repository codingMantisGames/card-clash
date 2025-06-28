using UnityEngine;

namespace CodingMantisGames.SimpleAI 
{
    public abstract class Action : ScriptableObject
    {
        #region VARIABLES

        #endregion

        #region FUNCTIONS
        public abstract void PerformAction(AIBrain ai);
        #endregion
    }
}