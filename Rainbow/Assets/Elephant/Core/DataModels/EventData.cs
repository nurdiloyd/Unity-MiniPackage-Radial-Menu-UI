using System;

namespace ElephantSDK
{
    [Serializable]
    public class EventData : BaseData 
    {
        public string type;
        public int level;
        
        public string  key_string1;
        public string  value_string1;
        public string  key_string2;
        public string  value_string2;

        public string  key_int1;
        public int  value_int1;
        public string  key_int2;
        public int  value_int2;

        public string  key_double1;
        public double  value_double1;
        public string  key_double2;
        public double  value_double2;
        
        public string custom_data;
        
        private EventData()
        {
            
        }

        public static EventData CreateEventData()
        {
            var a = new EventData();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            return a;
        }
    }
}
