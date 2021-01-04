using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantAndroid
{
    #if UNITY_ANDROID
    
    private static AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    private static AndroidJavaObject elephantController;
    
    public static void Init()
    {
        try
        {
            AndroidJavaClass elephantCoreClass = new AndroidJavaClass("com.rollic.elephantsdk.ElephantController");
            ElephantAndroid.elephantController = elephantCoreClass.CallStatic<AndroidJavaObject>("create", currentActivity);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
    }
    
    

    public static void ElephantPost(string url, string body, string gameID, string authToken, int tryCount)
    {
        if (elephantController != null)
        {
            elephantController.Call("ElephantPost", url, body, gameID, authToken, tryCount);    
        }
    }
    
    
    #endif
    
}
