using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public SpriteRenderer hair;      // 头发层
    public SpriteRenderer face;      // 脸部层
    public SpriteRenderer cloth;      // 身体层
    public SpriteRenderer shoes;     // 鞋子层
    public SpriteRenderer accessories; // 配饰层

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
    public void ChangeClothes(ClothesType type, Sprite newSprite)
    {
        //根据换装的类型获取中心点播放星星特效，星星特效怎么手搓一个？
        switch (type)
        {
            case ClothesType.Hair:
                hair.sprite = newSprite;
                break;
            case ClothesType.Face:
                face.sprite = newSprite;
                break;
            case ClothesType.Clothes:
                cloth.sprite = newSprite;
                break;
            case ClothesType.Shoes:
                shoes.sprite = newSprite;
                break;
            case ClothesType.Accessories:
                accessories.sprite = newSprite;
                break;
        }
    }
}
