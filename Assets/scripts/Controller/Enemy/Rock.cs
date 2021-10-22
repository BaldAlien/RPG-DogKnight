using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public enum RockStates
    {
        hitPlayer, hitEnemy, hitNothing
    }
    private RockStates rockStates;
    public int damage;
    public GameObject breakEffect;

    public float kickForce;
    private Rigidbody rb;

    [Header("Besic Setting")]
    public float force;
    [HideInInspector]
    public GameObject target;

    private Vector3 direction;

    protected float mass = 3.0f; // defines the character mass
    protected Vector3 impact = Vector3.zero;

    void Start()
    {
        rockStates = RockStates.hitPlayer;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        FlyToTatget();
    }

    void FixedUpdate()
    {
        //Debug.Log(rb.velocity.sqrMagnitude);
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.hitNothing;
        }
    }

    void Update()
    {
        if (impact.magnitude > 0.2) target.GetComponent<CharacterController>().Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        Destroy(GameObject.Find("RockParticle(Clone)"), 1f);
    }

    public void FlyToTatget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.hitPlayer: HitPlayer(other); break;
            case RockStates.hitEnemy: HitEnemy(other); break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            rb.velocity = Vector3.one;
            rockStates = RockStates.hitEnemy;
            rb.AddForce(FindObjectOfType<PlayerController>().gameObject.transform.forward * 90, ForceMode.Impulse);
        }
    }

    void HitPlayer(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Animator>().ResetTrigger("Attack");
            other.gameObject.GetComponent<Animator>().ResetTrigger("Attack02");
            other.gameObject.GetComponent<Animator>().ResetTrigger("getHit");
            other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
            Vector3 kickDir = other.gameObject.transform.position - transform.position;
            kickDir.y = 0;
            kickDir.Normalize();
            AddImpact(kickDir, kickForce);

            other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage);
            rockStates = RockStates.hitNothing;
        }
    }

    void HitEnemy(Collision other)
    {
        if (other.gameObject.GetComponent<GolemController>())
        {
            var otherStats = other.gameObject.GetComponent<CharacterStats>();
            otherStats.TakeDamage(damage);
            Instantiate(breakEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }
}
