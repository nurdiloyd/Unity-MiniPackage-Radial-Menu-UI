using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElephantSDK
{
    public class ElephantUI : MonoBehaviour
    {
        private GameObject loaderUI;
        private GameObject gdprUI;

        private List<ToggleController> toggles = new List<ToggleController>();
        private Button playBtn;

        public static ElephantUI Instance;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Application.targetFrameRate = 60;
            Init();
        }


        public void Init()
        {
            loaderUI = GameObject.Find("loader");

            ShowLoaderUI();

            ElephantCore.onInitialized += OnInitialized;
            ElephantCore.onOpen += OnOpen;
            ElephantCore.onRemoteConfigLoaded += OnRemoteConfigLoaded;

            bool isOldUser = false;
            Elephant.Init(isOldUser, true);
        }

        void OnInitialized()
        {
            Debug.Log("Elephant Initialized");
        }

        void OnOpen(bool gdprRequired)
        {
            Debug.Log("Elephant Open Result, we can start the game or show gdpr -> " + gdprRequired);
            if (gdprRequired)
            {
                ShowGDPRUI();
            }
            else
            {
#if ELEPHANT_DEBUG
                PlayGame();
#else
                PlayGame();
#endif
            }
        }

        void OnRemoteConfigLoaded()
        {
            Debug.Log(
                "Elephant Remote Config Loaded, we can retrieve configuration params via RemoteConfig.GetInstance().Get() or other variant methods..");
        }


        private void ShowLoaderUI()
        {
            if (gdprUI != null)
                gdprUI.SetActive(false);
            loaderUI.SetActive(true);
        }

        private void ShowGDPRUI()
        {
            SetupUI();

            loaderUI.SetActive(false);
            if (gdprUI != null)
                gdprUI.SetActive(true);
        }

        public void PlayGame()
        {
//#if !ELEPHANT_DEBUG
            SceneManager.LoadScene(1);
//#endif
        }

        public void AcceptGDPR()
        {
            ElephantCore.Instance.AcceptGDPR();
            var data = GDPRRequest.CreateGdprRequest(ElephantCore.Instance.idfa, ElephantCore.Instance.idfv);
            ElephantRequest req = new ElephantRequest(ElephantCore.GDPR_EP, data);
            ElephantCore.Instance.AddToQueue(req);
        }

        private void SetupUI()
        {
            var openResponse = ElephantCore.Instance.GetOpenResponse();

            // create event system

            GameObject eventSystemObj = new GameObject("EventSystem");
            var eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystem.sendNavigationEvents = true;
            eventSystem.pixelDragThreshold = 10;


            var inputModule = eventSystemObj.AddComponent<StandaloneInputModule>();
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
            inputModule.inputActionsPerSecond = 10;
            inputModule.repeatDelay = 0.5f;


            //create canvas

            GameObject newCanvas = new GameObject("ElephantCanvas");
            Canvas c = newCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.pixelPerfect = true;
            newCanvas.AddComponent<CanvasScaler>();
            newCanvas.AddComponent<GraphicRaycaster>();


            // add gdpr
            GameObject gdprPanel = new GameObject("gdpr");
            this.gdprUI = gdprPanel;
            gdprPanel.transform.SetParent(newCanvas.transform);
            gdprPanel.AddComponent<CanvasRenderer>();
            Image i = gdprPanel.AddComponent<Image>();
            i.color = new Color(255, 255, 255, 255);

            ResetRectTrans(gdprPanel);

            // add main page
            GameObject mainPanel = new GameObject("main");
            mainPanel.AddComponent<CanvasRenderer>();
            mainPanel.transform.SetParent(gdprPanel.transform);
            Image i2 = mainPanel.AddComponent<Image>();
            i2.color = new Color(255, 255, 255, 255);

            ResetRectTrans(mainPanel);


            var scrollView = CreateScrollView(openResponse.gdpr_body_text);

            scrollView.transform.SetParent(mainPanel.transform);
            RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.anchorMin = new Vector2(0f, 0.5f);
            scrollViewRect.anchorMax = new Vector2(1f, 1f);
            scrollViewRect.offsetMin = new Vector2(0.0f, 0.0f);
            scrollViewRect.offsetMax = new Vector2(0.0f, 0.0f);

            string[] pageMainTexts = new string[]
            {
                openResponse.gdpr_option_1_data.data,
                openResponse.gdpr_option_2_data.data,
                openResponse.gdpr_option_3_data.data
            };

            string[] actions = new string[]
            {
                openResponse.gdpr_option_1_data.action,
                openResponse.gdpr_option_2_data.action,
                openResponse.gdpr_option_3_data.action
            };

            string[] urls = new string[]
            {
                openResponse.gdpr_option_1_data.data,
                openResponse.gdpr_option_2_data.data,
                openResponse.gdpr_option_3_data.data
            };

            string[] optionsText = new string[]
            {
                openResponse.gdpr_option_1,
                openResponse.gdpr_option_2,
                openResponse.gdpr_option_3,
            };

            //  add options
            float optionSize = 0.125f;
            float curY = 0.375f;
            for (int j = 0; j < 3; ++j)
            {
                // create pages
                var page = CreatePage(gdprPanel, "page" + (j + 1), mainPanel, pageMainTexts[j], actions[j], urls[j]);
                ResetRectTrans(page);

                CreateOption(mainPanel, "option_" + (j + 1), curY, optionSize, page, optionsText[j], actions[j],
                    urls[j]);
                curY -= optionSize + 0.001f;

                page.SetActive(false);
            }

            // play button
            CreatePlayButton(mainPanel);


            // hide UI
//            this.gdprUI.SetActive(false);
        }


        private GameObject CreatePage(GameObject parent, string name, GameObject main, string mainText, string action,
            string url)
        {
            GameObject pagePanel = new GameObject(name);
            pagePanel.transform.SetParent(parent.transform);
            pagePanel.AddComponent<CanvasRenderer>();
            pagePanel.AddComponent<RectTransform>();


            var scrollView = CreateScrollView(mainText);

            scrollView.transform.SetParent(pagePanel.transform);
            RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.anchorMin = new Vector2(0f, 0.2f);
            scrollViewRect.anchorMax = new Vector2(1f, 1f);
            scrollViewRect.offsetMin = new Vector2(0.0f, 0.0f);
            scrollViewRect.offsetMax = new Vector2(0.0f, 0.0f);


            GameObject playBtn = new GameObject(name + "_back");
            playBtn.transform.SetParent(pagePanel.transform);
            var playBtnImg = playBtn.AddComponent<Image>();

            playBtnImg.sprite =
                Resources.Load<Sprite>(
                    "elephant_roundbox"); // AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            playBtnImg.color = new Color(120 / 255.0f, 85 / 255.0f, 246 / 255.0f);
            playBtnImg.type = Image.Type.Sliced;
            playBtnImg.fillCenter = true;


            var playBtnSc = playBtn.AddComponent<Button>();
            playBtnSc.interactable = true;

            playBtn.AddComponent<ToggleButton>();

            var playBtnRect = playBtn.GetComponent<RectTransform>();
            playBtnRect.anchorMin = new Vector2(0.32f, 0.02f);
            playBtnRect.anchorMax = new Vector2(0.73f, 0.114f);
            playBtnRect.offsetMin = new Vector2(0f, 0.0f);
            playBtnRect.offsetMax = new Vector2(0f, 0.0f);

            var actionScript = playBtn.AddComponent<GDPRNavigationButton>();
            actionScript.action = GetActionFromString(action);
            actionScript.page = main;
            actionScript.currentPage = pagePanel;
            actionScript.url = url;

            playBtnSc.onClick.AddListener(() => { actionScript.OnClick(); });

            // button text

            GameObject playTxt = new GameObject("text");
            playTxt.AddComponent<CanvasRenderer>();
            playTxt.transform.SetParent(playBtn.transform);
            var text = playTxt.AddComponent<Text>();
            text.font = Resources.Load<Font>("ProximaNova-Bold");
            text.text = "BACK";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.supportRichText = true;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 60;

            ResetRectTrans(playTxt);

            return pagePanel;
        }


        private void CreatePlayButton(GameObject parent)
        {
            GameObject playBtn = new GameObject("play");
            playBtn.transform.SetParent(parent.transform);
            var playBtnImg = playBtn.AddComponent<Image>();

            playBtnImg.sprite =
                Resources.Load<Sprite>(
                    "elephant_roundbox"); // AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            playBtnImg.color = new Color(120 / 255.0f, 85 / 255.0f, 246 / 255.0f);
            playBtnImg.type = Image.Type.Sliced;
            playBtnImg.fillCenter = true;


            var playBtnSc = playBtn.AddComponent<Button>();
            playBtnSc.interactable = true;

            playBtn.AddComponent<ToggleButton>();

            var playBtnRect = playBtn.GetComponent<RectTransform>();
            playBtnRect.anchorMin = new Vector2(0.32f, 0.02f);
            playBtnRect.anchorMax = new Vector2(0.73f, 0.114f);
            playBtnRect.offsetMin = new Vector2(0f, 0.0f);
            playBtnRect.offsetMax = new Vector2(0f, 0.0f);

            var actionScript = playBtn.AddComponent<GDPRNavigationButton>();
            actionScript.action = GDPRNavigationButton.GDPRButtonAction.PLAY;


            playBtnSc.onClick.AddListener(() => { actionScript.OnClick(); });


            this.playBtn = playBtnSc;
            this.playBtn.interactable = false;
            // button text

            GameObject playTxt = new GameObject("text");
            playTxt.AddComponent<CanvasRenderer>();
            playTxt.transform.SetParent(playBtn.transform);
            var text = playTxt.AddComponent<Text>();
            text.font = Resources.Load<Font>("ProximaNova-Bold");
            text.text = "PLAY";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.supportRichText = true;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 60;


            ResetRectTrans(playTxt);
        }

        private void CreateOption(GameObject parent, string name, float yPos, float size, GameObject refPage,
            string optionContentString, string openAction, string openURL)
        {
            GameObject optionCon = new GameObject(name);
            optionCon.transform.SetParent(parent.transform);
            optionCon.AddComponent<CanvasRenderer>();
            Image oBgImg = optionCon.AddComponent<Image>();
            oBgImg.type = Image.Type.Sliced;
            oBgImg.sprite =
                Resources.Load<Sprite>(
                    "elephant_roundbox"); // AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            oBgImg.color = new Color(207 / 255.0f, 207 / 255.0f, 207 / 255.0f);
            var optionConRect = optionCon.GetComponent<RectTransform>();

            optionConRect.anchorMin = new Vector2(0f, yPos);
            optionConRect.anchorMax = new Vector2(1f, yPos + size);
            optionConRect.offsetMin = new Vector2(0f, 0.0f);
            optionConRect.offsetMax = new Vector2(0f, 0.0f);

            // toggle
            var toggle = CreateToggle();
            toggle.transform.SetParent(optionCon.transform);

            var toggleRect = toggle.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.05f, 0.1f);
            toggleRect.anchorMax = new Vector2(0.25f, 0.9f);
            toggleRect.offsetMin = new Vector2(0f, 0.0f);
            toggleRect.offsetMax = new Vector2(0f, 0.0f);

            // text
            GameObject optionText = new GameObject(name + "_text");
            optionText.transform.SetParent(optionCon.transform);

            optionText.AddComponent<CanvasRenderer>();
            var text = optionText.AddComponent<Text>();
            text.font = Resources.Load<Font>("ProximaNova-Regular");
            text.text = optionContentString;

            text.supportRichText = true;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 80;
            text.color = Color.black;

            var optionTextRect = optionText.GetComponent<RectTransform>();
            optionTextRect.anchorMin = new Vector2(0.30f, 0.25f);
            optionTextRect.anchorMax = new Vector2(0.85f, 0.75f);
            optionTextRect.offsetMin = new Vector2(0f, 0.0f);
            optionTextRect.offsetMax = new Vector2(0f, 0.0f);


            // button

            GameObject actionBtn = new GameObject(name + "_action");
            actionBtn.transform.SetParent(optionCon.transform);
            var actionBtnImg = actionBtn.AddComponent<Image>();

            actionBtnImg.sprite = Resources.Load<Sprite>("elephant_arrow");
            actionBtnImg.preserveAspect = true;

            var actionBtnSc = actionBtn.AddComponent<Button>();
            actionBtnSc.interactable = true;

            actionBtn.AddComponent<ToggleButton>();

            var actionBtnRect = actionBtn.GetComponent<RectTransform>();
            actionBtnRect.anchorMin = new Vector2(0.87f, 0.1f);
            actionBtnRect.anchorMax = new Vector2(0.97f, 0.9f);
            actionBtnRect.offsetMin = new Vector2(0f, 0.0f);
            actionBtnRect.offsetMax = new Vector2(0f, 0.0f);

            var actionScript = toggle.AddComponent<GDPRNavigationButton>();

            actionScript.action = GetActionFromString(openAction);

            actionScript.currentPage = parent;
            actionScript.page = refPage;
            actionScript.url = openURL;

            actionBtnSc.onClick.AddListener(() => { actionScript.OnClick(); });
        }


        private void ResetRectTrans(GameObject g, float paddingH = 0.0f, float paddingV = 0.0f)
        {
            var t = g.GetComponent<RectTransform>();

            t.anchorMin = new Vector2(paddingH, paddingV);
            t.anchorMax = new Vector2(1 - paddingH, 1 - paddingV);

            t.offsetMin = new Vector2(0, 0);
            t.offsetMax = new Vector2(0, 0);
        }

        private GameObject CreateToggle()
        {
            GameObject toggle = new GameObject("toggle");
            toggle.AddComponent<CanvasRenderer>();
            toggle.AddComponent<RectTransform>();

            var toggleOnImg = Resources.Load<Sprite>("elephant_switch_on");
            var toggleOffImg = Resources.Load<Sprite>("elephant_switch_off");


            GameObject toggleBg = new GameObject("toggle-bg");
            toggleBg.transform.SetParent(toggle.transform);
            var toggleBgImg = toggleBg.AddComponent<Image>();
            toggleBgImg.preserveAspect = true;
            toggleBgImg.sprite = toggleOffImg;

            ResetRectTrans(toggleBg);


            GameObject toggleHandle = new GameObject("toggle-handle");
            toggleHandle.AddComponent<RectTransform>();

            toggleHandle.transform.SetParent(toggle.transform);
            var toggleHandleImg = toggleHandle.AddComponent<Image>();
            toggleHandleImg.preserveAspect = true;
            toggleHandleImg.sprite = Resources.Load<Sprite>("elephant_switcher");

            var toggleHandleImgRect = toggleHandleImg.GetComponent<RectTransform>();
            toggleHandleImgRect.anchorMin = new Vector2(0.05f, 0.1f);
            toggleHandleImgRect.anchorMax = new Vector2(0.45f, 0.9f);
            toggleHandleImgRect.offsetMin = new Vector2(0f, 0.0f);
            toggleHandleImgRect.offsetMax = new Vector2(0f, 0.0f);


            GameObject toggleBtn = new GameObject("toggle-btn");
            toggleBtn.transform.SetParent(toggle.transform);
            var toggleBtnImg = toggleBtn.AddComponent<Image>();
            toggleBtnImg.color = new Color(0, 0, 0, 0);
            var toggleBtnSc = toggleBtn.AddComponent<Button>();
            toggleBtnSc.interactable = true;

            toggleBtn.AddComponent<ToggleButton>();

            ResetRectTrans(toggleBtn);

            var toggleController = toggle.AddComponent<ToggleController>();
            toggleController.isOn = false;
            toggleController.toggleOnBGImage = toggleOnImg;
            toggleController.toggleOffBGImage = toggleOffImg;
            toggleController.toggleBgImage = toggleBgImg;
            toggleController.toggle = toggle.GetComponent<RectTransform>();
            toggleController.handle = toggleHandle.GetComponent<RectTransform>();
            toggleController.handleOffset = 7;
            toggleController.speed = 12;

            toggleBtnSc.onClick.AddListener(() => { toggleController.Switching(); });

            toggleController.SetupToggle();

            toggles.Add(toggleController);


            return toggle;
        }


        private GameObject CreateScrollView(string mainText)
        {
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.AddComponent<CanvasRenderer>();
            var scrollRect = scrollView.AddComponent<ScrollRect>();


            GameObject viewPort = new GameObject("Viewport");
            viewPort.transform.SetParent(scrollView.transform);
            viewPort.AddComponent<CanvasRenderer>();
            var viewportImg = viewPort.AddComponent<Image>();
            viewportImg.type = Image.Type.Sliced;
            viewportImg.fillCenter = true;
            viewportImg.sprite =
                Resources.Load<Sprite>(
                    "elephant_roundbox"); // AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            viewPort.AddComponent<Mask>();


            scrollRect.viewport = viewPort.GetComponent<RectTransform>();

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewPort.transform);
            content.AddComponent<RectTransform>();
            scrollRect.content = content.GetComponent<RectTransform>();


            GameObject mainContent = new GameObject("main_content");
            mainContent.transform.SetParent(content.transform);

            mainContent.AddComponent<CanvasRenderer>();
            var text = mainContent.AddComponent<Text>();
            text.font = Resources.Load<Font>("ProximaNova-Regular");
            text.text = mainText;
            text.lineSpacing = 1.2f;
//            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            text.resizeTextForBestFit = true;
            text.fontSize = 14;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = 60;
            text.color = Color.black;

            ResetRectTrans(mainContent, 0.05f, 0.1f);
            ResetRectTrans(viewPort);
            ResetRectTrans(content);

            return scrollView;
        }

        void Update()
        {
            if (playBtn != null)
            {
                bool i = true;
                foreach (var toggle in toggles)
                {
                    if (!toggle.isOn)
                    {
                        i = false;
                        break;
                    }
                }

                playBtn.interactable = i;
            }
        }

        private GDPRNavigationButton.GDPRButtonAction GetActionFromString(string a)
        {
            if (a.Equals(GDPRNavigationButton.GDPRButtonAction.OPEN_PAGE.ToString()))
            {
                return GDPRNavigationButton.GDPRButtonAction.OPEN_PAGE;
            }
            else
            {
                return GDPRNavigationButton.GDPRButtonAction.OPEN_URL;
            }
        }
    }
}