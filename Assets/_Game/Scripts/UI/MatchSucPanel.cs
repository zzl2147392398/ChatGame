using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class MatchSucPanel : MonoBehaviour
{

    private GameObject HeadInfo;
    private GameObject Name;
    private GameObject Hobby;
    private GameObject Match;
    private RawImage BgImage;
    private RawImage ShowLoveBgImage;
    private NpcConfig npcConfig;
    private UIManager uiManager;
    private GameObject Love1;
    private CanvasGroup Love2;
    private CanvasGroup Love3;
    // Start is called before the first frame update
    void Start()
    {
        BgImage = transform.Find("MatchBg").GetComponent<RawImage>();
        ShowLoveBgImage = transform.Find("ShowLoveBg").GetComponent<RawImage>();
        HeadInfo = transform.Find("MatchBg/HeadInfo").gameObject;
        Name = transform.Find("MatchBg/Name").gameObject;
        Match = transform.Find("MatchBg/Image/Match").gameObject;
        Hobby = transform.Find("MatchBg/Image/Hobby").gameObject;

        //爱心特效动画
        Love1 = transform.Find("ShowLoveBg/Love1").gameObject;
        Love2 = transform.Find("ShowLoveBg/Love2").GetComponent<CanvasGroup>();
        Love3 = transform.Find("ShowLoveBg/Love3").GetComponent<CanvasGroup>();

        HeadInfo.GetComponent<Image>().sprite = Resources.Load<Sprite>(npcConfig.NPCImage);
        Name.GetComponent<Text>().text = npcConfig.NPCName;
        Hobby.GetComponent<Text>().text = npcConfig.NPCHobby;
        //BgImage.texture = Resources.Load<Texture>($"chatGameIcon/SelPanelIcon{npcConfig.NPCID}/bgimage");
        ShowLoveBgImage.texture = Resources.Load<Texture>(npcConfig.BgImage.ToString());
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        //StartCoroutine(TransAnimation());
        StartCoroutine(ShowLove());
    }

    IEnumerator ShowLove()
    {
        Love2.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        // 隐藏 Love2，显示 Love3
        Love3.DOFade(1, 0.5f);
        yield return new WaitForSeconds(2f);
        uiManager.ShowAndCloseOtherPanel("chatpanel");
        uiManager.GetPanelComponent<ChatPanel>("chatpanel").InitNPCInfo(npcConfig);
    }

    IEnumerator TransAnimation()
    {
        Vector3 originalScale = new Vector3(1, 1, 1);
        Name.transform.localScale = Vector3.zero;
        Match.transform.localScale = Vector3.zero;
        Hobby.transform.localScale = Vector3.zero;
        Name.SetActive(true);
        Name.transform.DOScale(Vector3.zero, 0); 
        Name.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f); 
        Match.SetActive(true);
        Match.transform.DOScale(Vector3.zero, 0);
        Match.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack); 
        yield return new WaitForSeconds(0.5f); 
        Hobby.transform.DOScale(Vector3.zero, 0);
        Hobby.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);
        uiManager.ShowAndCloseOtherPanel("chatpanel");
        uiManager.GetPanelComponent<ChatPanel>("chatpanel").InitNPCInfo(npcConfig);
    }


    public void InitSucPanel(NpcConfig npcConfig)
    {
        this.npcConfig = npcConfig;    
    }
}
