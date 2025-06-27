using System.Linq;
using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class CharacterEvaluator : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private CharacterRankInfo[] characterCards;
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
        public OfflinePlacableItem EvaluateCharacters(OfflinePlacableItem[] offlinePlacableItems)
        {
            return null;
        }

        public CardInfo EvaluateCard(CardInfo[] cards, bool isTop = true, ActionPlanType planType = ActionPlanType.NORMAL)
        {
            float bestScore = 0;
            CardInfo selectedCard = null;
            foreach (var card in cards)
            {
                float score = GetScore(isTop ? card.topCard.cardID : card.bottomCard.cardID);
                if(score > bestScore)
                {
                    bestScore = score;
                    selectedCard = card;
                }
            }

            return selectedCard;
        }
        private float GetScore(string id, ActionPlanType planType = ActionPlanType.NORMAL)
        {
            CharacterRankInfo card = characterCards.FirstOrDefault(info => info.card.cardID == id);

            if (card == null) return 0;

            if (planType == ActionPlanType.NORMAL)
            {
                return card.attack + card.attackRange + card.life + card.moveRange;
            }

            return 0;
        }
        #endregion
    }
}
[System.Serializable]
public class CharacterRankInfo
{
    public string name;
    public CardData card;

    [Space(20)] public float attack;
    public float life;
    public float attackRange;
    public float moveRange;

    [HideInInspector] public float score;
}