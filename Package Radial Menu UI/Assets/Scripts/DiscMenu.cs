using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DiscMenu : MonoBehaviour
{
    public DiscSlice discSlice;

    [Header("Transition Settings")]
    public Ease HoverInEase;
    public Ease HoverOutEase;
    [Range(0.5f, 3)] public float HoverScale;
    public float HoverTime;
    
    [Header("Disc Settings")]
    public Color NormalColor;
    public Color HoverColor;
    [HideInInspector] public float InnerRadius;
    [HideInInspector] public float OuterRadius;

    [HideInInspector] public DiscData[] DiscDatas;
    [HideInInspector] public bool Opened;
    [HideInInspector] public List<DiscSlice> DiscSlices = new List<DiscSlice>();


    // Inits the disc menu
    public void InitDiscMenu(Vector2 pos) {
        transform.position = pos;

        var tempList = transform.Cast<Transform>().ToList();
        for (int i = 1; i < tempList.Count; i++) {
            DestroyImmediate(tempList[i].gameObject);
        }

        DiscSlices.Clear();
        discSlice.SetHover();
        discSlice.InitDiskSlice(0, this);
        DiscSlices.Add(discSlice);
        for (int i = 1; i < DiscDatas.Length; i++) {
            DiscSlice slice = Instantiate(discSlice);
            slice.InitDiskSlice(i, this);
            DiscSlices.Add(slice);
        }
    }

    private void Update() {
        if (Opened) {
            Vector2 dir = Input.mousePosition - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360;
            
            foreach (DiscSlice discSlice in DiscSlices) {
                discSlice.CheckHover(angle);
            }
        }
    }

    /// Opens the menu
    public void OpenMenu(Vector2 pos) {
        Opened = true;
        transform.position = pos;
        foreach (var discSlice in DiscSlices) {
            discSlice.ResetSlice();
            discSlice.gameObject.SetActive(true);
        }
    }

    // Closes the menu
    public void CloseMenu() {
        Opened = false;
        foreach (var discSlice in DiscSlices) {
            discSlice.gameObject.SetActive(false);
        }
    }
}

/// ************************************************** ///
[System.Serializable]
public class DiscData
{
    public int Order;
    public Sprite Icon;


#if UNITY_EDITOR
    public void OnInspectorGUI(DiscMenu menu, System.Action addSlice, System.Action<DiscData> removeSlice) {
        Order = Mathf.Clamp(Order, 0, menu.DiscDatas.Length);

        GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();

                if (Order > 0 && GUILayout.Button("▲")) {
                    Order = Mathf.Clamp(Order - 1, 0, menu.DiscDatas.Length);
                    foreach (var discData in menu.DiscDatas) {
                        if (discData != this && discData.Order == Order) {
                            discData.Order = Mathf.Clamp(discData.Order + 1, 0, menu.DiscDatas.Length);
                            break;
                        }
                    }
                }
                
                if (Order < menu.DiscDatas.Length - 1 && GUILayout.Button("▼")) {
                    Order = Mathf.Clamp(Order + 1, 0, menu.DiscDatas.Length);
                    foreach (var discData in menu.DiscDatas) {
                        if (discData != this && discData.Order == Order) {
                            discData.Order = Mathf.Clamp(discData.Order - 1, 0, menu.DiscDatas.Length);
                            break;
                        }
                    }
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("-", GUILayout.Width(32))) {
                    removeSlice.Invoke(this);
                }
                GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();

            Icon = (Sprite)EditorGUILayout.ObjectField("Icon", Icon, typeof(Sprite), false);
        
        GUILayout.EndVertical();

        if (Order == menu.DiscDatas.Length - 1) {    
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+")) {
                addSlice.Invoke();
            }
            GUI.backgroundColor = Color.white;
        }
    }
#endif
}