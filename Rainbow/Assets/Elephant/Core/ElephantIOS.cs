using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ElephantIOS 
{
   
    #if UNITY_IOS
    [DllImport ("__Internal")]
    public static extern void TestFunction(string a);
    
    [DllImport ("__Internal")]
    public static extern string IDFA();


    
    [DllImport ("__Internal")]
    public static extern void ElephantPost(string url, string body, string gameID, string authToken, int tryCount);
    
    #endif
}
