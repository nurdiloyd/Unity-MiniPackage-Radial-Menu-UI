using System.Collections.Generic;
using UnityEngine;

namespace ElephantSDK
{
    public class Params
    {
        internal Dictionary<string, string> stringVals = new Dictionary<string, string>();
        internal Dictionary<string, int> intVals = new Dictionary<string, int>();
        internal Dictionary<string, double> doubleVals = new Dictionary<string, double>();
        internal string customData;
        
        private Params()
        {
        }

        public static Params New()
        {
            return new Params();
        }


        public Params Set(string key, string value)
        {
            if (stringVals.Count >= 2)
            {
                Debug.LogError("You cannot set more than 2 string values for event parameters.");
                return this;
            }
            
            stringVals[key] = value;
            return this;
        }
        
        public Params Set(string key, int value)
        {
            if (intVals.Count >= 2)
            {
                Debug.LogError("You cannot set more than 2 string values for event parameters.");
                return this;
            }
            
            intVals[key] = value;
            return this;
        }
        
        
        public Params Set(string key, double value)
        {
            if (doubleVals.Count >= 2)
            {
                Debug.LogError("You cannot set more than 2 string values for event parameters.");
                return this;
            }
            
            doubleVals[key] = value;
            return this;
        }

        public Params CustomString(string data)
        {
            this.customData = data;
            return this;
        }
        
    }
}