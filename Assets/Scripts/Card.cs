using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    #region VARIABLES
    public CardType cardType;
    private CardManager cardManager;
    private float oldZ;
    private bool isRotating = false;
    private Vector3 offset;
    private bool isDraggiing = false;
    private Vector3 originalPosition;
    [SerializeField] private GameObject topCard;
    [SerializeField] private GameObject bottomCard;
    private SpriteRenderer topRenderer;
    private SpriteRenderer bottomRenderer;
    [HideInInspector] public string cardID;
    [SerializeField] private string topCardID;
    [SerializeField] private string bottomCardID;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        cardManager = GetComponentInParent<CardManager>();
        topCard.SetActive(true);
        bottomCard.SetActive(false);

        topRenderer = topCard.GetComponent<SpriteRenderer>();
        bottomRenderer = bottomCard.GetComponent<SpriteRenderer>();

        //cardID = topCardID;
        //SetCardType();
    }
    void Start()
    {

    }
    void Update()
    {
        if (isDraggiing)
            Drag();
    }
    #endregion

    #region FUNCTIONS
    private void OnMouseEnter()
    {
        if (cardManager.isDraging)
            return;

        oldZ = transform.position.z;

        transform.DOMoveZ(-1, cardManager.cardZoomTime).SetEase(cardManager.cardZoomEase);
        transform.DOScale(cardManager.newScale, cardManager.cardZoomTime).SetEase(cardManager.cardZoomEase);
    }
    private void OnMouseExit()
    {
        if (cardManager.isDraging)
            return;

        transform.DOMoveZ(oldZ, cardManager.cardZoomTime).SetEase(cardManager.cardZoomEase);
        transform.DOScale(1, cardManager.cardZoomTime).SetEase(cardManager.cardZoomEase);
    }
    private void OnMouseOver()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            RotateCard();
        }
    }
    public void RotateCard()
    {
        if (isRotating || cardManager.isDraging)
            return;

        topCard.SetActive(true);
        bottomCard.SetActive(true);

        isRotating = true;

        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y = rot.y == 0 ? 180 : 0;
        transform.DOLocalRotate(rot, cardManager.rotateCardTime).SetEase(cardManager.rotateCardEase).OnComplete(() =>
        {
            isRotating = false;

            if (transform.localRotation.eulerAngles.y == 0)
            {
                bottomCard.SetActive(false);

                cardID = topCardID;
            }
            else
            {
                topCard.SetActive(false);

                cardID = bottomCardID;
            }
            SetCardType();
        });
    }

    #region DRAG
    private void OnMouseDown()
    {
        isDraggiing = true;
        cardManager.isDraging = true;
        cardManager.DragStart(this);

        originalPosition = transform.position;

        Color color = Color.white;

        color.a = 0.2f;
        topRenderer.color = color;
        bottomRenderer.color = color;

        transform.DOScale(0.6f, cardManager.scakeDownTime).SetEase(cardManager.scaleDownEase);
    }
    private void OnMouseUp()
    {
        isDraggiing = false;
        cardManager.isDraging = false;

        bool flag = IsthereHit();
        if (!flag)
        {
            transform.DOMove(originalPosition, cardManager.cardAlignTime).SetEase(cardManager.cardAlignEase);

            Color color = Color.white;

            color.a = 1f;
            topRenderer.color = color;
            bottomRenderer.color = color;

            transform.DOScale(1f, cardManager.scakeDownTime).SetEase(cardManager.scaleDownEase);
        }
        else
        {
            Destroy(gameObject);
        }
        cardManager.DragEnd(this, flag);
    }
    public void Drag()
    {
        Vector2 mousePosition = cardManager.dragCamera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = mousePosition;
    }
    #endregion
    public bool IsthereHit()
    {
        return HexagonManager.activeHexagon == null ? false : true;
    }
    public void SetCardType()
    {
        string firstTwoLetters = cardID.Substring(0, 2);

        switch (firstTwoLetters)
        {
            case "PC":
                cardType = CardType.ADDON;
                break;
            case "HH":
                cardType = CardType.HEAL;
                break;
            case "CC":
                cardType = CardType.CHARACTER;
                break;
            case "IC":
                cardType = CardType.ICE;
                break;
        }
    }
    public void SetCard(CardInfo card)
    {
        topCardID = card.topCard.cardID;
        bottomCardID = card.bottomCard.cardID;

        cardID = topCardID;

        SetCardType();

        topRenderer.sprite = Gamemanager.instance.isLeft ? card.topCard.red : card.topCard.blue;
        bottomRenderer.sprite = card.bottomCard.blue;
    }
    #endregion
}
public enum CardType
{
    ADDON, BUIDLING, CHARACTER, HEAL, ICE
}