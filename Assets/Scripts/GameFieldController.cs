using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameFieldController : MonoBehaviour
{

    public GameObject gameField;
    public GameObject cellsContainer;
    public GameObject cellPrefab;
    public float gap = 0.15f;

    private CoreLogic logic;

    private List<List<CoreLogic.CellData>> field;

    private readonly List<GameObject> valuedCells = new();

    float gameFieldWidth;
    float gameFieldHeight;
    float cellWidth;
    float cellHeight;


    [SerializeField]
    private GameObject managers;
    private SwipeDetection swipeDetection;

    public int score = 0;

    [SerializeField]
    private TMP_Text scoreLabel;
    [SerializeField]
    private TMP_Text bestScoreLabel;

    private AudioSource audioSource;

    private void Awake()
    {
        swipeDetection = managers.GetComponent<SwipeDetection>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        swipeDetection.OnSwipe += OnSwipe;
    }

    private void OnDisable()
    {
        swipeDetection.OnSwipe -= OnSwipe;
    }

    private void OnSwipe(SwipeDetection.Direction direction)
    {
        switch (direction) {
            case SwipeDetection.Direction.Up:
                MakeMove(CoreLogic.Direction.Up);
                break;
            case SwipeDetection.Direction.Right:
                MakeMove(CoreLogic.Direction.Right);
                break;
            case SwipeDetection.Direction.Down:
                MakeMove(CoreLogic.Direction.Down);
                break;
            case SwipeDetection.Direction.Left:
                MakeMove(CoreLogic.Direction.Left);
                break;
        }
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("new game") == 1)
        {
            PlayerPrefs.SetInt("new game", 0);
            PlayerPrefs.SetInt("score", 0);
            int savedBestScore = PlayerPrefs.GetInt("bestScore");
            if (savedBestScore < score)
            {
                PlayerPrefs.SetInt("bestScore", score);
            }
            PlayerPrefs.SetString("field", "");
        }

        score = PlayerPrefs.GetInt("score");
        scoreLabel.text = score.ToString();

        int bestScore = PlayerPrefs.GetInt("bestScore");
        bestScoreLabel.text = bestScore.ToString();

        var cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Screen.width / Screen.height;
        gameField.transform.localScale = Vector3.one * cameraWidth * 0.9f;

        gameFieldWidth = gameField.GetComponent<SpriteRenderer>().bounds.size.x;
        gameFieldHeight = gameField.GetComponent<SpriteRenderer>().bounds.size.y;

        cellWidth = (gameFieldWidth - 5 * gap) / 4;
        cellHeight = (gameFieldHeight - 5 * gap) / 4;

        logic = ScriptableObject.CreateInstance<CoreLogic>();

        var savedJsonString = PlayerPrefs.GetString("field");
        var savedField = JsonConvert.DeserializeObject<List<List<CoreLogic.CellData>>>(savedJsonString);

        bool isAnimatedRender = savedField == null;
        if (savedField != null) {
            field = savedField;
        }
        else
        {
            field = logic.getStartField();
        }

        SaveCurrentField();
        BuuildFieldBackground();
        RenderField(isAnimatedRender);
    }

    private void MakeMove(CoreLogic.Direction direction)
    {
        List<List<CoreLogic.CellData>> fieldBeforeMove = new List<List<CoreLogic.CellData>>(field);
        field = logic.makeMove(field, direction);
        if (logic.ListsEqual(field, fieldBeforeMove))
        {
            // if out move didn't change something, then not generate new element
            return;
        }

        AddNewCell();
        RenderField();
        score += logic.GetScore(field);
        scoreLabel.text = score.ToString();

        SaveCurrentField();
        PlayerPrefs.SetInt("score", score);

        PlayMoveSoundIfNeeded();

        if (!logic.isPossibleToMove(field))
        {
            OnLoose();
        }
    }

    private void PlayMoveSoundIfNeeded()
    {
        var isSoundOn = PlayerPrefs.GetInt("isSoundOn");
        if (isSoundOn == 1) {
            audioSource.Play();
        }
    }

    private void SaveCurrentField()
    {
        var jsonString = JsonConvert.SerializeObject(field);
        PlayerPrefs.SetString("field", jsonString); 
    }

    private void AddNewCell()
    {
        (int, int)? newCellPosition = logic.getPositionForNewCell(field);
        if (!newCellPosition.HasValue)
        {
            OnLoose();
            return;
        }

        int newCellValue = logic.getNewCellValue();
        var newCell = new CoreLogic.CellData(newCellValue, false)
        {
            isNew = true
        };
        field[newCellPosition.GetValueOrDefault().Item1][newCellPosition.GetValueOrDefault().Item2] = newCell;
    }

    private void OnLoose()
    {
        PlayerPrefs.SetInt("score", score);
        int bestScore = PlayerPrefs.GetInt("bestScore");
        if (bestScore < score) {
            PlayerPrefs.SetInt("bestScore", score);
        }
        PlayerPrefs.SetString("field", "");
        swipeDetection.OnSwipe -= OnSwipe;
        StartCoroutine(LoadGameOverSceneWithDelay());
        return;
    }

    private IEnumerator LoadGameOverSceneWithDelay()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("GameOverScene");
    }

    private void BuuildFieldBackground()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var cell = Instantiate(cellPrefab);
                cell.GetComponent<CellController>().SetValue(0);
                cell.transform.parent = cellsContainer.transform;
                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                cell.transform.localPosition = PositionForCell(i, j);
            }
        }
        cellsContainer.transform.localPosition = gameField.transform.localPosition - new Vector3(gameFieldWidth / 2, gameFieldHeight / 2, 0);
    }

    private Vector3 PositionForCell(int i, int j)
    {
        return new Vector3(gap * (j + 1) + cellWidth * j, gap * (i + 1) + cellHeight * i) +
               new Vector3(cellWidth / 2, cellHeight / 2);
    }

    private Vector3 PositionForCellReversed(int i, int j, List<List<CoreLogic.CellData>> field)
    {
        var iReversed = field.Count - 1 - i;
        return PositionForCell(iReversed, j);
    }

    private void RenderField(bool animated = true)
    {
        foreach(var cell in valuedCells)
        {
            Destroy(cell);
        }
        valuedCells.Clear();
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.GetComponent<CellController>().SetValue(field[i][j].value);
                cell.transform.parent = cellsContainer.transform;
                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                cell.transform.localPosition = PositionForCellReversed(i, j, field);

                valuedCells.Add(cell);

                if (animated)
                {
                    if (field[i][j].isNew)
                    {
                        cell.GetComponent<CellController>().playPopUpAnimation(cell.transform.localScale);
                    }

                    PerformMovementAnimation(cell, field[i][j], i, j, field);
                }      
            }
        }
    }

    private void PerformMovementAnimation(  GameObject cell,
                                            CoreLogic.CellData cellData,
                                            int i,
                                            int j,
                                            List<List<CoreLogic.CellData>> field)
    {
        if (cellData.previousIndex.HasValue && cellData.previousIndex != (i, j))
        {
            var iPrevious = cellData.previousIndex.Value.Item1;
            var jPrevious = cellData.previousIndex.Value.Item2;

            var previousPosition = PositionForCellReversed(iPrevious, jPrevious, field);

            var newPosition = PositionForCellReversed(i, j, field);
            cell.GetComponent<CellController>().playAnimationOfMove(previousPosition, newPosition);
        }
    }

    public void tappedMenuButton()
    {
        SceneManager.LoadSceneAsync("Menu Scene");
    }
}
