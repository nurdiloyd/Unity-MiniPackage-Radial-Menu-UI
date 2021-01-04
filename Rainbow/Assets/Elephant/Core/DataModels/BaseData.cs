using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class BaseData
    {
#if UNITY_IOS
        public string platform = "ios";
#elif UNITY_ANDROID
        public string platform = "android";
#else
        public string platform = "editor";
#endif

        public string idfa;
        public string idfv;
        public string bundle;
        public string lang;
        public string app_version;
        public string os_version;
        public string sdk_version;
        public string device_model;
        public string user_tag;
        public long create_date;
        public long session_id;
        public string timezone_offset = "";

        public void FillBaseData(long sessionID)
        {
                this.bundle = Application.identifier;
                this.idfa = ElephantCore.Instance.idfa;
                this.idfv = ElephantCore.Instance.idfv;
                this.app_version = Application.version;
                this.lang = Utils.GetISOCODE(Application.systemLanguage);
                this.user_tag = RemoteConfig.GetInstance().GetTag();
                this.os_version = SystemInfo.operatingSystem;
                this.sdk_version = ElephantVersion.SDK_VERSION;
                this.device_model = SystemInfo.deviceModel;
                this.create_date = Utils.Timestamp();
                this.session_id = sessionID;

                try
                {
                        TimeZone localZone = TimeZone.CurrentTimeZone;
                        DateTime currentDate = DateTime.Now;
                        TimeSpan currentOffset = 
                                localZone.GetUtcOffset( currentDate );
                        this.timezone_offset = currentOffset.ToString();
                }
                catch (Exception e)
                {
                        Debug.Log(e);
                }
                
        }


        
    }
}