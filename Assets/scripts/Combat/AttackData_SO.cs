using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Attack",menuName="Attack Data/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public int baseATK, currentATK;
    public float CD, CIR, CIR_DMG, atkRange, skillRange;
}