using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

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
    private RectTransform ScheImg;
    private Transform TipsIcon;
    private ScrollRect scrollRect; // ������ͼ�������  
    private Image HeadInfo;//����ͼƬ
    private Text NpcName;//��������
    private Text NpcHobby;//���ﰮ��
    private bool isSel = false; // �Ƿ�ѡ����A��B
    private NpcConfig npcConfig;
    //private GameObject SelectChooseAB
    float yPos = -20;
    float InityPos = 646;
    int curselindex = 1;
    Vector3 tipsPos = new Vector3(348, 20, 0); // TipsIcon��ʼλ��
    public Dictionary<int, ChatGroup> chatInfoDic = new Dictionary<int, ChatGroup>(); // ������Ϣ�ֵ�
    private Dictionary<string, List<int>> NameToChatInfo = new Dictionary<string, List<int>>(); // ���ֵ�������Ϣ��ӳ��
    private UIManager uiManager;
    private Sequence animationSequence; // ����ʱ����ת��Ϊȫ�ֱ���

    void Awake()
    {
        LoadChatDataFromJson();
    }

    void LoadChatDataFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("ChatData");
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
            chatInfoDic[group.chatId] = group;
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
        TipsIcon = SelectChooseAB.transform.Find("TipsIcon");
        tipsPos = new Vector3(0, TipsIcon.localPosition.y, TipsIcon.localPosition.z);
        ScheImg = transform.Find("PhoneBK/HeartRate /Sche")?.GetComponent<RectTransform>();
        scrollRect = transform.Find("PhoneBK/Scroll View")?.GetComponent<ScrollRect>();
        HeadInfo = transform.Find("PhoneBK/HeadInfo")?.GetComponent<Image>();
        NpcName = transform.Find("PhoneBK/Name")?.GetComponent<Text>();
        NpcHobby = transform.Find("PhoneBK/Hobby")?.GetComponent<Text>();
        SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        ASelBtn.onClick.AddListener(() =>
        {
            isSel = true;
            ChatProcess(curselindex + 1, true);
            SelectChooseAB.SetActive(false); // ��ʾѡ��ť
            SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        });
        BSelBtn.onClick.AddListener(() =>
        {
            isSel = true;
            ChatProcess(curselindex + 2, true);
            SelectChooseAB.SetActive(false); // ��ʾѡ��ť
            SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        });
        SelectChooseAB.SetActive(false); // ��ʼ����ѡ��ť
        ChatProcess(1, false);

        var npcid = npcConfig.NPCID;
        PhoneBKImg.sprite = Resources.Load<Sprite>($"chatGameIcon/SelPanelIcon{npcid}/bgimage");
        SelABBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chooseBg{npcid}");
        ASelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        BSelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        HeadInfo.sprite = Resources.Load<Sprite>(npcConfig.NPCSmallImg);
        NpcName.text = npcConfig.NPCName;
        NpcHobby.text = npcConfig.NPCHobby;
    }
    private void OnEnable()
    {

    }

    void OnDestroy()
    {
        DOTween.Kill(animationSequence); // ������ʱֹͣ������animationSequence��صĶ���
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
            uiManager.EndCardPresented();
        }
    }

    void ChatProcess(int selIndex, bool ismychat)//��������
    {
        StartCoroutine(ChatCoroutine(selIndex, ismychat));

        //scrollRect.verticalNormalizedPosition = 0;
    }

    // ��ChatCoroutine�����У�ȷ��ÿ��������������������Զ��������ײ�
    IEnumerator ChatCoroutine(int selIndex, bool ismychat)
    {
        if (!chatInfoDic.ContainsKey(selIndex)) yield break;
        List<ChatMessage> chatList = chatInfoDic[selIndex].messages;
        curselindex = selIndex;
        var ys = 0;
        for (int i = 0; i < chatList.Count; i++)
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
            itemChatBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcConfig.NPCID}");
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
            yield return new WaitForSeconds(1f);

            animationSequence = DOTween.Sequence(); // ʹ��ȫ�ֱ���
            animationSequence.Append(fadeChat.DOScale(originalScale, 0.2f))
               .Join(fadeChat.GetComponent<CanvasGroup>()?.DOFade(1, 0.2f));
            //Debug.LogError("����������С");
            if (itemText != null)
            {
                itemText.enabled = true;
            }

            // �Զ��������ײ�
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        scrollRect.verticalNormalizedPosition = 0f;

        if (selIndex + 1 > chatInfoDic.Count)
            yield break;
        yield return new WaitForSeconds(1f);

        if (ismychat)
        {
            SelectChooseAB.SetActive(false);
            //PlayerAnimation();
            if (chatInfoDic[selIndex].sche <= 90)
            {
                ChatProcess(selIndex + 2, !ismychat);
                curselindex += 2;
            }

        }
        else
        {
            SelectChooseAB.SetActive(true);
            PlayerAnimation();
            
        }
        ASelBtn.GetComponentInChildren<TextMeshProUGUI>().text = chatInfoDic[selIndex + 1].messages[0].text;
        BSelBtn.GetComponentInChildren<TextMeshProUGUI>().text = chatInfoDic[selIndex + 2].messages[0].text;
        isSel = false;
    }

    //���Ŷ���
    void PlayerAnimation()
    {
        if (animationSequence != null)
        {
            animationSequence.Kill();
        }
        TipsIcon.localPosition = tipsPos;
        TipsIcon.localScale = Vector3.one;
        ASelBtn.transform.localScale = Vector3.one;
        BSelBtn.transform.localScale = Vector3.one;
        if (SelectChooseAB.activeSelf && !isSel)
        {
            animationSequence = DOTween.Sequence(); // ʹ��ȫ�ֱ���
            // TipsIcon�ƶ�
            animationSequence.Append(TipsIcon.DOLocalMoveY(-250f, 0.5f));
            // ASelBtn��TipsIconͬ���Ŵ�
            animationSequence.Append(BSelBtn.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
               .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
            // ASelBtn��TipsIconͬ����ԭ
            animationSequence.Append(BSelBtn.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
               .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));
            animationSequence.Append(TipsIcon.DOLocalMoveY(50f, 0.5f));
            // BSelBtn��TipsIconͬ���Ŵ�
            animationSequence.Append(ASelBtn.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
               .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
            // BSelBtn��TipsIconͬ����ԭ
            animationSequence.Append(ASelBtn.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
               .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));

            // TipsIcon�ص�ԭ��
            animationSequence.SetLoops(-1);
        }
    }

    //���ݸ�ֵ
    public void InitNPCInfo(NpcConfig Npcconfig)
    {
        npcConfig = Npcconfig;

        //HeadInfo = transform.Find("PhoneBK/HeadInfo")?.GetComponent<Image>();
        //NpcName = transform.Find("PhoneBK/Name")?.GetComponent<Text>();
        //NpcHobby = transform.Find("PhoneBK/Hobby")?.GetComponent<Text>();
    }
}

[System.Serializable]
public class ChatMessage
{
    public string sender;
    public string text;
    public string timestamp;
    public bool isMyChat;
}

[System.Serializable]
public class ChatGroup
{
    public int chatId;
    public List<ChatMessage> messages;
    public int sche; // ����ֵ
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
