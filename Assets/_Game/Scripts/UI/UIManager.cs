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
    private AudioSource bgmSource;              // �������� AudioSource
    private AudioSource sfxSource;              // ��Ч AudioSource
    public AudioClip backgroundMusic;       //��������Ƭ��
    public float backgroundMusicVolume = 0.5f; // ������������
    public bool playBackgroundMusicOnStart = true; // �Ƿ��ڿ�ʼʱ���ű�������
    public bool loopBackgroundMusic = true; // �Ƿ�ѭ�����ű�������

    [Header("Audio Clips")]
    public AudioClip matchSuccessSound;         // ����ƥ��ɹ�����
    public AudioClip matchFailureSound;         // ����ƥ��ʧ������
    public AudioClip heartMomentSound;          // �Ķ�ʱ������
    public AudioClip newMessagePopSound;        // ����Ϣ��������
    public AudioClip sendMessageSound;          // ������Ϣ����

    

    [System.Serializable]
    public struct UIPanelPair
    {
        public string key;
        public GameObject value;
    }
    [SerializeField] private List<UIPanelPair> uiPanels = new List<UIPanelPair>();//�������
    // ���� lang ������Ϸ����
    

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
        // �������ֺ���Ч�ֿ�����
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
        Debug.Log("��ʼ��װ������Ϸ...");
        try
        {
            // ����Luna Unity Playable�İ�װ����
            Luna.Unity.Playable.InstallFullGame();
            Debug.Log("InstallFullGame() ���óɹ�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InstallFullGame() ����ʧ��: {e.Message}");
        }
    //Debug.Log("��������ڵ�");
    //Luna.Unity.LifeCycle.GameEnded();
    }

    // ���ű�������
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        bgmSource.clip = backgroundMusic;
        bgmSource.loop = loopBackgroundMusic;
        bgmSource.volume = backgroundMusicVolume;
        bgmSource.Play();
    }

    // ��ͣ��������
    public void PauseBackgroundMusic()
    {
        if (bgmSource.isPlaying)
            bgmSource.Pause();
    }

    // ���Ż���ƥ��ɹ�����
    public void PlayMatchSuccessSound()
    {
        if (matchSuccessSound != null)
            sfxSource.PlayOneShot(matchSuccessSound);
    }

    // ���Ż���ƥ��ʧ������
    public void PlayMatchFailureSound()
    {
        if (matchFailureSound != null)
            sfxSource.PlayOneShot(matchFailureSound);
    }

    // �����Ķ�ʱ������
    public void PlayHeartMomentSound()
    {
        if (heartMomentSound != null)
            sfxSource.PlayOneShot(heartMomentSound);
    }

    // ��������Ϣ��������
    public void PlayNewMessagePopSound()
    {
        if (newMessagePopSound != null)
            sfxSource.PlayOneShot(newMessagePopSound);
    }

    // ���ŷ�����Ϣ����
    public void PlaySendMessageSound()
    {
        if (sendMessageSound != null)
            sfxSource.PlayOneShot(sendMessageSound);
    }

    // ��ͣ������Ч
    public void PauseAllSfx()
    {
        sfxSource.Pause();
    }

    // ��ʾָ��UI���
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
    // ����һ��ͨ�õĶ������ı���ȡ����
    public string GetLocalizedText(object messageObj, string baseField = "text")
    {
        string fieldName = baseField + "_" + curlanguage; // ���� text_en, text_zh
        var type = messageObj.GetType();
        var prop = type.GetProperty(fieldName);
        if (prop != null)
        {
            var value = prop.GetValue(messageObj, null);
            if (value != null) return value.ToString();
        }
        // û�ж�Ӧ�����ֶ��򷵻�Ĭ��
        prop = type.GetProperty(baseField);
        if (prop != null)
        {
            var value = prop.GetValue(messageObj, null);
            if (value != null) return value.ToString();
        }
        // ������ֶΣ�field����Ҳ����������ȡ
        var field = type.GetField(baseField);
        if (field != null)
        {
            var value = field.GetValue(messageObj);
            if (value != null) return value.ToString();
        }
        return "";
    }
}
