using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumber;
    [SerializeField] private Image imageCleared;
    private int number;
    private Admob admob;

    private void Awake()
    {
        admob = GetComponent<Admob>();
    }

    public void DisableAd()
    {
        admob.DestroyAd();
    }

    public void LoadAd()
    {
        admob.LoadAd();
    }

    public void ShowAd()
    {
        admob.ShowAd();
    }

    public void RewardAd()
    {
        GameManager.instance.SetOpenClearMapData(number, "O");
        GetComponent<Button>().interactable = true;
        textNumber.alpha = 1.0f;
    }

    public void SetNumberAlpha()
    {
        textNumber.alpha = 0.5f;
    }

    /// <summary>
    /// 맵 번호 지정하기
    /// </summary>
    /// <param name="value">맵 번호</param>
    public void SetNumber(int value)
    {
        number = value;
        textNumber.text = $"{number:00}";
    }

    /// <summary>
    /// 클리어 여부 지정하기
    /// </summary>
    /// <param name="value">클리어 여부</param>
    public void SetCleared(bool value)
    {
        imageCleared.gameObject.SetActive(value);
    }

    /// <summary>
    /// 맵 데이터 부르고 시작하기
    /// </summary>
    public void LoadMap()
    {
        GameManager.instance.StartPlay(number);
    }
}
