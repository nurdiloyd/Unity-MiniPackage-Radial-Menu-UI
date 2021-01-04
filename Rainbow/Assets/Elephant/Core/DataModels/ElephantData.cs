using System;

namespace ElephantSDK
{
    [Serializable]
    public class ElephantData
    {
        public string data;
        public long current_session_id;
        
        public ElephantData(string data, long sessionId)
        {
            this.current_session_id = sessionId;
            this.data = data;
        }
    }
}