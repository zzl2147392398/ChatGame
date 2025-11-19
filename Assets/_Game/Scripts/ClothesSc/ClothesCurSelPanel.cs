using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ClothesCurSelPanel : MonoBehaviour
{
    public Button LeftBtn;
    public Button RightBtn;
    private GameObject ScheContent;
    private GameObject SelectItemView;
    public GameObject SelectItemContent;
    private GameObject SelectItem;
    private GameObject ScheItem;
    private Button DownLoadBtn;
    private Button CameraBtn;
    private Button GoOutBtn;
    public Transform Tips;
    private Effector2D Effector;
    public Transform characterObj;
    private float timer = 0f;
    private float interval = 2f;
    public bool bisPlayAni = true;
    private Vector2 targetPos = new Vector2(0, 320);
    public int Curselitem;
    
    // Start is called before the first frame update
    void Start()
    {
        LeftBtn = transform.Find("stage/Left").GetComponent<Button>();
        RightBtn = transform.Find("stage/Right").GetComponent<Button>();
        DownLoadBtn = transform.Find("stage/DownLoadBtn").GetComponent<Button>();
        GoOutBtn = transform.Find("GoOutBtn").GetComponent<Button>();
        CameraBtn = transform.Find("stage/CameraBtn").GetComponent<Button>();
        SelectItemView = transform.Find("SelectItemView").gameObject;
        ScheContent  = transform.Find("ScheView/Viewport/Content").gameObject;
        SelectItemContent = transform.Find("SelectItemView/Viewport/Content").gameObject;
        SelectItem = Resources.Load<GameObject>("Prefabs/Clothes/SelectItem");
        ScheItem = Resources.Load<GameObject>("Prefabs/Clothes/ScheItem");
        characterObj = transform.Find("stage/Root");
        Tips = transform.Find("TipsEff");
        //RefershSelectItemList();
        //Vector3 startPos = new Vector3(0,-302f,0);
        //SelectItemView.GetComponent<RectTransform>().localPosition = startPos;
        SelectItemView.GetComponent<RectTransform>().DOMoveY(320, 1f).SetEase(Ease.OutCubic);
        LeftBtn.onClick.AddListener(() => ChangeCurScheBtn(false));
        RightBtn.onClick.AddListener(() => ChangeCurScheBtn(true));
        ChangeCurScheBtn(true);
        Curselitem = -1;
        var endscale = characterObj.localScale*2;
        //characterObj.localScale = new Vector3(characterObj.localScale.x * 2, characterObj.localScale.y * 2, 1);// Vector3.one * 2f;
        Sequence seq = DOTween.Sequence();
        seq.Append(characterObj.DOScale(endscale, 1).SetEase(Ease.OutBack));
           //.Append(characterObj.DOMoveY(0, 1));
    }
    private void Update()
    {
        if (bisPlayAni)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                //触发函数
                TipsEffect();
                timer = 0f; // 重置计时器
                bisPlayAni = false;
            }
        }

    }

    private void OnDestroy()
    {
        timer = 0;
    }
    private void RefreshScheList()
    {
        //1.根据进度CurSelectIndex来进行调用ClothesDataBase.Instance.allClothes来进行数据的获取，里面会有type类型根据进度对应拿到所有相同type类型的进行填充展示
        //2.
        var curindex = ClothesMain.CurSelectIndex;
        var ItemList= ClothesDataBase.Instance.clothesConfigs;
        var contentTransform = ScheContent.transform;
            for (int i = 0; i < contentTransform.childCount - 1; i++)
            {
                var itemCom = contentTransform.GetChild(i).gameObject.GetComponent<ClothesSchetItem>();
                var status = CurSche.NOSec;
                if (i + 1 == curindex)
                {
                    status = CurSche.CurSec;
                }
                else if (i + 1 < curindex)
                {
                    status = CurSche.FinishSec;
                }
                itemCom.SetInfo(ItemList[i], status);
                //DestroyImmediate(child);
            }
        for (int i = 0; i < ItemList.Count; i++)
        {
            ClothesSchetItem itemCom;
            if (contentTransform.childCount != ItemList.Count)
            {
                var item = Instantiate(ScheItem, ScheContent.transform);
                itemCom = item.GetComponent<ClothesSchetItem>();
            }
            else
            {
                itemCom = contentTransform.GetChild(i).gameObject.GetComponent<ClothesSchetItem>();
            }
            var status = CurSche.NOSec;
             if (i+1 == curindex)
            {
                status = CurSche.CurSec;
            }
            else if (i+1 < curindex)
            {
                status = CurSche.FinishSec;
            }
            itemCom.SetInfo(ItemList[i],status);
            //btn.onClick.AddListener(() => ItemOnClick(ItemList[i]));
        }
    }

    private void RefershSelectItemList()
    {
        //1.根据进度和是否选择做表现，如果有物品就不进行加载，没有进行加载，
        //2.刷新列表当中的表现具体item为ClothesSchetItem，根据进度CurSelectIndex来进行调用SetStatus，进度小于循环i就是1，等于就是2，小于就是3
        var curindex = ClothesMain.CurSelectIndex;
        var ItemList = ClothesDataBase.Instance.allClothes.FindAll(c => c.type == (ClothesType)curindex);
        if (SelectItemContent != null)
        {
            var contentTransform = SelectItemContent.transform;
            for (int i = contentTransform.childCount - 1; i >= 0; i--)
            {
                var child = contentTransform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }
        for (int i = 0; i < ItemList.Count; i++)
        {
            var item = Instantiate(SelectItem, SelectItemContent.transform);
            var itemCom = item.GetComponent<ClothesSeltem>();
            itemCom.SetIcon(ItemList[i]);
            
        }
    }

    private void ItemOnClick(ClothesItem item)
    {
        //1.出现星星特效，具体需求是我会根据具体的物体type然后去调用CharacterDressUp中的ChangeClothes方法
        //RightBtn.gameObject.SetActive(true);
        //LeftBtn.gameObject.SetActive(true);
        //var image = Resources.Load<Sprite>(item.Icon);
        //CharacterDressUp.Instance.ChangeClothes(item.type, image);
        //if(ClothesMain.CurSelectIndex>= ClothesDataBase.Instance.clothesConfigs.Count)
        //{
        //    TakePhotoProgress();
        //}
    }
    private void ChangeCurScheBtn(bool bIsNext)
    {

        if (bIsNext && ClothesMain.CurSelectIndex <= ClothesDataBase.Instance.clothesConfigs.Count)
        {
            ClothesMain.CurSelectIndex++;
        }
        else if(ClothesMain.CurSelectIndex>=0)
        {
            ClothesMain.CurSelectIndex--;
        }
        RefershSelectItemList();
        RefreshScheList();
        RightBtn.gameObject.SetActive(false);
        LeftBtn.gameObject.SetActive(false);
    }
    private void TipsEffect()
    {
        Tips.gameObject.SetActive(true);
        //1,固定出现在一个位置进行点击动画播放
        Sequence sequence = DOTween.Sequence();
        
        // 初始向右旋转45度
        sequence.Append(Tips.DORotate(new Vector3(0, 0, 45), 0.5f));

        // 向左旋转90度（从45到-45）
        sequence.Append(Tips.DORotate(new Vector3(0, 0, -45), 0.5f));

        // 向右旋转90度（从-45回到45）
        sequence.Append(Tips.DORotate(new Vector3(0, 0, 0), 0.5f));

        sequence.AppendInterval(1f);

        // 设置循环
        sequence.SetLoops(-1, LoopType.Restart);
    }

    //拍照进度
    public void TakePhotoProgress()
    {
        //进度list和展示list消失
        //出现画框和中间点击按钮
        ScheContent.SetActive(false);
        GoOutBtn.gameObject.SetActive(false);
        Vector2 startPos = new Vector2(0, -302f);
        SelectItemView.GetComponent<RectTransform>().DOAnchorPos(startPos, 1f).SetEase(Ease.OutCubic);
    }
}


