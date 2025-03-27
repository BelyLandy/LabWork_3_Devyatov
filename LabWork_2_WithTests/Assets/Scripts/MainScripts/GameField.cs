using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Этот класс управляет игровым полем, создаёт и перемещает клетки, а также контролирует основную логику игры. 
/// </summary>
public class GameField : MonoBehaviour
{
    [SerializeField, Header("Настройки игры")]
    private int gridSize = 4;

    [SerializeField, Header("Ссылки")]
    private UIManager uiManager;
    [SerializeField] 
    private GameObject cellPrefab;
    [SerializeField] 
    private GameObject cellsParent;
    
    [SerializeField, Header("Сохранения")] 
    private string saveSlotName = "SaveSlot_1";

    [HideInInspector] 
    public bool endGame;
    
    private List<Cell> _cells = new();
    private bool[,] _gridCells;
    private int _currScore = 0;
    private string _savePath;
    private GameSaveData _gameSaveData;
    private bool _isDoingAnim;

    private string _objName = "Cell";
    
    /// <summary>
    /// Инициализирует игровое поле, загружая сохранённую игру или начиная новую игру. 
    /// </summary>
    private void Start()
    {
        _savePath = Path.Combine(
            Application.persistentDataPath,
            $"{saveSlotName}.dat");

        // Инициализируем поле.
        _gridCells = new bool[gridSize, gridSize];

        // Загружаем данные игры через отдельный менеджер.
        _gameSaveData = SaveLoadManager.LoadGame(_savePath, gridSize);

        if (_gameSaveData != null)
        {
            // Если данные загружены, воссоздаём клетки на игровом поле.
            for (int x = 0; x < gridSize; ++x)
            {
                for (int y = 0; y < gridSize; ++y)
                {
                    if (_gameSaveData.CellValues[x, y] > 0)
                    {
                        StartCoroutine(CreateCellAtPosition(new Vector2Int(x, y), _gameSaveData.CellValues[x, y]));
                    }
                }
            }
            uiManager?.UpdateHighScore(_gameSaveData.hightScore);
        }
        else
        {
            StartNewGame();
        }
        
        uiManager?.UpdateScore(_currScore);
    }

    /// <summary>
    /// Обновляет текст на экране с текущим счётом. 
    /// </summary>
    private void UpdateScoreText()
    {
        uiManager?.UpdateScore(_currScore);
    }
    
    /// <summary>
    /// Ищет свободную позицию в сетке для размещения новой клетки. 
    /// </summary>
    /// <returns>Возвращает случайную пустую позицию или (-1,-1), если места нет.</returns>
    private Vector2Int GetEmptyPosition()
    {
        var _emptyPositions = new List<Vector2Int>();

        for (var x = 0; x < gridSize; ++x)
        {
            for (var y = 0; y < gridSize; ++y)
            {
                if (!_gridCells[x, y])
                {
                    _emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        if (_emptyPositions.Count > 0)
        {
            return _emptyPositions[Random.Range(0, _emptyPositions.Count)];
        }
        
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Создаёт новую клетку с указанной вероятностью появления сильного значения. 
    /// </summary>
    private IEnumerator CreateCell(float _chance)
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x != -1 && pos.y != -1)
        {
            Vector3 posVec = Cell.GetPos(pos);
            
            GameObject cellViewObj = Instantiate(
                cellPrefab, 
                posVec, 
                Quaternion.identity);
        
            Cell _cell = cellViewObj.GetComponent<Cell>();
            
            if (_cell == null)
            {
                _cell = cellViewObj.AddComponent<Cell>();
            }
        
            int _value = Random.value <= _chance ? 2 : 1;
            _cell.Init(
                _value, 
                pos, 
                cellViewObj);
            _cells.Add(_cell);
            
            _gridCells[pos.x, pos.y] = true;
            var cellView = cellViewObj.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.Init(_cell);
            }

            if (cellsParent != null)
            {
                cellViewObj.transform.SetParent(
                    cellsParent.transform, 
                    false);
            }
            cellViewObj.name = _objName;
            
            yield return StartCoroutine(CellAnimations.CellGenerateAnim(_cell));
            _currScore += (int)Mathf.Pow(2, _value);
            
            UpdateScoreText();
        }
    }
    
    /// <summary>
    /// Создаёт клетку в заданной позиции с указанным значением, используется при загрузке игры. 
    /// </summary>
    private IEnumerator CreateCellAtPosition(Vector2Int pos, int _value)
    {
        if (pos.x != -1 && pos.y != -1)
        {
            Vector3 positionVector = Cell.GetPos(pos);
            var cellViewObject = Instantiate(cellPrefab, positionVector, Quaternion.identity);
        
            var cell = cellViewObject.GetComponent<Cell>();
            if (cell == null)
            {
                cell = cellViewObject.AddComponent<Cell>();
            }
            cell.Init(_value, pos, cellViewObject);
        
            _cells.Add(cell);
            _gridCells[pos.x, pos.y] = true;
        
            var cellView = cellViewObject.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.Init(cell);
            }

            if (cellsParent != null)
            {
                cellViewObject.transform.SetParent(cellsParent.transform, false);
            }
            cellViewObject.name = _objName;
            
            yield return StartCoroutine(CellAnimations.CellGenerateAnim(cell));
            
            _currScore += (int)Mathf.Pow(2, _value);
            UpdateScoreText();
        }
    }
    
