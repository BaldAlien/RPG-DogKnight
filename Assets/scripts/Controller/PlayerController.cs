using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator animator;

    private CharacterController controller;

    private GameObject attackTarget;



    // public Rigidbody rg;
    private float lastAttackTime;

    //public float gravity;//重力
    //private Vector3 velocity;//加速度
    //public float JumpForce;
    public Transform FollowedCamera;

    private Vector3 dir;

    [HideInInspector]
    public bool walkable;
    [HideInInspector]
    public float atkTime;
    private bool isDead;
    public float jumpForce;


    //[HideInInspector]
    //public int count = 0;
    [HideInInspector]
    public CharacterStats characterStats;
    protected float mass = 3.0f; // defines the character mass
    protected Vector3 impact = Vector3.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        characterStats = GetComponent<CharacterStats>();
        //rg = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // MouseManager.Instance.OnMouseClicked += MoveToTarget;
        // MouseManager.Instance.OnEnemyClicked += EventAttack;
        //walkable=true;
        atkTime = 1f;
        //walkable = true;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    void OnEnable()
    {
        Camera.main.GetComponent<CameraManager>().playerTransform = transform;
        FollowedCamera = Camera.main.transform;
    }

    void Update()
    {
        isDead = (characterStats.currentHP <= 0);

        if (isDead)
        {
            animator.SetBool("isDead", isDead);
            GameManager.Instance.NotifyObservers();
        }

        lastAttackTime -= Time.deltaTime;
        atkTime -= Time.deltaTime;
        //MoveTo();
        if (!isDead)
        {
            SwitchAnimation();
            Move();
        }
        if (impact.magnitude > 0.2) transform.GetComponent<CharacterController>().Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }


    private void SwitchAnimation()
    {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }

    /*
        public void MoveToTarget(Vector3 target)
        {
            StopAllCoroutines();
            agent.isStopped = false;
            agent.destination = target;
        }

        private void EventAttack(GameObject target)
        {
            if (target != null)
            {
                attackTarget = target;
                StartCoroutine(MoveToAttackTarget());
            }
        }

        IEnumerator MoveToAttackTarget()
        {
            agent.isStopped = false;

            transform.LookAt(attackTarget.transform);

            while (Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.atkRange)
            {
                agent.destination = attackTarget.transform.position;
                yield return null;
            }

            agent.isStopped = true;

            if (lastAttackTime < 0)
            {
                animator.SetTrigger("Attack");

                //重置攻击间隔
                lastAttackTime = 0.5f;
            }
        }

    */
    private float moveSpeed; //人物移动速度

    private Vector3 Dir = Vector3.zero; //移动方向

    // private AnimatorStateInfo animatorInfo;
    private float keyHorizontal, keyVertical;

    AnimatorStateInfo STATE;



    private void Move()
    {
        //animatorInfo=animator.GetCurrentAnimatorStateInfo(0);
        Dir = Vector3.zero;
        //count = 0;
        if (controller.isGrounded)
        {
            keyHorizontal = Input.GetAxis("Horizontal");
            keyVertical = Input.GetAxis("Vertical");
            if (atkTime <= 0)
            {
                atkTime = 0f;
                walkable = true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                //agent.isStopped = true;
                walkable = false;
                //atkTime=1f;
                STATE = animator.GetCurrentAnimatorStateInfo(0);

                //Debug.Log("ATK");
                animator.SetTrigger("Attack");
                if (STATE.IsName("Locomotion"))
                {
                    animator.SetTrigger("Attack");
                    atkTime = 1f;
                    //count = 1;
                }
                else if (STATE.IsName("Attack01"))
                {
                    animator.ResetTrigger("Attack");
                    animator.SetTrigger("Attack02");
                    atkTime = 1f;
                    //count = 2;
                    //重置攻击间隔
                    lastAttackTime = 0.7f;
                }
            }
            else if ((keyHorizontal != 0 || keyVertical != 0) && walkable)
            {
                if (keyVertical > 0)
                {
                    Dir =
                        new Vector3(FollowedCamera.transform.forward.x,
                            transform.position.y,
                            FollowedCamera.transform.forward.z);
                    Quaternion newRotation = Quaternion.LookRotation(Dir);
                    transform.rotation =
                        Quaternion
                            .RotateTowards(transform.rotation,
                            newRotation,
                            100);
                }
                if (keyVertical < 0)
                {
                    Dir =
                        new Vector3(-FollowedCamera.transform.forward.x,
                            transform.position.y,
                            -FollowedCamera.transform.forward.z);
                    Quaternion newRotation = Quaternion.LookRotation(Dir);
                    transform.rotation =
                        Quaternion
                            .RotateTowards(transform.rotation,
                            newRotation,
                            100);
                }
                if (keyHorizontal > 0)
                {
                    Dir =
                        new Vector3(FollowedCamera.transform.right.x,
                            transform.position.y,
                            FollowedCamera.transform.right.z);
                    Quaternion newRotation = Quaternion.LookRotation(Dir);
                    transform.rotation =
                        Quaternion
                            .RotateTowards(transform.rotation,
                            newRotation,
                            100);
                }
                if (keyHorizontal < 0)
                {
                    Dir =
                        new Vector3(-FollowedCamera.transform.right.x,
                            transform.position.y,
                            -FollowedCamera.transform.right.z);
                    Quaternion newRotation = Quaternion.LookRotation(Dir);
                    transform.rotation =
                        Quaternion
                            .RotateTowards(transform.rotation,
                            newRotation,
                            100);
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetFloat("Speed", 4f);
                    moveSpeed = 4f;
                }
                else
                {
                    animator.SetFloat("Speed", 1f);
                    moveSpeed = 1.7f;
                }
            }
            if (Input.GetButtonDown("Jump") && controller.isGrounded)
            {
                animator.SetTrigger("Jump");
            }
        }

        controller.SimpleMove(Dir * moveSpeed);
    }

    void Jump()
    {
        Vector3 dir = Vector3.up;
        dir.Normalize();
        AddImpact(dir, jumpForce);
    }

    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }
}
