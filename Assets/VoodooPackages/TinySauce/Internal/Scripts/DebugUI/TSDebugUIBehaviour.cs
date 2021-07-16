using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Voodoo.Sauce.Internal
{
    public class TSDebugUIBehaviour : MonoBehaviour
    {
        public enum TSDebugUIActiveScreen
        {
            Info,
            Events
        }

        [Header("== Tabs ==")]
        [SerializeField] private string infoScreenName = "INFO";
        [SerializeField] private string eventsScreenName = "EVENTS";
        [Space(4)]
        [SerializeField] private Button infoTabBtn;
        [SerializeField] private Button eventsTabBtn;

        [Header("== Screens ==")]
        [SerializeField] private TSDebugUIScreen infoScreen;
        [SerializeField] private TSDebugUIScreen eventsScreen;

        [Header("== App Info Fields ==")]
        [SerializeField] private Text unityVerion;
        [SerializeField] private Text tsVersion;
        [SerializeField] private Text appNameTop;


        private static TSDebugUIBehaviour _instance;
        public static TSDebugUIBehaviour Instance { get => _instance; }


        private TinySauceSettings _tsSettings;

        private TSDebugUIActiveScreen activeScreen = TSDebugUIActiveScreen.Info;
        private Dictionary<TSDebugUIActiveScreen, Button> tabDictionary = new Dictionary<TSDebugUIActiveScreen, Button>();
        private Dictionary<TSDebugUIActiveScreen, TSDebugUIScreen> screenDictionary = new Dictionary<TSDebugUIActiveScreen, TSDebugUIScreen>();


        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (FindObjectOfType<EventSystem>() == null) Instantiate(new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)));


            _tsSettings = TinySauceSettings.Load();
            UpdateInfo();

            infoScreen.TSSettings = _tsSettings;
        }

        private void Start()
        {
            InitDictionaries();
            Set_ActiveScreen(infoScreenName);
        }

        private void OnDestroy()
        {
            _instance = null;
        }


        public void CloseDebugUI()
        {
            Destroy(gameObject);
        }

        private void UpdateInfo()
        {
            unityVerion.text = Application.unityVersion;
            tsVersion.text = "TS v. " + TinySauce.Version;
            appNameTop.text = Application.productName;
        }

        #region [TABS]
        private void InitDictionaries()
        {
            tabDictionary[TSDebugUIActiveScreen.Info] = infoTabBtn;
            tabDictionary[TSDebugUIActiveScreen.Events] = eventsTabBtn;

            screenDictionary[TSDebugUIActiveScreen.Info] = infoScreen;
            screenDictionary[TSDebugUIActiveScreen.Events] = eventsScreen;

            infoScreen.gameObject.SetActive(false);
            eventsScreen.gameObject.SetActive(false);
        }

        private void ToggleTab(bool isActive)
        {
            tabDictionary[activeScreen].interactable = !isActive;
            screenDictionary[activeScreen].gameObject.SetActive(isActive);
        }

        public void Set_ActiveScreen(string screenName)
        {
            TSDebugUIActiveScreen newActiveScreen;

            if (screenName == infoScreenName)
                newActiveScreen = TSDebugUIActiveScreen.Info;
            else if (screenName == eventsScreenName)
                newActiveScreen = TSDebugUIActiveScreen.Events;
            else
            {
                Debug.LogError("Screen name is not existing");
                return;
            }


            ToggleTab(false);
            activeScreen = newActiveScreen;
            ToggleTab(true);
        }
        #endregion []
    }
}