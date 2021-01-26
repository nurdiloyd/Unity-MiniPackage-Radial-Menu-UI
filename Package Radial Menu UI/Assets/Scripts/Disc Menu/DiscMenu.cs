using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DiscMenu : MonoBehaviour
{
    public DiscSlice discSlice;

    [Header("Transition Settings")]
    public Ease OpenEase;
    public Ease CloseEase;
    [Min(0.1f)] public float TransitionTime;
    public TransitionType OpenTransition;
    public TransitionType CloseTransition;
    
    [System.Flags] public enum TransitionType {
        Scale = (1 << 0), Rotate = (1 << 1), Fade = (1 << 2), ScaleSlice = (1 << 3)
    }

    [Header("Hover Settings")]
    public Ease HoverInEase;
    public Ease HoverOutEase;
    [Min(0.1f)] public float HoverTime;
    [Range(0.5f, 3)] public float HoverScale;
    
    [Header("Disc Settings")]
    public Color NormalColor;
    public Color HoverColor;
    [HideInInspector] public float InnerRadius;
    [HideInInspector] public float OuterRadius;

    [HideInInspector] public DiscData[] DiscDatas;
    [HideInInspector] public bool Opened = true;
    [HideInInspector] public List<DiscSlice> DiscSlices = new List<DiscSlice>();
    
    private List<Tween> _openingTweens = new List<Tween>();
    private List<Tween> _closingTweens = new List<Tween>();
    private bool _inited;


    private void Start() {
        if (!_inited) {
            InitDiscMenu(Vector2.one * -1000);
        }

        CloseMenu();
    }

    // Inits the disc menu
    public void InitDiscMenu(Vector2 pos) {
        transform.localPosition = pos;

        var tempList = transform.Cast<Transform>().ToList();
        for (int i = 1; i < tempList.Count; i++) {
            DestroyImmediate(tempList[i].gameObject);
        }

        DiscSlices.Clear();
        discSlice.InitDiskSlice(0, this);
        DiscSlices.Add(discSlice);
        for (int i = 1; i < DiscDatas.Length; i++) {
            DiscSlice slice = Instantiate(discSlice);
            slice.InitDiskSlice(i, this);
            DiscSlices.Add(slice);
        }

        _inited = true;
    }

    private void Update() {
        foreach (DiscSlice discSlice in DiscSlices) {
            discSlice.ManualUpdate();
        }
    }

    // Opens the menu
    public void OpenMenu(Vector2 pos) {
        transform.position = pos;
        DiscActive(true);

        /// TRANSITIONS
        Tween tween = Trans(0, OpenTransition, OpenEase);

        if (tween != null) {
            tween.OnStepComplete(() => {
                Opened = true;
            });
        }
        else {
            Opened = true;
        }
    }

    // Closes the menu
    public void CloseMenu() {
        Opened = false;

        /// TRANSITIONS
        Tween tween = Trans(1, CloseTransition, CloseEase);

        if (tween != null) {
            tween.OnStepComplete(() => {
                DiscActive(false);
            });
        }
        else {
            DiscActive(false);
        }
    }

    private Tween Trans(float from, TransitionType transType, Ease ease) {
        List<Tween> tweensNew = from == 1 ? _closingTweens : _openingTweens;
        List<Tween> tweensTrash = from == 1 ? _openingTweens : _closingTweens;

        foreach (Tween twn in tweensTrash) {
            twn.Pause();
        }
        tweensNew.Clear();

        Tween tween = null;

        // SCALE
        if (transType.HasFlag(TransitionType.Scale)) {
            transform.localScale *= from;
            tween = transform.DOScale(Vector3.one * (1 - from), TransitionTime).SetEase(ease);
            tweensNew.Add(tween);
        }
        
        // ROTATE
        if (transType.HasFlag(TransitionType.Rotate)) {
            transform.eulerAngles = (1 - from) * Vector3.forward * 360f / DiscDatas.Length;
            tween = transform.DORotate(from * Vector3.forward * 360f / DiscDatas.Length, TransitionTime).SetEase(ease);
            tweensNew.Add(tween);
        }

        // FADE
        if (transType.HasFlag(TransitionType.Fade)) {
            for (int i = 0; i < DiscSlices.Count; i++) {
                DiscSlice slice = DiscSlices[i];

                slice.SetImgAlpha(from);
                tween = slice.FadeImage(1 - from, TransitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);

                slice.SetIconAlpha(from);
                tween = slice.FadeIcon(1 - from, TransitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);
            }
        }
        
        // SCALE SLICES
        if (transType.HasFlag(TransitionType.ScaleSlice)) {
            for (int i = 0; i < DiscSlices.Count; i++) {
                DiscSlice slice = DiscSlices[i];

                slice.SetRtOutSize(from * OuterRadius);
                tween = slice.ScaleRtOut((1 - from) * OuterRadius, TransitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);

                slice.SetRtInSize(from * InnerRadius);
                tween = slice.ScaleRtIn((1 - from) * InnerRadius, TransitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);
            }
        }

        return tween;
    }

    private void DiscActive(bool active) {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        foreach (var slice in DiscSlices) {
            slice.ResetSlice(active, false);
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