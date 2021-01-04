using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ElephantSDK
{
    public delegate void OnInitialized();

    public delegate void OnOpenResult(bool gdprRequired);

    public delegate void OnRemoteConfigLoaded();

    public class ElephantCore : MonoBehaviour
    {
        private string GameID = "";
        private string GameSecret = "";


        private string defaultGameID = "";
        private string defaultGameSecret = "";


        public static ElephantCore Instance = null;

#if ELEPHANT_DEBUG
        private static bool debug = true;
        private const string ELEPHANT_BASE_URL = "http://localhost:3100/v2";
#elif UNITY_EDITOR
        private static bool debug = true;
        private const string ELEPHANT_BASE_URL = "https://newapi.rollic.gs/v2";
#else
        private static bool debug = true;
        private const string ELEPHANT_BASE_URL = "https://newapi.rollic.gs/v2";
#endif

        public const string OPEN_EP = ELEPHANT_BASE_URL + "/open";
        public const string GDPR_EP = ELEPHANT_BASE_URL + "/gdpr";
        public const string EVENT_EP = ELEPHANT_BASE_URL + "/event";
        public const string SESSION_EP = ELEPHANT_BASE_URL + "/session";
        public const string TRANSACTION_EP = ELEPHANT_BASE_URL + "/transaction";


        private Queue<ElephantRequest> _queue = new Queue<ElephantRequest>();
        private List<ElephantRequest> _failedQueue = new List<ElephantRequest>();
        private bool processQueues = false;

        private static string QUEUE_DATA_FILE = "ELEPHANT_DATA_QUEUE_";


        private bool sdkIsReady = false;

        private bool openRequestWaiting;
        private SessionData currentSession;
        internal string idfa = "";
        internal string idfv = "";
        private OpenResponse openResponse;

        private static int MAX_FAILED_COUNT = 100;

        private static string OLD_ELEPHANT_FILE = "elephant.json";
#if ELEPHANT_DEBUG
        private static string REMOTE_CONFIG_FILE = "ELEPHANT_REMOTE_CONFIG_DATA_6";
#else
        private static string REMOTE_CONFIG_FILE = "ELEPHANT_REMOTE_CONFIG_DATA";
#endif


        internal bool gdprSupported = false;


        public static event OnInitialized onInitialized;
        public static event OnOpenResult onOpen;
        public static event OnRemoteConfigLoaded onRemoteConfigLoaded;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            RebuildQueue();
            processQueues = true;

#if !UNITY_EDITOR && UNITY_ANDROID
            ElephantAndroid.Init();
#endif
        }

        public void Init(bool isOldUser, bool gdprSupported)
        {
            var settings = Resources.Load<ElephantSettings>("ElephantSettings");

            if (settings == null)
            {
                Debug.LogError(
                    "[Elephant SDK]  Elephant SDK settings isn't setup, use Window -> Elephant -> Edit Settings to enter your Game ID and Game Secret");
            }
            else
            {
                this.GameID = settings.GameID;
                this.GameSecret = settings.GameSecret;
            }


            if (GameID.Equals(defaultGameID) || GameSecret.Equals(defaultGameSecret) || GameID.Trim().Length == 0 ||
                GameSecret.Trim().Length == 0)
            {
                Debug.LogError(
                    "[Elephant SDK]  Game ID and Game Secret are not present, make sure you replace them with yours using Window -> Elephant -> Edit Settings");
            }

            this.gdprSupported = gdprSupported;
            StartCoroutine(InitSDK(isOldUser));
        }

        private IEnumerator InitSDK(bool isOldUser)
        {
            if (Utils.IsFileExists(OLD_ELEPHANT_FILE))
            {
                isOldUser = true;
            }

            string savedConfig = Utils.ReadFromFile(REMOTE_CONFIG_FILE);

            Log("Remote Config From File --> " + savedConfig);

            openResponse = new OpenResponse();

            if (savedConfig != null)
            {
                RemoteConfig.GetInstance().Init(savedConfig);
                openResponse.remote_config_json = savedConfig;
            }

            openRequestWaiting = true;

            float startTime = Time.time;

            RequestIDFAAndOpen(isOldUser);

            while (openRequestWaiting && (Time.time - startTime) < 5f)
            {
                yield return null;
            }

            Log(JsonUtility.ToJson(openResponse));

            RemoteConfig.GetInstance().Init(openResponse.remote_config_json);
            Utils.SaveToFile(REMOTE_CONFIG_FILE, openResponse.remote_config_json);
            currentSession.user_tag = RemoteConfig.GetInstance().GetTag();

            if (onOpen != null)
            {
                if (openResponse.consent_required)
                {
                    onOpen(true);
                }
                else
                {
                    onOpen(false);
                }
            }
            else
            {
                Debug.LogWarning("ElephantSDK onOpen event is not handled");
            }

            sdkIsReady = true;
            if (onRemoteConfigLoaded != null)
                onRemoteConfigLoaded();
        }


        public OpenResponse GetOpenResponse()
        {
            return openResponse;
        }

        private void RequestIDFAAndOpen(bool isOldUser)
        {
            idfv = SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
            idfa = "UNITY_EDITOR_IDFA";
            StartCoroutine(OpenRequest(isOldUser));
#elif UNITY_IOS
            idfa = ElephantIOS.IDFA();
            Debug.Log("Native IDFA -> " + idfa);
            StartCoroutine(OpenRequest(isOldUser));
#else
            Application.RequestAdvertisingIdentifierAsync(
                (string advertisingId, bool trackingEnabled, string error) =>
                {

                    if (!trackingEnabled)
                    {
                        advertisingId = "";
                    }

                    idfa = advertisingId;
                    
                   Debug.Log("IDFA -> " + idfa);

                    StartCoroutine(OpenRequest(isOldUser));
                }
            );
#endif
        }

        private IEnumerator OpenRequest(bool isOldUser)
        {
            // initialized event
            if (onInitialized != null)
                onInitialized();


            currentSession = SessionData.CreateSessionData();

            var openData = OpenData.CreateOpenData();
            openData.is_old_user = isOldUser;
            openData.gdpr_supported = gdprSupported;
            openData.session_id = currentSession.GetSessionID();

            var json = JsonUtility.ToJson(openData);
            var bodyJson = JsonUtility.ToJson(new ElephantData(json, GetCurrentSession().GetSessionID()));
            yield return PostWithResponse(OPEN_EP, bodyJson);
        }

        IEnumerator PostWithResponse(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.SetRequestHeader("Authorization", Utils.SignString(bodyJsonString, GameSecret));
            request.SetRequestHeader("GameID", GameID);

            yield return request.SendWebRequest();

            Log("Status Code: " + request.responseCode);
            Log("Body: " + request.downloadHandler.text);

            if (request.isNetworkError)
            {
                Log("Request Error");
            }
            else
            {
                if (request.responseCode == 200)
                {
                    try
                    {
                        var a = JsonUtility.FromJson<OpenResponse>(request.downloadHandler.text);
                        if (a != null)
                        {
                            openResponse = a;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }


            openRequestWaiting = false;
        }


        public bool UserGDPRConsent()
        {
            return this.openResponse.consent_status;
        }

        public SessionData GetCurrentSession()
        {
            return currentSession;
        }

        public void AddToQueue(ElephantRequest data)
        {
            this._queue.Enqueue(data);
        }


        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                currentSession = SessionData.CreateSessionData();

                Log("Focus Gained");
                // rebuild queues from disk..
                RebuildQueue();

                // start queue processing
                processQueues = true;
            }
            else
            {
                Log("Focus Lost");
                // pause late update
                processQueues = false;

                // send session log
                var currentSession = ElephantCore.Instance.GetCurrentSession();
                currentSession.end_time = Utils.Timestamp();

                var sessionReq = new ElephantRequest(SESSION_EP, currentSession);
                AddToQueue(sessionReq);

                // process queues
                ProcessQueues(true);

                // drain queues and persist them to send after gaining focus
                SaveQueues();
            }
        }

        private void RebuildQueue()
        {
            string json = Utils.ReadFromFile(QUEUE_DATA_FILE);
            if (json != null)
            {
                Log("QUEUE <- " + json);
                var d = JsonUtility.FromJson<QueueData>(json);
                if (d.queue != null)
                {
                    _failedQueue = d.queue;
                    foreach (var r in _failedQueue)
                    {
                        r.tryCount = 0;
                    }
                }
            }
        }

        private void SaveQueues()
        {
            while (_queue.Count > 0)
            {
                ElephantRequest data = _queue.Dequeue();
                _failedQueue.Add(data);
            }

            var queueJson = JsonUtility.ToJson(new QueueData(_failedQueue));
            Log("QUEUE -> " + queueJson);

            Utils.SaveToFile(QUEUE_DATA_FILE, queueJson);

            _failedQueue.Clear();
        }

        private void LateUpdate()
        {
            ProcessQueues(false);
        }


        private void ProcessQueues(bool forceToSend)
        {
            if (forceToSend || (processQueues && sdkIsReady))
            {
                int failedCount = _failedQueue.Count;
                for (int i = failedCount - 1; i >= 0; --i)
                {
                    ElephantRequest data = _failedQueue[i];
                    int tc = data.tryCount % 6;
                    int backoff = (int) (Math.Pow(2, tc) * 1000);

                    if (Utils.Timestamp() - data.lastTryTS > backoff)
                    {
                        _failedQueue.RemoveAt(i);
                        StartCoroutine(Post(data));
                    }
                }

                while (_queue.Count > 0)
                {
                    ElephantRequest data = _queue.Dequeue();
                    StartCoroutine(Post(data));
                }
            }
        }

        IEnumerator Post(ElephantRequest elephantRequest)
        {
            Log(elephantRequest.tryCount + " - " + (Utils.Timestamp() - elephantRequest.lastTryTS) + " -> " +
                elephantRequest.url + " : " + elephantRequest.data);

            elephantRequest.tryCount++;
            elephantRequest.lastTryTS = Utils.Timestamp();

            var elephantData = new ElephantData(elephantRequest.data, GetCurrentSession().GetSessionID());

            string bodyJsonString = JsonUtility.ToJson(elephantData);

            string authToken = Utils.SignString(bodyJsonString, GameSecret);


#if UNITY_EDITOR

            var request = new UnityWebRequest(elephantRequest.url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.SetRequestHeader("Authorization", authToken);
            request.SetRequestHeader("GameID", ElephantCore.Instance.GameID);

            yield return request.SendWebRequest();

            Log("Status Code: " + request.responseCode);

            if (request.responseCode != 200)
            {
                // failed will be retried
                if (_failedQueue.Count < MAX_FAILED_COUNT)
                {
                    _failedQueue.Add(elephantRequest);
                }
                else
                {
                    Log("Failed Queue size -> " + _failedQueue.Count);
                }
            }

#else
#if UNITY_IOS
            ElephantIOS.ElephantPost(elephantRequest.url, bodyJsonString, GameID, authToken, elephantRequest.tryCount);
#elif UNITY_ANDROID
            ElephantAndroid.ElephantPost(elephantRequest.url, bodyJsonString, GameID, authToken, elephantRequest.tryCount);
#endif
            yield return null;
#endif
        }

        public void FailedRequest(string reqJson)
        {
            try
            {
                var req = JsonUtility.FromJson<ElephantRequest>(reqJson);
                req.lastTryTS = Utils.Timestamp();

                // trick..
                var body = JsonUtility.FromJson<ElephantData>(req.data);
                req.data = body.data;

//                Log("Request Failed -> " + reqJson);
//                Log("Request Failed Parsed -> " + req.tryCount + " => " + req.url + " - " + req.data);

                if (_failedQueue.Count < MAX_FAILED_COUNT)
                {
                    _failedQueue.Add(req);
                }
                else
                {
                    Log("Failed Queue size -> " + _failedQueue.Count);
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        public static void Log(string s)
        {
            if (debug)
            {
                Debug.Log(s);
            }
        }

        public void AcceptGDPR()
        {
            openResponse.consent_required = false;
            openResponse.consent_status = true;
        }
    }
}