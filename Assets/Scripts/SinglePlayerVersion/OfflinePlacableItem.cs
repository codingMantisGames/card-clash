using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using CodingMantisGames.SimpleAI;

public class OfflinePlacableItem : MonoBehaviour
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
    public bool isRangeCardUsed;
    public bool isStrikeCardUsed;
    public bool isPowerboostCardUsed;
    public bool canAttackMore;
    [Range(1, 3)] public int m_AttackRange = 1;
    [SerializeField] private LayerMask hexagonLayer;
    [SerializeField] private LayerMask playerLayer;
    public bool isBot;
    public int tileIndex;
    bool isSelected;
    bool isHighlighted;
    Vector3 targetPos;
    bool canMove;
    [SerializeField] private Animator animController;
    [SerializeField] private Transform textHolder;
    public int moveCount;
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
    public int currentAttackIndex;
    Vector3 startPoint;
    [SerializeField, Space(20)] private Collider[] cardColliders;
    [SerializeField] private LayerMask cardLayer;
    public List<OfflineDropableCards> dropableCards;
    //private ChangeDetector _changeDetector;
    public int totalLife;
    [SerializeField] private int realAttackValue;
    public int life;
    public int attackValue;
    public bool isFreezed;
    public int freezedCounter;
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
    public bool isFlaggedCharacter;
    public OfflineHexagon hexagonFlagged;
    public OfflineHexagon hexagonToAttack;
    public OfflineHexagon hexagonToMove;
    public OfflinePlacableItem enemyToAttack;
    public OfflineDropableCards cardToAttack;
    public float treatLevel;
    public bool planToAtttackTower;
    #endregion

    #region UNITY FUNCTIONS
    IEnumerator Start()
    {
        timeBtwTiletoTileMovement = 0.1f;//THis is only for testing

        targetPos = transform.position;

        yield return new WaitForEndOfFrame();
        if (isBot && textHolder)
        {
            textHolder.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        moveCount = 1;
        lines = new List<GameObject>();
        dropableCards = new List<OfflineDropableCards>();

        BotGameManager.instance.ResetRound += ResetRound;
        BotGameManager.instance.ChangeTurn += ChangeTurn;

        runAudio = GetComponent<AudioSource>();

        Spawned();
    }
    private void Awake()
    {
        transform.name = nameOfCharacter + "_" + BotGameManager.instance.index++;
    }
    void Update()
    {
        if (canMove)
        {
            transform.position = targetPos;
        }

        attackValueLabel.text = attackValue.ToString();
    }

    #endregion

    #region FUNCTIONS
    public void ChangeTurn()
    {
        isFlaggedCharacter = false;
        hexagonFlagged = null;
        enemyToAttack = null;

        if (isFreezed)
        {
            if (freezedCounter > 1)
            {
                isFreezed = false;

                //RPC_UnFreezePlayer();
                //RPC_SetMaterial(isBot);
            }

            freezedCounter++;
        }
    }
    public int GetEnemyNearByCount()
    {
        colliders = new Collider[50];

        int num = Physics.OverlapSphereNonAlloc(transform.position, range[2], colliders, hexagonLayer);
        int count = 0;
        for (int i = 0; i < num; i++)
        {
            if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
            {
                if (tile.isUsed && !tile.isUsedByEnemy) count++;
            }
        }

        return count;
    }
    public int AnyTargetInAttackRange(RoundStage stage)
    {
        colliders = new Collider[50];
        int r = 0;
        if (stage == RoundStage.MOVE_ITEM)
        {
            r = m_MovementRange - 1;
            if (isRangeCardUsed)
                r = 2;
        }
        else
            r = m_AttackRange - 1;

        int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
        int count = 0;
        for (int i = 0; i < num; i++)
        {
            if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
            {
                //There is small chance we get a issue here
                if (tile.isUsed && tile.isUsedByEnemy != isBot && tile.isTowerRegion) count++;
                else if (tile.isUsed && tile.isUsedByEnemy != isBot) count++;
            }
        }

        return count;
    }
    public bool AnythingBlocking(Transform enemy)
    {
        return !HasLineOfSight(transform.position, enemy);
    }
    public bool CanAttack(OfflineHexagon offlineHexagon, Transform enemy)
    {
        if (!HasLineOfSight(transform.position, enemy)) return false;

        colliders = new Collider[50];
        int r = 0;
        r = m_AttackRange - 1;

        int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
        bool flag = false;
        for (int i = 0; i < num; i++)
        {
            if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile) && tile == offlineHexagon)
            {
                flag = true; break;
            }
        }

        return flag;
    }
    private bool CanAttackFromPoint(OfflineHexagon offlineHexagon, Vector3 pos, Transform enemy = null)
    {
       /* if (!HasLineOfSight(pos, enemy))
        {
            Debug.Log("not in line of site!");
            return false;
        }*/

        int r = 0;
        r = m_AttackRange - 1;

        var result = Physics.OverlapSphere(pos, range[r], hexagonLayer);
        foreach (var item in result)
        {
            if (item.gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile) && tile == offlineHexagon)
                return true;
        }

        return false;
    }

    public OfflineHexagon CanMoveAndAttack(OfflineHexagon offlineHexagon, bool useCard = false, Transform enemy = null)
    {
        colliders = new Collider[50];
        int r = 0;
        r = m_MovementRange - 1;

        if (useCard) r = 2;

        int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
        for (int i = 0; i < num; i++)
        {
            if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile) && !tile.isUsed && !tile.isMarkedByAI)//this is addded to fix bug not locations 
            {
                if (CanAttackFromPoint(offlineHexagon, tile.buildPoint.position, enemy))
                    return tile;
            }
        }

        return null;
    }

    public bool CanAttackAIsTower(RoundStage stage)
    {
        colliders = new Collider[50];
        int r = 0;
        if (stage == RoundStage.MOVE_ITEM)
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
            if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
            {
                if (tile.isTowerRegion) return true;
            }
        }

        return false;
    }
    public void ResetRound()
    {
        ResetMoveCounter();

        if (isStrikeCardUsed)
            canAttackMore = true;

        outline.enabled = false;
        isHighlighted = false;
        BotGameManager.instance.HideCharacterDetails();
    }
    public void ResetMoveCounter()
    {
        moveCount = 10; //Changed here for testing
    }
    public void SetBuilding(bool flag = false)
    {
        isBot = flag;

        SetMaterial(isBot);

        Invoke("CheckForPowerCards", 0.1f);

        BotGameManager.instance.CheckPlayerPosition += CheckForCards;

        life = totalLife;
        attackValue = realAttackValue;
    }
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void SetMaterial(bool isBot)
    {
        foreach (var item in skinnedMeshRenderers)
        {
            if (isBot)
                item.material = blueMat;
            else
                item.material = redMat;
        }
        foreach (var item in meshRenderers)
        {
            if (isBot)
                item.material = blueMat;
            else
                item.material = redMat;
        }
    }
    public void Freeze()
    {
        isFreezed = true;
        freezedCounter = 0;

        FreezePlayer();
    }
    // [Rpc(RpcSources.All, RpcTargets.All)]
    public void FreezePlayer()
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
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UnFreezePlayer()
    {
        if (animController)
            animController.enabled = true;
    }
    public void Heal()
    {
        RPC_ChangeLife(totalLife);
    }
    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeLife(int val)
    {
        Debug.Log("Healed ---> " + transform.name);
        life = val;
    }
    public void Spawned()
    {
        //_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        lifeLabel.text = life.ToString();
        attackValueLabel.text = attackValue.ToString();
    }

    private void OnEnable()
    {
        BotGameManager.instance.OnItemSelected += HideOutline;
    }
    public void SetInitialRotation()
    {
        SetRotation(Quaternion.Euler(0, 270, 0));
    }
    private void OnDestroy()
    {
        BotGameManager.instance.OnItemSelected -= HideOutline;

        BotGameManager.instance.ResetRound -= ResetRound;
        BotGameManager.instance.ChangeTurn -= ChangeTurn;

        if (BotGameManager.instance.brain.allyCharacters.Contains(this)) BotGameManager.instance.brain.allyCharacters.Remove(this);
        if (BotGameManager.instance.brain.enemyCharacters.Contains(this)) BotGameManager.instance.brain.enemyCharacters.Remove(this);

        try
        {
            BotGameManager.instance.CheckPlayerPosition -= CheckForCards;
        }
        catch
        {
            Debug.LogWarning("Not Added!");
        }

        OfflineHexagonManager.instance.FreeHexSpace(tileIndex);
        if (AIBrain.instance.allyCharacters.Contains(this))
            AIBrain.instance.allyCharacters.Remove(this);

        if (AIBrain.instance.enemyCharacters.Contains(this))
            AIBrain.instance.enemyCharacters.Remove(this);

        RemoveAllCards();
    }
    public void OnMouseDownFun()
    {
        if (BotGameManager.instance.isBotsTurn/* || !BotGameManager.instance.canInteract*/)
            return;


        if ((BotGameManager.instance.isBotsTurn == isBot && moveCount == 0))
        {
            BotGameManager.instance.ShowNoMovesPending(transform.position, "No Moves Remaining");
            return;
        }

        if (BotGameManager.instance.isBotsTurn == isBot && isFreezed)
        {
            BotGameManager.instance.ShowNoMovesPending(transform.position, "Character Frozen");
            return;
        }
        /*if (BotGameManager.instance.isLeft != isLeft && BotGameManager.instance.currentRoundStage == RoundStage.ATTACK)
        {
            HexagonManager.instance.CallOnMouseDown(tileIndex);
        }*/


        if (BotGameManager.instance.isBotsTurn == isBot && BotGameManager.instance.currentRoundStage != RoundStage.USING_CARDS && !isHighlighted)
        {
            BotGameManager.instance.OnItemSelected?.Invoke();

            outline.enabled = true;
            //HexagonManager.instance.ShowMovableTiles(tileIndex, movementType);
            //this is place i want to change logic 
            if (OfflineHexagonManager.instance.isHexMoveOn)
                OfflineHexagonManager.instance.HideAllHex();

            if (BotGameManager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
                CursorChanger.instance.SetMoveCursor();
            else if (BotGameManager.instance.currentRoundStage == RoundStage.ATTACK)
                CursorChanger.instance.SetAttackCursor();

            colliders = new Collider[50];
            int r = 0;
            if (BotGameManager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
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
                if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
                {
                    if (BotGameManager.instance.currentRoundStage == RoundStage.MOVE_ITEM)
                        OfflineHexagonManager.instance.SelectHexagon(tile);
                    else
                        OfflineHexagonManager.instance.SelectHexagonAll(tile);

                    if (tile.isUsed && BotGameManager.instance.currentRoundStage == RoundStage.ATTACK)
                    {
                        playerColliders = new Collider[1];
                        int k = Physics.OverlapSphereNonAlloc(tile.buildPoint.position, 0.5f, playerColliders, playerLayer);


                        if (k != 0)
                        {
                            if (playerColliders[0].gameObject.TryGetComponent<OfflinePlacableItem>(out OfflinePlacableItem item))
                            {
                                if (BotGameManager.instance.isBotsTurn != item.isBot)
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
                            else if (playerColliders[0].gameObject.TryGetComponent<OfflinePlayerTower>(out OfflinePlayerTower tower))
                            {
                                if (BotGameManager.instance.isBotsTurn != tower.isBot)
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
                            else if (playerColliders[0].gameObject.TryGetComponent<OfflineDropableCards>(out OfflineDropableCards card))
                            {
                                if (BotGameManager.instance.isBotsTurn != card.isBot)
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
                OfflineHexagonManager.instance.isHexMoveOn = true;

            isHighlighted = true;

            BotGameManager.instance.currentItemToMove = this;
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
            BotGameManager.instance.ShowCharacterDetails(nameOfCharacter, attackRange.ToString(), m_AttackRange.ToString(), life.ToString(), attackValue.ToString(), chance);
            #endregion
        }
        else if (isHighlighted)
        {
            OfflineHexagonManager.instance.HideAllHex();

            isHighlighted = false;
            outline.enabled = false;

            CursorChanger.instance.SetNormalCursor();

            BotGameManager.instance.currentItemToMove = null;

            foreach (var item in lines)
            {
                Destroy(item);
            }
            lines = new List<GameObject>();

            BotGameManager.instance.HideCharacterDetails();
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
        BotGameManager.instance.HideCharacterDetails();
        BotGameManager.instance.currentItemToMove = null;

        foreach (var item in lines)
        {
            Destroy(item);
        }
        lines = new List<GameObject>();
    }
    public void Attack(OfflineHexagon tile)
    {
        Attack(tile.index);
    }
    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Attack(int hexIndex)
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
            OfflineHexagon tile = OfflineHexagonManager.instance.GetHexagon(hexIndex);
            startPoint = transform.position;
            Sequence sequence = DOTween.Sequence();
            Vector3 pos = GetStoppingPoint(tile.buildPoint.position);
            sequence.Append(DOTween.To(() => targetPos, x => targetPos = x, pos, timeBtwTiletoTileMovement).SetEase(Ease.Linear).OnComplete(() =>
            {
                StartAnimation("move", false);
                RPC_StartTriggerAnimation("attack");
            }).OnStart(() =>
            {
                canMove = true;
                StartAnimation("move", true);
                //textHolder.gameObject.SetActive(false);
                HideText(false);
                Vector3 direction = pos - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                SetRotation(targetRot);
            }));

            sequence.Append(DOTween.To(() => targetPos, x => targetPos = x, startPoint, timeBtwTiletoTileMovement).SetEase(Ease.Linear).SetDelay(goBackDelay).OnComplete(() =>
            {
                StartAnimation("move", false);
                SetRotation(Quaternion.Euler(0, isBot ? 270 : 90, 0));
                canMove = false;

                OfflineHexagonManager.activeHexagon = null;
                //textHolder.gameObject.SetActive(true);
                HideText(true);

                BotGameManager.instance.EnableButtons();

                if (BotGameManager.instance.isBotsTurn) BotGameManager.instance.brain.ContinueAttack();

            }).OnStart(() =>
            {
                BotGameManager.instance.DisableButtons();

                StartAnimation("move", true);

                Vector3 direction = startPoint - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                SetRotation(targetRot);
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

            OfflineHexagon tile = OfflineHexagonManager.instance.GetHexagon(currentAttackIndex);

            Vector3 direction = tile.buildPoint.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            SetRotation(targetRot);

            Invoke("AttackAnimation", 0.5f);
        }

    }
    // [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_HideOutline()
    {
        OfflineHexagonManager.instance.HideAllHex();
        HideOutline();
    }
    // [Rpc(RpcSources.All, RpcTargets.All)]
    public void HideText(bool isBot)
    {
        textHolder.gameObject.SetActive(isBot);
    }
    void AttackAnimation()
    {
        RPC_StartTriggerAnimation("attack");
    }
    public void OnAttack()
    {
        GameObject gm = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        OfflineHexagon tile = OfflineHexagonManager.instance.GetHexagon(currentAttackIndex);
        Vector3 pos = tile.buildPoint.position;

        pos.y = gm.transform.position.y;

        if (isBot)
            gm.transform.GetChild(2).gameObject.SetActive(true);
        else
            gm.transform.GetChild(1).gameObject.SetActive(true);

        float dis = Vector3.Distance(pos, gm.transform.position);
        float time = dis / projectileSpeed;

        gm.transform.DOMove(pos, time).SetEase(projecileMovementEase).OnComplete(() =>
        {
            Destroy(gm);

            SetRotation(Quaternion.Euler(0, isBot ? 270 : 90, 0));

            if (projectileHitAudio)
                projectileHitAudio.Play();

            DealDamageToEnemy();
            if (BotGameManager.instance.isBotsTurn) BotGameManager.instance.brain.ContinueAttack();
        });
    }
    public void DealDamageToEnemy()
    {
        OfflinePlacableItem item = OfflineHexagonManager.instance.GetHexagon(currentAttackIndex).GetPlayer();
        if (item)
        {
            item.Damage(attackValue);
            ShowDamage.instance.ShowDamageValue(attackValue, item.transform.position);
        }/*
        else if (item)
        {
            ShowDamage.instance.ShowDamageValue(attackValue, item.transform.position);
        }*/
        else
        {
            OfflinePlayerTower playerTower = OfflineHexagonManager.instance.GetHexagon(currentAttackIndex).GetTower();
            if (playerTower)
            {
                playerTower.Damage(attackValue);
                ShowDamage.instance.ShowDamageValue(attackValue, playerTower.transform.position);
            }
            else if (playerTower)
            {
                ShowDamage.instance.ShowDamageValue(attackValue, playerTower.transform.position);
            }
            OfflineDropableCards card = OfflineHexagonManager.instance.GetHexagon(currentAttackIndex).GetCard();
            if (card)
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
        RPC_ShakeCamera();
        RPC_Damage(damage);
        lifeLabel.text = life.ToString();
    }
    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Damage(int damage)
    {
        life -= damage;

        RPC_StartTriggerAnimation("hit");

        if (life <= 0)
        {
            //textHolder.gameObject.SetActive(false);
            HideText(false);
            RPC_SpawnGhost();

            // Runner.Despawn(Object);
            Destroy(gameObject);
        }
    }
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ShakeCamera()
    {
        CameraShake.instance.ShakeCamera(1, 0.5f);
    }
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnGhost()
    {
        Instantiate(!isBot ? ghost_red : ghost_blue, transform.position, Quaternion.identity);
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

    public void MoveToPosition(Vector3[] pos, int index, bool isBot, int cIndex)
    {
        MoveTo(pos, index, isBot, cIndex);
    }
    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void MoveTo(Vector3[] pos, int i, bool isBot, int cIndex)
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
               SetRotation(targetRot);
           }));
        }
        StartAnimation("move", true);
        sequence.OnComplete(() =>
        {
            //textHolder.gameObject.SetActive(true);
            HideText(true);
            canMove = false;
            StartAnimation("move", false);
            SetRotation(Quaternion.Euler(0, isBot ? 270 : 90, 0));
            tileIndex = i;

            OfflineHexagon tile = OfflineHexagonManager.instance.GetHexagon(i);
            tile.isUsed = true;
            tile.isUsedByEnemy = !isBot;
            tile = OfflineHexagonManager.instance.GetHexagon(cIndex);
            tile.isUsed = false;

            //CheckForPowerCards();
            BotGameManager.instance.CheckPlayerPosition?.Invoke();

            OfflineHexagonManager.activeHexagon = null;
            BotGameManager.instance.EnableButtons();


            if (BotGameManager.instance.isBotsTurn) BotGameManager.instance.brain.ContinueMovement();
        }).OnStart(() =>
        {
            BotGameManager.instance.DisableButtons();
            //textHolder.gameObject.SetActive(false);
            HideText(false);
            RemoveAllCards();
        });

        sequence.Play();
        canMove = true;
    }

    // [Rpc(RpcSources.All, RpcTargets.All)]
    public void StartAnimation(string anim, bool isBot)
    {
        animController.SetBool(anim, isBot);

        if (anim == "move" && isBot)
            runAudio.Play();
        else if (anim == "move" && !isBot)
            runAudio.Pause();
    }
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartTriggerAnimation(string anim)
    {
        animController.SetTrigger(anim);
    }
    // [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetRotation(Quaternion rot)
    {
        transform.GetChild(0).localRotation = rot;
    }
    /*public override void Render()
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
    }*/

    public void CheckForCards()
    {
        Invoke("CheckForPowerCards", 0.1f);
    }
    public void CheckForPowerCards()
    {
        isPowerboostCardUsed = false;
        isRangeCardUsed = false;
        isStrikeCardUsed = false;
        canAttackMore = false;

        cardColliders = new Collider[6];
        dropableCards = new List<OfflineDropableCards>();
        int num = Physics.OverlapSphereNonAlloc(transform.position, 2f, cardColliders, cardLayer);
        int totalAttackValue = realAttackValue;
        for (int i = 0; i < num; i++)
        {
            if (cardColliders[i].TryGetComponent<OfflineDropableCards>(out OfflineDropableCards card) && card.isBot == isBot)
            {
                dropableCards.Add(card);

                card.AddItem(transform);

                if (card.dropCardType == DropCardType.POWER_BOOST)
                {
                    totalAttackValue++;
                    isPowerboostCardUsed = true;
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
            try
            {
                item.HideItem(transform);
            }
            catch { }
            attackValue = realAttackValue;
            isRangeCardUsed = false;
            isPowerboostCardUsed = false;
            isStrikeCardUsed = false;
            canAttackMore = false;
        }
    }
    public OfflineHexagon[] GetAllTilesInRange(RoundStage stage)
    {
        List<OfflineHexagon> result = new List<OfflineHexagon>();
        colliders = new Collider[50];
        int r = 0;
        if (stage == RoundStage.MOVE_ITEM)
        {
            r = m_MovementRange - 1;
            if (isRangeCardUsed)
                r = 2;
        }
        else
            r = m_AttackRange - 1;


        if (stage == RoundStage.MOVE_ITEM)
        {
            int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
            for (int i = 0; i < num; i++)
            {
                if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
                {
                    if (!tile.isUsed && !tile.isMarkedByAI) result.Add(tile);
                }
            }
        }
        else
        {
            int num = Physics.OverlapSphereNonAlloc(transform.position, range[r], colliders, hexagonLayer);
            for (int i = 0; i < num; i++)
            {
                if (colliders[i].gameObject.TryGetComponent<OfflineHexagon>(out OfflineHexagon tile))
                {
                    result.Add(tile);
                }
            }
        }


        return result.ToArray();
    }
    /*public override void Despawned(NetworkRunner runner, bool hasState)
    {
        HexagonManager.instance.FreeHexSpace(tileIndex);

        RemoveAllCards();

        Instantiate(isLeft ? ghost_red : ghost_blue, transform.position, Quaternion.identity);
    }*/
    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(transform.position, 5.4f);

        if (isFlaggedCharacter)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + new Vector3(0, 1.65f, 0), 0.15f);

            if (hexagonFlagged)
            {
                Gizmos.DrawLine(transform.position, hexagonFlagged.buildPoint.position);
            }
        }
        if (hexagonToMove)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, hexagonToMove.buildPoint.position);
            Gizmos.DrawSphere(hexagonToMove.buildPoint.position, 0.1f);
            Gizmos.DrawRay(hexagonToMove.buildPoint.position, Vector3.up * 2);
        }

        if (enemyToAttack)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, enemyToAttack.transform.position);
            Gizmos.DrawSphere(enemyToAttack.transform.position, 0.1f);
            Gizmos.DrawRay(enemyToAttack.transform.position, Vector3.up * 2);
        }
    }
    #endregion
}