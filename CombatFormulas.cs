using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFormulas
{
    //Add weapon functionality later
    
    // DAMAGE FORMULAS HP -= DMG to subtract damage from health.
    public int DMG_PHYS(CharacterManager Attacker, CharacterManager Defender)
    {
       return Mathf.Max(0, Attacker._stats.STR - Defender._stats.DEF); 
    } 

    public int DMG_MAG(CharacterManager Attacker, CharacterManager Defender)
    {
        return Mathf.Max(0, Attacker._stats.INT - Defender._stats.RES);
    }

    // HIT RATE FORMULAS
    public bool DOES_HIT(CharacterManager Attacker, CharacterManager Defender)
    {
        int RNG = Random.Range(1,100);
        if (RNG < HIT(Attacker, Defender)) return true;
        else return false;
    }

    public int HIT(CharacterManager Attacker, CharacterManager Defender)
    {
        return Mathf.Max(0, ACCURACY(Attacker) - AVOID(Defender)); 
    }

    public int ACCURACY(CharacterManager Unit)
    {
        // ACC = WPN_HIT + (DEX x 2)
        return 100;
    }

    public int AVOID(CharacterManager Unit)
    {
        // AVO = SPDx2 + Terrain bonus=0 (get terrain bonus from ActiveTile)
        return 0;
    }


}
