using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Base Data")]
    public int maxHP, currentHP, baseDEF, currentDEF;

    [Header("Level")]
    public int maxLevel, currentLevel, fullExp, currentExp;
    public float levelBuff;

    public float levelMultipliter
    {
        get
        {
            return 1 + (currentLevel - 1) * levelBuff;
        }
    }

    [Header("Kill Exp")]
    public int killExp;

    public void UpdateExp(int exp)
    {
        currentExp += exp;
        if (currentExp >= fullExp && currentLevel < maxLevel)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        fullExp += (int)(fullExp * levelMultipliter);
        maxHP = (int)(maxHP * levelMultipliter);
        currentHP = maxHP;
        baseDEF = (int)(baseDEF * levelMultipliter);
        currentDEF = baseDEF;

        GameObject.Find("PlayerUI").GetComponent<PlayerUI>().UpdateLevel();
    }
}
