using UnityEngine;
using Random = UnityEngine.Random;

namespace ElephantSDK
{
    public class ElephantSDKCodeExamples : MonoBehaviour
    {
        void Start()
        {
            // listen Elephant SDK lifecycle events..
            ElephantCore.onInitialized += OnInitialized;
            ElephantCore.onOpen += OnOpen;
            ElephantCore.onRemoteConfigLoaded += OnRemoteConfigLoaded;

            // detect is this user new to the Elephant SDK
            bool isOldUser = false;


            // initialize Elephant SDK
            Elephant.Init(isOldUser);
        }

        void OnInitialized()
        {
            Debug.Log("Elephant SDK Initialized");
        }

        void OnOpen(bool gdprRequired)
        {
            Debug.Log("Elephant Open Result, we can start the game or show gdpr -> " + gdprRequired);
            if (gdprRequired)
            {
                // show gdpr UI
            }
            else
            {
                // start the game
            }
        }

        void OnRemoteConfigLoaded()
        {
            Debug.Log(
                "Elephant Remote Config Loaded, we can retrieve configuration params via RemoteConfig.GetInstance().Get() or other variant methods..");

            // Ex:  RemoteConfig.GetInstance().Get("test", "default");

            ExampleSDKUsage();
        }


        void ExampleSDKUsage()
        {
            int currentLevel = 1;

            // make sure you send this  at the start of the each level
            Elephant.LevelStarted(currentLevel);

            // basic SDK event
            Elephant.Event("user_clicked_something", currentLevel);


            // SDK event with parameter
            Elephant.Event("custom_reward_event", currentLevel, Params.New().Set("gold", 1000));

            
            // SDK transaction for currencies (consumable user properties)
            Elephant.Transaction("gem", currentLevel, -10, 90, "skin_unlock");


            // SDK event with some parameters
            Params param2 = Params.New()
                .Set("gems", 10)
                .Set("source", "level_reward")
                .Set("some_double", 3.141592).CustomString("{\"t\": 1}");
            

            Elephant.Event("custom_reward_event2", currentLevel, param2);


            // send level completed or failed event
            if (LevelPassed())
                Elephant.LevelCompleted(currentLevel);
            else
                Elephant.LevelFailed(currentLevel);


            // use some remote config parameters to change the game remotely ( useful for A\B testing )
            string someString = RemoteConfig.GetInstance().Get("test_string", "default");
            int someInt = RemoteConfig.GetInstance().GetInt("test_int", 0);
            long someLong = RemoteConfig.GetInstance().GetLong("test_long", 1L);

            
            
        }

        bool LevelPassed()
        {
            return Random.Range(0, 2) == 1;
        }
    }
}