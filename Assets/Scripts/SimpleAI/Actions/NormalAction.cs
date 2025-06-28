using System.Collections;
using CodingMantisGames.UtilityAI;
using UnityEngine;


namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Action/Normal Action", fileName = "Normal Action")]
    public class NormalAction : Action
    {
        #region VARIABLES
        [SerializeField] private float randomWaitTimeMin;
        [SerializeField] private float randomWaitTimeMax;
        [SerializeField] private ActionPlanTypes plan;
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
        public override void PerformAction(AIBrain ai)
        {
            int numberOfCharaterNeeded = ai.enemyCharacters.Count - ai.allyCharacters.Count;
            int totalWeCanSpawn = ai.cardManager.cardInHand.Count + ai.cardManager.cardCounter;

            int spawnCount = Random.Range(1, Mathf.Clamp(numberOfCharaterNeeded, 0, totalWeCanSpawn));

            ai.ShowMessage("💭 Enemy has " + ai.enemyCharacters.Count + ". So we have to spawn atleast " + spawnCount + " charaters!");

            ai.StartRoutine(CardDropProcedure(spawnCount, ai));
        }
        IEnumerator CardDropProcedure(int spawnCount, AIBrain ai)
        {
            yield return null;

            for (int i = 0; i < spawnCount; i++)
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));

                if (ai.cardManager.cardInHand.Count == 0) ai.cardManager.GetNewCard();

                CardInfo selectedCard = ai.cardEvaluator.EvaluateCard(ai.cardManager.cardInHand.ToArray(), true, plan);

                if (selectedCard != null && ai.cardManager.cardInHand.Contains(selectedCard))
                {
                    ai.cardManager.cardInHand.Remove(selectedCard);
                }

                //OfflineHexagon spawnLocation = spawnLocationChooser.GetASpawnLocation();
                //offlineHexagonManager.SpawnItem(selectedCard.topCard.cardID, spawnLocation);

                ai.ShowMessage("🧙 " + selectedCard.topCard.name + " Character Spawned");
            }
        }
        #endregion
    }
}