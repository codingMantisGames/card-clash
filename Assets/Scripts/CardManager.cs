using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private List<CardData> cardDatas;
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
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        AlignCard();
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
    }
    public void DragEnd(Card card, bool isHit)
    {
        if (card.cardType == CardType.BUIDLING)
            hexagonManager.ToggleHexagon(false);

        if (isHit)
        {
            Invoke("AlignCard", 0.1f);

            foreach (var item in cardDatas)
            {
                if (item.cardID == card.cardID)
                    hexagonManager.SpawnBuilding(item.prefab);
            }
        }

        HexagonManager.activeHexagon = null;
    }
    [SimpleButton]
    public void AddNewCard()
    {
        if (transform.childCount >= 3)
            return;

        int count = transform.childCount / 2;

        Vector3 pos = transform.position + new Vector3(0, 1.5f, 1);

        if (transform.childCount != 0)
            pos = transform.GetChild(count).position + new Vector3(0, 1.5f, 1);

        Transform temp = Instantiate(card, pos, Quaternion.identity, transform).transform;

        AlignCard();
    }
    #endregion
}
