using System.Linq;
using CodingMantisGames.SimpleAI;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    public class TilesEvaluator : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private TileEvaluatorData[] tileEvaluatorData;
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
        public OfflineHexagon ChooseBestTileToMove(ActionPlanTypes plan, OfflineHexagon[] tiles, Transform pos)
        {
            float bestScore = 0;
            OfflineHexagon bestHexagonToMove = null;
            TileEvaluatorData tileEvaluator = tileEvaluatorData.FirstOrDefault(info => info.actionPlan == plan);

            foreach (OfflineHexagon tile in tiles)
            {
                float score = 0;
                OfflineHexagon[] adj = tile.GetAllAdjacnetTile();
                foreach (OfflineHexagon a in adj)
                {
                    if(tileEvaluator.safeSpot)
                    {
                        if (a.isUsed && a.isUsedByEnemy) score += -hasEnemyAdjacent;
                        else if (a.isUsed && !a.isUsedByEnemy) score += hasAllyAdjacent;
                    }
                    else if(tileEvaluator.attackSpot)
                    {
                        if (a.isUsed && a.isUsedByEnemy) score += hasEnemyAdjacent;
                    }
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
        public static float GetInFrontFactor(Transform source, Transform target)
        {
            Vector3 toTarget = (target.position - source.position);
            float dot = Vector3.Dot(source.right, toTarget);
            return dot;
        }
        #endregion
    }
}
[System.Serializable]
public class TileEvaluatorData
{
    public ActionPlanTypes actionPlan;

    public bool safeSpot;
    public bool attackSpot;
    public bool towerAttackSpot;
    public bool protectTowerSpot;
}
