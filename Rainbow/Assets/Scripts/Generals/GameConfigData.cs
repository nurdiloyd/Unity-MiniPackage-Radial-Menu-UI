
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game Config")]
public class GameConfigData : ScriptableObject
{
    // TODO: Hold general needed information or data, that can be reached from GameManager
    public HueData[] HueDatas;
}

[System.Serializable]
public class HueData {
    public HueColor Hue;
    public Material HueMaterial;
    public Color ButtonColor;
}

public enum HueColor {
    Green, Cyan, Blue, Purple, Pink, Red, Orange, Yellow, SELF
};