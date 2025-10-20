using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClothesSeltem : MonoBehaviour
{
    Image IconImage;
    ClothesItem Item;
    public Button CgClothesBtn;
    private Renderer itemRenderer; // 物体的Renderer组件
    private float duration = 1f; // 动画持续时间
    private Vector3 targetPosition = new Vector3(0,0,0); // 目标位置
    CanvasGroup effCanvasGroup;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        transform.localPosition = new Vector3(0, -150, 0); // 初始位置在屏幕下方
        IconImage = transform.Find("Eff/Icon").GetComponent<Image>();
        CgClothesBtn = transform.Find("Eff/Button").GetComponent<Button>();
        transform.DOMove(targetPosition, duration)
                 .SetEase(Ease.Linear); // 可以根据需要更改缓动类型
        effCanvasGroup = transform.Find("Eff").GetComponent<CanvasGroup>();
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            effCanvasGroup.alpha = newAlpha;
        }
        effCanvasGroup.alpha = 1f;
        //itemRenderer.material.DOFade(1, duration).From(0) // 透明度从0到1
        //                    .SetEase(Ease.Linear); // 可以根据需要更改缓动类型
    }

    /// <summary>
    /// 根据路径设置子物体Icon的图片
    /// </summary>
    /// <param name="iconPath">Resources下的Sprite路径，如"Icons/hat1"</param>
    public void SetIcon(ClothesItem item)
    {
        Item = item;
        string iconPath = item.Icon;
        Sprite sprite = Resources.Load<Sprite>(iconPath);
        if (sprite == null)
        {
            return;
        }
        IconImage.sprite = sprite;
    }
}
