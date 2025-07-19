using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Action/Attack Enemy Tower Action", fileName = "Attack Enemy Tower Action")]
    public class AttackEnemyTowerAction : Action
    {
        #region VARIABLES
        public int spawnCount;
        [SerializeField] private ActionPlanTypes plan;
        [SerializeField] private int minAllyNeeded = 2;

        [SerializeField] private float randomWaitTimeMin;
        [SerializeField] private float randomWaitTimeMax;

        [SerializeField] public List<MovementPlanData> movementPlanDatas;

        int attackIndex;
        int moveIndex;
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
            ai.ShowMessage("💭 Lets Destroy enemy tower!");

            attackIndex = 0;
            moveIndex = 0;

            if (ai.allyCharacters.Count < minAllyNeeded)
            {
                int actualMoreNeeded = ai.allyCharacters.Count - minAllyNeeded;

                int totalWeCanSpawn = ai.cardManager.cardInHand.Count + ai.cardManager.cardCounter;

                //int spawnCount = Random.Range(1, Mathf.Clamp(actualMoreNeeded, 0, totalWeCanSpawn));

                ai.ShowMessage("💭 Enemy has " + ai.enemyCharacters.Count + ". So we have to spawn atleast " + spawnCount + " charaters!");

                ai.StartRoutine(CardDropProcedure(spawnCount, ai));
            }
            else
            {
                ai.ShowMessage("💭 We have minimum ally!");
                HandleOnSpawnComplete(ai);
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
                OfflineHexagonManager.instance.SpawnItem(selectedCard.topCard.cardID, ai.testPosition);

                ai.ShowMessage("🧙 " + selectedCard.topCard.name + " Character Spawned");
            }

            ai.ShowMessage("✅ Successfully Dropped Cards as needed!");
            HandleOnSpawnComplete(ai);
        }

        private void HandleOnSpawnComplete(AIBrain ai)
        {
            movementPlanDatas = new List<MovementPlanData>();
            float life = ai.enemyTower.life;
            OfflineHexagon tileToAttack = OfflineHexagonManager.instance.GetHexagon(52);
            bool flag = false;
            foreach (var ally in ai.allyCharacters)
            {
                bool canAttackWithoutMovement = ally.CanAttack(tileToAttack, ai.enemyTower.transform);
                if (!ally.planToAtttackTower && canAttackWithoutMovement)
                {
                    ally.planToAtttackTower = true;
                    life -= ally.attackValue + (ally.isPowerboostCardUsed ? 1 : 0);
                    flag = true;
                    if (life <= 0) break;
                }
                else if (!ally.planToAtttackTower && !canAttackWithoutMovement)
                {
                    OfflineHexagon hex = ally.CanMoveAndAttack(tileToAttack, ally.isRangeCardUsed, ai.enemyTower.transform);
                    if (hex != null)
                    {
                        MovementPlanData data = new MovementPlanData();
                        data.ally = ally;
                        data.hexagonToMove = hex;
                        ally.hexagonToMove = hex;
                        hex.isMarkedByAI = true;
                        movementPlanDatas.Add(data);
                        data.data = ally.name + " moves to " + hex.transform.name;


                        life -= ally.attackValue + (ally.isPowerboostCardUsed ? 1 : 0);

                        ally.planToAtttackTower = true;
                        flag = true;
                        if (life <= 0) break;
                    }
                }
            }

            if (flag)
            {
                ai.ShowMessage("💭 Now we have a attack plan. Let's start attack!");
                HandleOnAttackPlanComplete(ai);
            }
            else
            {
                ai.ShowMessage("💭 Oooh no!. Cant attack now. We have to plan Normal");
                ai.EndRound();
            }

        }
        public override void AttackIfAnyFlagged(AIBrain ai)
        {
            bool flag = false;
            foreach (var item in ai.allyCharacters)
            {
                if (item.planToAtttackTower)
                {
                    flag = true;
                    item.planToAtttackTower = false;
                    ai.StartRoutine(AttackProcedure(ai, item));
                    break;
                }

            }

            if (!flag)
            {
                ai.EndRound();
            }
        }

        IEnumerator AttackProcedure(AIBrain ai, OfflinePlacableItem item)
        {
            yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));

            if (item.enemyToAttack == null || (item.enemyToAttack != null && item.AnythingBlocking(ai.enemyTower.transform)))
            {
                AttackIfAnyFlagged(ai);
            }
            else
            {
                item.Attack(item.enemyToAttack.tileIndex);
                ai.ShowMessage("🗡️ Let's attack " + item.enemyToAttack.name);
            }
        }

        public override void MoveIfAnyFlagged(AIBrain ai)
        {
            ai.StartRoutine(MoveProcedure(ai));
        }
        IEnumerator MoveProcedure(AIBrain ai)
        {
            yield return null;
            if (moveIndex >= movementPlanDatas.Count)
            {
                AttackEnemy(ai);
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(randomWaitTimeMin, randomWaitTimeMax));

                MovementPlanData item = movementPlanDatas[moveIndex];

                BotGameManager.instance.MoveCurentItem(item.hexagonToMove, OfflineHexagonManager.instance.GetIndex(item.hexagonToMove), item.ally);
                item.ally.isFlaggedCharacter = false;
                ai.ShowMessage("🧙 Started Moving " + item.ally.gameObject.name);

                moveIndex++;
            }
        }
        private void AttackEnemy(AIBrain ai)
        {
            ai.ShowMessage("💭 Oh yaaa...Not Lets Kill some enemy.");

            ai.UpdateRound();

            AttackIfAnyFlagged(ai);
        }

        private void HandleOnAttackPlanComplete(AIBrain ai)
        {
            MoveCharactersAsNeeded(ai);
        }
        private void MoveCharactersAsNeeded(AIBrain ai)
        {
            ai.UpdateRound();
            if (movementPlanDatas.Count > 0)
            {
                ai.ShowMessage("💭 OK!! Lets move come characters.");

                MoveIfAnyFlagged(ai);
            }
            else
            {
                AttackEnemy(ai);
            }
        }
        #endregion
    }
}