    public bool IsDoingAnim => _isDoingAnim;
    
    /// <summary>
    /// Перемещает клетки в указанном направлении и создаёт новую клетку после движения. 
    /// </summary>
    public IEnumerator MoveCells(Vector2 _dir)
    {
        if (_isDoingAnim) yield break;
        _isDoingAnim = true;
        
        bool revOrder = (
            _dir == Vector2.right ||
            _dir == Vector2.up);
    
        foreach (var _cell in _cells)
        {
            _cell.Merged = false;
        }
    
        var sortedCells = GetSortedCells(
            _dir,
            revOrder);
        foreach (var cell in sortedCells)
        {
            yield return StartCoroutine(MoveCell(cell, _dir));
        }
        yield return StartCoroutine(CreateCell(0.2f));
        CheckGameOver();
        _isDoingAnim = false;
    }
    
    /// <summary>
    /// Перемещает отдельную клетку в заданном направлении, объединяя её с похожей клеткой, если это возможно. 
    /// </summary>
    private IEnumerator MoveCell(Cell _cell, Vector2 _dir)
    {
        Vector2Int newPos = _cell.Position;
        Cell mergeCell = null;
    
        while (true)
        {
            Vector2Int nextPos = newPos + new Vector2Int(
                (int)_dir.x, 
                (int)_dir.y);
        
            if (nextPos.x < 0 || nextPos.x >= gridSize ||
                nextPos.y < 0 || nextPos.y >= gridSize)
            {
                break;
            }
        
            var nextCell = _cells.Find(nextCell => nextCell.Position == nextPos);
            if (nextCell == null)
            {
                newPos = nextPos;
            }
            else if (nextCell.Value == _cell.Value 
                     && !nextCell.Merged)
            {
                mergeCell = nextCell;
                newPos = nextPos;
                
                break;
            }
            else
            {
                break;
            }
        }
    
        if (newPos != _cell.Position)
        {
            yield return StartCoroutine(CellAnimations.CellMovementAnim(
                _cell,
                newPos));
            if (mergeCell != null)
            {
                yield return StartCoroutine(CellAnimations.CellMergedAnim(
                    _cell,
                    mergeCell));
                
                mergeCell.Value++;
                
                _cells.Remove(_cell);
                _gridCells[_cell.Position.x, _cell.Position.y] = false;
                
                mergeCell.Merged = true;
                
                UpdateScoreText();
            }
        
            _gridCells[_cell.Position.x, _cell.Position.y] = false;
            _cell.Position = newPos;
            
            _gridCells[newPos.x, newPos.y] = true;
        }
    }
    
