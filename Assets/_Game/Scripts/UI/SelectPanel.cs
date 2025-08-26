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

public class SelectPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // �ӽڵ�����
    public GameObject SlideCard;
    private RawImage Bgimage;
    private Button DownLoadBtn;
    public string PanelName = "SelectPanel"; // �������
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private Image scheImg; // ��¼��ǰ���Ƶ��ı�
    private Text scheText; // ��¼��ǰ���Ƶ��ı�
    private Image targetImage; // Ŀ��ͼƬ
    private float roz = 0f;
    private int sche =1; // ��¼��ǰ�Ŀ������
    private int tarsche = 3;
    private float timer = 0f;
    private float interval = 2f;
    private bool bisPlayAni = true;
    private Vector3 pos = Vector3.zero; // ��¼��ǰ����λ��
    private GameObject GuideAnimation;
    private Dictionary<int, NpcConfig> chatInfoDic = new Dictionary<int, NpcConfig>(); // ������Ϣ�ֵ�
    private GameObject MatchIcon;
    private UIManager uiManager;
    void Awake()
    {
        LoadNPCDataFromJson();
    }

    void LoadNPCDataFromJson()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("NPData");
        if (jsonText == null)
        {
            Debug.LogError("δ�ҵ����������ļ� NPData.json");
            return;
        }

        // �����л�
        //List<NpcConfig> chatGroups = JsonUtilityWrapper.FromJsonList<NpcConfig>();
        List<NpcConfig> chatGroups = JsonConvert.DeserializeObject<List<NpcConfig>>(jsonText.text);
        chatInfoDic.Clear();
        foreach (var group in chatGroups)
        {
            chatInfoDic[group.NPCID]= group;
         }
        tarsche = chatInfoDic.Count; // �����ܿ�����
    }
    void Start()
    {
        //LoadNPCDataFromJson();
        //uiManager = GetComponentInParent<GameObject>().transform.Find("UI").GetComponent<UIManager>();
        // ���ڴ˴��Խڵ�����ʼ������
        Transform bgimage = transform.Find("BgImage");
        Bgimage = bgimage.GetComponent<RawImage>();    
        
        Sprite sprite = Resources.Load<Sprite>("chatGameIcon/touxiang1");
        //SlideCard = transform.Find("BgImage/SlideCard")?.gameObject;
        // �滻ԭ�е� SlideCard ʵ������λ�����ô���
        SlideCard = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SlideCard"));
        SlideCard.transform.SetParent(bgimage, false); 
        SlideCard.transform.localPosition = new Vector3(0, 91, 0); // ���ñ���λ�� 
        targetImage = SlideCard.transform.Find("Zhezhao/PictureIcon")?.GetComponent<Image>();
        GuideAnimation = SlideCard.transform.Find("GuideAnimation").gameObject;
        scheText = SlideCard.transform.Find("CurSche/Sche")?.GetComponent<Text>();
        scheImg = SlideCard.transform.Find("CurSche")?.GetComponent<Image>();
        DownLoadBtn = bgimage?.Find("DownLoadBtn")?.GetComponent<Button>();
        MatchIcon = bgimage?.Find("MatchIcon").gameObject;
        DownLoadBtn.onClick.AddListener(() => {
            uiManager.EndCardPresented();
        });
        pos = new Vector3(0, GuideAnimation.transform.localPosition.y, 0);
        SetPanelInfo(sche);
        if (bgimage != null)
        {
            SlideCard.transform.SetParent(bgimage);
        }
        else
        {
            Debug.LogError("SlideCard����Ϊ�գ��޷����ø�����");
        }
        dragStartPos = SlideCard.transform.localPosition; // ��ʼ����ק��ʼλ��
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        scheText.text = sche+"/"+tarsche; // ��ʼ����ǰ��������ı�
        //transform.FindChild("SlideCard");
    }

    void SetPanelInfo(int index)
    {
        Texture bgTexture = Resources.Load<Texture>(chatInfoDic[index].BgImage.ToString());//����ͼ
        Sprite sprite = Resources.Load<Sprite>(chatInfoDic[index].NPCImage.ToString());//����ͼƬDownBtnImg
        DownLoadBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(chatInfoDic[index].DownBtnImg.ToString());//���ذ�ťͼƬ
        scheImg.sprite = Resources.Load<Sprite>(chatInfoDic[index].ScheImg.ToString());//����ͼƬ
        SlideCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(chatInfoDic[index].Bottomframe.ToString());//��������ͼƬ
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
                //��������
                SlideCardAnimation();
                timer = 0f; // ���ü�ʱ��
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
        roz += eventData.delta.x / 10; // ������Ҫ����������
        // Debug.LogError("roz"+roz);
        SlideCard.transform.rotation = Quaternion.Euler(0, 0, -roz);
        SlideCard.transform.position = new Vector3(SlideCard.transform.position.x + eventData.delta.x, SlideCard.transform.position.y, SlideCard.transform.position.z);// dragStartPos + (eventData.position - dragStartPos); // ������Ե�����ק��������
        // ���������ﴦ����ק�����е��߼�
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
            var ismatch = (eAz > 300 && eAz < 350) && (chatInfoDic[cursche]?.SliderDir == "right"); // �ж��Ƿ�ƥ��
            if (sche >= tarsche|| ismatch)
            {
                var NpcData = chatInfoDic[cursche];
                uiManager.ShowAndCloseOtherPanel("matchsucpanel");
                uiManager.GetPanelComponent<MatchSucPanel>("matchsucpanel").InitSucPanel(NpcData);
                return;
            }
            //SlideCard.SetActive(false); // ���ؿ���
            sche += 1;
            scheText.text = sche + "/" + tarsche; // ��ʼ����ǰ��������ı�
            SetPanelInfo(sche);
            SlideCard.GetComponent<CanvasGroup>().alpha = 0;//��ȡ͸���Ƚ����޸�֮��dotween������ʾ
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
        // �����ﴦ����ק��������߼��������жϻ�������
        //Debug.Log($"�����켣: {dragDelta}");
    }

    private System.Collections.IEnumerator nextCard(bool ismatch)
    {
        IsMatchAnimation(ismatch);
        yield return new WaitForSeconds(0.5f);
        SetCardPlayerInfo();
        // �������������Ҫִ�еĴ���
    }

    private void SetCardPlayerInfo()
    {
        SlideCard.GetComponent<CanvasGroup>()?.DOFade(1, 0.2f);
        SlideCard.transform.localPosition = dragStartPos;
        SlideCard.transform.rotation = Quaternion.Euler(0, 0, 0);
        roz = 0;
        
    }

    //��������
    private void SlideCardAnimation()
    {
        if (GuideAnimation == null) return;
        GuideAnimation.SetActive(bisPlayAni);
        // ������λ�ã���ֹ��ε��õ���ƫ��
        GuideAnimation.transform.localPosition = pos;
        // ֹͣ���ж�������ֹ����
        DOTween.Kill(GuideAnimation.transform);
        // �������ж���
        Sequence seq = DOTween.Sequence();
        seq.Append(GuideAnimation.transform.DOLocalMoveX(-300, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(0, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(300, 0.5f))
           .Append(GuideAnimation.transform.DOLocalMoveX(0, 0.5f))
           .SetLoops(-1); // ����ѭ��
    }

    private void IsMatchAnimation(bool ismatch)
    {
        // ����ƥ��ͼ��
        MatchIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("chatGameIcon/DynamicEffect/" + (ismatch ?"Ture":"False"));
        CanvasGroup canvasGroup = MatchIcon.GetComponent<CanvasGroup>();
        MatchIcon.SetActive(true);
        // ���ó�ʼ״̬
        canvasGroup.alpha = 0;
        MatchIcon.transform.localScale = Vector3.zero;

        DOTween.Kill(MatchIcon.transform);

        Sequence seq = DOTween.Sequence();
        // ͸���ȴ�0��1�����Ŵ�0��1
        seq.Append(canvasGroup.DOFade(1, 0.2f));
        seq.Join(MatchIcon.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        // ΢΢��
        seq.Append(MatchIcon.transform.DOShakeScale(0.2f, 0.15f, 10, 80, false));
        // ��ʧ
        seq.Append(canvasGroup.DOFade(0, 0.1f));
        seq.AppendCallback(() => MatchIcon.SetActive(false));
    }
}

[System.Serializable]
public class NpcConfig
{
    public int NPCID;
    public string NPCName;
    public string NPCHobby;
    public string NPCImage;
    public string BgImage;
    public string SliderDir;
    public string DownBtnImg;
    public string Bottomframe;
    public string ScheImg;
    public string NPCSmallImg;

}
