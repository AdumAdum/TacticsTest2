using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTool : MonoBehaviour
{
    public Animator transitionLevel;
    public Animator transitionPlayerPhase;
    public Animator transitionEnemyPhase;

    public IEnumerator TransitionLoad(string level)
    {
        transitionLevel.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(level);
    }

    
    public IEnumerator TransitionPhaseChange(string team, int delay=0)
    {
        if (team == "player")
        {
            yield return new WaitForSeconds(delay);
            transitionPlayerPhase.SetTrigger("Start");
            yield return new WaitForSeconds(1);
            transitionPlayerPhase.SetTrigger("End");
            yield return new WaitForSeconds(1);
        }
        else if (team == "enemy")
        {
            yield return new WaitForSeconds(delay);
            transitionEnemyPhase.SetTrigger("Start");
            yield return new WaitForSeconds(1);
            transitionEnemyPhase.SetTrigger("End");
            yield return new WaitForSeconds(1);
        }
    }
}
