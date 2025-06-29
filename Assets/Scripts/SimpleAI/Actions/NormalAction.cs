using System.Collections;
using System.Linq;
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

                OfflineHexagon spawnLocation = ai.spawnLocationChooser.GetASpawnLocation(plan);
                OfflineHexagonManager.instance.SpawnItem(selectedCard.topCard.cardID, spawnLocation);

                ai.ShowMessage("🧙 " + selectedCard.topCard.name + " Character Spawned");
            }

            ai.ShowMessage("✅ Successfully Dropped Cards as needed!");
            HandleOnSpawnComplete(ai);
        }

        private void HandleOnSpawnComplete(AIBrain ai)
        {
            ai.UpdateRound();

            foreach (var item in ai.allyCharacters)
            {
                if (item.life > 3.5f || item.attackValue > 2.5f)
                {
                    item.isFlaggedCharacter = true;
                }
            }
            bool flag = false;
            foreach (var item in ai.allyCharacters)
            {
                if (item.isFlaggedCharacter)
                {
                    OfflineHexagon[] result = item.GetAllTilesInRange(RoundStage.MOVE_ITEM);
                    OfflineHexagon hexgonToMove = ai.tileEvaluator.ChooseBestTileToMove(plan, result, item.transform);

                    hexgonToMove.isMarkedByAI = true;
                    item.hexagonFlagged = hexgonToMove;

                    flag = true;
                }
            }

            if (!flag)
            {
                ai.ShowMessage("⚠️ No Enemy to attack lets Skip attack round.");

                HandleOnMoveComplete(ai);
            }

            MoveIfAnyFlagged(ai);
        }
        public override void MoveIfAnyFlagged(AIBrain ai)
        {
            ai.StartRoutine(MoveProcedure(ai));
        }
        IEnumerator MoveProcedure(AIBrain ai)
        {
            OfflinePlacableItem item = ai.allyCharacters.FirstOrDefault(info => info.isFlaggedCharacter);

            if (item != null && item.hexagonFlagged)
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));

                BotGameManager.instance.MoveCurentItem(item.hexagonFlagged, OfflineHexagonManager.instance.GetIndex(item.hexagonFlagged), item);
                item.isFlaggedCharacter = false;
                ai.ShowMessage("🧙 Started Moving " + item.gameObject.name);
            }
            else
            {
                ai.ShowMessage("✅ Successfully Moved all Characters as needed!");

                HandleOnMoveComplete(ai);
            }
        }
        public void HandleOnMoveComplete(AIBrain ai)
        {
            ai.UpdateRound();

            bool flag = false;
            foreach (var item in ai.allyCharacters)
            {
                OfflineHexagon[] result = item.GetAllTilesInRange(RoundStage.ATTACK);
                foreach (var tile in result)
                {
                    if (tile.isUsed && tile.isUsedByEnemy)
                    {
                        OfflinePlacableItem enemy = tile.GetPlayer();
                        enemy.isFlaggedCharacter = true;
                        item.enemyToAttack = enemy;
                        item.isFlaggedCharacter = true;

                        flag = true;
                    }
                }
            }

            if (!flag)
            {
                ai.ShowMessage("⚠️ No Enemy to attack lets Skip attack round.");

                HandleOnAttackComplete(ai);
            }

            AttackIfAnyFlagged(ai);
        }

        public override void AttackIfAnyFlagged(AIBrain ai)
        {
            ai.StartRoutine(AttackProcedure(ai));
        }
        IEnumerator AttackProcedure(AIBrain ai)
        {
            OfflinePlacableItem item = ai.allyCharacters.FirstOrDefault(info => info.isFlaggedCharacter);

            if (item != null && item.isFlaggedCharacter && item.enemyToAttack != null)
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));
                ai.ShowMessage("🗡️ Let's attack " + item.enemyToAttack.name);
                item.Attack(item.enemyToAttack.tileIndex);
                item.enemyToAttack = null;
                item.isFlaggedCharacter = false;
            }
            else if (item != null && item.isFlaggedCharacter && item.enemyToAttack == null) //This is means enemy died by ally
            {
                ai.ShowMessage("⚠️ I think enemy is already dead! Sad..!");
                item.isFlaggedCharacter = false;
                AttackIfAnyFlagged(ai);
            }
            else
            {
                ai.ShowMessage("✅ Attack Action Completed Successfully.");
                HandleOnAttackComplete(ai);
            }
        }

        public void HandleOnAttackComplete(AIBrain ai)
        {
            ai.UpdateRound();
        }
        #endregion
    }
}