    /// <summary>
    /// Возвращает список клеток, отсортированных по направлению движения. 
    /// </summary>
    private List<Cell> GetSortedCells(
        Vector2 _dir, 
        bool revOrder)
    {
        var orderedCells = new List<Cell>(_cells);
        if (_dir == Vector2.up || _dir == Vector2.down)
        {
            orderedCells.Sort((a, b) => 
                a.Position.y.CompareTo(b.Position.y));
        }
        else if (_dir == Vector2.left || _dir == Vector2.right)
        {
            orderedCells.Sort((a, b) =>
                a.Position.x.CompareTo(b.Position.x));
        }
        if (revOrder)
        {
            orderedCells.Reverse();
        }
        return orderedCells;
    }
    
    /// <summary>
    /// Проверяет, закончилась ли игра, и запускает проверку с задержкой. 
    /// </summary>
    private void CheckGameOver()
    {
        StartCoroutine(DelayedGameOverCheck());
    }
    
    /// <summary>
    /// Проводит проверку состояния игры после завершения кадра и определяет, наступил ли конец игры. 
    /// </summary>
    private IEnumerator DelayedGameOverCheck()
    {
        yield return new WaitForEndOfFrame();
        
        bool IsEndGame = true;
        if (GetEmptyPosition().x != -1)
        {
            IsEndGame = false;
        }
        else
        {
            foreach (Cell cell in _cells)
            {
                Vector2Int[] directions =
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = cell.Position + dir;
                    
                    if (!(neighborPos.x < 0 || 
                          neighborPos.x >= gridSize || 
                          neighborPos.y < 0 ||
                          neighborPos.y >= gridSize))
                    {
                        Cell neighbor = _cells.Find(nextCell => nextCell.Position == neighborPos);
                        
                        if (neighbor != null && 
                            neighbor.Value == cell.Value)
                        {
                            IsEndGame = false;
                        }
                    }
                }
            }
        }
        if (IsEndGame)
        {
            endGame = true;
            Debug.Log("Вы проиграли! Увы!");
            if (_gameSaveData.hightScore < _currScore)
            {
                _gameSaveData.hightScore = _currScore;
                uiManager?.UpdateHighScore(_gameSaveData.hightScore);
                
                SaveLoadManager.SaveGame(
                    _savePath, 
                    _gameSaveData, 
                    gridSize, 
                    _cells, 
                    _currScore);
            }
        }
    }
    
    /// <summary>
    /// Начинает новую игру, сбрасывая текущий счёт и удаляя все клетки с поля. 
    /// </summary>
    private void StartNewGame()
    {
        endGame = false;
        _currScore = 0;
        
        foreach (Cell cell in _cells)
        {
            Destroy(cell.gameObject);
        }
        _cells.Clear();
        
        // Инициализируем новые данные для сохранения игры.
        _gameSaveData = new GameSaveData 
        {
            CellValues = new int[gridSize, gridSize],
            currScore = 0,
            hightScore = _gameSaveData?.hightScore ?? 0
        };
        
        _gridCells = new bool[gridSize, gridSize];
        StartCoroutine(CreateCell(0.1f));
        StartCoroutine(CreateCell(0.1f));
        
        UpdateScoreText();
    }
    
    /// <summary>
    /// Сбрасывает игру, сохраняя лучший счёт, если он превышает предыдущий, и перезапуская игру. 
    /// </summary>
    public void ResetGame()
    {
        if (_gameSaveData.hightScore < _currScore)
        {
            _gameSaveData.hightScore = _currScore;
            uiManager?.UpdateHighScore(_gameSaveData.hightScore);
        }
        Debug.Log("Перезапуск...");
        
        StartNewGame();
    }
    
    /// <summary>
    /// Сохраняет игру при выходе из приложения. 
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveLoadManager.SaveGame(_savePath,
            _gameSaveData, 
            gridSize, 
            _cells, 
            _currScore);
    }
}
