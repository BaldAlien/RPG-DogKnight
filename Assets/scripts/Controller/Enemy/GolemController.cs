using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : EnemyController
{
    [Header("Skill")]
    public float kickForce;
    public GameObject rockPrefab;
    public Transform handPosition;

    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            attackTarget.GetComponent<Animator>().ResetTrigger("Attack");
            attackTarget.GetComponent<Animator>().ResetTrigger("Attack02");
            attackTarget.GetComponent<Animator>().ResetTrigger("getHit");
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            Vector3 kickDir = attackTarget.transform.position - transform.position;
            kickDir.Normalize();
            AddImpact(kickDir, kickForce);
        }
    }

    void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPosition.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
            rock.GetComponent<Rock>().FlyToTatget();
        }
    }


    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }
}
