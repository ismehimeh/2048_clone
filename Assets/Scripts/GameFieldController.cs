using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameFieldController : MonoBehaviour
{

    [SerializeField]
    private GameObject gameField;

    [SerializeField]
    private GameObject cellsContainer;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private float gap = 0.15f;

    [SerializeField]
    private GameObject managers;

    [SerializeField]
    private TMP_Text scoreLabel;

    [SerializeField]
    private TMP_Text bestScoreLabel;

    private CoreLogic logic;
    private SwipeDetection swipeDetection;
    private AudioSource audioSource;

    private float gameFieldWidth;
    private float gameFieldHeight;
    private float cellWidth;
    private float cellHeight;

    private readonly List<GameObject> valuedCells = new();

    public int Score
    {
        get
        {
            return PlayerPrefs.GetInt(PrefsStrings.score);
        }

        set
        {
            scoreLabel.text = value.ToString();
            PlayerPrefs.SetInt(PrefsStrings.score, value);
        }
    }

    private List<List<CoreLogic.CellData>> field;
    private List<List<CoreLogic.CellData>> Field
    {
        set
        {
            Score += logic.GetScore(Field);
            SaveCurrentFieldToPrefs();
        }

        get
        {
            return field;
        }
    }

    private void Awake()
    {
        swipeDetection = managers.GetComponent<SwipeDetection>();
        audioSource = gameObject.GetComponent<AudioSource>();
        logic = ScriptableObject.CreateInstance<CoreLogic>();
    }

    private void OnEnable()
    {
        swipeDetection.OnSwipe += OnSwipe;
    }

    private void OnDisable()
    {
        swipeDetection.OnSwipe -= OnSwipe;
    }

    private void OnSwipe(Direction direction)
    {
        MakeMove(direction);
    }

    void Start()
    {
        SetupNewGameIfNeeded();
        SetupGUI();
        SetupGameFieldUI();

        var storedField = GetStoredFieldFromPrefs();
        Field = storedField ?? logic.getStartField();

        SaveCurrentFieldToPrefs();
        BuuildFieldBackground();

        bool isAnimatedRender = storedField == null;
        RenderField(isAnimatedRender);
    }

    private void SetupNewGameIfNeeded()
    {
        if (PlayerPrefs.GetInt(PrefsStrings.isNewGame) == 1)
        {
            PlayerPrefs.SetInt(PrefsStrings.isNewGame, 0);

            int savedBestScore = PlayerPrefs.GetInt(PrefsStrings.bestScore);
            PlayerPrefs.SetInt(PrefsStrings.bestScore, Mathf.Max(savedBestScore, Score));
            PlayerPrefs.SetInt(PrefsStrings.score, 0);

            PlayerPrefs.SetString(PrefsStrings.field, "");
        }
    }

    private void SetupGUI()
    {
        scoreLabel.text = Score.ToString();

        int bestScore = PlayerPrefs.GetInt(PrefsStrings.bestScore);
        bestScoreLabel.text = bestScore.ToString();
    }

    private void SetupGameFieldUI()
    {
        var cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Screen.width / Screen.height;
        gameField.transform.localScale = 0.9f * cameraWidth * Vector3.one;

        gameFieldWidth = gameField.GetComponent<SpriteRenderer>().bounds.size.x;
        gameFieldHeight = gameField.GetComponent<SpriteRenderer>().bounds.size.y;

        cellWidth = (gameFieldWidth - 5 * gap) / 4;
        cellHeight = (gameFieldHeight - 5 * gap) / 4;
    }

    private void MakeMove(Direction direction)
    {
        List<List<CoreLogic.CellData>> fieldBeforeMove = new(Field);

        var updatedField = logic.makeMove(Field, direction);

        if (logic.ListsEqual(Field, fieldBeforeMove))
            return;

        AddNewCell(ref updatedField);

        Field = updatedField;

        RenderField();
        PlayMoveSoundIfNeeded();

        if (!logic.isPossibleToMove(Field))
            OnLoose();
    }

    private void PlayMoveSoundIfNeeded()
    {
        var isSoundOn = PlayerPrefs.GetInt(PrefsStrings.isSoundOn);
        if (isSoundOn == 1) {
            audioSource.Play();
        }
    }

    private void SaveCurrentFieldToPrefs()
    {
        var jsonString = JsonConvert.SerializeObject(Field);
        PlayerPrefs.SetString(PrefsStrings.field, jsonString); 
    }

    private List<List<CoreLogic.CellData>> GetStoredFieldFromPrefs()
    {
        var savedJsonString = PlayerPrefs.GetString(PrefsStrings.field);
        return JsonConvert.DeserializeObject<List<List<CoreLogic.CellData>>>(savedJsonString);
    }

    private void AddNewCell(ref List<List<CoreLogic.CellData>> field)
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
        PlayerPrefs.SetInt(PrefsStrings.isNewGame, 1);

        int bestScore = PlayerPrefs.GetInt(PrefsStrings.bestScore);
        if (bestScore < Score) {
            PlayerPrefs.SetInt(PrefsStrings.bestScore, Score);
        }

        PlayerPrefs.SetString(PrefsStrings.field, "");
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

        var cellsContainerPosition = gameField.transform.localPosition -
                                        new Vector3(gameFieldWidth / 2, gameFieldHeight / 2, 0);
        cellsContainer.transform.localPosition = cellsContainerPosition;
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
                cell.GetComponent<CellController>().SetValue(Field[i][j].value);
                cell.transform.parent = cellsContainer.transform;
                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                cell.transform.localPosition = PositionForCellReversed(i, j, Field);

                valuedCells.Add(cell);

                if (animated)
                {
                    if (Field[i][j].isNew)
                    {
                        cell.GetComponent<CellController>().playPopUpAnimation(cell.transform.localScale);
                    }

                    PerformMovementAnimation(cell, Field[i][j], i, j, Field);
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

    public void TappedMenuButton()
    {
        SceneManager.LoadSceneAsync("Menu Scene");
    }
}
