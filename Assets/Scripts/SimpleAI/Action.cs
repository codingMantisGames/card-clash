using UnityEngine;

namespace CodingMantisGames.SimpleAI 
{
    public abstract class Action : ScriptableObject
    {
        #region VARIABLES

        #endregion

        #region FUNCTIONS
        public abstract void PerformAction(AIBrain ai);
        public abstract void MoveIfAnyFlagged(AIBrain ai);
        public abstract void AttackIfAnyFlagged(AIBrain ai);
        #endregion
    }
}