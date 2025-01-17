using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using DG.Tweening;
using TMPro;

public class PlaceableItem : NetworkBehaviour
{
    #region VARIABLES
    [SerializeField] private List<SkinnedMeshRenderer> skinnedMeshRenderers;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField, Space(20)] private Outline outline;
    [SerializeField] private bool isMainBuilding = false;
    private float[] range = new float[] { 1.8f, 3.7f, 5.4f };
    [SerializeField, Range(1, 3)] private int m_MovementRange = 1;
    [SerializeField, Range(1, 3)] private int m_AttackRange = 1;
    [SerializeField] private LayerMask hexagonLayer;
    [SerializeField] private LayerMask playerLayer;
    [Networked] public bool isLeft { set; get; }
    [Networked] public int tileIndex { set; get; }
    bool isSelected;
    bool isHighlighted;
    Vector3 targetPos;
    bool canMove;
    [SerializeField] private Animator animController;
    [SerializeField] private Transform textHolder;
    [HideInInspector] public int moveCount;
    public GameObject itemToDisable;
    private Collider[] colliders;
    private Collider[] playerColliders;
    [SerializeField] private GameObject line;
    [SerializeField] private List<GameObject> lines;
    [SerializeField] private bool checkLineOfSite = true;
    [SerializeField] private bool isNearByAttack = false;
    [SerializeField] private float attackStoppingDistance;
    [SerializeField] private float goBackDelay;
    [SerializeField] private float timeBtwTiletoTileMovement = 1;
    private int currentAttackIndex;
    Vector3 startPoint;
    [SerializeField, Space(20)] private Collider[] cardColliders;
    [SerializeField] private LayerMask cardLayer;
    private List<DropableCard> dropableCards;
    private ChangeDetector _changeDetector;
    [SerializeField] private int totalLife;
    [SerializeField] private int realAttackValue;
    [Networked] public int life { set; get; }
    [Networked] public int attackValue { set; get; }
    [Networked] public bool isFreezed { set; get; }
    [Networked] public int freezedCounter { set; get; }
    [SerializeField] private TMP_Text lifeLabel;
    [SerializeField] private TMP_Text attackValueLabel;
    [SerializeField] private Material freezeMaterial;
    #endregion

    #region UNITY FUNCTIONS
    IEnumerator Start()
    {
        targetPos = transform.position;

        yield return new WaitForEndOfFrame();
        if (!Gamemanager.instance.isLeft && textHolder)
        {
            textHolder.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        moveCount = 1;
        lines = new List<GameObject>();
        dropableCards = new List<DropableCard>();

        Gamemanager.instance.ResetRound += ResetRound;

    }
    void Update()
    {

    }

    #endregion

    #region FUNCTIONS
    public void ResetRound()
    {
        moveCount = 1;
        if (freezedCounter != 0)
        {
            isFreezed = false;

            RPC_UnFreezePlayer();
            RPC_SetMaterial(isLeft);
        }
        freezedCounter++;
    }
    public void SetBuilding(bool flag = false)
    {
        isLeft = flag;

        RPC_SetMaterial(flag);

        Invoke("CheckForPowerCards", 0.1f);

        if (Gamemanager.instance.isLeft == isLeft)
            Gamemanager.instance.CheckPlayerPosition += CheckForCards;

        life = totalLife;
        attackValue = realAttackValue;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetMaterial(bool flag)
    {
        foreach (var item in skinnedMeshRenderers)
        {
            if (flag)
                item.material = redMat;
            else
                item.material = blueMat;
        }
    }
    public void Freeze()
    {
        isFreezed = true;
        freezedCounter = 0;

        RPC_FreezePlayer();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_FreezePlayer()
    {
        foreach (var item in skinnedMeshRenderers)
        {
            item.material = freezeMaterial;
        }
        if (animController)
            animController.enabled = false;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UnFreezePlayer()
    {
        if (animController)
            animController.enabled = true;
    }
    public void Heal()
    {
        RPC_ChangeLife(10);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeLife(int val)
    {
        life = val;
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    private void OnEnable()
    {
        Gamemanager.instance.OnItemSelected += HideOutline;
    }
    public void SetInitialRotation()
    {
        RPC_SetRotation(Quaternion.Euler(0, 270, 0));
    }
    private void OnDestroy()
    {
        Gamemanager.instance.OnItemSelected -= HideOutline;
        Gamemanager.instance.ResetRound -= ResetRound;
    }
    public void OnMouseDownFun()
    {
        if (Gamemanager.instance.isLeft == isLeft && moveCount == 0)
        {
            Debug.LogWarning("Cant Move!.How this as some message");
            return;
        }

        if (Gamemanager.instance.isLeft == isLeft && isFreezed)
        {
            Debug.LogWarning("freezed");
            return;
        }
        /*if (Gamemanager.instance.isLeft != isLeft && Gamemanager.instance.currentRoundStage == RoundStage.ATTACK)
        {
            HexagonManager.instance.CallOnMouseDown(tileIndex);
        }*/


        if (Gamemanager.instance.isLeft == isLeft && Gamemanager.instance.currentRoundStage != RoundStage.USING_CARDS && !isHighlighted)
        {
            Gamemanager.instance.OnItemSelected?.Invoke();

            outline.enabled = true;
            //HexagonManager.instance.ShowMovableTiles(tileIndex, movementType);
            //this is place i want to change logic 
            if (HexagonManager.instance.isHexMoveOn)
                HexagonManager.instance.HideAllHex();

            if (Gamemanager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
                CursorChanger.instance.SetMoveCursor();
            else if (Gamemanager.instance.currentRoundStage == RoundStage.ATTACK)
                CursorChanger.instance.SetAttackCursor();

            colliders = new Collider[20];
            int r = 0;
            if (Gamemanager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
                r = m_MovementRange - 1;
            else
                r = m_AttackRange - 1;

            int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
            for (int i = 0; i < num; i++)
            {
                if (colliders[i].gameObject.TryGetComponent<HexagonTile>(out HexagonTile tile))
                {
                    if (Gamemanager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
                        HexagonManager.instance.SelectHexagon(tile);
                    else
                        HexagonManager.instance.SelectHexagonAll(tile);

                    if (tile.isUsed && Gamemanager.instance.currentRoundStage == RoundStage.ATTACK)
                    {
                        playerColliders = new Collider[1];
                        int k = Physics.OverlapSphereNonAlloc(tile.buildPoint.position, 0.5f, playerColliders, playerLayer);

                        if (k != 0)
                        {
                            if (playerColliders[0].gameObject.TryGetComponent<PlaceableItem>(out PlaceableItem item))
                            {
                                if (Gamemanager.instance.isLeft != item.isLeft)
                                {
                                    if ((!checkLineOfSite) || (checkLineOfSite && HasLineOfSight(transform.position, item.transform)))
                                    {
                                        tile.canAttack = true;
                                        tile.HighlightHexagon(true);

                                        GameObject gm = Instantiate(line, transform);
                                        lines.Add(gm);

                                        if (gm.TryGetComponent<Line>(out Line l))
                                        {
                                            l.SetPosition(transform.position, playerColliders[0].transform.position);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (num != 0)
                HexagonManager.instance.isHexMoveOn = true;

            isHighlighted = true;

            Gamemanager.instance.currentItemToMove = this;
            // CursorChanger.instance.SetMoveCursor();
        }
        else if (isHighlighted)
        {
            HexagonManager.instance.HideAllHex();

            isHighlighted = false;
            outline.enabled = false;

            CursorChanger.instance.SetNormalCursor();

            Gamemanager.instance.currentItemToMove = null;

            foreach (var item in lines)
            {
                Destroy(item);
            }
            lines = new List<GameObject>();
        }
    }
    public bool HasLineOfSight(Vector3 pointA, Transform target)
    {
        Vector3 pointB = target.position;

        pointA += new Vector3(0, 0.1f, 0);
        pointB += new Vector3(0, 0.1f, 0);

        Vector3 direction = pointB - pointA;
        float distance = direction.magnitude;

        if (Physics.Raycast(pointA, direction.normalized, out RaycastHit hit, distance, playerLayer))
        {
            if (hit.transform == target)
                return true;
            else
                return false;
        }
        return false;
    }
    public void HideOutline()
    {
        if (outline)
            outline.enabled = false;
        isHighlighted = false;

        CursorChanger.instance.SetNormalCursor();

        Gamemanager.instance.currentItemToMove = null;

        foreach (var item in lines)
        {
            Destroy(item);
        }
        lines = new List<GameObject>();
    }
    public void Attack(HexagonTile tile)
    {
        RPC_Attack(tile.index);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Attack(int hexIndex)
    {
        moveCount--;
        currentAttackIndex = hexIndex;

        if (isNearByAttack)
        {
            HexagonTile tile = HexagonManager.instance.GetHexagon(hexIndex);
            startPoint = transform.position;
            Sequence sequence = DOTween.Sequence();
            Vector3 pos = GetStoppingPoint(tile.buildPoint.position);
            sequence.Append(DOTween.To(() => targetPos, x => targetPos = x, pos, timeBtwTiletoTileMovement).SetEase(Ease.Linear).OnComplete(() =>
            {
                RPC_StartAnimation("move", false);
                RPC_StartTriggerAnimation("attack");
            }).OnStart(() =>
            {
                canMove = true;
                RPC_StartAnimation("move", true);
                Vector3 direction = pos - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                RPC_SetRotation(targetRot);
            }));

            sequence.Append(DOTween.To(() => targetPos, x => targetPos = x, startPoint, timeBtwTiletoTileMovement).SetEase(Ease.Linear).SetDelay(goBackDelay).OnComplete(() =>
            {
                RPC_StartAnimation("move", false);
                RPC_SetRotation(Quaternion.Euler(0, isLeft ? 90 : 270, 0));
                canMove = false;

                HexagonManager.activeHexagon = null;
            }).OnStart(() =>
            {
                RPC_StartAnimation("move", true);

                Vector3 direction = startPoint - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                RPC_SetRotation(targetRot);
            }));

            sequence.Play();
            HexagonManager.instance.HideAllHex();
            HideOutline();
        }
        else
        {

        }

    }
    public void DealDamageToEnemy()
    {
        PlaceableItem item = HexagonManager.instance.GetHexagon(currentAttackIndex).GetPlayer();
        if (item)
            item.Damage(20);
    }
    public void Damage(int damage)
    {
        RPC_Damage(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Damage(int damage)
    {
        Debug.Log("Damage " + damage + " ||" + gameObject.name);
    }

    public Vector3 GetStoppingPoint(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        if (attackStoppingDistance >= distanceToTarget)
        {
            Debug.LogWarning("Stop distance is greater than or equal to the distance to the target. Returning startPoint.");
            return transform.position;
        }

        return targetPoint - direction * attackStoppingDistance;
    }

    public void MoveToPosition(Vector3[] pos, int index, bool isLeft, int cIndex)
    {
        RPC_MoveTo(pos, index, isLeft, cIndex);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_MoveTo(Vector3[] pos, int i, bool isLeft, int cIndex)
    {
        moveCount--;

        Sequence sequence = DOTween.Sequence();

        for (int j = 1; j < pos.Length; j++)
        {
            Vector3 direction = pos[j] - transform.position;

            Quaternion targetRot = Quaternion.LookRotation(direction);

            sequence.Append(
           DOTween.To(() => targetPos, x => targetPos = x, pos[j], timeBtwTiletoTileMovement).SetEase(Ease.Linear).OnStart(() =>
           {
               RPC_SetRotation(targetRot);
           }));
        }
        RPC_StartAnimation("move", true);
        sequence.OnComplete(() =>
        {
            canMove = false;
            RPC_StartAnimation("move", false);
            RPC_SetRotation(Quaternion.Euler(0, isLeft ? 90 : 270, 0));
            tileIndex = i;

            HexagonTile tile = HexagonManager.instance.GetHexagon(i);
            tile.isUsed = true;
            tile = HexagonManager.instance.GetHexagon(cIndex);
            tile.isUsed = false;

            CheckForPowerCards();

            HexagonManager.activeHexagon = null;
        }).OnStart(() =>
        {
            RemoveAllCards();
        });

        sequence.Play();
        canMove = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartAnimation(string anim, bool flag)
    {
        animController.SetBool(anim, flag);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartTriggerAnimation(string anim)
    {
        animController.SetTrigger(anim);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetRotation(Quaternion rot)
    {
        transform.GetChild(0).localRotation = rot;
    }
    public override void Render()
    {
        if (canMove)
        {
            transform.position = targetPos;
        }

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(life):
                    lifeLabel.text = life.ToString();
                    break;
                case nameof(attackValue):
                    attackValueLabel.text = attackValue.ToString();
                    break;
            }
        }
    }
    public void CheckForCards()
    {
        Invoke("CheckForPowerCards", 0.1f);
    }
    public void CheckForPowerCards()
    {
        cardColliders = new Collider[6];
        int num = Physics.OverlapSphereNonAlloc(transform.position, 2f, cardColliders, cardLayer);
        for (int i = 0; i < num; i++)
        {
            if (cardColliders[i].TryGetComponent<DropableCard>(out DropableCard card) && card.isLeft)
            {
                dropableCards.Add(card);

                card.AddItem(transform);
            }
        }
    }
    public void RemoveAllCards()
    {
        foreach (var item in dropableCards)
        {
            item.HideItem(transform);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
    #endregion
}