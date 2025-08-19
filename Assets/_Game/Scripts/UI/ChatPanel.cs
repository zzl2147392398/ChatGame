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

public class ChatPanel : MonoBehaviour
{
    private GameObject chatView; // 聊天视图对象引用
    private GameObject chatItem;
    private GameObject mychatItem;
    private GameObject SelectChooseAB;
    private Image SelABBg;
    private Image PhoneBKImg;
    private Button ASelBtn;
    private Button BSelBtn;
    private RectTransform ScheImg;
    private Transform TipsIcon;
    private ScrollRect scrollRect; // 滚动视图组件引用  
    private Image HeadInfo;//人物图片
    private Text NpcName;//人物名字
    private Text NpcHobby;//人物爱好
    private bool isSel = false; // 是否选择了A或B
    private NpcConfig npcConfig;
    //private GameObject SelectChooseAB
    float yPos = -20;
    float InityPos = 646;
    int curselindex = 1;
    Vector3 tipsPos = new Vector3(348, 20, 0); // TipsIcon初始位置
    public Dictionary<int, ChatGroup> chatInfoDic = new Dictionary<int, ChatGroup>(); // 聊天信息字典
    private Dictionary<string, List<int>> NameToChatInfo = new Dictionary<string, List<int>>(); // 名字到聊天信息的映射

    void Awake()
    {
        LoadChatDataFromJson();
    }

    void LoadChatDataFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("ChatData");
        if (jsonText == null)
        {
            Debug.LogError("未找到聊天数据文件 ChatData.json");
            return;
        }

        // 反序列化
        List<ChatGroup> chatGroups = JsonUtilityWrapper.FromJsonList<ChatGroup>(jsonText.text);
        chatInfoDic.Clear();
        foreach (var group in chatGroups)
        {
            chatInfoDic[group.chatId] = group;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
            SelectChooseAB.SetActive(false); // 显示选择按钮
            SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        });
        BSelBtn.onClick.AddListener(() =>
        {
            isSel = true;
            ChatProcess(curselindex + 2, true);
            SelectChooseAB.SetActive(false); // 显示选择按钮
            SetProportionalSize(ScheImg, chatInfoDic[curselindex].sche);
        });
        SelectChooseAB.SetActive(false); // 初始隐藏选择按钮
        ChatProcess(1, false);

        var npcid = npcConfig.NPCID;
        PhoneBKImg.sprite = Resources.Load<Sprite>($"chatGameIcon/SelPanelIcon{npcid}/bgimage");
        SelABBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chooseBg{npcid}");
        ASelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
        BSelBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcid}");
    }
    private void OnEnable()
    {

    }


    void OnDestroy()
    {

    }

    //void CreateMyChat(int index, bool ismychat)
    //{
    //    //GameObject item = GameObject.Instantiate(mychatItem, chatView.transform);
    //    //Text itemText = item.transform.Find("chatText")?.GetComponent<Text>();
    //    //if (itemText != null)
    //    //{
    //    //    itemText.text = ASelBtn.GetComponentInChildren<Text>().text;
    //    //    //itemText.enabled = false; // 默认隐藏
    //    //}
    //    ChatProcess(index);
    //}
    void SetProportionalSize(RectTransform img, float sc)
    {
        Image imageComponent = img.GetComponent<Image>();
        imageComponent.DOFillAmount(sc/100, 0.5f);
    }

    void ChatProcess(int selIndex, bool ismychat)//聊天流程
    {
        StartCoroutine(ChatCoroutine(selIndex, ismychat));

        //scrollRect.verticalNormalizedPosition = 0;
    }

    // 在ChatCoroutine方法中，确保每次添加新聊天项后滚动条自动滚动到底部
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
            Text itemText = item.transform.Find("ChatBg/chattext")?.GetComponent<Text>();
            RectTransform rt = item.GetComponent<RectTransform>();
            Transform fadeChat = item.transform.Find("ChatBg");
            Image itemChatBg = fadeChat?.GetComponent<Image>();
            itemChatBg.sprite = Resources.Load<Sprite>($"chatGameIcon/ChatPanelIcon/chatTextBg{npcConfig.NPCID}");
            Vector3 originalScale = fadeChat.localScale;
            if (itemText != null)
            {
                itemText.text = chatList[i].text;
                //itemText.ForceMeshUpdate();
                fadeChat.localScale = Vector3.zero;
                float singleLineHeight = 2;// itemText.lineSpacing;
                float totalHeight = itemText.preferredHeight;
                Vector2 newSize = rt.sizeDelta;
                newSize.y = singleLineHeight > 3 ? totalHeight : rt.sizeDelta.y;
                rt.sizeDelta = newSize;
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

            Sequence seq = DOTween.Sequence();
            seq.Append(fadeChat.DOScale(originalScale, 0.2f))
               .Join(fadeChat.GetComponent<CanvasGroup>()?.DOFade(1, 0.2f));
            //Debug.LogError("重新算描绘大小");
            if (itemText != null)
            {
                itemText.enabled = true;
            }

            // 自动滚动到底部
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        scrollRect.verticalNormalizedPosition = 0f;

        if (chatInfoDic[selIndex + 1] == null)
            yield break;
        yield return new WaitForSeconds(1f);
        ASelBtn.GetComponentInChildren<Text>().text = chatInfoDic[selIndex + 1].messages[0].text;
        BSelBtn.GetComponentInChildren<Text>().text = chatInfoDic[selIndex + 2].messages[0].text;
        isSel = false;
        if (ismychat)
        {
            SelectChooseAB.SetActive(false);
            if (chatInfoDic[selIndex].sche!=100)
            {
                UIManager.Instance.EndCardPresented();
            }
            else {
                ChatProcess(selIndex + 2, !ismychat);
                curselindex += 2;
            }
                
        }
        else
        {
            SelectChooseAB.SetActive(true);
            PlayerAnimation();
        }
    }

    //播放动画
    void PlayerAnimation()
    {
        TipsIcon.localPosition = tipsPos;
        TipsIcon.localScale = Vector3.one;
        ASelBtn.transform.localScale = Vector3.one;
        BSelBtn.transform.localScale = Vector3.one;
        DOTween.Kill(TipsIcon);
        DOTween.Kill(ASelBtn.transform);
        DOTween.Kill(BSelBtn.transform);
        if (SelectChooseAB.activeSelf && !isSel)
        {
            Sequence seq = DOTween.Sequence();
            // TipsIcon移动
            seq.Append(TipsIcon.DOLocalMoveY(-250f, 0.5f));
            // ASelBtn和TipsIcon同步放大
            seq.Append(BSelBtn.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
               .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
            // ASelBtn和TipsIcon同步还原
            seq.Append(BSelBtn.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
               .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));
            seq.Append(TipsIcon.DOLocalMoveY(50f, 0.5f));
            // BSelBtn和TipsIcon同步放大
            seq.Append(ASelBtn.transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
               .Join(TipsIcon.DOScale(0.7f, 0.15f).SetEase(Ease.OutQuad));
            // BSelBtn和TipsIcon同步还原
            seq.Append(ASelBtn.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
               .Join(TipsIcon.DOScale(1f, 0.15f).SetEase(Ease.InQuad));

            // TipsIcon回到原点
            seq.SetLoops(-1);
        }
    }

    //数据赋值
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
    public int sche; // 心率值
}

// JsonUtility 不支持 List<T>，需自定义包装
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