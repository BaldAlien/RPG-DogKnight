using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHPBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;


    [HideInInspector]
    public bool isCritical;
    private float damage;

    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    #region  Read from Character Data_SO
    public int maxHP
    {
        get
        {
            if (characterData != null)
            {
                return characterData.maxHP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.maxHP = value;
        }
    }

    public int currentHP
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentHP;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.currentHP = value;
        }
    }

    public int baseDEF
    {
        get
        {
            if (characterData != null)
            {
                return characterData.baseDEF;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.baseDEF = value;
        }
    }

    public int currentDEF
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentDEF;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.currentDEF = value;
        }
    }
    #endregion


    #region Character Combat
    public void TakeDamage(CharacterStats attacker)
    {
        damage = Mathf.Max(attacker.attackData.currentATK - currentDEF, 1);

        //isCritical = UnityEngine.Random.value < attacker.attackData.CIR;

        if (isCritical)
        {
            damage *= attacker.attackData.CIR_DMG;
        }
        currentHP = Mathf.Max(currentHP - (int)damage, 0);
        //更新 HPBar UI
        UpdateHPBarOnAttack?.Invoke(currentHP, maxHP);
        //TODO:更新经验值
        if (currentHP <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killExp);
        }
    }

    public void TakeDamage(int damage)
    {
        int practicalDamage = Mathf.Max(damage, 1);
        currentHP = Mathf.Max(currentHP - practicalDamage, 0);
        UpdateHPBarOnAttack?.Invoke(currentHP, maxHP);
        GameObject.Find("PlayerUI").GetComponent<PlayerUI>().UpdatePlayerHPBar();
        if (currentHP <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killExp);
            
        }
    }
    #endregion
}