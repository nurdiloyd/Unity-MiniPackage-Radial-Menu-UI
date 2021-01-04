using System;

namespace ElephantSDK
{
    [Serializable]
    public class SessionData : BaseData
    {
        public long start_time;
        public long end_time;

        private SessionData()
        {
            this.start_time = Utils.Timestamp();
        }

        public long GetSessionID()
        {
            return start_time;
        }

        public static SessionData CreateSessionData()
        {
            var a = new SessionData();
            a.FillBaseData(a.GetSessionID());
            return a;
        }
    }
}