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
    private TextMeshProUGUI Num;
    private Image CheckMark;
    private CurSche Cursche = CurSche.NOSec; 

    // Start is called before the first frame update
    void Start()
    {
        Icon = GameObject.Find("Icon").GetComponent<Image>();
        //Num = GameObject.Find("Num").GetComponent<TextMeshProUGUI>();
        CheckMark = GameObject.Find("CheckMark").GetComponent<Image>();
    }

    public void SetInfo(ClothesConfig config,CurSche status)
    {
        Icon.sprite = Resources.Load<Sprite>(config.SelectIcon);
        if (status==Cursche)
            return;
        if (status == CurSche.CurSec && Cursche == CurSche.FinishSec)//已完成状态变成当前选中状态，进度-1的情况
        {
            //尺寸变大两倍
            transform.DOScale(transform.localScale * 2f, 0.3f);
            transform.GetComponent<Image>().color = Color.green;
            CheckMark.gameObject.SetActive(false);
            Icon.gameObject.SetActive(true);
            //对勾消失衣服标志出现
        }
        else if (status == CurSche.NOSec && Cursche == CurSche.CurSec)//当前状态变成灰色状态,进度-1
        {
            //尺寸变小一半
            transform.DOScale(transform.localScale * 0.5f, 0.3f);
            transform.GetComponent<Image>().color = Color.gray;
            CheckMark.gameObject.SetActive(false);
            Icon.gameObject.SetActive(false);
            //颜色换成灰色，check和衣服标志消失
        }
        else if (status == CurSche.CurSec && Cursche == CurSche.NOSec)//灰色变成当前环节状态，进度+1
        {
            //尺寸变大两倍
            transform.DOScale(transform.localScale * 2f, 0.3f);
            transform.GetComponent<Image>().color = Color.green;
            Icon.gameObject.SetActive(true);
            //颜色变成绿色，衣服标志出现
        }
        else if (status == CurSche.FinishSec && Cursche == CurSche.CurSec)//当前环节状态变成选完状态，选完衣服
        {
            //尺寸变小一半
            transform.DOScale(transform.localScale * 0.5f, 0.3f);
            CheckMark.gameObject.SetActive(true);
            Icon.gameObject.SetActive(false);
            //对勾出现，衣服标志消失
        }

    }
}
