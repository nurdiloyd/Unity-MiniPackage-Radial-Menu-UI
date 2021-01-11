using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] protected PieMenu pieMenu;


    private void Start() {
        SetPieMenu();
    }

    private void SetPieMenu() {
        int dataCount = pieMenu.PieDatas.Length;
        for(int i = 0; i < dataCount; i++) {
            PieMenu.PieData dat = pieMenu.PieDatas[i];
            dat.OnPressed += Clicked;
        }

        pieMenu.UpdatePiUI();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {      // Opening 
            pieMenu.OpenMenu(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0)) {   // Closing
            pieMenu.CloseMenu();
        }
    }

    public void Clicked(int value) {
        Debug.Log("Clicked" + value);
    }

    public void OnHoverEnter() {
        Debug.Log("Hey get off of me!");
    }

    public void OnHoverExit() {
        Debug.Log("That's right and dont come back!");
    }
}