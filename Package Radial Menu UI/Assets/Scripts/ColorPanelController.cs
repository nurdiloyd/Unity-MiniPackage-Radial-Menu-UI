using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPanelController : MonoBehaviour
{
    public Joystick Joystick;

    [SerializeField] protected PiUIManager piUi;

    private GameManager _gameManager;
    private PiUI _pieMenu;
    private bool _isClicked;
    private Color _currentColor;
    private HueColor _currentHue;


    private void Start() {
        _gameManager = FindObjectOfType<GameManager>();
        SetPieMenu();
    }

    private void SetPieMenu() {
        //Get menu for easy not repetitive getting of the menu when setting joystick input
        _pieMenu = piUi.GetPieMenu();
        
        HueData[] hueDatas = _gameManager.CurrentLevel.GetPaletteColors();
        int dataCount = hueDatas.Length;

        //Changes the data length and adds new piData
        _pieMenu.piData = new PiUI.PiData[dataCount];
        for(int j = 0; j < dataCount; j++) {
            _pieMenu.piData[j] = new PiUI.PiData( );
            PiUI.PiData dat = _pieMenu.piData[j];

            dat.isInteractable = true;
            dat.buttonColor = hueDatas[j].ButtonColor;
            dat.materialColor = hueDatas[j].HueMaterial.GetColor("_Albedo");
            dat.hue = hueDatas[j].Hue;
            dat.onSlicePressed = new UnityEngine.Events.UnityAction<HueColor, Color>(Clicked);
        }

        piUi.RegeneratePiMenu();
    }

    private void Update() {
        if (_gameManager.CurrentLevel.LevelCompletedMe) {
            piUi.ClosePieMenu();
            return;
        }

        // Opening 
        if (Input.GetMouseButtonDown(0)) {
            piUi.OpenPieMenu(Joystick.background.position);

            _isClicked = false;
            _gameManager.SlowMotion(true);
            _gameManager.CurrentLevel.ResetCurrentColor();
        }
        // Closing the panel
        else if (Input.GetMouseButtonUp(0)) {
            piUi.ClosePieMenu();

            if (!_isClicked) {
                _gameManager.CurrentLevel.SetCurrentColor(_currentHue, _currentColor);
            }

            _gameManager.SlowMotion(false);
        }

        //Set joystick input on the normal menu which the piPieces check
        _pieMenu.joystickInput = new Vector2(Joystick.Horizontal, Joystick.Vertical);
        //Set the bool to detect if the controller button has been pressed
        _pieMenu.joystickButton = Input.GetMouseButtonDown(0);
        //If the button isnt pressed check if has been released
        if (!_pieMenu.joystickButton) {
            _pieMenu.joystickButton = Input.GetMouseButtonUp(0);
        }
    }

    public void Clicked(HueColor hue, Color matColor) {
        _isClicked = true;

        TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Medium);

        _currentHue = hue;
        _currentColor = matColor;
        _gameManager.CurrentLevel.SetCurrentColor(_currentHue, _currentColor);
    }

    public void OnHoverEnter() {
        Debug.Log("Hey get off of me!");
    }

    public void OnHoverExit() {
        Debug.Log("That's right and dont come back!");
    }
}