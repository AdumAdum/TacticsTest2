using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class BattleForecast : MonoBehaviour
{
    string executeButton = "ButtonExecute";
    Dictionary<string, TMP_Text> ForecastTexts;

    CanvasGroup BFcanvasGroup;
    CombatFormulas CF;

    void Awake()
    {
        CF = new CombatFormulas();
        BFcanvasGroup = GetComponent<CanvasGroup>();
        ForecastTexts = new Dictionary<string, TMP_Text>();
        foreach (Transform child in transform)
        {
            if (child.transform.TryGetComponent(out TMP_Text tmp))
            {
                ForecastTexts.Add(child.transform.name, tmp);
            }
        }
    }

    public void CharacterSetup(CharacterManager Attacker, CharacterManager Defender)
    {
        ShowCanvas();
        ActivateButton(executeButton);

        ForecastTexts["PlayerHP"].SetText($"HP: {Defender._stats.HP}");
        ForecastTexts["EnemyHP"].SetText($"HP: {Defender._stats.HP}");

        var playerDMG = CF.DMG_PHYS(Attacker, Defender);
        var enemyDMG = CF.DMG_PHYS(Defender, Attacker);

        ForecastTexts["PlayerDMG"].SetText($"DMG: {playerDMG}");
        ForecastTexts["EnemyDMG"].SetText($"DMG: {enemyDMG}");
        
        var playerHIT = CF.HIT(Attacker, Defender);
        var enemyHIT = CF.HIT(Defender, Attacker);

        ForecastTexts["PlayerHit"].SetText($"HIT: {playerHIT}");
        ForecastTexts["EnemyHit"].SetText($"HIT: {enemyHIT}"); 
    }

    void ShowCanvas()
    {
        BFcanvasGroup.alpha = 1;
        BFcanvasGroup.interactable = true;
        BFcanvasGroup.blocksRaycasts = true;
    }

    public void HideCanvas()
    {
        BFcanvasGroup.alpha = 0;
        BFcanvasGroup.interactable = false;
        BFcanvasGroup.blocksRaycasts = false;
    }
    // public void BFSwitchScreenSide(bool goOppositeSide=true)
    // {
    //     if (!goOppositeSide) 
    //         transform.position = startingPosition;
    //     else 
    //        transform.position = inversePosition;
    // }

    private void ActivateButton(string buttonName)
    {
        Transform childTransform = gameObject.transform.Find(buttonName);
        if (childTransform != null)
        {
            childTransform.gameObject.SetActive(true);
        }
    }
}
