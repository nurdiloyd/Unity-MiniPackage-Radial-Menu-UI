using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DiscMenu : MonoBehaviour
{
    [SerializeField] protected DiscSlice discSlice;
    public UnityAction<DiscData> ClickEvent;

    [Header("Transition Settings")]
    [SerializeField] protected Ease openEase;
    [SerializeField] protected Ease closeEase;
    [SerializeField] [Range(0.1f, 2f)] protected float transitionTime;
    [SerializeField] protected TransitionType openTransition;
    [SerializeField] protected TransitionType closeTransition;
    [System.Flags] protected enum TransitionType {
        Scale = (1 << 0), Rotate = (1 << 1), FadeSlice = (1 << 2), ScaleSlice = (1 << 3)
    }

    [Header("Hover Settings")]
    public Ease HoverInEase;
    public Ease HoverOutEase;
    [Range(0.1f, 2f)] public float HoverTime;
    [Range(0.5f, 3f)] public float HoverScale;
    
    [Header("Disc Settings")]
    public Color NormalColor;
    public Color HoverColor;
    [HideInInspector] public float InnerRadius;
    [HideInInspector] public float OuterRadius;
    [HideInInspector] public float ScaleModifier;

    [HideInInspector] public DiscData[] DiscDatas;
    [HideInInspector] public bool Opened = true;

    private List<DiscSlice> _discSlices = new List<DiscSlice>();
    private List<Tween> _openingTweens = new List<Tween>();
    private List<Tween> _closingTweens = new List<Tween>();


    // Inits the disc menu
    public void InitDiscMenu(Vector2 pos) {
        transform.localPosition = pos;

        var tempList = transform.Cast<Transform>().ToList();
        for (int i = 1; i < tempList.Count; i++) {
            DestroyImmediate(tempList[i].gameObject);
        }

        _discSlices.Clear();
        discSlice.InitDiskSlice(0, this);
        _discSlices.Add(discSlice);
        for (int i = 1; i < DiscDatas.Length; i++) {
            DiscSlice slice = Instantiate(discSlice);
            slice.InitDiskSlice(i, this);
            _discSlices.Add(slice);
        }
    }

    private void Update() {
        foreach (DiscSlice slice in _discSlices) {
            slice.ManualUpdate();
        }
    }

    // Opens the menu
    public void OpenMenu(Vector2 pos) {
        transform.position = pos;
        ResetDisc(true);

        /// TRANSITIONS
        Tween tween = Trans(0, openTransition, openEase);

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

        // Check Selected
        foreach (var slice in _discSlices) {
            if (slice.IsHover()) {
                ClickEvent?.Invoke(slice.GetData());
                break;
            }
        }

        /// TRANSITIONS
        Tween tween = Trans(1, closeTransition, closeEase);

        if (tween != null) {
            tween.OnStepComplete(() => {
                ResetDisc(false);
            });
        }
        else {
            ResetDisc(false);
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
            transform.localScale *= ScaleModifier * from;
            tween = transform.DOScale(Vector3.one * ScaleModifier * (1 - from), transitionTime).SetEase(ease);
            tweensNew.Add(tween);
        }
        
        // ROTATE
        if (transType.HasFlag(TransitionType.Rotate)) {
            transform.eulerAngles = (1 - from) * Vector3.forward * 360f / DiscDatas.Length;
            tween = transform.DORotate(from * Vector3.forward * 360f / DiscDatas.Length, transitionTime).SetEase(ease);
            tweensNew.Add(tween);
        }

        // FADE SLICES
        if (transType.HasFlag(TransitionType.FadeSlice)) {
            for (int i = 0; i < _discSlices.Count; i++) {
                DiscSlice slice = _discSlices[i];

                slice.SetImgAlpha(from);
                tween = slice.FadeImage(1 - from, transitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);

                slice.SetIconAlpha(from);
                tween = slice.FadeIcon(1 - from, transitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);
            }
        }
        
        // SCALE SLICES
        if (transType.HasFlag(TransitionType.ScaleSlice)) {
            for (int i = 0; i < _discSlices.Count; i++) {
                DiscSlice slice = _discSlices[i];

                slice.SetRtOutSize(from * OuterRadius);
                tween = slice.ScaleRtOut((1 - from) * OuterRadius, transitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);

                slice.SetRtInSize(from * InnerRadius);
                tween = slice.ScaleRtIn((1 - from) * InnerRadius, transitionTime).SetEase(ease).SetDelay(i * 0.05f);
                tweensNew.Add(tween);
            }
        }

        return tween;
    }

    public void ResetDisc(bool active) {
        transform.localScale = Vector3.one * ScaleModifier;
        transform.rotation = Quaternion.identity;
        foreach (var slice in _discSlices) {
            slice.ResetSlice(active, false);
        }
    }
}

/// ************************************************** ///
[System.Serializable]
public class DiscData
{
    public int Order;
    public string Name;
    public Sprite Icon;


    public void SetData(DiscData data) {
        Order = data.Order;
        Name = data.Name;
        Icon = data.Icon;
    }

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

            Name = EditorGUILayout.TextField("Name", Name);
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