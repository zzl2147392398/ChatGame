using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 人物分层结构
public class CharacterDressUp : MonoBehaviour
{
    // 单例实例
    private static CharacterDressUp _instance;
    public static CharacterDressUp Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterDressUp>();
                if (_instance == null)
                {
                    Debug.LogError("场景中没有CharacterDressUp实例！");
                }
            }
            return _instance;
        }
    }

    [Header("身体部位")]
    public Image BcBg;      // 背景层
    public Image FrontCloth;      // 前衣层
    public Image FrontHair;      // 前发层
    public Image Face;      // 表情层
    public Image Hair;     // 后发层
    public Image Clothes; // 后衣层
    public Image HairAcs;   // 配饰层

    private void Awake()
    {
        // 保证单例唯一
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 换装方法
    public void ChangeClothes(ClothesType type, List<Sprite> newSprite)
    {
        //根据换装的类型获取中心点播放星星特效，星星特效怎么手搓一个？
        switch (type)
        {
            case ClothesType.BcBg:
                BcBg.sprite = newSprite[1];
                break;
            case ClothesType.Hair:
                FrontHair.sprite = newSprite[1];
                Hair.sprite = newSprite[0];
                break;
            case ClothesType.Clothes:
                FrontCloth.sprite = newSprite[1];
                Clothes.sprite = newSprite[0];
                break;
            case ClothesType.HairAcs:
                HairAcs.sprite = newSprite[0];
                break;
        }
    }
}
