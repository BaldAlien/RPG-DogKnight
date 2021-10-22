using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GruntController : EnemyController
{
    [Header("Skill")]
    public float kickForce;

    public void KickOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            Vector3 kickDir = attackTarget.transform.position - transform.position;
            kickDir.Normalize();
            AddImpact(kickDir,kickForce);
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }


     void AddImpact(Vector3 dir, float force)
     {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
            impact += dir.normalized * force / mass;
    }

}
