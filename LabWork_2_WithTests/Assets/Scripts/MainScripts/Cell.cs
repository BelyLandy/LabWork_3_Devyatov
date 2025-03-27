using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    /// <summary>
    /// Текущая позиция ячейки в сетке.
    /// </summary>
    private Vector2Int _position;
    
    /// <summary>
    /// Текущее значение ячейки.
    /// </summary>
    private int _value;

    /// <summary>
    /// Событие изменения значения ячейки.
    /// </summary>
    public event Action<int> OnValueChanged;
    
    /// <summary>
    /// Событие изменения позиции ячейки.
    /// </summary>
    public event Action<Vector2Int> OnPositionChanged;
    
    public GameObject CellView { get; set; }

    public bool Merged { get; set; }
    
    /// <summary>
    /// Текущее значение ячейки. При изменении вызывает событие OnValueChanged.
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    /// <summary>
    /// Текущая позиция ячейки. При изменении вызывает событие OnPositionChanged.
    /// </summary>
    public Vector2Int Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPositionChanged?.Invoke(_position);
            }
        }
    }

    /// <summary>
    /// Инициализация Cell.
    /// </summary>
    public void Init(int _initValue, Vector2Int _initPosition, GameObject _initCellView)
    {
        Value = _initValue;
        Position = _initPosition;
        CellView = _initCellView;
    }

    /// <summary>
    /// Метод для получения позиции клетки в мире.
    /// </summary>
    public static Vector2 GetPos(Vector2Int position)
    {
        return new Vector2(
            position.x * 144 - 208.5f,
            position.y * 144 - 318
            );
    }
}