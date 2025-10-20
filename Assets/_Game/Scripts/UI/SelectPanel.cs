using DG.Tweening;
using Luna.Unity.FacebookInstantGames;
using System;
using System.Collections.Generic;

//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class SelectPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // 子节点引用
    public GameObject SlideCard;
    private RawImage Bgimage;
    private Button DownLoadBtn;
    public string PanelName = "SelectPanel"; // 面板名称
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private Image scheImg; // 记录当前卡牌的文本
    private TextMeshProUGUI scheText; // 记录当前卡牌的进度文本
    private TextMeshProUGUI sliderTips;//滑动卡片提示文本
    private TextMeshProUGUI MainTips;//主界面的提示文本
    private Image targetImage; // 目标图片
    private float roz = 0f;
    private int sche =1; // 记录当前的卡牌序号
    private int tarsche = 3;
    private float timer = 0f;
    private float interval = 2f;
    private bool bisPlayAni = true;
    private Vector3 pos = Vector3.zero; // 记录当前卡牌位置
    private GameObject GuideAnimation;
    private Dictionary<int, NpcConfig> chatInfoDic = new Dictionary<int, NpcConfig>(); // 聊天信息字典
    private GameObject MatchIcon;
    private UIManager uiManager;
    void Awake()
    {
        LoadNPCDataFromJson();
    }

    void LoadNPCDataFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("npc");
        if (jsonText == null)
        {
            Debug.LogError("未找到聊天数据文件 NPData.json");
            return;
        }

        // 反序列化
        //List<NpcConfig> chatGroups = JsonUtilityWrapper.FromJsonList<NpcConfig>();
        List<NpcConfig> chatGroups = JsonConvert.DeserializeObject<List<NpcConfig>>(jsonText.text);
        chatInfoDic.Clear();
        foreach (var group in chatGroups)
        {
            chatInfoDic[group.NPCID]= group;
         }
        tarsche = chatInfoDic.Count; // 更新总卡牌数
    }
    void Start()
    {
        //LoadNPCDataFromJson();
        //uiManager = GetComponentInParent<GameObject>().transform.Find("UI").GetComponent<UIManager>();
        // 可在此处对节点做初始化操作
        Transform bgimage = transform.Find("BgImage");
        Bgimage = bgimage.GetComponent<RawImage>();    
        
        Sprite sprite = Resources.Load<Sprite>("chatGameIcon/touxiang1");
        //SlideCard = transform.Find("BgImage/SlideCard")?.gameObject;
        // 替换原有的 SlideCard 实例化和位置设置代码
        SlideCard = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SlideCard"));
        SlideCard.transform.SetParent(bgimage, false); 
        SlideCard.transform.localPosition = new Vector3(0, 91, 0); // 设置本地位置 
        targetImage = SlideCard.transform.Find("Zhezhao/PictureIcon")?.GetComponent<Image>();
        GuideAnimation = SlideCard.transform.Find("GuideAnimation").gameObject;
        scheText = SlideCard.transform.Find("CurSche/Sche")?.GetComponent<TextMeshProUGUI>();
        sliderTips = SlideCard.transform.Find("Tips")?.GetComponent<TextMeshProUGUI>();
        scheImg = SlideCard.transform.Find("CurSche")?.GetComponent<Image>();
        DownLoadBtn = bgimage?.Find("DownLoadBtn")?.GetComponent<Button>();
        MatchIcon = bgimage?.Find("MatchIcon").gameObject;
        MainTips  = bgimage?.Find("XunWen")?.GetComponent<TextMeshProUGUI>();
        DownLoadBtn.onClick.AddListener(() => {
            uiManager.EndCardPresented();
        });
        pos = new Vector3(0, GuideAnimation.transform.localPosition.y, 0);
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        SetPanelInfo(sche);
        if (bgimage != null)
        {
            SlideCard.transform.SetParent(bgimage);
        }
        else
        {
            Debug.LogError("SlideCard对象为空，无法设置父对象");
        }
        dragStartPos = SlideCard.transform.localPosition; // 初始化拖拽起始位置
        scheText.text = sche+"/"+tarsche; // 初始化当前卡牌序号文本
        //transform.FindChild("SlideCard");
    }

    void SetPanelInfo(int index)
    {
        if (!chatInfoDic.ContainsKey(index) || chatInfoDic[index] == null)
        {
            Debug.LogError($"chatInfoDic[{index}] 不存在或为 null");
            return;
        }
        Texture bgTexture = Resources.Load<Texture>(chatInfoDic[index].BgImage.ToString());//背景图
        Sprite sprite = Resources.Load<Sprite>(chatInfoDic[index].NPCImage.ToString());//人物图片DownBtnImg
        DownLoadBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(chatInfoDic[index].DownBtnImg.ToString());//下载按钮图片
        scheImg.sprite = Resources.Load<Sprite>(chatInfoDic[index].ScheImg.ToString());//进度图片
        SlideCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(chatInfoDic[index].Bottomframe.ToString());//滑动卡牌图片
        sliderTips.text = uiManager.GetLocalizedText(chatInfoDic[index],"SliderCardTips");
        MainTips.text = uiManager.GetLocalizedText(chatInfoDic[index],"XunWenTips");
        Bgimage.texture = bgTexture;
        targetImage.sprite = sprite;
    }

    void Update()
    {
        if (bisPlayAni)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                //触发函数
                SlideCardAnimation();
                timer = 0f; // 重置计时器
                bisPlayAni = false;
            }
        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        bisPlayAni = false;
        SlideCardAnimation();
    }

    public void OnDrag(PointerEventData eventData)
    {
        bisPlayAni = false;
        roz += eventData.delta.x / 10; // 根据需要调整灵敏度
        // Debug.LogError("roz"+roz);
        SlideCard.transform.rotation = Quaternion.Euler(0, 0, -roz);
        SlideCard.transform.position = new Vector3(SlideCard.transform.position.x + eventData.delta.x, SlideCard.transform.position.y, SlideCard.transform.position.z);// dragStartPos + (eventData.position - dragStartPos); // 这里可以调整拖拽的灵敏度
        // 可以在这里处理拖拽过程中的逻辑
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float eAz = SlideCard.GetComponent<RectTransform>().rotation.eulerAngles.z;
        if ((eAz > 10 && eAz < 60) || (eAz > 300 && eAz < 350))
        {
            var cursche = sche > tarsche ? tarsche : sche;
            if (!chatInfoDic.ContainsKey(cursche))
            {
                Debug.LogError($"Key {cursche} not found in chatInfoDic.");
                return;
            }
            var ismatch = (eAz > 300 && eAz < 350) && (chatInfoDic[cursche]?.SliderDir == "right"); // 判断是否匹配
            if (sche >= tarsche|| ismatch)
            {
                var NpcData = chatInfoDic[cursche];
                uiManager.PlayMatchSuccessSound();
                uiManager.ShowAndCloseOtherPanel("matchsucpanel");
                uiManager.GetPanelComponent<MatchSucPanel>("matchsucpanel").InitSucPanel(NpcData);
                return;
            }
            //SlideCard.SetActive(false); // 隐藏卡牌
            sche += 1;
            scheText.text = sche + "/" + tarsche; // 初始化当前卡牌序号文本
            SetPanelInfo(sche);
            SlideCard.GetComponent<CanvasGroup>().alpha = 0;//获取透明度进行修改之后dotween进行显示
            uiManager.PlayMatchFailureSound();
            StartCoroutine(nextCard(ismatch));
        }
        else
        {
            SlideCard.transform.localPosition = dragStartPos;//= eventData.position;
            SlideCard.transform.rotation = Quaternion.Euler(0, 0, 0);
            roz = 0;
            bisPlayAni = true;
        }
        
        //dragEndPos = eventData.position;
        //Vector2 dragDelta = dragEndPos - dragStartPos;
        // 在这里处理拖拽结束后的逻辑，比如判断滑动方向
        //Debug.Log($"滑动轨迹: {dragDelta}");
    }

    private System.Collections.IEnumerator nextCard(bool ismatch)
    {
        IsMatchAnimation(ismatch);
        yield return new WaitForSeconds(0.5f);
        SetCardPlayerInfo();
        // 在这里添加你想要执行的代码
    }

    private void SetCardPlayerInfo()
    {
        SlideCard.GetComponent<CanvasGroup>()?.DOFade(1, 0.2f);
        SlideCard.transform.localPosition = dragStartPos;
        SlideCard.transform.rotation = Quaternion.Euler(0, 0, 0);
        roz = 0;
        
    }

    //滑动动画
    private void SlideCardAnimation()
    {
        if (GuideAnimation == null) return;
        GuideAnimation.SetActive(bisPlayAni);
        // 先重置位置，防止多次调用导致偏移
        GuideAnimation.transform.localPosition = pos;
        // 停止已有动画，防止叠加
        DOTween.Kill(GuideAnimation.transform);
        // 创建序列动画
        Sequence seq = DOTween.Sequence();
        seq.Append(GuideAnimation.transform.DOLocalMoveX(-300, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(0, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(300, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(0, 0.5f))
           .SetLoops(-1); // 无限循环
    }

    private void IsMatchAnimation(bool ismatch)
    {
        // 设置匹配图标
        MatchIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("chatGameIcon/DynamicEffect/" + (ismatch ?"Ture":"False"));
        CanvasGroup canvasGroup = MatchIcon.GetComponent<CanvasGroup>();
        MatchIcon.SetActive(true);
        // 重置初始状态
        canvasGroup.alpha = 0;
        MatchIcon.transform.localScale = Vector3.zero;

        DOTween.Kill(MatchIcon.transform);

        Sequence seq = DOTween.Sequence();
        // 透明度从0到1，缩放从0到1
        seq.Append(canvasGroup.DOFade(1, 0.2f));
        seq.Join(MatchIcon.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        // 微微震动
        seq.Append(MatchIcon.transform.DOShakeScale(0.2f, 0.15f, 10, 80, false));
        // 消失
        seq.Append(canvasGroup.DOFade(0, 0.1f));
        seq.AppendCallback(() => MatchIcon.SetActive(false));
    }
}

[System.Serializable]
public class NpcConfig
{
    public int NPCID;//人物id
    public string NPCName;//人物名字
    public string NPCName_zh;//人物名字
    public string NPCName_ko;//人物名字
    public string NPCHobby;//人物爱好
    public string NPCImage;//人物图片
    public string BgImage;//背景颜色图
    public string SliderDir;//滑动匹配方向
    public string DownBtnImg;//下载图片
    public string Bottomframe;//按钮
    public string ScheImg;//进度图片
    public string NPCSmallImg;//npc头像
    public string XunWenTips;//滑动界面主界面提示语
    public string XunWenTips_zh;//滑动界面主界面提示语
    public string XunWenTips_ko;//滑动界面主界面提示语
    public string SliderCardTips;//滑动界面卡片提示语
    public string SliderCardTips_zh;//滑动界面卡片提示语
    public string SliderCardTips_ko;//滑动界面卡片提示语
    public List<int> selectList;//第一次进来npc讲话的聊天列表
    public int ResourseID;
}
