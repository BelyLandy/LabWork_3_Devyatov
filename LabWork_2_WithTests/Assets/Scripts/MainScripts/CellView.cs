using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Класс CellView отвечает за визуальное отображение ячейки игры.
/// </summary>
public class CellView : MonoBehaviour
{
    // Ссылка на соответствующую модель ячейки.
    private Cell _cell;
    
    // Текстовое поле для отображения числового значения ячейки.
    [SerializeField] 
    private TextMeshProUGUI _cellValueTxt;
    
    // Изображение ячейки для задания цвета фона.
    [SerializeField] 
    private Image _cellImg;

    /// <summary>
    /// Инициализация ячейки и подписка на события изменения значения и позиции.
    /// </summary>
    public void Init(Cell cell)
    {
        _cell = cell;

        // Подписываемся на событие изменения значения ячейки.
        cell.OnValueChanged += UpdateValue;
        // Подписываемся на событие изменения позиции ячейки.
        cell.OnPositionChanged += UpdatePosition;

        // Обновляем значение и позицию при инициализации.
        UpdateValue(_cell.Value);
        UpdatePosition(_cell.Position);
    }

    /// <summary>
    /// Обновление текстового значения ячейки.
    /// </summary>
    private void UpdateValue(int value)
    {
        int displayedValue = (int)Mathf.Pow(2, value);
        _cellValueTxt.text = displayedValue.ToString();
        
        // Обновляем цвет ячейки в зависимости от значения.
        UpdateColorCell();
    }
    
    /// <summary>
    /// Обновление позиции ячейки.
    /// </summary>
    private void UpdatePosition(Vector2Int position)
    {
        var rectTransform = GetComponent<RectTransform>();
        
        rectTransform.anchoredPosition = Cell.GetPos(position);
    }
    
    /// <summary>
    /// Деструктор для отписки от событий.
    /// </summary>
    private void OnDestroy()
    {
        if (_cell != null)
        {
            _cell.OnValueChanged -= UpdateValue;
            _cell.OnPositionChanged -= UpdatePosition;
        }
    }
    
    /// <summary>
    /// Обновление цвета ячейки в зависимости от её значения.
    /// </summary>
    private void UpdateColorCell()
    {
        if (_cell.Value <= 11 && _cell.Value > 0)
        {
            // Вычисление коэффициента интерполяции цвета.
            var interpolationVal = (_cell.Value - 1) / 10f;
            // Определение начального цвета.
            var color_1 = new Color(0 / 255f, 93 / 255f, 255 / 255f);
            // Определение конечного цвета.
            var color_2 = new Color(255 / 255f, 7 / 255f, 0 / 255f);
            // Интерполяция каждого компонента цвета.
            var red = Mathf.Lerp(color_1.r, color_2.r, interpolationVal);
            var green = Mathf.Lerp(color_1.g, color_2.g, interpolationVal);
            var blue = Mathf.Lerp(color_1.b, color_2.b, interpolationVal);

            _cellImg.color = new Color(red, green, blue);
        }
        else
        {
            // Если значение ячейки не в допустимом диапазоне, делаем ячейку прозрачной.
            _cellImg.color = Color.clear;
        }
    }
}
