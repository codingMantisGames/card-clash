using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class TileEvaluator : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private OfflineHexagon[] offlineHexagons;

        [Header("Scores")]
        [SerializeField] private float hasEnemyAdjacent;
        [SerializeField] private float hasAllyAdjacent;
        [SerializeField] private float hasTowerAdjacent;
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
        public OfflineHexagon ChooseBestTileToMove(ActionPlanType planType, OfflineHexagon[] tiles, Transform pos)
        {
            if (planType == ActionPlanType.NORMAL)
            {
                float bestScore = 0;
                OfflineHexagon bestHexagonToMove = null;
                foreach (OfflineHexagon tile in tiles)
                {
                    float score = 0;
                    OfflineHexagon[] adj = tile.GetAllAdjacnetTile();
                    foreach (OfflineHexagon a in adj)
                    {
                        if (a.isUsed && a.isUsedByEnemy) score += -hasEnemyAdjacent;
                        else if (a.isUsed && !a.isUsedByEnemy) score += hasAllyAdjacent;
                    }

                    score += GetInFrontFactor(tile.buildPoint, pos);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestHexagonToMove = tile;
                    }
                }

                return bestHexagonToMove;
            }

            return null;
        }
        public static float GetInFrontFactor(Transform source, Transform target)
        {
            Vector3 toTarget = (target.position - source.position);
            float dot = Vector3.Dot(source.right, toTarget);
            return dot;
        }
        #endregion
    }
}