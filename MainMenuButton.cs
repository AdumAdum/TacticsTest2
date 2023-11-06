using System.Collections;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] GameObject Transitiontool;
    TransitionTool TTool;

    [SerializeField] protected string _levelName;

    public bool locked = false;

    public string LevelName => _levelName;

    void Start()
    {
        TTool = Transitiontool.GetComponent<TransitionTool>();
    }

    public void LoadLevel()
    {
        StartCoroutine( TTool.TransitionLoad(_levelName) );
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}