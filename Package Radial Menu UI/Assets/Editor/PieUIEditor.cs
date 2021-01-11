using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PieMenu))]
public class PieUIEditor : Editor
{
    private PieMenu _target;
    
    private System.Action _addSlice;
    private bool _sliceToAdd;

    private System.Action<PieMenu.PieData> _removeSlice;
    private PieMenu.PieData _sliceToRemove = null;
    
    private System.Action<int> _updateAngle;


    private void OnEnable() {
        _target = (PieMenu) target;
        _addSlice = AddSlice;
        _removeSlice = RemoveSlice;
        _updateAngle = UpdateAngle;
    }

    public override void OnInspectorGUI() {
        DrawPropertiesExcluding(serializedObject);

        // Drawing Slices
        _target.PieDatas = _target.PieDatas.ToList().OrderBy(x => x.Order).ToArray();
        for (var i = 0; i < _target.PieDatas.Length; i++) {
            if (_sliceToRemove != _target.PieDatas[i]) {
                _target.PieDatas[i].OnInspectorGUI(_target, _addSlice, _removeSlice, _updateAngle);
            }
        }

        if (_target.PieDatas.Length < 1) {
            _target.PieDatas = new PieMenu.PieData[1];
        }
        
        // Adding a Slice
        if (_sliceToAdd) {
            Undo.RecordObject(_target, "Add Slice");
            PieMenu.PieData[] tempArray = new PieMenu.PieData[_target.PieDatas.Length + 1];
            for (int i = _target.PieDatas.Length; i < tempArray.Length; i++) {
                tempArray[i] = new PieMenu.PieData();
                tempArray[i].SetValues(_target.PieDatas[_target.PieDatas.Length - 1]);
                tempArray[i].Order = i;
            }

            for (int i = 0; i < _target.PieDatas.Length; i++) {
                tempArray[i] = _target.PieDatas[i];
            }

            _target.PieDatas = tempArray;
            _sliceToAdd = false;
        }

        // Removing a Slice
        if (_sliceToRemove != null && _target.PieDatas.Length > 1) {
            Undo.RecordObject(_target, "Removed Slice");
            PieMenu.PieData[] tempArray = new PieMenu.PieData[_target.PieDatas.Length - 1];
            int added = 0;
            for (int i = 0; i < _target.PieDatas.Length; i++) {
                if (_sliceToRemove != _target.PieDatas[i]) {
                    tempArray[added] = _target.PieDatas[i];
                    tempArray[added].Order = added;
                    added++;
                }
            }

            _target.PieDatas = tempArray;
            _sliceToRemove = null;
        }

        serializedObject.ApplyModifiedProperties( );
        
        // Updating angles of slices
        if(SumOfAngles() > 360) {
            for (int i = 0; i < _target.PieDatas.Length; i++) {
                UpdateAngle(i);
            }
        }

        if(SumOfAngles() < 360) {
            _target.PieDatas[_target.PieDatas.Length - 1].Angle = 360 - SumOfAngles( );
        }
    }

    public void AddSlice() {
        _sliceToAdd = true;
    }

    public void RemoveSlice(PieMenu.PieData sliceToRemove) {
        _sliceToRemove = sliceToRemove;
    }
    
    public float SumOfAngles() {
        float sum = 0;
        for(int i = 0; i< _target.PieDatas.Length;i++) {
            sum += Mathf.Abs(_target.PieDatas[i].Angle);
        }

        return sum;
    }

    public void UpdateAngle(int order) {
        float sumBefore = 0;
        for (int i = 0; i <= order; i++) {
            sumBefore += _target.PieDatas[i].Angle;
        }

        float remainder = (360 - sumBefore) / (_target.PieDatas.Length - order - 1);
        for (int i = order + 1; i < _target.PieDatas.Length; i++) {
            _target.PieDatas[i].Angle = remainder;
        }
    }
}