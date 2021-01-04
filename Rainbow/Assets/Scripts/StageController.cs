using System;
using System.Collections;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public HueColor[] HueColorsOnStage;
    public event Action<bool, HueColor> DissolveObject;  // To select a building

    [SerializeField] protected Transform road;

    private MeshRenderer[] _boardMeshRenderers;
    private HueColor _currentHue;
    private Color _defaultColor;
     
    
    private void Start() {
        _boardMeshRenderers = road.GetComponentsInChildren<MeshRenderer>();
        _defaultColor = _boardMeshRenderers[0].material.GetColor("_AlbedoA");
    }

    public void SetColor(HueColor hue, Color matColor) {
        foreach (var boardMR in _boardMeshRenderers) {
            StartCoroutine(LerpColor(boardMR, matColor)); 
        }               

        DissolveObject?.Invoke(false, _currentHue);
        DissolveObject?.Invoke(true, hue);
        _currentHue = hue;
    }

    public void ResetColor() {
        foreach (var boardMR in _boardMeshRenderers) {
            StartCoroutine(LerpColor(boardMR, _defaultColor)); 
        }
        DissolveObject?.Invoke(false, _currentHue);
    }

    private IEnumerator LerpColor(MeshRenderer boardMR, Color toColor) {
        boardMR.material.SetColor("_AlbedoB", toColor);

        float transition = 0;
        while (transition != 1) {
            yield return new WaitForSeconds(Time.deltaTime);
            transition = Mathf.MoveTowards(transition, 1, 0.2f);
            boardMR.material.SetFloat("_Transition", transition);
        }

        boardMR.material.SetColor("_AlbedoA", toColor);
        boardMR.material.SetFloat("_Transition", 0);
    }
}