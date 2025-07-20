using System.Linq;
using CodingMantisGames.SimpleAI;
using CodingMantisGames.UtilityAI;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    public class SpawnLocationChooser : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private OfflineHexagon[] characterSpawnLocations;
        [SerializeField] private OfflineHexagon[] cardSpawnLocations;
        [SerializeField] private LocationChooserData[] locationChooserData;
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
        public OfflineHexagon GetASpawnLocation(ActionPlanTypes plan)
        {
            float bestScore = 0;
            OfflineHexagon seletedHexagon = null;
            LocationChooserData data = locationChooserData.FirstOrDefault(info => info.actionPlan == plan);
            foreach (var location in characterSpawnLocations)
            {
                if (!location.isUsed)
                {
                    float score = 0;
                    if (data._attack) score += location.attack;
                    if (data._defent) score += location.defent;
                    if (data._initialMoves) score += location.initialMoves + Random.Range(-0.25f, 0.25f);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        seletedHexagon = location;
                    }
                }
            }
            return seletedHexagon;
        }

        public OfflineHexagon GetACardSpawnLocationRandom()
        {
            return cardSpawnLocations[Random.Range(0, cardSpawnLocations.Length)];
        }
        #endregion
    }
}

[System.Serializable]
public class LocationChooserData
{
    public ActionPlanTypes actionPlan;

    public bool _attack;
    public bool _defent;
    public bool _initialMoves;
}