using System;
using System.Collections.Generic;

namespace ElephantSDK
{
    [Serializable]
    public class QueueData
    {
        public List<ElephantRequest> queue;

        public QueueData(List<ElephantRequest> q)
        {
            this.queue = q;
        }
    }
}