using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionMenu : MonoBehaviour
{
    private static ActionMenu _instance;
    public static ActionMenu Instance { get { return _instance; } }

    [SerializeField] GameObject Transitiontool;
    TransitionTool TTool;

    [SerializeField] GameObject BattleForecast;
    BattleForecast battleForecast;

    Camera m_camera;
    Vector3 startingPosition;
    Vector3 inversePosition;

    CanvasGroup canvasGroup;
    string SettingsSceneName;

    public CharacterManager playerUnit;


    string[] menuButtons = {
        "ButtonQuit",
        "ButtonSettings"
    };
    string[] actionButtons = {
        "ButtonWait",
        "ButtonAttack"
    };

    public enum MenuState
    {
        closed,
        mapOpen,
        unitOpen,
        atkMode,
        battleForecast
    }
    public MenuState menuState;

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
        battleForecast = BattleForecast.GetComponent<BattleForecast>();
        TTool = Transitiontool.GetComponent<TransitionTool>();
        canvasGroup = GetComponent<CanvasGroup>();
        SettingsSceneName = "Settings";
        menuState = MenuState.closed;
        
        m_camera = Camera.main;
        startingPosition = transform.position;
        //Debug.Log($"{startingPosition}");
        inversePosition = new Vector3(-startingPosition.x, startingPosition.y, transform.position.z);
        //Debug.Log($"{inversePosition}");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log($"{menuState}");
            switch (menuState)
            {
                case MenuState.closed:
                    OpenActionMenuMap();
                    break;

                case MenuState.mapOpen:
                    CloseActionMenu();
                    break;

                default:
                    break;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log($"{menuState}");
            switch (menuState)
            {
                case MenuState.closed:
                    break;

                case MenuState.mapOpen:
                    CloseActionMenu();
                    break;

                case MenuState.unitOpen:
                    Debug.Log($"{menuState}");
                    CloseActionMenu();
                    break;

                case MenuState.atkMode:
                    ExitAttackMode();
                    break;

                case MenuState.battleForecast:
                    CloseBattleForecast();
                    break;

                default:
                    break;
            }
        }
            
    }

    public void Wait()
    {
        Debug.Log($"Waiting. Turn {TurnManager.Instance.turnCount}");
        if (playerUnit == null) return;
        playerUnit.OutOfActions();
        //CloseActionMenu();
    }

    public void Quit()
    {
        //CloseActionMenu();
        StartCoroutine( TTool.TransitionLoad("MainMenu") );
    }

    public void Settings()
    {
        if (!SceneManager.GetSceneByName(SettingsSceneName).isLoaded)
        {
            SceneManager.LoadScene(SettingsSceneName, LoadSceneMode.Additive);
        }
    }

    public void ExecuteAttack()
    {
        //disable battle forecast, close menu
        battleForecast.HideCanvas();
        CloseActionMenu();
        playerUnit.Attack();

        // coroutine spin animation then call out of actions
    }

    public void OpenBattleForecast(CharacterManager Attacker, CharacterManager Defender)
    {
        menuState = MenuState.battleForecast;
        battleForecast.gameObject.SetActive(true);
        battleForecast.CharacterSetup(Attacker, Defender);
    }

    public void CloseBattleForecast()
    {
        menuState = MenuState.atkMode;
        battleForecast.gameObject.SetActive(false);
    }

    // Move these to Character Manager later?
    public void EnterAttackMode()
    {
        MouseController.Instance.atkCursor = true;

        playerUnit.HideAllTiles();

        var singleTileList = new List<OverlayTile>() { playerUnit.activeTile };
        playerUnit.ShowInRangeTiles(singleTileList);

        var inRangeFromSingle = playerUnit.rangeFinder.getAtkTilesFromSingle(playerUnit.activeTile, playerUnit._stats.RANGE);
        playerUnit.ShowAtkRangeTiles(inRangeFromSingle);

        //Debug.Log($"Switching State to atkMode");
        menuState = MenuState.atkMode;
    }

    public void ExitAttackMode()
    {
        MouseController.Instance.atkCursor = false;

        var singleTileList = new List<OverlayTile>() { playerUnit.activeTile };
        playerUnit.HideInRangeTiles(singleTileList);

        var inRangeFromSingle = playerUnit.rangeFinder.getAtkTilesFromSingle(playerUnit.activeTile, playerUnit._stats.RANGE);
        playerUnit.HideInRangeTiles(inRangeFromSingle);

        playerUnit.ShowAllTiles();
        //Debug.Log($"Switching State to unitOpen");
        menuState = MenuState.unitOpen;
    }


    public void OpenActionMenuUnit(CharacterManager unit)
    {
        if (unit.isPlayer == false) return;
        playerUnit = unit;
        //CheckSwitchScreenSide();

        DeactivateButtons();
        ActivateButtons(actionButtons);

        //Debug.Log($"Switching State to unitOpen");
        menuState = MenuState.unitOpen;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OpenActionMenuMap()
    {
        DeactivateButtons();

        ActivateButtons(menuButtons);
        //CheckSwitchScreenSide();

        //Debug.Log($"Switching State to mapOpen");
        menuState = MenuState.mapOpen;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        MenuPause();
    }

    public void CloseActionMenu()
    {
        MouseController.Instance.atkCursor = false;
        if (SceneManager.GetSceneByName(SettingsSceneName).isLoaded) 
            return;
        MenuUnpause();
        DeactivateButtons();
        
        //Debug.Log($"Switching State to closed");
        menuState = MenuState.closed;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // private void CheckSwitchScreenSide()
    // {
    //     if (playerUnit != null && m_camera.WorldToScreenPoint(playerUnit.transform.position).x < (Screen.width / 2))
    //     {
    //         transform.position = startingPosition;
    //         battleForecast.BFSwitchScreenSide();
    //     }
    //     else 
    //     {
    //        transform.position = inversePosition;
    //        battleForecast.BFSwitchScreenSide(true);
    //     }
    // }

    private void ActivateButtons(string[] buttons)
    {
        foreach (string button in buttons)
        {
            Transform childTransform = gameObject.transform.Find(button);
            if (childTransform != null)
            {
                childTransform.gameObject.SetActive(true);
            }
            //Debug.Log($"{button}");
            if (button == "ButtonAttack" && !playerUnit.CanAttack())
            {
                childTransform.gameObject.SetActive(false);
            }
        }
    }

    private void DeactivateButtons()
    {
        foreach(Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(false);
        }
    }

    private void MenuPause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            transform.parent.Find("PauseOverlay").gameObject.SetActive(true);
            MouseController.Instance.cursorDisabled = true;
        }
        else return;
    }
    private void MenuUnpause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            transform.parent.Find("PauseOverlay").gameObject.SetActive(false);
            MouseController.Instance.cursorDisabled = false;
        }
        else return;
    }
}
