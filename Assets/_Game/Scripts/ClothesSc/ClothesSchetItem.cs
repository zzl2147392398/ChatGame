using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CurSche
{
    NOSec, //默认灰色
    CurSec, //当前环节
    FinishSec, //选完
    
}

public class ClothesSchetItem : MonoBehaviour
{
    private Image Icon;
    private Image Gary;
    private TextMeshProUGUI Num;
    private Image CheckMark;
    private CurSche Cursche; 

    // Start is called before the first frame update
    void Awake()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        Gary = transform.Find("Gary").GetComponent<Image>();
        //Num = GameObject.Find("Num").GetComponent<TextMeshProUGUI>();
        CheckMark = transform.Find("CheckMark").GetComponent<Image>();
    }

    public void SetInfo(ClothesConfig config,CurSche status)
    {
        Icon.sprite = Resources.Load<Sprite>(config.SelectIcon);
        Debug.Log("图标是======="+config.SelectIcon.ToString()+"=====状态是======="+status.ToString());
        if (status == Cursche)
        {
            Cursche = status;
            return;
        }  
        if (status == CurSche.CurSec && Cursche == CurSche.FinishSec)//已完成状态变成当前选中状态，进度-1的情况
        {
            Gary.gameObject.SetActive(false);
            CheckMark.gameObject.SetActive(false);
            Icon.gameObject.SetActive(true);
            transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
            //对勾消失衣服标志出现
        }
        else if (status == CurSche.NOSec && Cursche == CurSche.CurSec)//当前状态变成灰色状态,进度-1
        {
            Gary.gameObject.SetActive(true);
            CheckMark.gameObject.SetActive(false);
            Icon.gameObject.SetActive(false);
            transform.DOScale(Vector3.one * 1f, 0.5f).SetEase(Ease.OutBack);
            //颜色换成灰色，check和衣服标志消失
        }
        else if (status == CurSche.CurSec && Cursche == CurSche.NOSec)//灰色变成当前环节状态，进度+1
        {
            CheckMark.gameObject.SetActive(false);
            //transform.GetComponent<Image>().color = Color.green;
            Gary.gameObject.SetActive(false);
            transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
            Icon.gameObject.SetActive(true);
            //颜色变成绿色，衣服标志出现
        }
        else if (status == CurSche.FinishSec && Cursche == CurSche.CurSec)//当前环节状态变成选完状态，选完衣服
        {
            Debug.Log("对勾出现");
            transform.DOScale(Vector3.one*1f, 0.5f).SetEase(Ease.OutBack);
            CheckMark.gameObject.SetActive(true);
            Icon.gameObject.SetActive(false);
            Gary.gameObject.SetActive(false);
            //对勾出现，衣服标志消失
        }
        Cursche = status;
    }
}
