using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour
{

    private Image Sche;
    private Image Bgimage;
    private UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        Sche = transform.Find("Aperture/Sche").GetComponent<Image>();
        Bgimage = transform.GetComponent<Image>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        StartCoroutine(AddSche(0.5f, 10,() =>
        {
            Debug.Log("sche" + Sche.fillAmount);
            if (Sche.fillAmount < 0.9f)
            {
                Sche.fillAmount += 0.1f;
            }
            else {
                Sche.fillAmount = 1;
                Debug.Log("enter selectpanel");
                uiManager.ShowAndCloseOtherPanel("selectpanel");
            }
        }));
    }

    void OnDestroy()
    {
        Sche.fillAmount = 0;
    }
    IEnumerator AddSche(float interval,float num, System.Action action)
    {
        while (num>=0)
        {
            num -= 1;
            yield return new WaitForSeconds(interval);
            action?.Invoke();
        }
    }
}
