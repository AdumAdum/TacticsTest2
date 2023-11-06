using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{   
    [Header("Base Stats")]
    public int MOV;
    public int HPMAX;
    public int STR;
    public int INT;
    public int DEX;
    public int SPD;
    public int DEF;
    public int RES;

    [Header("Mutable Values")]
    public int HP;
    
    [Header("Item Properties")]
    public int[] RANGE;
}
