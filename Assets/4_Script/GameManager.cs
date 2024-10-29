using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct MapStruct // 맵구조체
{
    public int SizeX;
    public int SizeY;
    public string Data;
}

public enum MapEnum // 맵에 등장하는 오브젝트들의 타입
{
    Ground, Wall, Goal, Box, BoxOnGoal, Player
}

public enum GameState // 게임 상태
{
    Title, ChooseMap, Play, Goal
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // 싱글톤
    public static int MAX_MAP_COUNT = 96; // 전체 맵 갯수
    private const float MOVE_SPEED = 7.0f; // 플레이어 움직이는 속도
    [SerializeField] private CanvasControl canvas; // 캔버스
    [SerializeField] private GameObject mapPrefab; // 맵 오브젝트 프레팝
    [SerializeField] private GameObject ground; // 바닥 오브젝트들 부모
    [SerializeField] private GameObject walls; // 벽 오브젝트 부모
    [SerializeField] private GameObject goals; // 골 오브젝트 부모
    [SerializeField] private GameObject boxes; // 상자 오브젝트 부모
    [SerializeField] private GameObject player; // 플레이어
    private MapEnum[,] board; // 로직을 검사할 맵
    private GameState state; // 게임 상태 저장 변수
    private List<Vector2> boxList; // 로직에 사용되는 상자 리스트
    private List<Vector2> goalList; // 로직에 사용되는 골 리스트
    private List<Vector2> copyList; // 재실행에 사용되는 상자 리스트
    private Vector2 copyPosition; // 재실행에 사용되는 플레이어 위치
    private Vector2 playerPosition; // 플레이어 위치
    private Vector2 playerDirection; // 플레이어 방향
    public int mapNumber; // 맵 번호
    public MapStruct map; // 맵 정보

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // 시작 맵 번호는 1
        mapNumber = 1;
        // 로직에 사용될 상자와 골의 리스트 만들기
        boxList = new List<Vector2>();
        goalList = new List<Vector2>();

