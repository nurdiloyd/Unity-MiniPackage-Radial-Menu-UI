using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] protected PiUIManager piUi;

    private bool _isClicked;


/*
    private void SetPieMenu() {
        //Get menu for easy not repetitive getting of the menu when setting joystick input
        _pieMenu = piUi.GetPieMenu();
        
        int dataCount = HueDatas.Length;

        //Changes the data length and adds new piData
        _pieMenu.piData = new PiUI.PiData[dataCount];
        for(int j = 0; j < dataCount; j++) {
            _pieMenu.piData[j] = new PiUI.PiData( );
            PiUI.PiData dat = _pieMenu.piData[j];

            dat.isInteractable = true;
            dat.buttonColor = HueDatas[j].ButtonColor;
            dat.hue = HueDatas[j].Hue;
            dat.onSlicePressed = new UnityEngine.Events.UnityAction<HueColor, Color>(Clicked);
        }

        piUi.RegeneratePiMenu();
    }
*/

    private void Update() {

        // Opening 
        if (Input.GetMouseButtonDown(0)) {
            piUi.OpenPieMenu(Input.mousePosition);

            _isClicked = false;
        }

        // Closing the panel
        else if (Input.GetMouseButtonUp(0)) {
            piUi.ClosePieMenu();

            if (!_isClicked) {
                Debug.Log("Clicked");
            }
        }
    }

    public void Clicked(HueColor hue, Color matColor) {
        _isClicked = true;
    }

    public void OnHoverEnter() {
        Debug.Log("Hey get off of me!");
    }

    public void OnHoverExit() {
        Debug.Log("That's right and dont come back!");
    }
}


public enum HueColor {
    Green, Cyan, Blue, Purple, Pink, Red, Orange, Yellow, SELF
};