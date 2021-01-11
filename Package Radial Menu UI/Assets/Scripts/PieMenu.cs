using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PieMenu : MonoBehaviour
{
    public PiPiece piCut;

    [Header("Transition Settings")]
    [SerializeField] protected float transitionSpeed;
    [SerializeField] public TransitionType openTransition;
    [SerializeField] public TransitionType closeTransition;
    public enum TransitionType { Scale };
    
    public bool useController;
    [HideInInspector] public bool joystickButton;
    [HideInInspector] public Vector2 joystickInput;

    [Range(1, 3)] public float hoverScale;
    public float scaleModifier = 1;

    [HideInInspector] public bool interactable = false;
    public readonly List<PiPiece> piList = new List<PiPiece>( );

    [Header("Slice Data")]
    public bool equalSlices;
    [HideInInspector] [Range(1, 30)] public int sliceCount;
    [HideInInspector] public PieData[] PieDatas;


    [HideInInspector] public bool openedMenu;
    private Vector2 menuPosition;
    [SerializeField] [HideInInspector] private float[] angleList;
    [HideInInspector] public bool overMenu;


    private void Awake() {
        GeneratePie(new Vector2(-1000, -1000));
    }

    public void GeneratePie(Vector2 screenPosition) {
        sliceCount = PieDatas.Length;
        if (piList.Count > 1) {
            ClearMenu( );
        }

        transform.position = screenPosition;
        float lastRot = 0;

        angleList = new float[sliceCount];
        for (int i = 0; i < sliceCount; i++)
        {
            PiPiece currentPi = Instantiate(piCut);
            Image currentImage = currentPi.GetComponent<Image>( );
            

            float fillPercentage = (1f / sliceCount);
            float angle = fillPercentage * 360;
            
            if (!equalSlices) {
                angle = PieDatas[i].Angle;
                fillPercentage = PieDatas[i].Angle / 360;
            }
            currentImage.fillAmount = fillPercentage;
            angle = (angle + 360) % 360;
            
            if (angle == 0) {
                angle = 360;
            }

            currentPi.transform.SetParent(transform);
            int rot = Mathf.Clamp((int)(angle + lastRot), 0, 360);
            
            if (rot == 360) {
                rot = 0;
            }
            currentPi.transform.rotation = Quaternion.Euler(0, 0, rot);
            lastRot += angle;
            angleList[i] = rot;
            
            currentImage.rectTransform.localPosition = Vector2.zero;
            currentPi.SetData(PieDatas[i], this);
            piList.Add(currentPi);
        }
        openedMenu = false;
    }

    private void Update() {
        overMenu = false;
        //Open the menu with the selected opening transition, or if !openmenu close menu with selected closing transition
        if (openedMenu)
        {
            switch (openTransition)
            {
                case TransitionType.Scale:
                    Scale( );
                    transform.position = menuPosition;
                    break;
            }
        }
        else if (!openedMenu)
        {
            interactable = false;
            switch (closeTransition)
            {
                case TransitionType.Scale:
                    Scale( );
                    transform.position = menuPosition;
                    break;
            }
        }

        foreach (PiPiece pi in piList) {
            if (pi.gameObject.activeInHierarchy) {
                pi.ManualUpdate( );
            }
        }
    }

    public void CloseMenu() {
        openedMenu = false;
    }

    /// <summary>
    /// Open menu and if the menu isn't created create the slices, then do their transition.
    /// </summary>
    /// <param name="screenPos">Place in screen position to open the menu</param>
    public void OpenMenu(Vector2 screenPos)
    {
        menuPosition = screenPos;
        openedMenu = true;
        foreach (PiPiece pi in piList) {
            pi.gameObject.SetActive(true);
        }

        if (piList.Count == 0) {
            GeneratePie(screenPos);
        }
        else {
            ResetPiRotation( );
        }
        
        switch (openTransition)
        {
            case TransitionType.Scale:
                transform.localScale *= 0;
                break;
        }
    }

    // Clear menu and destroy all pi slices
    public void ClearMenu() {
        foreach (PiPiece pi in piList) {
            if (pi == null) {
                piList.Clear( );
                break;
            }
            DestroyImmediate(pi.gameObject);
        }
        piList.Clear( );
    }

    #region TRANSITIONS
    private void Scale()
    {
        if (openedMenu)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one * scaleModifier, Time.deltaTime * transitionSpeed);
            if (Mathf.Abs((Vector2.one * scaleModifier).sqrMagnitude - transform.localScale.sqrMagnitude) < .05f)
            {

                interactable = true;
            }
        }
        else if (!openedMenu)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, Time.deltaTime * transitionSpeed);
            if (transform.localScale.x < .05f)
            {
                transform.localScale = Vector2.zero;
                foreach (PiPiece pi in piList)
                {
                    pi.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Set pi rotation to proper rotation, useful for ensuring rotational transitions work.
    /// </summary>
    private void ResetPiRotation()
    {
        float lastRot = 0;
        for (int i = 0; i < piList.Count; i++)
        {
            float fillPercentage = (1f / sliceCount);
            float angle = fillPercentage * 360;
            if (!equalSlices) {
                angle = PieDatas[i].Angle;
            }
            int rot = Mathf.Clamp((int)(angle + lastRot), 0, 359);
            Vector3 rotVec = new Vector3(0, 0, rot);
            piList[i].transform.rotation = Quaternion.Euler(rotVec);
            lastRot += angle;
        }
    }

    /// Set Pis Rotation to zero
    private void PiRotationToNil() {
        foreach (PiPiece pi in piList) {
            pi.transform.rotation = Quaternion.identity;
        }
    }

    /// Use This Function To Update the Slices When pi count maintains the same count
    public void UpdatePiUI() {
        foreach (PiPiece currentPi in piList) {   
            currentPi.SetData(PieDatas[piList.IndexOf(currentPi)], this);
        }
    }

    [System.Serializable]
    public class PieData
    {
        [Range(20, 360)] public float Angle;
        public int Order;
        public Color ButtonColor;
        public bool Interactable = true;
        public UnityAction<int> OnPressed;

        public void SetValues(PieData pieData) {
            ButtonColor = pieData.ButtonColor;
            Angle = pieData.Angle;
            Interactable = pieData.Interactable;
        }

#if UNITY_EDITOR
        public void OnInspectorGUI(PieMenu menu, System.Action AddSlice, System.Action<PieData> RemoveSlice, System.Action<int> angleUpdate) {
            Order = Mathf.Clamp(Order, 0, menu.PieDatas.Length);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            
                GUILayout.BeginHorizontal( );

                    if (Order > 0 && GUILayout.Button("▲", GUILayout.Width(32))) {
                        Order = Mathf.Clamp(Order - 1, 0, menu.PieDatas.Length);
                        foreach (PieData pie in menu.PieDatas) {
                            if (pie != this && pie.Order == Order) {
                                pie.Order = Mathf.Clamp(pie.Order + 1, 0, menu.PieDatas.Length);
                                break;
                            }
                        }
                    }
                    
                    if (Order < menu.PieDatas.Length - 1 && GUILayout.Button("▼", GUILayout.Width(32))) {
                        Order = Mathf.Clamp(Order + 1, 0, menu.PieDatas.Length);
                        foreach (PieData pie in menu.PieDatas) {
                            if (pie != this && pie.Order == Order) {
                                pie.Order = Mathf.Clamp(pie.Order - 1, 0, menu.PieDatas.Length);
                                break;
                            }
                        }
                    }

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("-", GUILayout.Width(32))) {
                        RemoveSlice.Invoke(this);
                    }
                    GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal( );


                GUILayout.BeginHorizontal( );
                EditorGUILayout.LabelField("Button Color");
                ButtonColor = EditorGUILayout.ColorField(ButtonColor);
                GUILayout.EndHorizontal( );

                GUILayout.BeginHorizontal( );
                GUILayout.Label("Interactable", GUILayout.Width(96));
                Interactable = EditorGUILayout.Toggle(Interactable);
                GUILayout.EndHorizontal( );

            GUILayout.EndVertical( );

            if (Order == menu.PieDatas.Length - 1) {    
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("+")) {
                    AddSlice.Invoke();
                }
                GUI.backgroundColor = Color.white;
            }
        }
#endif
    }
}