using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //private static UIManager _instance;

    //public static UIManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = FindObjectOfType<UIManager>();
    //            if (_instance == null)
    //            {
    //                Debug.LogError("UIManager not found in the scene.");
    //            }
    //        }
    //        return _instance;
    //    }
    //}

    [SerializeField] private AudioClip thudSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip popSound;
    [System.Serializable]
    public struct UIPanelPair
    {
        public string key;
        public GameObject value;
    }
    [SerializeField] private List<UIPanelPair> uiPanels = new List<UIPanelPair>();//竖版界面
    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private int endCardHash = Animator.StringToHash("endCard");

    private void Awake()
    {
        //if (_instance == null)
        //{
        //    _instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else if (_instance != this)
        //{
        //    Destroy(gameObject);
        //}

        m_Animator = GetComponent<Animator>();
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        //uiPanels.Add(new UIPanelPair() { key = "chatpanel", value = transform.Find("ChatPanel").gameObject });
        //uiPanels.Add(new UIPanelPair() { key = "selectpanel", value = transform.Find("SelectPanel").gameObject });
        //uiPanels.Add(new UIPanelPair() { key = "matchsucpanel", value = transform.Find("MatchSucPanel").gameObject });
        //uiPanels.Add(new UIPanelPair() { key = "loadpanel", value = transform.Find("LoadPanel").gameObject });
        ShowAndCloseOtherPanel("selectpanel");
        //ShowAndCloseOtherPanel("SelectPanel");
    }

    public void CallToAction()
    {
        PlayPop();
        Luna.Unity.Playable.InstallFullGame();
    }

    public void PlayThud() { m_AudioSource.PlayOneShot(thudSound); }
    public void PlayWin() { m_AudioSource.PlayOneShot(winSound); }
    public void PlayPop() { m_AudioSource.PlayOneShot(popSound); }

    public void ShowEndCard() { m_Animator.SetTrigger(endCardHash); }
    public void EndCardPresented()
    {
        //Debug.Log("进入结束节点");
        Luna.Unity.LifeCycle.GameEnded();
    }

    // 显示指定UI面板
    public void ShowAndCloseOtherPanel(string panelName)
    {
        //Debug.Log("enter ShowAndCloseOtherPanel");
        var panelStr = VerOrHorScrene(panelName);
        for (int i = 0; i < uiPanels.Count; i++)
        {
            uiPanels[i].value.SetActive(panelStr == uiPanels[i].key);
        }
    }

    public T GetPanelComponent<T>(string panelName) where T : Component
    {
        var panelStr = VerOrHorScrene(panelName);
        for (int i = 0; i < uiPanels.Count; i++)
        {
            if (panelStr == uiPanels[i].key)
            {
                return uiPanels[i].value.GetComponent<T>();
            }
        }
        return null;
    }

    private string VerOrHorScrene(string panelName)
    {
        bool isLandscape = false;// Screen.width > Screen.height;
        var panelStr = isLandscape ? "hor" + panelName : panelName; 
        return panelStr;
    }
}
