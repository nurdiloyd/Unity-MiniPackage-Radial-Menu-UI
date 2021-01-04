using ElephantSDK;
using UnityEngine;


public class ElephantSDKExample : MonoBehaviour
{
    public GameObject[] objects;

    private int currentLevel = 1;
    
    void Start()
    {
        LoadLevel(currentLevel);
    }


    private void LoadLevel(int level)
    {
        currentLevel = level;
        
        // send level started event to Elephant
        Elephant.LevelStarted(currentLevel);
        
        // elephant remote config parameter to customize in game stuff remotely..
        string objName = RemoteConfig.GetInstance().Get("object", "Cube");
        int c = this.transform.childCount;
        for (int i = 0; i < c; ++i)
        {
            GameObject g = transform.GetChild(i).gameObject;
            if (g.name.Equals(objName))
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
        
        
        // SDK event with some parameters
        Params param2 = Params.New()
            .Set("gems", 10)
            .Set("source", "level_reward")
            .Set("some_double", 3.141592);

        Elephant.Event("custom_reward_event2", currentLevel, param2);
        
        // SDK transaction for currencies (consumable user properties)
        Elephant.Transaction("gem", currentLevel, -10, 90, "skin_unlock");

    }

    private void CompleteLevel()
    {
        // finish current level with success
        Elephant.LevelCompleted(currentLevel);
        
        // load the next one
        LoadLevel(currentLevel + 1);
        
    }
    
    private void FailLevel()
    {
        // finish current level with fail
        Elephant.LevelFailed(currentLevel);
        
        // restart the level
        LoadLevel(currentLevel);
        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            CompleteLevel();
        }else if (Input.GetKeyUp(KeyCode.Backspace))
        {
            FailLevel();
        }
    }
}
