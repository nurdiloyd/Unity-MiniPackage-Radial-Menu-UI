using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    class RemoteParameters
    {
        public List<string> keys;
        public List<string> values;

        public RemoteParameters()
        {
            this.keys = new List<string>();
            this.values = new List<string>();
        }
    }

    [Serializable]
    class ConfigResponse
    {
        public string tag;
        public RemoteParameters data;

        public ConfigResponse()
        {
            this.tag = "NO_TAG";
            this.data = new RemoteParameters();
        }
    }

    public class RemoteConfig
    {
        private Dictionary<string, string> variables = new Dictionary<string, string>();
        private static RemoteConfig instance;

        private string userTag = "NOT_TAGGED";

        public static RemoteConfig GetInstance()
        {
            if (instance == null)
            {
                instance = new RemoteConfig();
            }

            return instance;
        }

        private RemoteConfig()
        {
        }
        
        

        public void Init(string jsonText)
        {
            try
            {
                var configData = JsonUtility.FromJson<ConfigResponse>(jsonText);
                var configVariables = configData.data;
                this.userTag = configData.tag;
            
                for (int i = 0; i < configVariables.keys.Count; i++)
                {
                    var k = configVariables.keys[i];
                    var v = configVariables.values[i];
                    variables[k] = v;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }

        public string GetTag()
        {
            return userTag;
        }

        public void SetTag(string tag)
        {
            this.userTag = tag;
        }
        
        public string Get(string key, string def = null)
        {
            if (!variables.ContainsKey(key))
                return def;
            var a = variables[key];
            if (a == null)
                return def;
            return a;
        }

        public int GetInt(string key, int def = 0)
        {
            if (!variables.ContainsKey(key))
                return def;
            var a = variables[key];
            if (a == null)
                return def;

            return int.Parse(a);
        }

        public long GetLong(string key, long def = 0)
        {
            if (!variables.ContainsKey(key))
                return def;
            var a = variables[key];
            if (a == null)
                return def;
            return long.Parse(a);
        }


        public bool GetBool(string key, bool def = false)
        {
            if (!variables.ContainsKey(key))
                return def;
            var a = variables[key];
            if (a == null)
                return def;
            return bool.Parse(a);
        }
    }
}