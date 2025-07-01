using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    [CreateAssetMenu(menuName = "Simple AI/Action/Attack Action", fileName = "Attack Action")]
    public class AttackAction : Action
    {
        #region VARIABLES
        [SerializeField] private ActionPlanTypes plan;
        [SerializeField] private int minAllyNeeded = 2;
        [SerializeField] private float randomWaitTimeMin;
        [SerializeField] private float randomWaitTimeMax;

        [Header("Treat Level Calculation")]
        [SerializeField, Space(20)] private float nearTowerScore;
        [SerializeField] private float nearEnemyScore;
        [SerializeField] private float moreAttackScore;
        [SerializeField] private float moreLifeScore;

        private List<OfflinePlacableItem> allyCharacters;
        private List<AttackPlanData> attackPlanDatas;
        private List<MovementPlanData> movementPlanDatas;
        private List<CardSpawnData> cardSpawnDatas;
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
        public override void AttackIfAnyFlagged(AIBrain ai)
        {

        }

        public override void MoveIfAnyFlagged(AIBrain ai)
        {

        }

        public override void PerformAction(AIBrain ai)
        {
            ai.ShowMessage("💭 We have to kill some enemy this round!");

            if (ai.allyCharacters.Count < minAllyNeeded)
            {
                int actualMoreNeeded = ai.allyCharacters.Count - minAllyNeeded;

                int totalWeCanSpawn = ai.cardManager.cardInHand.Count + ai.cardManager.cardCounter;

                //int spawnCount = Random.Range(1, Mathf.Clamp(actualMoreNeeded, 0, totalWeCanSpawn));
                int spawnCount = 2;

                ai.ShowMessage("💭 Enemy has " + ai.enemyCharacters.Count + ". So we have to spawn atleast " + spawnCount + " charaters!");

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
            HandleOnSpawnComplete(ai);
        }

        public void HandleOnSpawnComplete(AIBrain ai) //the first round is not complete. still we have to make plan for attack ussing cards in hand. if we need more ally we have to spawn 
        {
            ai.ShowMessage("💭 Now we need a attack plan...");

            foreach (var item in ai.enemyCharacters)
            {
                item.treatLevel = 0;

                int c = item.AnyTargetInAttackRange(ai.roundStage);
                item.treatLevel += c * nearEnemyScore;

                if (item.CanAttackAIsTower(ai.roundStage)) item.treatLevel += nearTowerScore;

                if (item.life >= item.totalLife / 2) item.treatLevel += moreLifeScore;
                if (item.attackValue >= 2.5f) item.treatLevel += moreAttackScore;
            }

            ai.enemyCharacters = ai.enemyCharacters.OrderByDescending(x => x.treatLevel).ToList();
            allyCharacters = new List<OfflinePlacableItem>();

            foreach (var item in ai.allyCharacters) allyCharacters.Add(item);

            allyCharacters = allyCharacters.OrderByDescending(x => x.attackValue).ToList();

            attackPlanDatas = new List<AttackPlanData>();
            movementPlanDatas = new List<MovementPlanData>();
            cardSpawnDatas = new List<CardSpawnData>();

            foreach (var item in ai.enemyCharacters)
            {
                AttackPlanData attackPlan = new AttackPlanData();
                attackPlan.allyUsed = new List<OfflinePlacableItem>();

                float life = item.life;
                attackPlan.enemyToAttack = item;
                attackPlan.tileToAttack = OfflineHexagonManager.instance.GetHexagon(item.tileIndex);

                foreach (var ally in allyCharacters)
                {
                    if (!ally.isFlaggedCharacter && ally.CanAttack(attackPlan.tileToAttack)) //Player can attack using his attack range no movement needed.
                    {
                        OfflineHexagon hex = OfflineHexagonManager.instance.GetHexagon(ally.tileIndex);
                        CardSpawnData res = CanUsePowerBoost(hex, ai); //This is because we plan to spawn near ally
                        if (res != null)
                        {
                            life -= ally.attackValue + 1;

                            CardSpawnData spawnData = new CardSpawnData();
                            spawnData.hexToSpawn = res.hexToSpawn;
                            spawnData.card = res.card;
                            cardSpawnDatas.Add(spawnData);


                            ai.cardManager.cardInHand.Remove(res.card);
                            attackPlan.allyUsed.Add(ally);

                            ally.isFlaggedCharacter = true;
                            ally.enemyToAttack = item;

                            if (life <= 0) break;
                        }
                        else
                        {
                            CardSpawnData res2 = CanUseStrikeFlow(hex, ai); //This is also we plan to spawn near ally
                            if (res2 != null)
                            {
                                CardSpawnData spawnData = new CardSpawnData();
                                spawnData.hexToSpawn = res2.hexToSpawn;
                                spawnData.card = res2.card;
                                cardSpawnDatas.Add(spawnData);

                                life -= ally.attackValue * 2;
                                ai.cardManager.cardInHand.Remove(res2.card);
                                attackPlan.allyUsed.Add(ally);
                                attackPlan.allyUsed.Add(ally);

                                ally.isFlaggedCharacter = true;
                                ally.enemyToAttack = item;

                                if (life <= 0) break;
                            }
                            else
                            {
                                life -= ally.attackValue;
                                attackPlan.allyUsed.Add(ally);

                                ally.isFlaggedCharacter = true;
                                ally.enemyToAttack = item;

                                if (life <= 0) break;
                            }
                        }
                    }
                    else
                    {
                        OfflineHexagon tile = OfflineHexagonManager.instance.GetHexagon(ally.tileIndex);
                        CardSpawnData res = CanUseRangeSurge(tile, ai);
                        OfflineHexagon hex = ally.CanMoveAndAttack(attackPlan.tileToAttack, res == null ? false : true);
                        if (hex != null)
                        {
                            if (res != null) //that means we can use card
                            {
                                CardSpawnData spawnData = new CardSpawnData();
                                spawnData.hexToSpawn = res.hexToSpawn;
                                spawnData.card = res.card;
                                cardSpawnDatas.Add(spawnData);
                            }

                            MovementPlanData data = new MovementPlanData();
                            data.ally = ally;
                            data.hexagonToMove = hex;

                            ai.cardManager.cardInHand.Remove(res.card);

                            life -= ally.attackValue;
                            attackPlan.allyUsed.Add(ally);
                            allyCharacters.Remove(ally);

                            ally.isFlaggedCharacter = true;
                            ally.enemyToAttack = item;

                            if (life <= 0) break;
                        }
                        else
                        {
                            //we cant move and attack also
                        }
                    }
                }

                if (attackPlan.allyUsed.Count > 0)
                    attackPlanDatas.Add(attackPlan);

                if (allyCharacters.Count == 0)
                    break;
            }

            foreach (var attack in attackPlanDatas)
            {
                attack.enemyToAttack.isFlaggedCharacter = true;

                foreach (var item in allyCharacters)
                {
                    item.enemyToAttack = attack.enemyToAttack;
                    item.isFlaggedCharacter = true;
                }
            }

            if (attackPlanDatas.Count > 0)
                ai.ShowMessage("💭 Now we have a attack plan. Let's start attack! ");
            else
            {
                ai.ShowMessage("💭 Oooh no!. Cant attack now. We have to plan Normal");
            }

            //make a small movement only if attack is not found
        }

        //if we have 3 card then check consition or draw cards as needed. if cant draw more card check this cards in hand
        //Also check can we spawn this tile near posaition we need 
        private CardSpawnData CanUsePowerBoost(OfflineHexagon tile, AIBrain ai) //Attack value  +1
        {
            return CanUseCard(tile, ai, "PC32");
        }

        private CardSpawnData CanUseRangeSurge(OfflineHexagon tile, AIBrain ai) //Movement range = 3
        {
            return CanUseCard(tile, ai, "PC32");
        }

        private CardSpawnData CanUseStrikeFlow(OfflineHexagon tile, AIBrain ai) //Attck per round = 2. ie damage * 2
        {
            return CanUseCard(tile, ai, "PC33");
        }

        CardSpawnData CanUseCard(OfflineHexagon tile, AIBrain ai, string id)
        {
            if (ai.cardManager.cardInHand.Count != 3)
            {
                while (ai.cardManager.cardInHand.Count != 3 && ai.cardManager.cardCounter > 0)
                {
                    ai.ShowMessage("💭 Mmm! We need one more card!");

                    ai.cardManager.GetNewCard();
                }
            }

            CardSpawnData result = null;
            foreach (var card in ai.cardManager.cardInHand)
            {
                if (card.bottomCard.cardID == id)
                {
                    result = new CardSpawnData();

                    result.card = card;
                    break;
                }
            }

            if (result != null)
            {
                OfflineHexagon[] hexs = tile.GetAllAdjacnetTile();
                result.hexToSpawn = hexs.FirstOrDefault(info => !info.isUsed);

                if (result.hexToSpawn == null)
                    return null;
            }

            return result;
        }
        #endregion
    }
}

[System.Serializable]
public class AttackPlanData
{
    public OfflinePlacableItem enemyToAttack;
    public OfflineHexagon tileToAttack;
    public List<OfflinePlacableItem> allyUsed;
}

[System.Serializable]
public class MovementPlanData
{
    public OfflinePlacableItem ally;
    public OfflineHexagon hexagonToMove;
}

[System.Serializable]
public class CardSpawnData
{
    public CardInfo card;
    public OfflineHexagon hexToSpawn;
}