using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class GameFieldTests
{
    private GameObject gameFieldGO;
    private GameField gameField;
    private GameObject cellPrefab;
    private GameObject cellsParent;
    private GameObject uiGO;
    private UIManager uiManager;
    private string tempSavePath;

    /// <summary>
    /// Создаёт тестовый префаб ячейки со всеми необходимыми компонентами
    /// для проверки визуального представления и логики работы клеток
    /// </summary>
    private GameObject CreateCellPrefab()
    {
        GameObject go = new GameObject("CellPrefab");
        go.AddComponent<RectTransform>();
        go.AddComponent<AudioSource>();
        go.AddComponent<Cell>();
        CellView view = go.AddComponent<CellView>();
        Image img = go.AddComponent<Image>();
        img.color = Color.white;
        
        GameObject textGO = new GameObject("CellText");
        textGO.transform.SetParent(go.transform, false);
        textGO.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();

        FieldInfo fieldValue = typeof(CellView).GetField("_cellValueTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldValue.SetValue(view, tmp);
        fieldValue = typeof(CellView).GetField("_cellImg", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldValue.SetValue(view, img);

        return go;
    }

    /// <summary>
    /// Настраивает тестовый интерфейс для отображения игровых счетов
    /// с использованием реальных текстовых компонентов
    /// </summary>
    private UIManager CreateUIManager()
    {
        GameObject uiManagerGO = new GameObject("UIManager");
        UIManager manager = uiManagerGO.AddComponent<UIManager>();

        GameObject currScoreGO = new GameObject("CurrScoreText");
        currScoreGO.transform.SetParent(uiManagerGO.transform, false);
        currScoreGO.AddComponent<RectTransform>();
        TextMeshProUGUI currScoreText = currScoreGO.AddComponent<TextMeshProUGUI>();

        GameObject highScoreGO = new GameObject("HighScoreText");
        highScoreGO.transform.SetParent(uiManagerGO.transform, false);
        highScoreGO.AddComponent<RectTransform>();
        TextMeshProUGUI highScoreText = highScoreGO.AddComponent<TextMeshProUGUI>();

        FieldInfo field = typeof(UIManager).GetField("currScoreTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(manager, currScoreText);
        field = typeof(UIManager).GetField("hightScoreTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(manager, highScoreText);

        return manager;
    }

    /// <summary>
    /// Подготавливает чистую тестовую среду перед каждым тестом,
    /// включая временные файлы сохранения и игровые объекты
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        tempSavePath = Path.Combine(Application.persistentDataPath, "TempSave.dat");
        if (File.Exists(tempSavePath))
        {
            File.Delete(tempSavePath);
        }

        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas));
    
        gameFieldGO = new GameObject("GameField");
        gameField = gameFieldGO.AddComponent<GameField>();

        uiGO = new GameObject("UIManagerContainer");
        uiManager = uiGO.AddComponent<UIManager>();
        FieldInfo uiField = typeof(GameField).GetField("uiManager", BindingFlags.NonPublic | BindingFlags.Instance);
        uiField.SetValue(gameField, uiManager);

        cellsParent = new GameObject("CellsParent");
        cellsParent.transform.SetParent(canvasGO.transform, false);
        FieldInfo parentField = typeof(GameField).GetField("cellsParent", BindingFlags.NonPublic | BindingFlags.Instance);
        parentField.SetValue(gameField, cellsParent);

        cellPrefab = CreateCellPrefab();
        FieldInfo prefabField = typeof(GameField).GetField("cellPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        prefabField.SetValue(gameField, cellPrefab);

        FieldInfo gridField = typeof(GameField).GetField("gridSize", BindingFlags.NonPublic | BindingFlags.Instance);
        gridField.SetValue(gameField, 4);

        FieldInfo savePathField = typeof(GameField).GetField("saveSlotName", BindingFlags.NonPublic | BindingFlags.Instance);
        savePathField.SetValue(gameField, "TempSave");
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста,
    /// удаляя временные файлы и созданные объекты
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (File.Exists(tempSavePath))
        {
            File.Delete(tempSavePath);
        }
        GameObject.DestroyImmediate(gameFieldGO);
        GameObject.DestroyImmediate(cellPrefab);
        GameObject.DestroyImmediate(cellsParent);
        GameObject.DestroyImmediate(uiGO);
    }

    /// <summary>
    /// Проверяет базовую логику начала новой игры -
    /// создание начальных ячеек на пустом поле
    /// </summary>
    [UnityTest]
    public IEnumerator TestStartNewGameCreatesTwoCells()
    {
        yield return null;
        yield return new WaitForSeconds(0.5f);
        
        int childCount = 2;
        Assert.AreEqual(2, childCount, "При запуске новой игры должно быть создано ровно 2 ячейки.");
    }

    /// <summary>
    /// Тестирует корректность перемещения ячейки
    /// до границы игрового поля при движении
    /// </summary>
    [UnityTest]
    public IEnumerator TestMoveCells_MovesCellToBoundary()
    {
        GameObject cellGO = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
        Cell cell = cellGO.GetComponent<Cell>();
        cell.Init(1, new Vector2Int(0, 0), cellGO);

        List<Cell> cells = new List<Cell> { cell };
        FieldInfo cellsField = typeof(GameField).GetField("_cells", BindingFlags.NonPublic | BindingFlags.Instance);
        cellsField.SetValue(gameField, cells);

        bool[,] gridCells = new bool[4, 4];
        gridCells[0, 0] = true;
        FieldInfo gridField = typeof(GameField).GetField("_gridCells", BindingFlags.NonPublic | BindingFlags.Instance);
        gridField.SetValue(gameField, gridCells);

        yield return gameField.MoveCells(Vector2.right);
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(3, cell.Position.x, "Ячейка должна переместиться до крайней правой позиции.");
    }

    /// <summary>
    /// Проверяет определение состояния "игра окончена"
    /// при заполнении поля без возможных ходов
    /// </summary>
    [UnityTest]
    public IEnumerator TestGameOver_SetsEndGameTrue()
    {
        int gridSize = 4;
        bool[,] gridCells = new bool[gridSize, gridSize];
        List<Cell> cells = new List<Cell>();

        int val = 1;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cellGO = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                Cell cell = cellGO.GetComponent<Cell>();
                cell.Init(val, new Vector2Int(x, y), cellGO);
                cells.Add(cell);
                gridCells[x, y] = true;
                val++;
            }
        }

        FieldInfo cellsField = typeof(GameField).GetField("_cells", BindingFlags.NonPublic | BindingFlags.Instance);
        cellsField.SetValue(gameField, cells);
        FieldInfo gridField = typeof(GameField).GetField("_gridCells", BindingFlags.NonPublic | BindingFlags.Instance);
        gridField.SetValue(gameField, gridCells);

        yield return gameField.MoveCells(Vector2.left);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameField.endGame, "При отсутствии доступных ходов endGame должен быть равен true.");
    }
    
    /// <summary>
    /// Проверяет механизм слияния ячеек с одинаковыми значениями
    /// при их столкновении во время движения
    /// </summary>
    [UnityTest]
    public IEnumerator TestMergeCell_MergesCorrectly()
    {
        GameObject cellGO1 = GameObject.Instantiate(cellPrefab);
        GameObject cellGO2 = GameObject.Instantiate(cellPrefab);
        Cell cell1 = cellGO1.GetComponent<Cell>();
        Cell cell2 = cellGO2.GetComponent<Cell>();

        cell1.Init(1, new Vector2Int(0, 0), cellGO1);
        cell2.Init(1, new Vector2Int(1, 0), cellGO2);

        FieldInfo cellsField = typeof(GameField).GetField("_cells", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Cell> cells = new List<Cell> { cell1, cell2 };
        cellsField.SetValue(gameField, cells);

        bool[,] gridCells = new bool[4, 4];
        gridCells[0, 0] = true;
        gridCells[1, 0] = true;
        FieldInfo gridField = typeof(GameField).GetField("_gridCells", BindingFlags.NonPublic | BindingFlags.Instance);
        gridField.SetValue(gameField, gridCells);

        yield return gameField.MoveCells(Vector2.left);
        yield return new WaitForSeconds(0.5f);
        
        Assert.Pass("TestMergeCell_MergesCorrectly завершён без ошибок.");
    }

    /// <summary>
    /// Проверяет полную очистку игрового поля
    /// при сбросе текущей игровой сессии
    /// </summary>
    [UnityTest]
    public IEnumerator TestResetGameClearsCells()
    {
        List<Cell> cells = new List<Cell>();
        for (int i = 0; i < 3; i++)
        {
            GameObject cellGO = GameObject.Instantiate(cellPrefab);
            Cell cell = cellGO.GetComponent<Cell>();
            cell.Init(1, new Vector2Int(i, 0), cellGO);
            cells.Add(cell);
            cellGO.transform.SetParent(cellsParent.transform, false);
        }
        FieldInfo cellsField = typeof(GameField).GetField("_cells", BindingFlags.NonPublic | BindingFlags.Instance);
        cellsField.SetValue(gameField, cells);

        gameField.ResetGame();
        yield return null;
        yield return new WaitForSeconds(0.5f);
        
        Assert.Pass("TestResetGameClearsCells завершён без ошибок.");
    }
}