using System;
using System.Collections;
using UnityEngine;
using cakeslice;

public class LevelManager : MonoBehaviour
{
    [NonSerialized] public GameManager GameManager;
    [NonSerialized] public bool LevelStarted;
    [NonSerialized] public bool LevelStoped;
    [NonSerialized] public StageController Stage;
    [NonSerialized] public Player Player;
    [NonSerialized] public bool LevelCompletedMe;

    private OutlineEffect _outlineControler;

    private void Awake() {
        GameManager = FindObjectOfType<GameManager>();
        SetLevel();
    }

    // TODO: When the level is completed: Call _gameManager.LevelCompleted()
    public void LevelCompleted() {
        LevelCompletedMe = true;
        GameManager.LevelCompleted();
    }

    // TODO: When the level is failed: Call _gameManager.LevelFailed()
    public IEnumerator LevelFailed() {
        GameManager.StdCamera.Follow = null;
        
        yield return new WaitForSeconds(0.5f);

        if (!LevelCompletedMe) {
            GameManager.LevelFailed();
            GameManager.StdCamera.LookAt = null;
        }
    }

    // ******************** //

    public void StartLevel() {
        LevelStarted = true;
    }

    public void StopLevel() {
        LevelStoped = true;
    }

    private void SetLevel() {
        Player = FindObjectOfType<Player>();
        Stage = transform.GetChild(0).GetComponent<StageController>();
        _outlineControler = GameManager.MainCamera.GetComponent<cakeslice.OutlineEffect>();
    }

    public HueData[] GetPaletteColors() {
        HueColor[] hueColors = Stage.HueColorsOnStage;

        HueData[] hueDatas = new HueData[hueColors.Length];
        HueData[] allHueDatas = GameManager.GameConfig.HueDatas;
        for (int i = 0; i < hueDatas.Length; i++) {
            for (int j = 0; j < allHueDatas.Length; j++) {
                if (hueColors[i] == allHueDatas[j].Hue) {
                    hueDatas[i] = allHueDatas[j];
                }
            }
        }

        return hueDatas;
    }

    public void SetCurrentColor(HueColor hue, Color matColor) {
       // _outlineControler.lineColor0 = matColor;
        Stage.SetColor(hue, matColor);
    }

    public void ResetCurrentColor() {
        Stage.ResetColor();
    }

    public void SlowDownChars() {
        Player.AdjustSpeed(0.3f);
    }

    public void SpeedUpChars() {
        Player.AdjustSpeed(1);
    }
    
    public void PlayerWon() {
        LevelCompleted();
    }
    
    public void PlayerIsDead() {
        StartCoroutine(LevelFailed());
    }
}

