using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour
{
    private GameObject chatView; // ������ͼ��������
    private GameObject chatItem;
    private GameObject mychatItem;
    private GameObject SelectChooseAB;
    private Image SelABBg;
    private Image PhoneBKImg;
    private Button ASelBtn;
    private Button BSelBtn;
    private Button CSelBtn;
    private RectTransform ScheImg;
    private Transform TipsIcon;
    private ScrollRect scrollRect; // ������ͼ�������  
    private Image HeadInfo;//����ͼƬ
    private TextMeshProUGUI NpcName;//��������
    private TextMeshProUGUI NpcHobby;//���ﰮ��
    private bool isSel = false; // �Ƿ�ѡ����A��B
    private NpcConfig npcConfig;
    //private GameObject SelectChooseAB
    float yPos = -20;
    float InityPos = 646;
    int curselindex = 1;
    Vector3 tipsPos = new Vector3(348, 20, 0); // TipsIcon��ʼλ��
    public List<ChatGroup> allChatInfoDic = new List<ChatGroup>(); // ����������Ϣ�ֵ�
    public Dictionary<int, ChatGroup> chatInfoDic = new Dictionary<int, ChatGroup>(); // ������Ϣ�ֵ�
    private UIManager uiManager;
    private Sequence animationSequence; // ����ʱ����ת��Ϊȫ�ֱ���
    private Sequence animationSequence1;
    void Awake()
    {
        LoadChatDataFromJson();
    }

    void LoadChatDataFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("chat");
        if (jsonText == null)
        {
            Debug.LogError("δ�ҵ����������ļ� ChatData.json");
            return;
        }

        // �����л�
        List<ChatGroup> chatGroups = JsonConvert.DeserializeObject<List<ChatGroup>>(jsonText.text); //JsonUtilityWrapper.FromJsonList<ChatGroup>(jsonText.text);
        chatInfoDic.Clear();
        foreach (var group in chatGroups)
        {
            allChatInfoDic.Add(group);
            //allChatInfoDic[group.chatId] = group;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        PhoneBKImg = transform.Find("PhoneBK").GetComponent<Image>();
        chatView = transform.Find("PhoneBK/Scroll View/Viewport/Content")?.gameObject;
        chatItem = Resources.Load<GameObject>("Prefabs/UI/ChatItem");
        mychatItem = Resources.Load<GameObject>("Prefabs/UI/MyChatItem"); //GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/ChatItem"));
        SelectChooseAB = transform.Find("PhoneBK/SelectChooseAB")?.gameObject;
        SelABBg = SelectChooseAB.GetComponent<Image>();
        ASelBtn = SelectChooseAB.transform.Find("ABtn")?.GetComponent<Button>();
        BSelBtn = SelectChooseAB.transform.Find("BBtn")?.GetComponent<Button>();
        CSelBtn = SelectChooseAB.transform.Find("CBtn")?.GetComponent<Button>();
        TipsIcon = SelectChooseAB.transform.Find("TipsIcon");
        tipsPos = new Vector3(0, TipsIcon.localPosition.y, TipsIcon.localPosition.z);
        ScheImg = transform.Find("PhoneBK/HeartRate /Sche")?.GetComponent<RectTransform>();
        scrollRect = transform.Find("PhoneBK/Scroll View")?.GetComponent<ScrollRect>();
        HeadInfo = transform.Find("PhoneBK/HeadInfo")?.GetComponent<Image>();
        NpcName = transform.Find("PhoneBK/Name")?.GetComponent<TextMeshProUGUI>();
        NpcHobby = transform.Find("PhoneBK/Hobby")?.GetComponent<TextMeshProUGUI>();
        //������Ҫ����ɸѡһ����ص�npc��������
        foreach (var chatinfo in allChatInfoDic)
        {
            if (chatinfo.npcId == npcConfig.NPCID)
            {
                chatInfoDic[chatinfo.chatId] = chatinfo;
            }
        };
        if (npcConfig.selectList!=null && chatInfoDic[npcConfig.selectList[npcConfig.selectList.Count-1]]!=null)
        {
            SetProportionalSize(ScheImg, chatInfoDic[npcConfig.selectList[npcConfig.selectList.Count-1]].sche);
        }
        SelectChooseAB.SetActive(false); // ��ʼ����ѡ��ť
        ChatProcess(-1, false);

        var npcid = npcConfig.ResourseID;
        PhoneBKImg.sprite = Resources.Load<Sprite>($"chatGameIcon/SelPanelIcon{npcid}/bgimage");
        SelABBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chooseBg{npcid}");
        ASelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        BSelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        CSelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        HeadInfo.sprite = Resources.Load<Sprite>(npcConfig.NPCSmallImg);
        NpcName.text = uiManager.GetLocalizedText(npcConfig,"NPCName");// npcConfig.NPCName;//
        NpcHobby.text = npcConfig.NPCHobby;
    }
    private void OnEnable()
    {

    }

    void OnDestroy()
    {
        DOTween.Kill(animationSequence); // ������ʱֹͣ������animationSequence��صĶ���
        DOTween.Kill(animationSequence1);
    }

    //void CreateMyChat(int index, bool ismychat)
    //{
    //    //GameObject item = GameObject.Instantiate(mychatItem, chatView.transform);
    //    //Text itemText = item.transform.Find("chatText")?.GetComponent<Text>();
    //    //if (itemText != null)
    //    //{
    //    //    itemText.text = ASelBtn.GetComponentInChildren<Text>().text;
    //    //    //itemText.enabled = false; // Ĭ������
    //    //}
    //    ChatProcess(index);
    //}
    void SetProportionalSize(RectTransform img, float sc)
    {
        Image imageComponent = img.GetComponent<Image>();
        imageComponent.DOFillAmount(sc / 100, 0.5f);
        if(sc>=90)
        {
            //Debug.LogError("��ʼ��ת");
            uiManager.EndCardPresented();
        }
    }

    void ChatProcess(int selIndex, bool ismychat)//��������
    { //����Ҫ�Ӹ��ж��ǵ�һ�λ���֮������
        StartCoroutine(ChatCoroutine(selIndex, ismychat));

        //scrollRect.verticalNormalizedPosition = 0;
    }

    // ��ChatCoroutine�����У�ȷ��ÿ���������������������Զ��������ײ�
    IEnumerator ChatCoroutine(int selIndex, bool ismychat)
    {
        List<ChatMessage> chatList = new List<ChatMessage>();
        if (selIndex < 0)
        {
            for (int i = 0; i < npcConfig.selectList.Count; i++)
            {
                chatList.Add(chatInfoDic[npcConfig.selectList[i]].messages);
            }
            selIndex = curselindex = npcConfig.selectList[npcConfig.selectList.Count - 1];
        }
        else if (!chatInfoDic.ContainsKey(selIndex))
        { 
            yield break;
        }
        else {
            if (ismychat)
            {
                chatList.Add(chatInfoDic[curselindex].messages);
                curselindex = selIndex;
            }
            else
            {
                foreach (var i in chatInfoDic[curselindex].selectList)
                {
                    chatList.Add(chatInfoDic[i].messages);
                }
                curselindex = chatInfoDic[selIndex].selectList[chatInfoDic[selIndex].selectList.Count - 1]; ;
            }
        }
        //������и��µ�ǰ����һ���Ի���
        
        var ys = 0;
        for (int i = 0; i < chatList.Count; i++)//������ص���������
        {
            yield return new WaitForSeconds(0.5f);
            GameObject item = GameObject.Instantiate(ismychat ? mychatItem : chatItem, chatView.transform);
            TextMeshProUGUI itemText = item.transform.Find("ChatBg/chatText")?.GetComponent<TextMeshProUGUI>();
            RectTransform rt = item.GetComponent<RectTransform>();
            Transform fadeChat = item.transform.Find("ChatBg");
            Image icon = item.transform.Find("icon")?.GetComponent<Image>();
            Image itemChatBg = fadeChat?.GetComponent<Image>();
            if (icon != null)
            {
                icon.sprite = Resources.Load<Sprite>(npcConfig.NPCSmallImg);
            }
            itemChatBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcConfig.ResourseID}");
            Vector3 originalScale = fadeChat.localScale;
            if (itemText != null)
            {
                itemText.text = chatList[i].text;
                //itemText.ForceMeshUpdate();
                fadeChat.localScale = Vector3.zero;
                float singleLineHeight = itemText.lineSpacing;// itemText.lineSpacing;
                float totalHeight = itemText.GetPreferredValues().y;

                Vector2 newSize = rt.sizeDelta;
                newSize.y += totalHeight > 84 ? totalHeight : 0;
                rt.sizeDelta = newSize;
                //Debug.LogError("���г���" + totalHeight + "ʵ�ʳ���" + singleLineHeight);
                //Debug.LogError("Ԥ����߶�" + rt.sizeDelta.x + "Ԥ���峤��" + rt.sizeDelta.y);
            }
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yPos);
            yPos -= rt.sizeDelta.y + 10;
            Animator animator = item.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Show");
            }
            ys = Mathf.Abs((int)yPos);
            chatView.GetComponent<RectTransform>().sizeDelta = new Vector2(chatView.GetComponent<RectTransform>().sizeDelta.x, ys > InityPos ? ys : InityPos);
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatView.GetComponent<RectTransform>());
            yield return new WaitForSeconds(0.5f);
            if (ismychat)
            {
                uiManager.PlaySendMessageSound();
            }
            else {
                uiManager.PlayNewMessagePopSound();
            }

            animationSequence1 = DOTween.Sequence(); // ʹ��ȫ�ֱ���
            animationSequence1.Append(fadeChat.DOScale(originalScale, 0.2f))
               .Join(fadeChat.GetComponent<CanvasGroup>()?.DOFade(1, 0.2f));
            //Debug.LogError("����������С");
            if (itemText != null)
            {
                itemText.enabled = true;
            }
            //curselindex += 1;
            // �Զ��������ײ�
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        //scrollRect.verticalNormalizedPosition = 0f;

        //if (selIndex + 1 > chatInfoDic.Count)
        //    yield break;
        yield return new WaitForSeconds(0.1f);

        if (ismychat)//ѡ��ѡ������뵽����������npc�ظ�
        {
            SelectChooseAB.SetActive(false);
            //PlayerAnimation();
            if (chatInfoDic[selIndex].sche <= 90 && chatInfoDic[selIndex].selectList.Count > 0)//��������ĳ��Ƿ���selectlist
            {
                
                Debug.Log("���ŵ��Ǳ��˵�����");
                ChatProcess(curselindex, !ismychat);
                curselindex = chatInfoDic[selIndex].selectList[chatInfoDic[selIndex].selectList.Count-1];
            }
            else
            {
                uiManager.EndCardPresented();
                Debug.LogWarning("û������selectlist0");
            }

        }
        else 
        {
            if (!ismychat && chatInfoDic[selIndex].selectList.Count > 0)
            {
                SelectChooseAB.SetActive(true);
                ChatGroup tempchatInfo = chatInfoDic[curselindex];
                ASelBtn.onClick.RemoveAllListeners();
                BSelBtn.onClick.RemoveAllListeners();
                CSelBtn.onClick.RemoveAllListeners();
                ASelBtn.onClick.AddListener(() => HandleButtonClick(0));
                BSelBtn.onClick.AddListener(() => HandleButtonClick(1));
                CSelBtn.onClick.AddListener(() => HandleButtonClick(2));
                ASelBtn.GetComponentInChildren<TextMeshProUGUI>().text = uiManager.GetLocalizedText(chatInfoDic[tempchatInfo.selectList[0]].messages,"text");
                BSelBtn.GetComponentInChildren<TextMeshProUGUI>().text = uiManager.GetLocalizedText(chatInfoDic[tempchatInfo.selectList[1]].messages, "text");
                CSelBtn.GetComponentInChildren<TextMeshProUGUI>().text = uiManager.GetLocalizedText(chatInfoDic[tempchatInfo.selectList[2]].messages, "text");
                PlayerAnimation();
            }
            else {
                uiManager.EndCardPresented();
                Debug.LogWarning("û������selectlist1");
            }
        }
        
        isSel = false;
    }
    private void HandleButtonClick(int index)
    {
        isSel = true;
        if (index<= chatInfoDic[curselindex].selectList.Count-1)
        {
            curselindex = chatInfoDic[curselindex].selectList[index];
            ChatProcess(curselindex, true);
            SelectChooseAB.SetActive(false); // �ر�ѡ��ť
            SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        }
        else
        {
            Debug.LogWarning("û������selectlist2");
         }
            
    }

    //���Ŷ���
    void PlayerAnimation()
    {
        // 完全停止并清理之前的动画
        if (animationSequence != null)
        {
            animationSequence.Kill();
            animationSequence = null;
        }
        // 完全重置所有UI元素的状态
        ResetUIElementsState();
        if (SelectChooseAB.activeSelf && !isSel)
        {
            animationSequence = DOTween.Sequence(); // ʹ��ȫ�ֱ���
            SingleAniFunc(animationSequence, CSelBtn, BSelBtn, -117f, -250f);
            SingleAniFunc(animationSequence, ASelBtn,BSelBtn, -117f, 50f);

            // TipsIcon�ص�ԭ��
            animationSequence.SetLoops(-1);
        }
    }
    
    // 新增：重置UI元素状态的方法
    void ResetUIElementsState()
    {
        TipsIcon.localPosition = tipsPos;
        TipsIcon.localScale = Vector3.one;
        ASelBtn.transform.localScale = Vector3.one;
        BSelBtn.transform.localScale = Vector3.one;
        CSelBtn.transform.localScale = Vector3.one;
        
        // 确保所有按钮的DOTween动画都被清理
        DOTween.Kill(ASelBtn.transform);
        DOTween.Kill(BSelBtn.transform);
        DOTween.Kill(CSelBtn.transform);
        DOTween.Kill(TipsIcon);
    }

    private void SingleAniFunc(Sequence ani,Button btn1,Button btn2,float startpos,float endpos)
    {
        ani.Append(TipsIcon.DOLocalMoveY(startpos, 0.5f));
        // ASelBtn��TipsIconͬ���Ŵ�
        ani.Append(btn2.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
           .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
        // ASelBtn��TipsIconͬ����ԭ
        ani.Append(btn2.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
           .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));

        ani.Append(TipsIcon.DOLocalMoveY(endpos, 0.5f));
        // BSelBtn��TipsIconͬ���Ŵ�
        ani.Append(btn1.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
           .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
        // BSelBtn��TipsIconͬ����ԭ
        ani.Append(btn1.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
           .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));
    }
    //���ݸ�ֵ
    public void InitNPCInfo(NpcConfig Npcconfig)
    {
        npcConfig = Npcconfig;
    }
}

[System.Serializable]
public class ChatMessage
{
    public string sender;
    public string text;//Ĭ��Ӣ��
    public string timestamp;
    public string text_zh;//��������
    public string text_ko;//����
}

[System.Serializable]
public class ChatGroup
{
    public int chatId;
    public int npcId;
    public bool isMyChat;
    public ChatMessage messages;
    public int sche; // ����ֵ
    public List<int> selectList;//ѡ���б�
}

// JsonUtility ��֧�� List<T>�����Զ����װ
//public static class JsonUtilityWrapper
//{
//    public static List<T> FromJsonList<T>(string json)
//    {
//        string newJson = "{ \"list\": " + json + "}";
//        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
//        return wrapper.list;
//    }

//    [System.Serializable]
//    private class Wrapper<T>
//    {
//        public List<T> list;
//    }
//}
