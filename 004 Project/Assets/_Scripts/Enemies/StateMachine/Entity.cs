using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Movement Movement { get => movement ?? Core.GetCoreComponent(ref movement); }

    private Movement movement;
    public Core Core { get; private set; }
    public Animator Anim { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }
    public D_Entity entityData;
    public StunState stunState;
    private EnemyStats stats;
    public bool IsKnockbackable { get; set; } = true;
    public Transform playerTransform;

    private int currentParryStunStack;
    protected int maxParryStunStack;
    protected float parryStunTimer;
    public MonsterStateMachine stateMachine { get; protected set; }

    public int lastDamageDirection { get; private set; }

    Transform effectParticles;
    Transform elementParticles;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform groundCheck;


  
    protected bool isDead;

    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();

        Anim = GetComponent<Animator>();
        atsm = GetComponent<AnimationToStateMachine>();
        stats = GetComponentInChildren<EnemyStats>();

        stateMachine = new MonsterStateMachine();

        currentParryStunStack = 0;

        effectParticles = transform.Find("Particles");
        elementParticles = transform.Find("Core/Element");
    }

    protected virtual void Update()
    {
        Core.LogicUpdate();
        stateMachine.currentState.LogicUpdate();


    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
    // �÷��̾� ���� ���� �޼���
    public int GetPlayerRelativePosition()
    {
        if (CheckPlayer() != null)
        {
            playerTransform = CheckPlayer();
            float direction = playerTransform.position.x - transform.position.x;

            // �����ʿ� ������ 1, ���ʿ� ������ -1
            return direction > 0 ? 1 : -1;
        }

        return 0; // �÷��̾ �������� ���� ��� 0 ��ȯ
    }

    public Transform CheckPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, 15f, LayerMasks.Player);
        return player != null ? player.transform : null;
    }
    /// <summary>
    /// �÷��̾ �ִ� Ž�� ���� ���� �ִ��� Ȯ���մϴ�.
    /// </summary>
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return CheckPlayerInBox(entityData.maxAgroDistance, entityData.agroHeight, entityData.maxForwardBias, Color.green);
    }

    /// <summary>
    /// �÷��̾ �ּ� Ž�� ���� ���� �ִ��� Ȯ���մϴ�.
    /// </summary>
    public virtual bool CheckPlayerInMinAgroRange()
    {
        return CheckPlayerInBox(entityData.minAgroDistance, entityData.agroHeight, entityData.minForwardBias, Color.yellow);
    }

    /// <summary>
    /// �÷��̾ ���� ���� ���� ���� �ִ��� Ȯ���մϴ�.
    /// </summary>
    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return CheckPlayerInBox(entityData.closeRangeActionDistance, entityData.agroHeight, entityData.closeForwardBias, Color.red);
    }
    public virtual bool CheckPlayerInMeleeAttackRangeAction()
    {
        return CheckPlayerInBox(entityData.closeRangeActionDistance - 0.3f, entityData.agroHeight, entityData.closeForwardBias, Color.red);
    }

    /// <summary>
    /// �簢�� ���� ������ �÷��̾ Ž���մϴ�.
    /// </summary>
    private bool CheckPlayerInBox(float range, float height, float forwardBias, Color debugColor)
    {
        // �簢���� �߽� ��ġ�� ������ �������� �̵�
        Vector2 boxCenter = playerCheck.position + transform.right * (range / 2 + forwardBias);

        // �簢���� ũ�� (����: Ž�� �Ÿ�, ����: ���� ����)
        Vector2 boxSize = new Vector2(range, height);

        // ����
        Collider2D player = Physics2D.OverlapBox(boxCenter, boxSize, 0f, LayerMasks.Player);

        // �簢���� �� �𼭸� ���
        Vector2 topLeft = boxCenter + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = boxCenter + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = boxCenter + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = boxCenter + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        // �簢���� �����ϴ� �� ���� ���� �׸�
        Debug.DrawLine(topLeft, topRight, debugColor, 0.1f); // ���
        Debug.DrawLine(topRight, bottomRight, debugColor, 0.1f); // ����
        Debug.DrawLine(bottomRight, bottomLeft, debugColor, 0.1f); // �ϴ�
        Debug.DrawLine(bottomLeft, topLeft, debugColor, 0.1f); // ����

        return player != null;
    }


    /// <summary>
    /// ����׿����� Ž�� ������ �ð�ȭ�մϴ�.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (playerCheck == null) return;

        // �ִ� Ž�� ����
        Gizmos.color = Color.green;
        DrawGizmoBox(entityData.maxAgroDistance, entityData.agroHeight, entityData.maxForwardBias);

        // �ּ� Ž�� ����
        Gizmos.color = Color.yellow;
        DrawGizmoBox(entityData.minAgroDistance, entityData.agroHeight, entityData.minForwardBias);

        // ���� Ž�� ����
        Gizmos.color = Color.red;
        DrawGizmoBox(entityData.closeRangeActionDistance, entityData.agroHeight, entityData.closeForwardBias);
    }

    /// <summary>
    /// �簢�� ������ �׸��ϴ�.
    /// </summary>
    private void DrawGizmoBox(float range, float height, float forwardBias)
    {
        Vector2 boxCenter = playerCheck.position + transform.right * (range / 2 + forwardBias);
        Vector2 boxSize = new Vector2(range, height);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
    public void AddcurrentParryStunStack(float stunTime)
    {
        ++currentParryStunStack;
        parryStunTimer = Time.time;
        StopCoroutine("CheckParryStunTimer");
        StartCoroutine("CheckParryStunTimer", stunTime);
    }

    private IEnumerator CheckParryStunTimer(float stunTime)
    {
        Debug.Log($"currentParryStunStack : {currentParryStunStack}, maxParryStunStack : {maxParryStunStack}");
        while (currentParryStunStack > 0)
        {
            if (currentParryStunStack >= maxParryStunStack)
            {
                stunState.SetParryStunTime(stunTime);
                stunState.stun = true;
                currentParryStunStack = 0;
            }
            if (parryStunTimer + entityData.parryStundurationTime <= Time.time)
                currentParryStunStack = 0;
            yield return 1f;
        }
        Debug.Log("���� ���� �ʱ�ȭ");
    }

    protected void OnEnable()
    {
        if (effectParticles != null)
        {
            RemoveAllChildObjects(effectParticles);
        }
        if (elementParticles != null)
        {
            RemoveAllChildObjects(elementParticles);
        }
    }
    protected virtual void OnDisable()
    {
        Movement?.SetVelocityZero();
        IsKnockbackable = true;
        stats.ChangeElement(Element.None);
    }
    protected void RemoveAllChildObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Debug.Log("child : " + child.name);
            Destroy(child.gameObject);
        }
    }
}
