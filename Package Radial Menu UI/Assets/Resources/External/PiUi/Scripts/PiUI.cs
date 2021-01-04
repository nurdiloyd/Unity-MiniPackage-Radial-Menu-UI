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
public class PiUI : MonoBehaviour
{
    public PiPiece piCut;

    [Header("Transition Settings")]
    [SerializeField] protected float transitionSpeed;
    [SerializeField] public TransitionType openTransition;
    [SerializeField] public TransitionType closeTransition;

    [Header("Platform Settings")]
    public bool dynamicallyScaleToResolution;
    [SerializeField] protected Vector2 defaultResolution;
    
    public bool useController;
    [HideInInspector] public bool joystickButton;
    [HideInInspector] public Vector2 joystickInput;

    [Range(1, 3)] public float hoverScale;
    public float scaleModifier = 1;

    [HideInInspector] public bool interactable = false;
    public readonly List<PiPiece> piList = new List<PiPiece>( );

    [Header("Slice Data")]
    public bool equalSlices;
    [HideInInspector]
    [Range(1, 30)]
    public int sliceCount;

    [HideInInspector]
    public PiData[] piData;


    [HideInInspector]
    public bool openedMenu;

    public enum TransitionType { Scale, Fan, ScaleAndFan };

    private Vector2 menuPosition;
    [SerializeField]
    [HideInInspector]
    private float[] angleList;
    [HideInInspector]
    public bool overMenu;


    private void Awake() {
        if (dynamicallyScaleToResolution) {
            if (Screen.width > Screen.height) {
                scaleModifier *= Screen.height / defaultResolution.y;
            }
            else {
                scaleModifier *= Screen.width / defaultResolution.x;
            }
        }

        GeneratePi(new Vector2(-1000, -1000));
    }

    // Clear menu and make a new pi with the updated pidata and position
    public void GeneratePi(Vector2 screenPosition) {
        sliceCount = piData.Length;
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
                angle = piData[i].angle;
                fillPercentage = piData[i].angle / 360;
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
            currentPi.SetData(piData[i], this);
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
                case TransitionType.Fan:
                    Fan( );
                    break;
                case TransitionType.ScaleAndFan:
                    Fan( );
                    Scale( );
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
                case TransitionType.ScaleAndFan:
                    Fan( );
                    Scale( );
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
            GeneratePi(screenPos);
        }
        else {
            ResetPiRotation( );
        }
        
        switch (openTransition)
        {
            case TransitionType.Scale:
                transform.localScale *= 0;
                break;
            case TransitionType.Fan:
                transform.localScale = Vector2.one * scaleModifier;
                PiRotationToNil( );
                break;
            case TransitionType.ScaleAndFan:
                transform.localScale *= 0;
                PiRotationToNil( );
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

    private void Fan()
    {
        transform.position = menuPosition;
        float rotZ = transform.GetChild(transform.childCount - 1).rotation.eulerAngles.z;
        int erase = 0;
        bool closeToAngle = true;
        for (int i = 0; i < piList.Count; i++)
        {
            if (openedMenu)
            {
                piList[i].transform.rotation = Quaternion.Lerp(piList[i].transform.rotation, Quaternion.Euler(0, 0, angleList[i]), Time.deltaTime * transitionSpeed);
                if (Mathf.Abs(angleList[i] - ((piList[i].transform.rotation.eulerAngles.z + 360) % 360)) > 3 && closeToAngle)
                {
                    closeToAngle = false;
                }
            }
            else if (!openedMenu)
            {
                piList[i].transform.rotation = Quaternion.Lerp(piList[i].transform.rotation, Quaternion.Euler(0, 0, rotZ), Time.deltaTime * transitionSpeed);
                float currentAngle = Mathf.Abs(piList[i].transform.rotation.eulerAngles.z + 360f) % 360;
                float lowComp = (rotZ - 10 + 360f) % 360;
                float highComp = (rotZ + 10 + 360f) % 360;
                bool rotNil = (currentAngle >= lowComp && rotZ == 0 || currentAngle <= highComp && rotZ == 0);
                if (currentAngle >= lowComp && currentAngle <= highComp || rotZ == piList[i].transform.rotation.eulerAngles.z || rotNil)
                {
                    erase++;
                }
            }
        }
        interactable = closeToAngle;
        if (erase == piList.Count)
        {
            foreach (PiPiece pi in piList)
            {
                pi.gameObject.SetActive(false);
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
                angle = piData[i].angle;
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
            currentPi.SetData(piData[piList.IndexOf(currentPi)], this);
        }
    }

    [System.Serializable]
    public class PiData
    {
        [Range(20, 360)]
        public float angle;
        public HueColor hue;
        public Color buttonColor;
        public Color materialColor;
        public UnityAction<HueColor, Color> onSlicePressed;
        public bool isInteractable = true;
        public int order;

        public void SetValues(PiData newData) {
            buttonColor = newData.buttonColor;
            materialColor = newData.materialColor;
            angle = newData.angle;
            isInteractable = newData.isInteractable;
        }

#if UNITY_EDITOR
        public void OnInspectorGUI(SerializedProperty sprop, PiUI menu, System.Action AddSlice, System.Action<PiData> RemoveSlice, System.Action<int> angleUpdate) {
            order = Mathf.Clamp(order, 0, menu.piData.Length);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            
                GUILayout.BeginHorizontal( );

                    if (order > 0 && GUILayout.Button("▲", GUILayout.Width(32))) {
                        order = Mathf.Clamp(order - 1, 0, menu.piData.Length);
                        foreach (PiData pi in menu.piData) {
                            if (pi != this && pi.order == order) {
                                pi.order = Mathf.Clamp(pi.order + 1, 0, menu.piData.Length);
                                break;
                            }
                        }
                    }
                    
                    if (order < menu.piData.Length - 1 && GUILayout.Button("▼", GUILayout.Width(32))) {
                        order = Mathf.Clamp(order + 1, 0, menu.piData.Length);
                        foreach (PiData pi in menu.piData) {
                            if (pi != this && pi.order == order) {
                                pi.order = Mathf.Clamp(pi.order - 1, 0, menu.piData.Length);
                                break;
                            }
                        }
                    }
                    
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("+", GUILayout.Width(32)))
                        AddSlice.Invoke( );
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("-", GUILayout.Width(32)))
                        RemoveSlice.Invoke(this);
                    GUI.backgroundColor = Color.white;

                GUILayout.EndHorizontal( );


                GUILayout.BeginHorizontal( );
                EditorGUILayout.LabelField("Non Selected Color");
                buttonColor = EditorGUILayout.ColorField(buttonColor);
                GUILayout.EndHorizontal( );

                GUILayout.BeginHorizontal( );
                GUILayout.Label("Interactable", GUILayout.Width(96));
                isInteractable = EditorGUILayout.Toggle(isInteractable);
                GUILayout.EndHorizontal( );

            GUILayout.EndVertical( );
        }
#endif
    }
}