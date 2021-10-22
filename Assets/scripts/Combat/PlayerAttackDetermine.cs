using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDetermine : PlayerController
{
    private int damage;
    public GameObject Weapon;
    private BoxCollider WeaponCollider;
    CharacterStats otherStats;

    void Start()
    {
        WeaponCollider = Weapon.GetComponent<BoxCollider>();
    }

    void AttackDetermine()
    {
        WeaponCollider.enabled = true;
    }

    void DisableHitBox()
    {
        WeaponCollider.enabled = false;
    }

    
}
