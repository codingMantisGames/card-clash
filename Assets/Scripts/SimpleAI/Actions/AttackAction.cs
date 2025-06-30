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

        private List<OfflinePlacableItem> allyCharacters;
        private List<AttackPlanData> attackPlanDatas;
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

                int spawnCount = Random.Range(1, Mathf.Clamp(actualMoreNeeded, 0, totalWeCanSpawn));

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
                item.treatLevel = item.life + item.attackValue; //also we need to change this score so that it will be treat level score. based on factors
            }

            ai.enemyCharacters = ai.enemyCharacters.OrderByDescending(x => x.treatLevel).ToList(); 
            allyCharacters = new List<OfflinePlacableItem>();

            foreach (var item in ai.allyCharacters) allyCharacters.Add(item);

            attackPlanDatas = new List<AttackPlanData>();

            foreach (var item in ai.enemyCharacters)
            {
                AttackPlanData attackPlan = new AttackPlanData();
                attackPlan.allyUsed = new List<OfflinePlacableItem>();

                float life = item.life;
                attackPlan.enemyToAttack = item;

                //here we need to find ally characters that can attack enenmy. 
                //we have to find they can reach enemy. or we need to try we can use power card 
                //then try using one ally attack value to kill or we can use any cards 
                //if needed we have to spawn some characters 

                attackPlanDatas.Add(attackPlan);

                if (allyCharacters.Count == 0)
                    break;
            }

            ai.ShowMessage("💭 Now we have a attack plan. Let's start attack! ");
        }
        #endregion
    }
}

[System.Serializable]
public class AttackPlanData
{
    public OfflinePlacableItem enemyToAttack;
    public List<OfflinePlacableItem> allyUsed;
}