        ClearAllObject();
        ChangeGameState(GameState.Title);
    }

    // 자식 오브젝트 지우기
    private void DeleteChild(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    // 게임 변수들 초기화
    private void ClearAllObject()
    {
        // 자식 오브젝트 지우기
        DeleteChild(ground);
        DeleteChild(walls);
        DeleteChild(goals);
        DeleteChild(boxes);
        // 맵고르기 화면에서는 플레이어 안 보이게
        player.SetActive(false);
    }

    // 게임 상태 바꾸기
    private void ChangeGameState(GameState value)
    {
        state = value;
        canvas.SetView(state);
    }

    // x,y 좌표에 따른 map에서의 위치 얻어오기
    private Vector2 GetPosition(float x, float y)
    {
        return new Vector2(x - map.SizeX * 0.5f, y - map.SizeY * 0.5f);
    }

    private void CheckKeyInput()
    {
        // 키가 눌리면
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            // x,y 축 값을 -1,0,1 로만 받아서
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            // 대각선 입력은 무시하고
            if (moveX == 0 || moveY == 0)
            {
                // 방향값을 정한다
                playerDirection = new Vector2(moveX, moveY);
                // 방향값에 따른 플레이어 애니메이션 실행
                player.GetComponent<PlayerAnimation>().SetAnimation(playerDirection);

                // 움직일 위치를 변수로 정하고
                Vector2 tryPlayerPos = playerPosition + playerDirection; 
                // 바닥이면 (board엔 grpund 아니면 wall만 남아있다)
                if (board[(int)tryPlayerPos.x, (int)tryPlayerPos.y] == MapEnum.Ground)
                {
                    // 움직임이 발생했으므로 버튼 다시하기, 되돌리기 버튼 활성화
                    canvas.SetRestartButton(true);
                    canvas.SetUndoButton(true);
                    // 움직일 위치에 상자가 있다면
                    if (boxList.Contains(tryPlayerPos))
                    {
                        // 상자가 움직일 위치도 변수로 정하고
                        Vector2 tryBoxPos = tryPlayerPos + playerDirection;
                        // 바닥이면 상자가 움직일 수 있다
                        if (board[(int)tryBoxPos.x, (int)tryBoxPos.y] == MapEnum.Ground)
                        {
                            // 움직일 위치에 상자가 없어야 움직일 수 있다
                            if (boxList.Contains(tryBoxPos) == false)
                            {
                                // 움직여야 할 상자를 상자리스트 중에서 찾는다
                                for (int i = 0; i< boxList.Count; i++)
                                {
                                    if ((tryPlayerPos.x == boxList[i].x) && (tryPlayerPos.y == boxList[i].y))
                                    {
                                        // 상자 이동
                                        boxList[i] += playerDirection;
                                        // 플레이어 이동
                                        playerPosition = tryPlayerPos;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else // 움직일 위치에 상자가 없으므로
                    {
                        // 플레이어 이동
                        playerPosition = tryPlayerPos;
                    }
                }
            }   
        }
    }

    private void Update()
    {
        // 플레이 상태에서만 아래 로직을 타면 된다
        if (state != GameState.Play) return;

        // 키입력 체크하고 방향, 위치값 변경
        CheckKeyInput();
        // 플레이어 이미지 위치 변경
        player.transform.position = Vector2.Lerp(player.transform.position,
            GetPosition(playerPosition.x, playerPosition.y), Time.deltaTime * MOVE_SPEED);

        // 모든 골 위에 상자가 있는지 여부(클리어)의 변수 선언: 일단 true로
        bool isAllBoxGoal = true;
        // boxes 아래 있는 child 상자 갯수만큼 검사
        for (int i = 0; i < boxes.transform.childCount; i++)
        {
            // 상자를 차례대로 얻어와서
            Component child = boxes.transform.GetChild(i);
            // 위치를 조정
            child.transform.position = Vector2.Lerp(child.transform.position,
                GetPosition(boxList[i].x, boxList[i].y), Time.deltaTime * MOVE_SPEED);
            // 골 위치에 상자가 있는지 검사
            bool check = goalList.Contains(boxList[i]);
            // 하나라도 상자 위에 있는게 아니면 클리어 여부는 false
            if (check == false) isAllBoxGoal = false;
            // 골위에 있는 상자는 sprite를 다르게 표시
            child.GetComponent<StuffChange>().IsOnGoal = check;
        }
        // 클리어했다면
        if (isAllBoxGoal == true)
        {
            // todo end goal
        }
    }
    
    // JSON 데이터로부터 맵 데이터 만들기
    private void MakeMapDataFromJSON()
    {
        board = new MapEnum[map.SizeX, map.SizeY]; // 사용되는 맵 크기만큼 로직 보드 설정
        boxList.Clear(); // 상자 리스트 클리어
        goalList.Clear(); // 골 리스트 클리어
        int index = 0;
        for (int y = 0; y < map.SizeY; y++)
        {
            for (int x = 0; x < map.SizeX; x++)
            {
                board[x, y] = MapEnum.Ground; // 기본으로 바닥을 모두 깐다
                switch (map.Data[index]) // 인덱스에 해당하는 char를 가지고 데이터를 만든다
                {
                    case 'X': // 벽
                        board[x, y] = MapEnum.Wall;
                        break;
                    case '*': // 상자
                        boxList.Add(new Vector2(x, y));
                        break;
                    case '.': // 골
                        goalList.Add(new Vector2(x, y));
                        break;
                    case '&': // 골 위의 상자이므로 상자와 골 리스트에 각각 넣어준다
                        boxList.Add(new Vector2(x, y));
                        goalList.Add(new Vector2(x, y));
                        break;
                    case '@': // 플레이어
                        playerPosition.x = x;
                        playerPosition.y = y;
                        break;
                }
                index++; // 인덱스 + 1로 다음 char 탐색
            }
            index++; // 줄바꿈 char가 있으므로 index + 1            
        }
    }

    // 화면에 오브젝트 생성
    private void MakeObject(MapEnum index, int x, int y)
    {
        // 만들어질 위치를 인덱스 값에 따라 지정하고
        Transform target = ground.transform;
        if (index == MapEnum.Goal)      target = goals.transform;
        else if (index == MapEnum.Wall) target = walls.transform;
        else if (index == MapEnum.Box)  target = boxes.transform;

        // 프레팝으로 오브젝트 생성 후, 타입을 지정한다
        Instantiate(mapPrefab, GetPosition(x, y), Quaternion.identity, target)
            .GetComponent<StuffChange>().MapEnumType = index;
    }

    // 맵 정보 초기화
    private void InitializeMap()
    {
        // 맵에 있는 모든 오브젝트를 지우고
        ClearAllObject();
        // 맵 오브젝트
        for (int y = 0; y < map.SizeY; y++)
        {
            for (int x = 0; x < map.SizeX; x++)
            {
                // 바닥은 맵 전체로 만들고
                MakeObject(MapEnum.Ground, x, y);
                // 벽 만들고
                if (board[x, y] == MapEnum.Wall) MakeObject(MapEnum.Wall, x, y);
                // 골 리스트에 좌표가 있으면 골 만들고
                if (goalList.Contains(new Vector2(x, y))) MakeObject(MapEnum.Goal, x, y);
                // 상자 리스트에 좌표가 있으면 상자 만든다
                if (boxList.Contains(new Vector2(x, y))) MakeObject(MapEnum.Box, x, y);
            }
        }
        // 카피리스트에 상자이스트 복사(Linq)
        copyList = boxList.ToList();
        // 카피위치에 플레이어위치 복사
        copyPosition = playerPosition;
        // 플레이어
        player.transform.position = GetPosition(playerPosition.x, playerPosition.y);
        player.SetActive(true);
        // 정면 바라보는 애니메이션 한번 실행해준다
        player.GetComponent<PlayerAnimation>().SetAnimation(Vector2.zero);
        // 게임 상태를 바꾼다
        ChangeGameState(GameState.Play);
    }

    /// <summary>
    /// 지정된 맵 번호로 게임 시작하기
    /// </summary>
    /// <param name="number">맵 번호</param>
    public void StartPlay(int number)
    {
        mapNumber = number;
        MakeMapDataFromJSON();
        InitializeMap();
    }

    /// <summary>
    /// 현재 맵 다시 시작하기
    /// </summary>
    public void Restart()
    {
        // 정면 바라보게 하고
        player.GetComponent<PlayerAnimation>().SetAnimation(Vector2.zero);
        // 상자리스트에 카피리스트 복사해서 원래 상자위치 복원(Linq)
        boxList = copyList.ToList();
        // 플레이어 위치도 카피위치에서 복원
        playerPosition = copyPosition;
        // 다시하기와 되돌리기 버튼은 안눌리게
        canvas.SetRestartButton(false);
        canvas.SetUndoButton(false);
    }

    /// <summary>
    /// 맵 고르기 화면 부르기
    /// </summary>
    public void ChooseMap()
    {
        ClearAllObject();
        ChangeGameState(GameState.ChooseMap);
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 되돌리기
    /// </summary>
    public void Undo()
    {
        canvas.SetUndoButton(false);
    }
}
