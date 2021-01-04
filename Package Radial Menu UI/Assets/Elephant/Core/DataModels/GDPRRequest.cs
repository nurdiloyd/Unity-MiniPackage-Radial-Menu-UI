namespace ElephantSDK
{
    public class GDPRRequest : BaseData
    {
        public bool gdpr_ad_consent;
        public bool gdpr_analytics_consent;
        public bool gdpr_privacy_consent;

        private GDPRRequest()
        {
            
        }

        public static GDPRRequest CreateGdprRequest(string idfa, string idfv)
        {
            var a = new GDPRRequest();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            a.idfa = idfa;
            a.idfv = idfv;
            a.gdpr_ad_consent = true;
            a.gdpr_analytics_consent = true;
            a.gdpr_privacy_consent = true;
            return a;
        }

      
    }
}