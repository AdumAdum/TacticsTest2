using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager Instance { get { return _instance; } }

    public List<CharacterManager> playerUnits;
    public List<CharacterManager> enemyUnits;

    [SerializeField] GameObject Transitiontool;
    TransitionTool TTool;

    private int playerCount;
    public int playerActions;

    private int enemyCount;
    public int enemyActions;

    public int turnCount = 0;

    enum TurnState
    {
        player,
        enemy,
        ally
    }
    TurnState turnState;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        _instance = this;
    }

    void Start()
    {
        TTool = Transitiontool.GetComponent<TransitionTool>();
        BeginPlayerPhase();
    }

    void BeginPlayerPhase()
    {
        turnCount++;
        MouseController.Instance.cursorLocked = false;
        Debug.Log($"Player Phase! Turn: {turnCount}");
        turnState = TurnState.player;
        
        enemyUnits.Clear();
        Transform enemyContainer = gameObject.transform.Find("EnemyUnits");
        Debug.Log($"{enemyContainer}");
        foreach (Transform transform in enemyContainer)
        {
            if (transform.gameObject.activeSelf)
            {
                CharacterManager chman = transform.GetComponent<CharacterManager>();
                chman.spriteRenderer.color = Color.red;
            }
        }

        playerUnits.Clear();
        Transform playerContainer = gameObject.transform.Find("PlayerUnits");
        foreach (Transform transform in playerContainer)
        {
            if (transform.gameObject.activeSelf)
            {
                CharacterManager chman = transform.GetComponent<CharacterManager>();
                playerUnits.Add(chman);
                chman.ReplenishActions();
            }
        }
        
        playerActions = playerCount = playerUnits.Count;
        if (turnCount == 1) return;
        StartCoroutine( TTool.TransitionPhaseChange("player", 2) );
    }

    void BeginEnemyPhase()
    {
        MouseController.Instance.cursorLocked = true;
        Debug.Log($"Enemy Phase!");
        turnState = TurnState.enemy;          
        
        playerUnits.Clear();
        Transform playerContainer = gameObject.transform.Find("PlayerUnits");
        foreach (Transform transform in playerContainer)
        {
            if (transform.gameObject.activeSelf)
            {
                CharacterManager chman = transform.GetComponent<CharacterManager>();
                chman.spriteRenderer.color = Color.white;
            }
        }

        enemyUnits.Clear();
        Transform enemyContainer = gameObject.transform.Find("EnemyUnits");
        foreach (Transform transform in enemyContainer)
        {
            if (transform.gameObject.activeSelf)
            {
                CharacterManager chman = transform.GetComponent<CharacterManager>();
                enemyUnits.Add(transform.GetComponent<CharacterManager>());
                chman.ReplenishActions();
            }

        }
        enemyActions = enemyCount = enemyUnits.Count;
        StartCoroutine( TTool.TransitionPhaseChange("enemy", 2) );
        
        //Debug.Log($"{enemyCount}");
        if (enemyCount <= 0)
        {
            StartCoroutine( WaitThenTransition() );
        }
        else
        {
            EnemyTeam.Instance.EnemyTeamBegin();
        }
    }

    IEnumerator WaitThenTransition(int delay=0)
    {
        yield return new WaitForSeconds(1);
        if (turnState == TurnState.enemy)
        {
            BeginPlayerPhase();
        }
        else if (turnState == TurnState.player)
        {
            BeginEnemyPhase();
        }
        yield break;
    }

    // Called in CharacterManager every time a Units is out of actions.
    // If implement dancing, make this more robust
    public void OnPlayerActionUsed()
    {
        playerActions--;
        if (playerActions <= 0)
        {
            StartCoroutine( WaitThenTransition() );
        }
    }
    
    public void onEnemyActionUsed()
    {
        enemyActions--;
        if (enemyActions <= 0)
        {
            StartCoroutine( WaitThenTransition() );
        } 
    }
}
