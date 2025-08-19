using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MatchSucPanel : MonoBehaviour
{

    private GameObject HeadInfo;
    private GameObject Name;
    private GameObject Hobby;
    private GameObject Match;
    private UIManager uiManager;
    private Image BgImage;
    private NpcConfig npcConfig;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        BgImage = transform.GetComponent<Image>();
        HeadInfo = transform.Find("HeadInfo").gameObject;
        Name = transform.Find("Name").gameObject;
        Match = transform.Find("Image/Match").gameObject;
        Hobby = transform.Find("Image/Hobby").gameObject;
        HeadInfo.GetComponent<Image>().sprite = Resources.Load<Sprite>(npcConfig.NPCImage);
        Name.GetComponent<Text>().text = npcConfig.NPCName;
        Hobby.GetComponent<Text>().text = npcConfig.NPCHobby;
        BgImage = Resources.Load<Image>($"chatGameIcon/SelPanelIcon{npcConfig.NPCID}/bgimage");
        StartCoroutine(TransAnimation());

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
        UIManager.Instance.ShowAndCloseOtherPanel("chatpanel");
        UIManager.Instance.GetPanelComponent<ChatPanel>("chatpanel").InitNPCInfo(npcConfig);
    }


    public void InitSucPanel(NpcConfig npcConfig)
    {
        this.npcConfig = npcConfig;    
    }
}
