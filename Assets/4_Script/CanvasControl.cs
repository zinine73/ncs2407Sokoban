using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    [SerializeField] private Button btnAudio;
    [SerializeField] private Button btnRestart; // 다시하기 버튼
    [SerializeField] private Button btnUndo; // 되돌리기 버튼
    [SerializeField] private GameObject panelTitle; // 타이틀 화면
    [SerializeField] private GameObject panelChooseMap; // 맵고르기 화면
    [SerializeField] private GameObject panelPlay; // 플레이 화면
    [SerializeField] private GameObject panelGoal; // 클리어 시 화면
    [SerializeField] private GameObject panelController; // 안드로이드 시 입력 UI 패널
    [SerializeField] private MapButton mapButtonPrefab; // 맵버튼프레팝 연결
    [SerializeField] private RectTransform contentTransform; // 스크롤뷰에서 버튼프레팝 들어갈 위치
    [SerializeField] private Slider sliderZoom; // 화면 줌 슬라이더
    [SerializeField] private Sprite[] audioOnOff;
    private MapButton[] mapButton = null; // 맵버튼 배열

    // 버튼이 미리 만들어져 있어야 클리어여부가 제대로 표시된다. 그래서 start가 이닌 awake
    private void Awake()
    {
        // 게임매니저에 있는 최대 맵 갯수만큼 배열을 만든다
        mapButton = new MapButton[GameManager.MAX_MAP_COUNT];
        for (int i = 0; i < GameManager.MAX_MAP_COUNT; i++)
        {
            // 스크롤뷰에 맵버튼프레팝으로 클론 만들어서 넣는다
            mapButton[i] = Instantiate(mapButtonPrefab, contentTransform);
            // 번호는 1부터 시작하게
            mapButton[i].SetNumber(i + 1);
        }
        // 화면 줌 슬라이더의 초기값 설정
        sliderZoom.minValue = GameManager.MAX_ZOOM;
        sliderZoom.maxValue = GameManager.MIN_ZOOM;
        sliderZoom.value = GameManager.START_ZOOM;

        AudioState audio = (AudioState)PlayerPrefs.GetInt(GameManager.KEY_AUDIO, (int)AudioState.Enable);
        btnAudio.GetComponent<Image>().sprite = audioOnOff[(int)audio];
    }

    public void SetAudioOnOff()
    {
        AudioState value = GameManager.instance.AudioEnable;
        if (value == AudioState.Disable) value = AudioState.Enable;
        else value = AudioState.Disable;
        GameManager.instance.AudioEnable = value;
        PlayerPrefs.SetInt(GameManager.KEY_AUDIO, (int)value);
        btnAudio.GetComponent<Image>().sprite = audioOnOff[(int)value];
    }

    /// <summary>
    /// 슬라이드로 줌을 조정한다
    /// </summary>
    public void SetSliderZoom()
    {
        GameManager.instance.TargetZoom = sliderZoom.value;
    }

    /// <summary>
    /// 맵 클리어 정보 지정. 번호 위에 별이 표시된다. 다시하기와 되돌리기 버튼은 비활성화
    /// </summary>
    /// <param name="number">클리어한 맵 번호</param>
    public void SetMapCleared(int number)
    {
        mapButton[number - 1].SetCleared(true);
        btnRestart.interactable = false;
        btnUndo.interactable = false;
    }

    /// <summary>
    /// 게임 상태에 따른 패널 켜고 끄기
    /// </summary>
    /// <param name="state">게임 상태</param>
    public void SetView(GameState state)
    {
        if (state == GameState.Title)
        {
            panelTitle.SetActive(true);
            panelChooseMap.SetActive(false);
            panelPlay.SetActive(false);
            panelGoal.SetActive(false);
            panelController.SetActive(false);
        }
        else if (state == GameState.ChooseMap)
        {
            panelTitle.SetActive(false);
            panelChooseMap.SetActive(true);
            panelPlay.SetActive(false);
            panelGoal.SetActive(false);
            panelController.SetActive(false);
            
            int lastClear = 0;
            string str = PlayerPrefs.GetString(GameManager.KEY_PP);
            for (int i = 0; i < GameManager.MAX_MAP_COUNT; i++)
            {
                mapButton[i].DisableAd();
                if (str[i].Equals('X'))
                {
                    mapButton[i].GetComponent<Button>().interactable = false;
                    mapButton[i].SetCleared(false);
                }
                else
                {
                    if (lastClear < i)
                    {
                        lastClear = i;
                    }
                    mapButton[i].GetComponent<Button>().interactable = true;
                    // 맵 클리어 상황에 따라 별 표시
                    mapButton[i].SetCleared(str[i].Equals('C'));
                }
            }
            if (lastClear + 1 < GameManager.MAX_MAP_COUNT)
            {
                mapButton[lastClear + 1].LoadAd();
            }
        }
        else if (state == GameState.Goal)
        {
            panelGoal.SetActive(true);
        }
        else // Play
        {
            panelTitle.SetActive(false);
            panelChooseMap.SetActive(false);
            panelPlay.SetActive(true);
            panelGoal.SetActive(false);
#if UNITY_ANDROID && !UNITY_EDITOR
            panelController.SetActive(true);
#endif
            btnRestart.interactable = false;
            btnUndo.interactable = false;
        }
    }

    /// <summary>
    /// 되돌리기 버튼 켜고 끄기
    /// </summary>
    /// <param name="value">켜고 끄기 값</param>
    public void SetUndoButton(bool value)
    {
        btnUndo.interactable = value;
    }

    /// <summary>
    /// 다시하기 버튼 켜고 끄기
    /// </summary>
    /// <param name="value">켜고 끄기 값</param>
    public void SetRestartButton(bool value)
    {
        btnRestart.interactable = value;
    }
}
