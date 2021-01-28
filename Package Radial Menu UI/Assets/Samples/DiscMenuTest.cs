using UnityEngine;

public class DiscMenuTest : MonoBehaviour
{
    [SerializeField] protected DiscMenu discMenu;


    private void Start() {
        discMenu.InitDiscMenu(Vector2.one * -1000);
        discMenu.ClickEvent = new UnityEngine.Events.UnityAction<DiscData>(Clicked);
        discMenu.CloseMenu();
    }

    private void Update() {
        // Opening
        if (Input.GetMouseButtonDown(0)) { 
            discMenu.OpenMenu(Vector2.zero);
        }
        // Closing
        if (Input.GetMouseButtonUp(0)) {
            discMenu.CloseMenu();
        }
    }

    public void Clicked(DiscData data) {
        Debug.Log(data.Name + " Selected");
    }
}