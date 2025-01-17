using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using DG.Tweening;

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
        ResetRound();

        lines = new List<GameObject>();
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ResetRound()
    {
        moveCount = 5;
    }
    public void SetBuilding(bool flag = false)
    {
        isLeft = flag;

        foreach (var item in skinnedMeshRenderers)
        {
            if (flag)
                item.material = redMat;
            else
                item.material = blueMat;
        }
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
    }
    public void OnMouseDownFun()
    {
        if (moveCount == 0)
        {
            Debug.LogWarning("Cant Move!.How this as some message");
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
    }
    #endregion
}