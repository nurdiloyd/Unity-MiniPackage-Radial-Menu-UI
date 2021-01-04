using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using ElephantSDK;

public class GameManager : MonoBehaviour
{
    public GameConfigData GameConfig;       // TODO: Assign GameConfig object that you created
    public GUIController GUIController;     // TODO: Assign GUIController gameobject
    public AudioManager AudioManager;
    public Transform Levels;                // TODO: Assign Levels gameobject that includes all levels as gameobject
    public bool ClearPlayerPrefs;           // You can delete PlayerPrefs by mading true. Don't forget to turn back to false
    public Camera MainCamera;
    public CinemachineVirtualCamera StdCamera;

    [NonSerialized] public LevelManager CurrentLevel;     // You can reach your current level gameobject. It will be assigned when started.
    private int _level;                     // Index of current level


    private void Awake() {
        if (ClearPlayerPrefs) {
            PlayerPrefs.DeleteAll();
        }
     
        LoadLevel();
    }
    
    // TODO: Call this method from Reset or Try-Again button in the game  
    public void ResetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TODO: Call this method, When the level failed
    public void LevelFailed() {
        Elephant.LevelFailed(_level);
        Stop();

        GUIController.FailScreenActive(true);
    }

    // TODO: Call this method, When the level completed 
    public void LevelCompleted() {
        Elephant.LevelCompleted(_level);
        Stop();

        StdCamera.enabled = false;
        GUIController.WinScreenActive(true);
    }

    // TODO: Call this method from Next Level button in the game
    public void NextLevel() {
        int levelText = PlayerPrefs.GetInt("levelText");

        levelText += 1;
        if (levelText >= Levels.childCount) {
            _level = UnityEngine.Random.Range(0, Levels.childCount);
        }
        else {
            _level += 1;
        }
 
        PlayerPrefs.SetInt("level", _level);
        PlayerPrefs.SetInt("levelText", levelText);

        ResetLevel();
    }

    private void LoadLevel() {
        _level = PlayerPrefs.GetInt("level");
        Elephant.LevelStarted(_level);

        Transform level = Levels.GetChild(_level);
        level.gameObject.SetActive(false);
        CurrentLevel = Instantiate(level).GetComponent<LevelManager>();
        CurrentLevel.gameObject.SetActive(true);

        GUIController.SetLevelText(PlayerPrefs.GetInt("levelText") + 1);
    }

    public void Stop() {
        GUIController.TopBarActive(false);
        CurrentLevel.StopLevel();
    }

    public void Play() {
        GUIController.StartScreenActive(false);
        CurrentLevel.StartLevel();
    }

    // ****** //

    public void SlowMotion(bool active) {
        if (active) {
            CurrentLevel.SlowDownChars();
        }
        else {
            CurrentLevel.SpeedUpChars();
        }
    }
}
