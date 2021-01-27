using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(DiscMenu))]
public class DiscUIEditor : Editor
{
    private DiscMenu _target;
    
    private System.Action _addSlice;
    private bool _sliceToAdd;
    private System.Action<DiscData> _removeSlice;
    private DiscData _sliceToRemove;
    private bool _updateFill;
    private bool _showDatas;


    private void OnEnable() {
        _target = (DiscMenu) target;
        _addSlice = AddSlice;
        _removeSlice = RemoveSlice;
        _sliceToRemove = null;
        _showDatas = true;
        _target.InitDiscMenu(Vector2.zero);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject);

        if(!EditorApplication.isPlaying ) {
            _target.ScaleModifier = EditorGUILayout.Slider("Scale Modifier", _target.ScaleModifier, 0.2f, 5);
            _target.InnerRadius = EditorGUILayout.Slider("Inner Radius", _target.InnerRadius, 0, 5);         
            _target.OuterRadius = EditorGUILayout.Slider("Outer Radius", _target.OuterRadius, 1, 10);
            _target.ResetDisc(true);
        }

        GUILayout.Space(10);
        _showDatas = EditorGUILayout.Foldout(_showDatas, "Slices");
        if (_showDatas) {
            if (Selection.activeTransform) {
                // Drawing Slices
                _target.DiscDatas = _target.DiscDatas.ToList().OrderBy(x => x.Order).ToArray();
                for (var i = 0; i < _target.DiscDatas.Length; i++) {
                    if (_sliceToRemove != _target.DiscDatas[i]) {
                        _target.DiscDatas[i].OnInspectorGUI(_target, _addSlice, _removeSlice);
                    }
                }
            }
        }

        if (!Selection.activeTransform) {
            _showDatas = false;
        }
        
        // Adding a Slice
        if (_sliceToAdd) {
            Undo.RecordObject(_target, "Add Slice");
            DiscData[] tempDatas = new DiscData[_target.DiscDatas.Length + 1];
            for (int i = _target.DiscDatas.Length; i < tempDatas.Length; i++) {
                tempDatas[i] = new DiscData();
                tempDatas[i].Order = i;
            }

            for (int i = 0; i < _target.DiscDatas.Length; i++) {
                tempDatas[i] = _target.DiscDatas[i];
            }

            _target.DiscDatas = tempDatas;
            _sliceToAdd = false;
        }

        // Removing a Slice
        if (_sliceToRemove != null && _target.DiscDatas.Length > 1) {
            Undo.RecordObject(_target, "Removed Slice");
            DiscData[] tempDatas = new DiscData[_target.DiscDatas.Length - 1];
            int added = 0;
            for (int i = 0; i < _target.DiscDatas.Length; i++) {
                if (_sliceToRemove != _target.DiscDatas[i]) {
                    tempDatas[added] = _target.DiscDatas[i];
                    tempDatas[added].Order = added;
                    added++;
                }
            }

            _target.DiscDatas = tempDatas;
            _sliceToRemove = null;
        }

        // If there is no slice
        if (_target.DiscDatas.Length < 1) {
            _target.DiscDatas = new DiscData[1];
        }

        if (_updateFill) {
            _updateFill = false;
            _target.InitDiscMenu(Vector2.zero);
        }

        EditorUtility.SetDirty(_target);
        serializedObject.ApplyModifiedProperties();
    }

    public void AddSlice() {
        _sliceToAdd = true;
        _updateFill = true;
    }

    public void RemoveSlice(DiscData sliceToRemove) {
        _sliceToRemove = sliceToRemove;
        _updateFill = true;
    }
}
