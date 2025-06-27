using UnityEngine;

namespace CodingMantisGames.UtilityAI 
{
    [CreateAssetMenu(menuName = "UtilityAI/Consideration/New Fixed Consideration", fileName = "Fixed Consideration")]
    public class FixedConsideration : Consideration
    {
        #region VARIABLES
        [SerializeField, Range(0, 1)] private float fixedScore;
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
            return fixedScore;
        }
        #endregion
    }
}