using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using DG.Tweening;
using TMPro;

public class PlaceableItem : NetworkBehaviour
{
    #region VARIABLES
    [SerializeField] private string nameOfCharacter;
    [SerializeField] private List<SkinnedMeshRenderer> skinnedMeshRenderers;
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField, Space(20)] private Outline outline;
    [SerializeField] private bool isMainBuilding = false;
    private float[] range = new float[] { 1.8f, 3.7f, 5.4f };
    [SerializeField, Range(1, 3)] private int m_MovementRange = 1;
    private bool isRangeCardUsed = false;
    private bool isStrikeCardUsed = false;
    private bool canAttackMore = false;
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
    [Networked] public int moveCount { set; get; }
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
    [Networked] public int currentAttackIndex { set; get; }
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

    [SerializeField, Space(20)] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Ease projecileMovementEase;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject ghost_red;
    [SerializeField] private GameObject ghost_blue;
    private AudioSource runAudio;
    public AudioSource projectileHitAudio;
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
        Gamemanager.instance.ChnageTurn += ChangeTurn;

        runAudio = GetComponent<AudioSource>();
    }
    void Update()
    {

    }

    #endregion

    #region FUNCTIONS
    public void ChangeTurn()
    {
        if (isFreezed)
        {
            isFreezed = false;

            RPC_UnFreezePlayer();
            RPC_SetMaterial(isLeft);

            freezedCounter++;
        }
    }
    public void ResetRound()
    {
        RPC_ResetMoveCounter();

        if (isStrikeCardUsed)
            canAttackMore = true;

        outline.enabled = false;
        isHighlighted = false;
        Gamemanager.instance.HideCharacterDetails();
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetMoveCounter()
    {
        moveCount = 1;
    }
    public void SetBuilding(bool flag = false)
    {
        isLeft = flag;

        RPC_SetMaterial(flag);

        Invoke("CheckForPowerCards", 0.1f);

        Gamemanager.instance.CheckPlayerPosition += CheckForCards;

        life = totalLife;
        attackValue = realAttackValue;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetMaterial(bool flag)
    {
        Debug.Log("asa " + gameObject.name);
        foreach (var item in skinnedMeshRenderers)
        {
            if (flag)
                item.material = redMat;
            else
                item.material = blueMat;
        }
        foreach (var item in meshRenderers)
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
        foreach (var item in meshRenderers)
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
        RPC_ChangeLife(totalLife);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeLife(int val)
    {
        life = val;
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        lifeLabel.text = life.ToString();
        attackValueLabel.text = attackValue.ToString();
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
        Gamemanager.instance.ChnageTurn -= ChangeTurn;

        try
        {
            Gamemanager.instance.CheckPlayerPosition -= CheckForCards;
        }
        catch
        {
            Debug.LogWarning("Not Added!");
        }
    }
    public void OnMouseDownFun()
    {
        if (!Gamemanager.instance.isPlayerTurn || !Gamemanager.instance.canInteract)
            return;

        if ((Gamemanager.instance.isLeft == isLeft && moveCount == 0))
        {
            Gamemanager.instance.ShowNoMovesPending(transform.position, "No Moves Remaining");
            return;
        }

        if (Gamemanager.instance.isLeft == isLeft && isFreezed)
        {
            Gamemanager.instance.ShowNoMovesPending(transform.position, "Character Frozen");
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

            colliders = new Collider[50];
            int r = 0;
            if (Gamemanager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
            {
                r = m_MovementRange - 1;
                if (isRangeCardUsed)
                    r = 2;
            }
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
                            else if (playerColliders[0].gameObject.TryGetComponent<PlayerTower>(out PlayerTower tower))
                            {
                                if (Gamemanager.instance.isLeft != tower.isLeft)
                                {
                                    if ((!checkLineOfSite) || (checkLineOfSite && HasLineOfSight(transform.position, tower.transform)))
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
                            else if (playerColliders[0].gameObject.TryGetComponent<DropableCard>(out DropableCard card))
                            {
                                if (Gamemanager.instance.isLeft != card.isLeft)
                                {
                                    if ((!checkLineOfSite) || (checkLineOfSite && HasLineOfSight(transform.position, card.transform)))
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

            #region CHARACTER DETAILS
            int attackRange = 0;
            attackRange = m_MovementRange;
            if (isRangeCardUsed)
                attackRange = 3;
            string chance = "1";
            if (isFreezed)
                chance = "Freezed";
            if (isStrikeCardUsed)
            {
                if (canAttackMore)
                    chance = "2";
                else
                    chance = "1";
            }
            Gamemanager.instance.ShowCharacterDetails(nameOfCharacter, attackRange.ToString(), m_AttackRange.ToString(), life.ToString(), attackValue.ToString(), chance);
            #endregion
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

            Gamemanager.instance.HideCharacterDetails();
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
        Gamemanager.instance.HideCharacterDetails();
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
        if (!canAttackMore)
            moveCount--;
        else
        {
            canAttackMore = false;
        }
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
                //textHolder.gameObject.SetActive(false);
                RPC_HideText(false);
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
                //textHolder.gameObject.SetActive(true);
                RPC_HideText(true);

                Gamemanager.instance.EnableButtons();

            }).OnStart(() =>
            {
                Gamemanager.instance.DisableButtons();

                RPC_StartAnimation("move", true);

                Vector3 direction = startPoint - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                RPC_SetRotation(targetRot);
            }));

            sequence.Play();
            //this is remove to fix bugs
            //HexagonManager.instance.HideAllHex();
            //HideOutline(); 
            RPC_HideOutline();
        }
        else
        {
            //this is remove to fix bugs
            //HexagonManager.instance.HideAllHex();
            //HideOutline();
            RPC_HideOutline();

            HexagonTile tile = HexagonManager.instance.GetHexagon(currentAttackIndex);

            Vector3 direction = tile.buildPoint.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            RPC_SetRotation(targetRot);

            Invoke("AttackAnimation", 0.5f);
        }

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_HideOutline()
    {
        HexagonManager.instance.HideAllHex();
        HideOutline();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_HideText(bool flag)
    {
        textHolder.gameObject.SetActive(flag);
    }
    void AttackAnimation()
    {
        RPC_StartTriggerAnimation("attack");
    }
    public void OnAttack()
    {
        GameObject gm = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        HexagonTile tile = HexagonManager.instance.GetHexagon(currentAttackIndex);
        Vector3 pos = tile.buildPoint.position;

        pos.y = gm.transform.position.y;

        if (isLeft)
            gm.transform.GetChild(2).gameObject.SetActive(true);
        else
            gm.transform.GetChild(1).gameObject.SetActive(true);

        float dis = Vector3.Distance(pos, gm.transform.position);
        float time = dis / projectileSpeed;

        gm.transform.DOMove(pos, time).SetEase(projecileMovementEase).OnComplete(() =>
        {
            Destroy(gm);

            if (Runner.IsServer)
                RPC_SetRotation(Quaternion.Euler(0, isLeft ? 90 : 270, 0));

            if (projectileHitAudio)
                projectileHitAudio.Play();

            DealDamageToEnemy();
        });
    }
    public void DealDamageToEnemy()
    {
        PlaceableItem item = HexagonManager.instance.GetHexagon(currentAttackIndex).GetPlayer();
        if (item && Runner.IsServer)
        {
            item.Damage(attackValue);
            ShowDamage.instance.ShowDamageValue(attackValue, item.transform.position);
        }
        else if (item)
        {
            ShowDamage.instance.ShowDamageValue(attackValue, item.transform.position);
        }
        else
        {
            PlayerTower playerTower = HexagonManager.instance.GetHexagon(currentAttackIndex).GetTower();
            if (playerTower && Runner.IsServer)
            {
                playerTower.Damage(attackValue);
                ShowDamage.instance.ShowDamageValue(attackValue, playerTower.transform.position);
            }
            else if (playerTower)
            {
                ShowDamage.instance.ShowDamageValue(attackValue, playerTower.transform.position);
            }
            DropableCard card = HexagonManager.instance.GetHexagon(currentAttackIndex).GetCard();
            if (card && Runner.IsServer)
            {
                card.Damage();
                ShowDamage.instance.ShowDamageValue(attackValue, card.transform.position);
            }
            else if (card)
            {
                ShowDamage.instance.ShowDamageValue(attackValue, card.transform.position);
            }
        }
    }
    public void Damage(int damage)
    {
        RPC_Damage(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Damage(int damage)
    {
        life -= damage;

        RPC_StartTriggerAnimation("hit");

        if (life <= 0)
        {
            //textHolder.gameObject.SetActive(false);
            RPC_HideText(false);
            // RPC_SpawnGhost();

            Runner.Despawn(Object);
        }

        CameraShake.instance.ShakeCamera(1, 0.5f);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnGhost()
    {
        Instantiate(isLeft ? ghost_red : ghost_blue, transform.position, Quaternion.identity);
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
            Vector3 direction = pos[j] - pos[j - 1];

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
            //textHolder.gameObject.SetActive(true);
            RPC_HideText(true);
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
            Gamemanager.instance.EnableButtons();
        }).OnStart(() =>
        {
            Gamemanager.instance.DisableButtons();
            //textHolder.gameObject.SetActive(false);
            RPC_HideText(false);
            RemoveAllCards();
        });

        sequence.Play();
        canMove = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartAnimation(string anim, bool flag)
    {
        animController.SetBool(anim, flag);

        if (anim == "move" && flag)
            runAudio.Play();
        else if (anim == "move" && !flag)
            runAudio.Pause();
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
        int totalAttackValue = realAttackValue;
        for (int i = 0; i < num; i++)
        {
            if (cardColliders[i].TryGetComponent<DropableCard>(out DropableCard card) && card.isLeft == isLeft)
            {
                dropableCards.Add(card);

                card.AddItem(transform);

                if (card.dropCardType == DropCardType.POWER_BOOST)
                {
                    totalAttackValue++;
                }
                else if (card.dropCardType == DropCardType.RANGE_SURGE)
                {
                    isRangeCardUsed = true;
                }
                else if (card.dropCardType == DropCardType.STRIKE_FLOW)
                {
                    isStrikeCardUsed = true;
                    canAttackMore = true;
                }
            }
        }
        attackValue = totalAttackValue;
    }
    public void RemoveAllCards()
    {
        foreach (var item in dropableCards)
        {
            item.HideItem(transform);
            attackValue = realAttackValue;
            isRangeCardUsed = false;
            isStrikeCardUsed = false;
            canAttackMore = false;
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        HexagonManager.instance.FreeHexSpace(tileIndex);

        Instantiate(isLeft ? ghost_red : ghost_blue, transform.position, Quaternion.identity);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5.4f);
    }
    #endregion
}