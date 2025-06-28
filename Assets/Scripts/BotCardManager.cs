using System.Collections.Generic;
using UnityEngine;

public class BotCardManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private Deck deck;

    [SerializeField] private int startingCardCount = 2;

    [Space(20)]
    public List<CardInfo> cards;
    public List<CardInfo> discardDeck;
    public List<CardInfo> cardInHand;

    public int cardCounter;
    public bool CanDrawMoreCards => cardCounter > 0;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        cardInHand = new List<CardInfo>();
        discardDeck = new List<CardInfo>();
        cards = new List<CardInfo>();

        ShuffleDeck();

        for (int i = 0; i < startingCardCount; i++)
        {
            CardInfo data = cards[0];
            cards.Remove(data);

            cardInHand.Add(data);
            discardDeck.Add(data);
        }

        cardCounter = 2;

        BotGameManager.instance.ChangeTurn += ResetTurn;
    }
    private void OnDestroy()
    {
        BotGameManager.instance.ChangeTurn -= ResetTurn;
    }

    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ResetTurn()
    {
        cardCounter = 2;
    }
    public void GetNewCard()
    {
        if (cards.Count == 0)
            ShuffleDeck();

        cardCounter--;
        if (cardCounter < 0) cardCounter = 0;


        CardInfo data = cards[0];
        cards.Remove(data);

        cardInHand.Add(data);
        discardDeck.Add(data);
    }
    private void ShuffleDeck()
    {
        cards = new List<CardInfo>();

        if (discardDeck.Count == 0)
        {
            foreach (var item in deck.cardInfos)
            {
                cards.Add(item);
            }
        }
        else
        {
            foreach (var item in discardDeck)
            {
                cards.Add(item);
            }
        }
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardInfo temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
    #endregion
}
