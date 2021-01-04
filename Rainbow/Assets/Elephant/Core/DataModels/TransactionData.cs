using System;

namespace ElephantSDK
{
    [Serializable]
    public class TransactionData : BaseData
    {
        public string type;
        public long amount;
        public long final_amount;
        public string source;
        public int level;
        
        private TransactionData()
        {
        }
        
        public static TransactionData CreateTransactionData()
        {
            var a = new TransactionData();
            a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
            return a;
        }
    }
}