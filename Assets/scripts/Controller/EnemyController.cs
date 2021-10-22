using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private Animator Enemy_Slime;
    public NavMeshAgent agent;
    private EnemyStates enemyStates;
    private CharacterStats characterStats;
    private CharacterStats attackTargetCharacterStats;
    private CharacterStats playerStats;
    private Collider enemyCollider;
    private PlayerUI playerUI;
    //private PlayerController playerCtl;


    [Header("Base Setting")]
    public float sights;
    protected GameObject attackTarget;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public bool isGuard;
    private bool isWalk, isChase, isFollow, isDie;


    private float speed;
    private float LookAtTime = 4f;
    private float lastAttackTime;


    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 nextPosition;

    private int count;
    private bool playerIsDead;

    protected float mass = 3.0f; // defines the character mass
    protected Vector3 impact = Vector3.zero;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        characterStats = GetComponent<CharacterStats>();
        Enemy_Slime = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider>();

        playerStats = GameObject.Find("Player").GetComponent<CharacterStats>();
        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        // playerCtl = GameObject.Find("Player").GetComponent<PlayerController>();

        speed = agent.speed;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        isWalk = false;
        isChase = false;
        isFollow = false;
        isDie = false;

        count = 0;
        playerIsDead = false;
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNextPosition();
        }
    }

    void Update()
    {
        if (characterStats.currentHP == 0)
            isDie = true;
        if (!playerIsDead)
        {
            SwitchStates();
            switchAnimation();
            lastAttackTime -= Time.deltaTime;
        }

        // apply the impact force:
        if (impact.magnitude > 0.2) attackTarget.GetComponent<CharacterController>().Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    void switchAnimation()
    {
        Enemy_Slime.SetBool("isWalk", isWalk);
        Enemy_Slime.SetBool("isChase", isChase);
        Enemy_Slime.SetBool("isFollow", isFollow);
        Enemy_Slime.SetBool("isDie", isDie);
    }

    void SwitchStates()
    {
        if (isDie)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD: Guard(); break;
            case EnemyStates.PATROL: Patrol(); break;
            case EnemyStates.CHASE: Chase(); break;
            case EnemyStates.DEAD: Dead(); break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sights);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.atkRange)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.atkRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.skillRange)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    void Chase()
    {
        agent.speed = speed;
        isWalk = false;
        isChase = true;

        if (!FoundPlayer())
        {

            isFollow = false;
            //agent.destination=initialPosition;
            isChase = false;
            if (LookAtTime > 0)
            {
                agent.destination = transform.position;
                LookAtTime -= Time.deltaTime;
            }
            else if (isGuard)
            {
                //agent.destination = initialPosition;
                enemyStates = EnemyStates.GUARD;
            }
            else
            {
                agent.destination = initialPosition;
                enemyStates = EnemyStates.PATROL;
            }
        }
        else
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
            if (TargetInAttackRange() || TargetInSkillRange())
            {
                isFollow = false;
                agent.isStopped = true;
                if (lastAttackTime < 0)
                {
                    Attack();
                    lastAttackTime = characterStats.attackData.CD;
                }
            }
        }
    }

    void Guard()
    {
        isChase = false;
        if (transform.position != initialPosition)
        {
            isWalk = true;
            agent.isStopped = false;
            agent.destination = initialPosition;
            if (Vector3.SqrMagnitude(initialPosition - transform.position) <= agent.stoppingDistance)
            {
                isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, 0.02f);
            }
        }
    }

    void Dead()
    {
        Enemy_Slime.ResetTrigger("attack");
        Enemy_Slime.ResetTrigger("skill");
        enemyCollider.enabled = false;
        agent.radius = 0;
        Destroy(gameObject, 3f);
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        //暴击
        attackTargetCharacterStats = attackTarget.GetComponent<CharacterStats>();
        attackTargetCharacterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.CIR;
        Enemy_Slime.SetBool("isCir", attackTargetCharacterStats.isCritical);
        isWalk = false;

        if (TargetInSkillRange())
        {
            Enemy_Slime.SetTrigger("skill");
        }
        else if (TargetInAttackRange())
        {
            Enemy_Slime.SetTrigger("attack");
        }
    }

    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            attackTargetCharacterStats.TakeDamage(characterStats);
            if (attackTargetCharacterStats.isCritical)
            {
                attackTarget.GetComponent<Animator>().SetTrigger("getHit");
            }
            playerUI.UpdatePlayerHPBar();
        }
    }

    void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;
        if (Vector3.Distance(nextPosition, transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            GetNextPosition();
        }
        else
        {
            isWalk = true;
            agent.destination = nextPosition;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sights);
    }

    void GetNextPosition()
    {
        NavMeshHit hit;
        Vector3 randomPosition;
        float randomX, randomZ;
        do
        {
            randomX = Random.Range(-patrolRange, patrolRange);
            randomZ = Random.Range(-patrolRange, patrolRange);
            randomPosition = new Vector3(initialPosition.x + randomX, transform.position.y, initialPosition.z + randomZ);
        } while (!NavMesh.SamplePosition(randomPosition, out hit, patrolRange, 1));
        nextPosition = hit.position;


        //nextPosition = randomPosition;


        // nextPosition = NavMesh.SamplePosition(randomPosition, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            //otherStats = other.gameObject.GetComponent<CharacterStats>();
            characterStats.isCritical = UnityEngine.Random.value < playerStats.attackData.CIR;
            characterStats.TakeDamage(playerStats);
            count += 1;
            if (count % 6 == 0)
            {
                Enemy_Slime.ResetTrigger("attack");
                Enemy_Slime.ResetTrigger("skill");
                Enemy_Slime.SetTrigger("getHit");
            }
        }
    }

    public void EndNotify()
    {
        Enemy_Slime.SetBool("isWin", true);
        playerIsDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
        agent.enabled = false;
    }

    // void OnEnable()
    // {

    // }

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;

        GameManager.Instance.RemoveObserver(this);
    }
}
