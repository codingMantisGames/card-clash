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

            int r = Random.Range(1, 100);
            bool isHealed = false;
            if (r >= 70)
            {
                ai.ShowMessage("💭 I think we can spawn a power card");
                if (ai.cardManager.cardInHand.Count == 0 && ai.cardManager.CanDrawMoreCards)
                {
                    ai.cardManager.GetNewCard();
                }
                else
                {
                }
                if (ai.cardManager.cardInHand.Count > 0)
                {
                    foreach (var item in ai.cardManager.cardInHand)
                    {
                        if (item.bottomCard.cardID == "PC31" || item.bottomCard.cardID == "PC32" || item.bottomCard.cardID == "PC33")
                        {
                            ai.cardManager.cardInHand.Remove(item);
                            OfflinePlacableItem all = ai.allyCharacters.OrderBy(c => c.life).FirstOrDefault();

                            OfflineHexagon spawnLocation = GetSpawnLocation(all, ai);
                            OfflineHexagonManager.instance.SpawnItem(item.bottomCard.cardID, spawnLocation);

                            foreach (var ally in ai.allyCharacters)
                            {
                                ally.CheckForCards();
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                isHealed = true;

                ai.StartRoutine(Heal(ai));
            }


            if (!isHealed)
            {
                bool isAllyFound = false;
                foreach (var item in ai.allyCharacters)
                {
                    if (item.life < item.totalLife)
                    {
                        isAllyFound = true;
                        break;
                    }
                }

                if (isAllyFound)
                {
                    ai.StartRoutine(Heal(ai));
                }
            }

            ai.ShowMessage("✅ Successfully Dropped Cards as needed!");
            HandleOnSpawnComplete(ai);
        }

        public OfflineHexagon GetSpawnLocation(OfflinePlacableItem item, AIBrain ai)
        {
            foreach (var h in item.GetAllTilesInRange(RoundStage.MOVE_ITEM))
            {
                if (!h.isUsed && ai.spawnLocationChooser.cardSpawnLocations.Contains(h))
                    return h;
            }

            return null;
        }

        private IEnumerator Heal(AIBrain ai)
        {
            ai.ShowMessage("💭 I think we can heal someone");
            if (ai.cardManager.cardInHand.Count == 0 && ai.cardManager.CanDrawMoreCards)
            {
                ai.cardManager.GetNewCard();
            }
            else
            {
            }

            if (ai.cardManager.cardCounter != 0)
            {
                foreach (var item in ai.cardManager.cardInHand)
                {
                    if (item.bottomCard.cardID == "HHHH")
                    {
                        Debug.Log("Heal!");
                        var res = ai.allyCharacters.OrderBy(c => c.life).FirstOrDefault();

                        OfflineHexagon spawnLocation = OfflineHexagonManager.instance.GetHexagon(res.tileIndex);
                        OfflineHexagonManager.instance.SpawnItem(item.bottomCard.cardID, spawnLocation);
                        yield return new WaitForSeconds(3.2f);
                        break;
                    }
                }
            }
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
                        OfflinePlayerTower offlinePlayerTower = tile.GetTower();
                        if (offlinePlayerTower != null)
                        {
                            item.planToAtttackTower = true;
                            item.isFlaggedCharacter = true;

                            flag = true;
                        }
                        else
                        {
                            OfflinePlacableItem enemy = tile.GetPlayer();

                            if (enemy)
                            {
                                enemy.isFlaggedCharacter = true;
                                item.enemyToAttack = enemy;
                                item.isFlaggedCharacter = true;

                                flag = true;
                            }
                            else
                            {
                                OfflineDropableCards card = tile.GetCard();

                                if (card)
                                {
                                    item.cardToAttack = card;
                                    item.isFlaggedCharacter = true;

                                    flag = true;
                                }
                            }
                        }
                    }
                }
            }

            if (!flag)
            {
                ai.ShowMessage("⚠️ No Enemy to attack lets Skip attack round.");

                HandleOnAttackComplete(ai);
                return;
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

            if (item != null && item.planToAtttackTower)
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));
                ai.ShowMessage("🗡️ Let's attack Tower!");
                item.Attack(OfflineHexagonManager.instance.GetHexagon(52));
                item.planToAtttackTower = false;
                item.isFlaggedCharacter = false;
            }
            else if (item != null && item.isFlaggedCharacter && item.enemyToAttack != null)
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));
                try
                {
                    ai.ShowMessage("🗡️ Let's attack " + item.enemyToAttack.name);
                    item.Attack(item.enemyToAttack.tileIndex);
                    item.enemyToAttack = null;
                    item.isFlaggedCharacter = false;
                }
                catch
                {
                    ai.ShowMessage("⚠️ I think enemy is already dead! Sad..!");
                    item.isFlaggedCharacter = false;
                    AttackIfAnyFlagged(ai);
                }
            }
            else if (item != null && item.isFlaggedCharacter && item.cardToAttack != null)//we plan to attack cards
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));
                try
                {
                    ai.ShowMessage("🗡️ Let's attack " + item.cardToAttack.name);
                    item.Attack(item.cardToAttack.tileIndex);
                    item.cardToAttack = null;
                    item.isFlaggedCharacter = false;
                }
                catch
                {
                    ai.ShowMessage("⚠️ I think enemy is already dead! Sad..!");
                    item.isFlaggedCharacter = false;
                    AttackIfAnyFlagged(ai);
                }
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