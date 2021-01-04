using System;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class ElephantRequest
    {
        public string url;
        public string data;
        public int tryCount;
        public long lastTryTS;
       
        
        public ElephantRequest(string url, BaseData data)
        {
            this.url = url;
            this.data = JsonUtility.ToJson(data);
        }
        
        
        
    }
    
}