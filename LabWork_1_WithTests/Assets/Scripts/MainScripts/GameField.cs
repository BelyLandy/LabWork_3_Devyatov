using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Класс игрового поля.
/// </summary>
public class GameField : MonoBehaviour
{
    [SerializeField, Header("Размер игрового поля по ширине.")] 
    private int gridWidth = 4;
    [SerializeField, Header("Размер игрового поля по высоте.")] 
    private int gridHeight = 4;
    
    [SerializeField, Header("Префаб для создания клетки с числовым значением.")] 
    private GameObject cellPrefab;
    
    [SerializeField, Header("Контейнер для отображения клеток.")] 
    private Transform cellParent;

    private List<Cell> _cells = new();

    private void Start()
    {
        InitializeGrid();
        CreateCell();
    }

    /// <summary>
    /// Инициализирует игровое поле, создавая для каждой позиции клетку со значением 0.
    /// </summary>
    private void InitializeGrid()
    {
        _cells.Clear();
        // Проходим по всем позициям поля и создаем пустые клетки (значение 0)
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Cell cell = new Cell(pos, 0);
                _cells.Add(cell);
            }
        }
    }

    /// <summary>
    /// Находит случайную пустую позицию на игровом поле.
    /// </summary>
    private Vector2Int GetEmptyPosition()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        foreach (Cell cell in _cells)
        {
            if (cell.Value == 0)
            {
                emptyPositions.Add(cell.Position);
            }
        }

        if (emptyPositions.Count > 0)
        {
            int index = Random.Range(0, emptyPositions.Count);
            return emptyPositions[index];
        }
        else
        {
            return new Vector2Int(-1, -1);
        }
    }

    /// <summary>
    /// Создает новую клетку на игровом поле в случайной пустой позиции.
    /// </summary>
    private void CreateCell()
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x < 0)
        {
            Debug.Log("Нет свободных позиций для создания клетки.");
            return;
        }
        
        int cellValue = (Random.value < 0.9f) ? 1 : 2;
        
        Cell cell = _cells.Find(c => c.Position == pos);
        cell.Value = cellValue;
        
        GameObject cellObj = Instantiate(cellPrefab, cellParent);
        cellObj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        CellView cellView = cellObj.GetComponent<CellView>();
        if (cellView != null)
        {
            cellView.Init(cell);
        }
        else
        {
            Debug.LogError("На префабе отсутствует компонент CellView.");
        }
    }
}
