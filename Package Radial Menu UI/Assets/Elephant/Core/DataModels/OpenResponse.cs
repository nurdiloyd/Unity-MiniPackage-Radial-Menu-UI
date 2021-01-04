using System;
using UnityEngine;

namespace ElephantSDK
{
    
    [Serializable]
    public class GDPROptionDetailModel
    {
        public string action;
        public string data;

        public GDPROptionDetailModel()
        {
            this.action = GDPRNavigationButton.GDPRButtonAction.OPEN_PAGE.ToString();
            this.data = "";
        }
    }
    
    
    [Serializable]
    public class OpenResponse
    {
        public bool consent_required;
        public string gdpr_body_text;
        public bool consent_status;
        public string gdpr_option_1;
        public string gdpr_option_2;
        public string gdpr_option_3;
        public GDPROptionDetailModel gdpr_option_1_data;
        public GDPROptionDetailModel gdpr_option_2_data;
        public GDPROptionDetailModel gdpr_option_3_data;
        public string remote_config_json; // json

        public OpenResponse()
        {
            this.consent_required = false;
            this.gdpr_body_text = "";
            this.gdpr_option_1 = "";
            this.gdpr_option_2 = "";
            this.gdpr_option_3 = "";
            this.gdpr_option_1_data = new GDPROptionDetailModel();
            this.gdpr_option_2_data = new GDPROptionDetailModel();
            this.gdpr_option_3_data = new GDPROptionDetailModel();
            this.remote_config_json = JsonUtility.ToJson(new ConfigResponse());
        }
    }
}