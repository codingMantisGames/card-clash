using System.Collections;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Action/Protect Ally Action", fileName = "Protect Ally Action")]
    public class ProtectAllyAction : Action
    {
        #region VARIABLES
        [SerializeField] private int minimumNumberOfCharacterNeeded = 0;
        [SerializeField] private float randomWaitTimeMin;
        [SerializeField] private float randomWaitTimeMax;
        [SerializeField] private ActionPlanTypes plan;
        #endregion

        #region FUNCTIONS

        public override void AttackIfAnyFlagged(AIBrain ai)
        {
            
        }

        public override void MoveIfAnyFlagged(AIBrain ai)
        {
        }

        public override void PerformAction(AIBrain ai)
        {
            if (ai.allyCharacters.Count < minimumNumberOfCharacterNeeded)
            {
                //Here we need to spawn cally chatacters
                int spawnCount = Mathf.Clamp(ai.allyCharacters.Count - minimumNumberOfCharacterNeeded, 1, 3);

                ai.ShowMessage("💭 Enemy has " + ai.enemyCharacters.Count + ". So we have to spawn atleast " + spawnCount + " charaters!##");

                ai.StartRoutine(CardDropProcedure(spawnCount, ai));
            }
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

                OfflineHexagon spawnLocation = ai.spawnLocationChooser.GetASpawnLocation(plan);
                OfflineHexagonManager.instance.SpawnItem(selectedCard.topCard.cardID, spawnLocation);

                ai.ShowMessage("🧙 " + selectedCard.topCard.name + " Character Spawned");
            }

            ai.ShowMessage("✅ Successfully Dropped Cards as needed!");
        }
        #endregion
    }
}