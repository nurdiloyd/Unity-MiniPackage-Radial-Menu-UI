using UnityEngine;

public class DisMenuController : MonoBehaviour
{
    [SerializeField] protected DiscMenu discMenu;


    private void Update() {
        // Opening
        if (Input.GetMouseButtonDown(0)) { 
            discMenu.OpenMenu(Input.mousePosition);
        }
        // Closing
        if (Input.GetMouseButtonUp(0)) {
            discMenu.CloseMenu();
        }
    }
}