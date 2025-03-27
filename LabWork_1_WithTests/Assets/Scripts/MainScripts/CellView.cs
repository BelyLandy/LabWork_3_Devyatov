using UnityEngine;
using TMPro;

/// <summary>
/// Визуальное представление клетки игрового поля.
/// </summary>
public class CellView : MonoBehaviour
{
    /// <summary>
    /// Ссылка на модель клетки, данные которой отображаются.
    /// </summary>
    private Cell _cell;
    
    [Tooltip("Компонент TextMeshProUGUI для вывода значения")]
    public TextMeshProUGUI valueText;
    
    [Tooltip("Размер клетки в мировых координатах")]
    public float cellSize = 100f;

    /// <summary>
    /// Инициализирует визуальное представление клетки.
    /// </summary>
    /// <param name="cell">Модель клетки, которую нужно визуализировать.</param>
    public void Init(Cell cell)
    {
        this._cell = cell;
        
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;
        
        UpdateValue(_cell.Value);
        UpdatePosition(_cell.Position);
    }

    /// <summary>
    /// Очищает подписки на события при отключении объекта.
    /// </summary>
    private void OnDisable()
    {
        if (_cell != null)
        {
            _cell.OnValueChanged -= UpdateValue;
            _cell.OnPositionChanged -= UpdatePosition;
        }
    }

    /// <summary>
    /// Обновляет отображаемое значение клетки.
    /// </summary>
    /// <param name="newValue">Новое значение клетки в степени двойки.</param>
    private void UpdateValue(int newValue)
    {
        int displayedValue = (int)Mathf.Pow(2, newValue);
        
        if (valueText != null)
        {
            valueText.text = displayedValue.ToString();
        }
        
        UpdateColor();
    }

    /// <summary>
    /// Обновляет позицию клетки в мировых координатах.
    /// </summary>
    /// <param name="newPosition">Новая позиция в координатах сетки.</param>
    private void UpdatePosition(Vector2Int newPosition)
    {
        transform.position = new Vector3(
            newPosition.x * cellSize, 
            newPosition.y * cellSize, 
            0
        );
    }
    
    /// <summary>
    /// Обновляет цвет клетки в зависимости от её значения.
    /// </summary>
    private void UpdateColor()
    {
        // Заглушка.
    }
}