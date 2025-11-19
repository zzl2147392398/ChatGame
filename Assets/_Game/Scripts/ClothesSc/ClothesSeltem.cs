using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ClothesSeltem : MonoBehaviour
{
    Image IconImage;
    public ClothesItem Item;
    public Button CgClothesBtn;
    private Renderer itemRenderer; // 物体的Renderer组件
    private float duration = 1f; // 动画持续时间
    //private Vector3 targetPosition = new Vector3(0,0,0); // 目标位置
    CanvasGroup effCanvasGroup;
    private ClothesCurSelPanel selPanel;
    // Start is called before the first frame update
    private void Awake()
    {
        //transform.localPosition = new Vector3(0, -150, 0); // 初始位置在屏幕下方
        IconImage = transform.Find("Eff/Icon").GetComponent<Image>();
        CgClothesBtn = transform.Find("Eff/Button").GetComponent<Button>();
        //transform.DOMove(targetPosition, duration)
        //         .SetEase(Ease.Linear); // 可以根据需要更改缓动类型
        effCanvasGroup = transform.Find("Eff").GetComponent<CanvasGroup>();
        selPanel = GameObject.Find("UI/SelectPanel").GetComponent<ClothesCurSelPanel>();
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            effCanvasGroup.alpha = newAlpha;
        }
        effCanvasGroup.alpha = 1f;
        var itemCom = transform.GetComponent<ClothesSeltem>();
        var btn = itemCom.CgClothesBtn;
        btn?.onClick.AddListener(() => {
            ItemOnClick(Item);
        });
        var itemBtn = transform.Find("Eff")?.GetComponent<Button>();
        itemBtn?.onClick.AddListener(() => {
            selPanel.Curselitem = Item.index;
            var ItemList = ClothesDataBase.Instance.allClothes.FindAll(c => c.type == (ClothesType)ClothesMain.CurSelectIndex);
            ItemOnClick(this.GetComponent<ClothesSeltem>().Item);
            for (int j = 0; j < ItemList.Count; j++)
            {
                Transform child = selPanel.SelectItemContent.transform.GetChild(j);
                child.Find("Eff/Button").gameObject.SetActive((j+1) == selPanel.Curselitem);
            }
            selPanel.bisPlayAni = false;
            selPanel.Tips.gameObject.SetActive(false);
        });

        btn.gameObject.SetActive(false);
    }
    void Start()
    {
        
        //itemRenderer.material.DOFade(1, duration).From(0) // 透明度从0到1
        //                    .SetEase(Ease.Linear); // 可以根据需要更改缓动类型
    }
    private void ItemOnClick(ClothesItem item)
    {
        selPanel.RightBtn.gameObject.SetActive(ClothesMain.CurSelectIndex < 4);
        selPanel.LeftBtn.gameObject.SetActive(ClothesMain.CurSelectIndex > 1);
        var spriteList = new List<Sprite>();
        //1.出现星星特效，具体需求是我会根据具体的物体type然后去调用CharacterDressUp中的ChangeClothes方法
        foreach (var value in item.idList)
        {
            var image = Resources.Load<Sprite>(item.Icon + "/" + value);
            spriteList.Add(image);
        }
        
        CharacterDressUp.Instance.ChangeClothes(item.type, spriteList);
        //if (ClothesMain.CurSelectIndex >= ClothesDataBase.Instance.clothesConfigs.Count)
        //{
        //    selPanel.TakePhotoProgress();
        //}
    }
    /// <summary>
    /// 根据路径设置子物体Icon的图片
    /// </summary>
    /// <param name="iconPath">Resources下的Sprite路径，如"Icons/hat1"</param>
    public void SetIcon(ClothesItem item)
    {
        Item = item;
        string iconPath = item.Icon;
        Sprite sprite = Resources.Load<Sprite>(iconPath+"/"+item.idList[0]);
        if (sprite == null)
        {
            return;
        }
        IconImage.sprite = sprite;
        CgClothesBtn.gameObject.SetActive(false);
    }

    public void ClickItem()
    {
        var ItemList = ClothesDataBase.Instance.allClothes.FindAll(c => c.type == (ClothesType)ClothesMain.CurSelectIndex);
        selPanel.RightBtn.gameObject.SetActive(ClothesMain.CurSelectIndex<4);
        selPanel.LeftBtn.gameObject.SetActive(ClothesMain.CurSelectIndex > 1);
        ItemOnClick(Item);
        for (int j = 0; j < ItemList.Count; j++)
        {
            Transform child = selPanel.SelectItemContent.transform.GetChild(j);
            child.gameObject.SetActive(j == selPanel.Curselitem);
        }
    }
}
