using System.Collections.Generic;
using UnityEngine;

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

    //[SerializeField] private AudioClip thudSound;
    //[SerializeField] private AudioClip winSound;
    //[SerializeField] private AudioClip popSound;

    [Header("Background Music Settings")]
    private AudioSource bgmSource;              // 背景音乐 AudioSource
    private AudioSource sfxSource;              // 音效 AudioSource
    public AudioClip backgroundMusic;       //背景音乐片段
    public float backgroundMusicVolume = 0.5f; // 背景音乐音量
    public bool playBackgroundMusicOnStart = true; // 是否在开始时播放背景音乐
    public bool loopBackgroundMusic = true; // 是否循环播放背景音乐

    [Header("Audio Clips")]
    public AudioClip matchSuccessSound;         // 滑动匹配成功音乐
    public AudioClip matchFailureSound;         // 滑动匹配失败音乐
    public AudioClip heartMomentSound;          // 心动时刻音乐
    public AudioClip newMessagePopSound;        // 新消息弹出音乐
    public AudioClip sendMessageSound;          // 发送消息音乐

    

    [System.Serializable]
    public struct UIPanelPair
    {
        public string key;
        public GameObject value;
    }
    [SerializeField] private List<UIPanelPair> uiPanels = new List<UIPanelPair>();//竖版界面
    // 根据 lang 设置游戏语言
    

    private Animator m_Animator;
    private int endCardHash = Animator.StringToHash("endCard");
    private string curlanguage = ""; 
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
        // 背景音乐和音效分开管理
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        curlanguage = GetSystemLanguage();
        ShowAndCloseOtherPanel("selectpanel");
        if (playBackgroundMusicOnStart)
        {
            PlayBackgroundMusic();
        }
    }

    public void CallToAction()
    {
        //PlayPop();
        Luna.Unity.Playable.InstallFullGame();
    }

    //public void PlayThud() { m_AudioSource.PlayOneShot(thudSound); }
    //public void PlayWin() { m_AudioSource.PlayOneShot(winSound); }
    //public void PlayPop() { m_AudioSource.PlayOneShot(popSound); }

    public void ShowEndCard() { m_Animator.SetTrigger(endCardHash); }
    public void EndCardPresented()
    {
        Debug.Log("开始安装完整游戏...");
        try
        {
            // 调用Luna Unity Playable的安装方法
            Luna.Unity.Playable.InstallFullGame();
            Debug.Log("InstallFullGame() 调用成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InstallFullGame() 调用失败: {e.Message}");
        }
    //Debug.Log("进入结束节点");
    //Luna.Unity.LifeCycle.GameEnded();
    }

    // 播放背景音乐
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        bgmSource.clip = backgroundMusic;
        bgmSource.loop = loopBackgroundMusic;
        bgmSource.volume = backgroundMusicVolume;
        bgmSource.Play();
    }

    // 暂停背景音乐
    public void PauseBackgroundMusic()
    {
        if (bgmSource.isPlaying)
            bgmSource.Pause();
    }

    // 播放滑动匹配成功音乐
    public void PlayMatchSuccessSound()
    {
        if (matchSuccessSound != null)
            sfxSource.PlayOneShot(matchSuccessSound);
    }

    // 播放滑动匹配失败音乐
    public void PlayMatchFailureSound()
    {
        if (matchFailureSound != null)
            sfxSource.PlayOneShot(matchFailureSound);
    }

    // 播放心动时刻音乐
    public void PlayHeartMomentSound()
    {
        if (heartMomentSound != null)
            sfxSource.PlayOneShot(heartMomentSound);
    }

    // 播放新消息弹出音乐
    public void PlayNewMessagePopSound()
    {
        if (newMessagePopSound != null)
            sfxSource.PlayOneShot(newMessagePopSound);
    }

    // 播放发送消息音乐
    public void PlaySendMessageSound()
    {
        if (sendMessageSound != null)
            sfxSource.PlayOneShot(sendMessageSound);
    }

    // 暂停所有音效
    public void PauseAllSfx()
    {
        sfxSource.Pause();
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

    public string GetSystemLanguage()
    {
        switch (Application.systemLanguage)
        {
            //case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                return "zh";
            case SystemLanguage.Korean:
                return "ko";
            default:
                return "";
        };
    }
    // 定义一个通用的多语言文本获取方法
    public string GetLocalizedText(object messageObj, string baseField = "text")
    {
        string fieldName = baseField + "_" + curlanguage; // 比如 text_en, text_zh
        var type = messageObj.GetType();
        var prop = type.GetProperty(fieldName);
        if (prop != null)
        {
            var value = prop.GetValue(messageObj, null);
            if (value != null) return value.ToString();
        }
        // 没有对应语言字段则返回默认
        prop = type.GetProperty(baseField);
        if (prop != null)
        {
            var value = prop.GetValue(messageObj, null);
            if (value != null) return value.ToString();
        }
        // 如果是字段（field），也可以这样获取
        var field = type.GetField(baseField);
        if (field != null)
        {
            var value = field.GetValue(messageObj);
            if (value != null) return value.ToString();
        }
        return "";
    }
}
