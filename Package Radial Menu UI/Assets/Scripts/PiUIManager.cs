using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiUIManager : MonoBehaviour
{
    public PiUI PieMenu;


    private void Awake() {
		transform.localScale = new Vector3(1f / Screen.width, 1f / Screen.height);
		transform.position = Vector2.zero;
    }

    public void OpenPieMenu(Vector2 position = default(Vector2)) {
        PieMenu.OpenMenu(position);
    }

    public void ClosePieMenu() {
        PieMenu.CloseMenu();
    }
   
    public bool PiOpened() {
        return PieMenu.openedMenu;
    }

    public PiUI GetPieMenu() {
        return PieMenu;
    }

    /// After changing the PiUI.sliceCount value and piData data,call this function
    public void RegeneratePiMenu(Vector2 newPos = default(Vector2)) {
        PieMenu.GeneratePi(newPos);
    }

    /// After changing the PiUI.PiData call this function to update the slices
    public void UpdatePiMenu(string menuName) {
        PieMenu.UpdatePiUI();
    }

    public bool OverAMenu() {
        if (PieMenu.overMenu) {
            return true;
        }
        return false;
    }
}
