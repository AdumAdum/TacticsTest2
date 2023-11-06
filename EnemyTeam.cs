using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeam : MonoBehaviour
{
    private static EnemyTeam _instance;
    public static EnemyTeam Instance { get { return _instance; } }

    [SerializeField] List<CharacterManager> enemyUnits;
    EnemyBehavior currentEnemy;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        _instance = this;
    }

    public void EnemyTeamBegin()
    {
        enemyUnits = TurnManager.Instance.enemyUnits;
        StartCoroutine(BeginEnemyLoop());
    }

    public IEnumerator BeginEnemyLoop()
    {
        for (int i = enemyUnits.Count - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(3);
            currentEnemy = enemyUnits[i].GetComponent<EnemyBehavior>();
            currentEnemy.BeginAction();
        }
    }

}
