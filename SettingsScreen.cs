using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingsScreen : MonoBehaviour
{
    private static SettingsScreen _instance;
    public static SettingsScreen Instance { get { return _instance; } }

    public AudioMixer _audioMixer;
    public int GameSpeed;

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
        //_audioMixer = ;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Tab))
        {
            CloseSettingsScreen();
        }
    }

    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("VOLUME", volume);
    }

    public void SetGameSpeed(int newSpeed)
    {
        switch (newSpeed)
        {
            case 0:
                MouseController.Instance.gameSpd = 5f;
                break;

            case 1:
                MouseController.Instance.gameSpd = 10f;
                break;

            case 2:
                MouseController.Instance.gameSpd = 15f;
                break;

            default:
                break;
        }
        Debug.Log($"{MouseController.Instance.gameSpd}");
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OpenSettingsScreen()
    {
        Debug.Log("Opened Settings Screen");
        
    }

    public void CloseSettingsScreen()
    {
        SceneManager.UnloadSceneAsync("Settings");
    }

   
}
