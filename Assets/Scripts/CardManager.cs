using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private Deck deck;
    public List<CardInfo> cards;
    public List<CardInfo> discardDeck;
    [SerializeField] private HexagonManager hexagonManager;
    public Camera dragCamera;
    [Header("Properties")]
    public Ease cardZoomEase;
    public float cardZoomTime;
    public float newScale = 1.1f;

    [Space(20)] public Ease cardAlignEase;
    public float cardAlignTime;

    [SerializeField, Space(20)] private float cardSpacing;

    [Space(20)] public Ease rotateCardEase;
    public float rotateCardTime;
    [HideInInspector] public bool isDraging;

    [Space(20)] public Ease scaleDownEase;
    public float scakeDownTime;

    public LayerMask itemLayer;
    public GameObject card;

    [SerializeField, Space(20)] private int startCardCount = 2;
    [SerializeField] private int cardDrawLimitPerRound = 1;
    private int cardCounter;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        cardCounter = startCardCount;
        for (int i = 0; i < startCardCount; i++)
        {
            AddNewCard();
        }

        cardCounter = 1;
    }
    void Update()
    {
        if (isDraging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, itemLayer))
            {

            }
            else
            {

            }
        }
    }
    #endregion

    #region FUNCTIONS
    private void OnEnable()
    {
        Gamemanager.instance.ResetRound += ResetData;
    }
    private void OnDisable()
    {
        Gamemanager.instance.ResetRound -= ResetData;
    }
    public void ResetData()
    {
        cardCounter = cardDrawLimitPerRound;
    }
    [SimpleButton]
    public void AlignCard()
    {
        if (transform.childCount == 0)
        {
            return;
        }

        float tempZ = (transform.childCount / 2);
        float totalWidth = (transform.childCount - 1) * cardSpacing;
        float startX = transform.position.x - (totalWidth / 2);

        for (int i = 0; i < transform.childCount; i++)
        {
            float xPos = startX + (i * cardSpacing);
            Vector3 position = new Vector3(xPos, transform.position.y, transform.position.z);
            position.z = tempZ < 0 ? tempZ * -1 : tempZ;
            tempZ--;

            transform.GetChild(i).DOMove(position, cardAlignTime).SetEase(cardAlignEase);
        }
    }
    public void DragStart(Card card)
    {
        if (card.cardType == CardType.BUIDLING)
            hexagonManager.ToggleHexagon(true);
        else if (card.cardType == CardType.CHARACTER)
            hexagonManager.ToggleHexagonForCharacter(true);
        else if (card.cardType == CardType.ADDON)
            hexagonManager.ToogleHexagonForCards(true);
        else if (card.cardType == CardType.HEAL)
            hexagonManager.ToogleForHealCards(true);
        else if (card.cardType == CardType.ICE)
            hexagonManager.ToogleForIceCards(true);

    }
    public void DragEnd(Card card, bool isHit)
    {
        if (card.cardType == CardType.BUIDLING)
            hexagonManager.ToggleHexagon(false);
        else if (card.cardType == CardType.CHARACTER)
            hexagonManager.ToggleHexagonForCharacter(false);
        else if (card.cardType == CardType.ADDON)
            hexagonManager.ToogleHexagonForCards(false);
        else if (card.cardType == CardType.HEAL)
            hexagonManager.ToogleForHealCards(false);
        else if (card.cardType == CardType.ICE)
            hexagonManager.ToogleForIceCards(false);

        if (isHit)
        {
            Invoke("AlignCard", 0.1f);

            hexagonManager.SpawnBuilding(card.cardID);
        }

        //HexagonManager.activeHexagon = null;
    }
    [SimpleButton]
    public void AddNewCard()
    {
        if (cardCounter <= 0)
            return;

        if (cards.Count == 0)
            ShuffleDeck();

        if (transform.childCount >= 3)
            return;

        cardCounter--;

        int count = transform.childCount / 2;

        Vector3 pos = transform.position + new Vector3(0, 1.5f, 1);

        if (transform.childCount != 0)
            pos = transform.GetChild(count).position + new Vector3(0, 1.5f, 1);

        Transform temp = Instantiate(card, pos, Quaternion.identity, transform).transform;

        if (temp.gameObject.TryGetComponent<Card>(out Card c))
        {
            CardInfo data = cards[0];
            cards.Remove(data);

            discardDeck.Add(data);

            c.SetCard(data);
        }

        AlignCard();
    }
    public void ShuffleDeck()
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
