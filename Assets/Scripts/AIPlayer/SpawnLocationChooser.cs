using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class SpawnLocationChooser : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private OfflineHexagon[] characterSpawnLocations;
        #endregion

        #region UNITY FUNCTIONS
        #endregion

        #region FUNCTIONS
        public OfflineHexagon GetASpawnLocation(ActionPlanType actionPlanType = ActionPlanType.NORMAL)
        {
            float bestScore = 0;
            OfflineHexagon seletedHexagon = null;

            if (actionPlanType == ActionPlanType.NORMAL)
            {
                foreach (var location in characterSpawnLocations)
                {
                    float score = location.initialMoves + Random.Range(-0.25f, 0.25f);
                    if (score > bestScore && !location.isUsed)
                    {
                        bestScore = score;
                        seletedHexagon = location;
                    }
                }
            }
            return seletedHexagon;
        }
        #endregion
    }
}