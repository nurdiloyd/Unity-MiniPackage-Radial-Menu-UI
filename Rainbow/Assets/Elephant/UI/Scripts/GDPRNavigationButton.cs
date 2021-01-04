using ElephantSDK;
using UnityEngine;

public class GDPRNavigationButton : MonoBehaviour
{

    public enum GDPRButtonAction
    {
        PLAY,
        OPEN_PAGE,
        OPEN_URL
    }

    public GDPRButtonAction action;

    public GameObject currentPage;
    public GameObject page;

    public string url = "https://rollicgames.com/privacy";

    private void DisableAllPages()
    {
        if(currentPage != null)
            currentPage.SetActive(false);
        
    }

    public void OnClick()
    {
        switch (action)
        {
            case GDPRButtonAction.PLAY:
                ElephantUI.Instance.AcceptGDPR();
                ElephantUI.Instance.PlayGame();
                break;
            case GDPRButtonAction.OPEN_PAGE:
                DisableAllPages();
                page.SetActive(true);
                break;
            case GDPRButtonAction.OPEN_URL:
                Application.OpenURL(url);
                break;
           
        }
    }
    
   
}
