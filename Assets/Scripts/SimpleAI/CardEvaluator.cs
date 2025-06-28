using System.Linq;
using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    public class CardEvaluator : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private CardEvaluatorData[] cardEvaluatorDatas;
        [SerializeField] private CharacterRankInfo[] characterRankInfos;
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
        public CardInfo EvaluateCard(CardInfo[] cards, bool isTop, ActionPlanTypes plan)
        {
            float bestScore = 0;
            CardInfo selectedCard = null;
            foreach (var card in cards)
            {
                float score = GetScore(plan, isTop ? card.topCard.cardID : card.bottomCard.cardID, isTop);
                if (score > bestScore)
                {
                    bestScore = score;
                    selectedCard = card;
                }
            }

            return selectedCard;
        }

        public float GetScore(ActionPlanTypes plan, string id, bool isTop)
        {
            CharacterRankInfo characterRankInfo = characterRankInfos.FirstOrDefault(info => info.card.cardID == id);

            foreach (var cardEvaluatorData in cardEvaluatorDatas)
            {
                if (cardEvaluatorData.actionPlan == plan)
                {
                    float score = 0;
                    if (cardEvaluatorData._attack) score += characterRankInfo.attack;
                    if (cardEvaluatorData._life) score += characterRankInfo.life;
                    if (cardEvaluatorData._attackRange) score += characterRankInfo.attackRange;
                    if (cardEvaluatorData._movementRange) score += characterRankInfo.moveRange;
                    break;
                }
            }
            return 0;
        }
        #endregion
    }
    [System.Serializable]
    public class CardEvaluatorData
    {
        public string name;
        public ActionPlanTypes actionPlan;

        public bool _attack;
        public bool _life;
        public bool _attackRange;
        public bool _movementRange;
    }
}