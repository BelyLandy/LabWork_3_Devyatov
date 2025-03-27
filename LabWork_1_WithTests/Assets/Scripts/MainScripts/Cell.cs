using System;
using UnityEngine;

/// <summary>
/// Класс, представляющий ячейку в игровой сетке. 
/// </summary>
public class Cell
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
    /// Флаг, указывающий на то, что ячейка участвовала в слиянии.
    /// </summary>
    private bool _merged;
    
    /// <summary>
    /// Событие изменения значения ячейки.
    /// </summary>
    public event Action<int> OnValueChanged;

    /// <summary>
    /// Событие изменения позиции ячейки.
    /// </summary>
    public event Action<Vector2Int> OnPositionChanged;
    
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
    /// Флаг, указывающий на участие ячейки в слиянии.
    /// </summary>
    public bool Merged
    {
        get => _merged;
        set => _merged = value;
    }
    
    
    /// <summary>
    /// Инициализирует новую ячейку с указанными параметрами.
    /// </summary>
    /// <param name="position">Начальная позиция ячейки.</param>
    /// <param name="value">Начальное значение ячейки.</param>
    public Cell(Vector2Int position, int value)
    {
        _position = position;
        _value = value;
    }
}