using System.Collections;
using UnityEngine;

/// <summary>
/// Анимации над клетками.
/// </summary>
public class CellAnimations : MonoBehaviour
{
    /// <summary>
    /// Анимация перемещения ячейки.
    /// </summary>
    public static IEnumerator CellMovementAnim(
        Cell cell_,
        Vector2Int newPos)
    {
        var _rectTransform = cell_.CellView.GetComponent<RectTransform>();
        float durationTime = 0.05f;
        float elapsedTime = 0f;
        
        Vector3 startingPos = Cell.GetPos(cell_.Position);
        Vector3 endingPos = Cell.GetPos(newPos);
        
        // Плавное перемещение ячейки.
        while (durationTime > elapsedTime)
        {
            _rectTransform.anchoredPosition = Vector3.Lerp(
                startingPos, 
                endingPos, 
                elapsedTime / durationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Гарантируем окончательную позицию.
        _rectTransform.anchoredPosition = endingPos;
    }
    
    /// <summary>
    /// Анимация слияния ячеек.
    /// </summary>
    public static IEnumerator CellMergedAnim(
        Cell oldCell,
        Cell newCell)
    {
        var _rectTransform = oldCell.CellView.GetComponent<RectTransform>();
        Vector3 fromScale = _rectTransform.localScale;
        float durationTime = 0.05f;
        float elapsedTime = 0f;
        
        // Анимация уменьшения старой ячейки до нуля.
        while (elapsedTime < durationTime)
        {
            _rectTransform.localScale = Vector3.Lerp(fromScale,
                Vector3.zero, 
                elapsedTime / durationTime);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        Destroy(oldCell.gameObject);
        _rectTransform = newCell.CellView.GetComponent<RectTransform>();
        
        Vector3 _newStartScale = _rectTransform.localScale;
        elapsedTime = 0f;
    
        // Анимация увеличения новой ячейки.
        while (durationTime > elapsedTime)
        {
            _rectTransform.localScale = Vector3.Lerp(
                _newStartScale, 
                _newStartScale * 1.1f, 
                elapsedTime / durationTime);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
    
        // Анимация возврата к исходному масштабу.
        elapsedTime = 0f;
        while (durationTime > elapsedTime)
        {
            _rectTransform.localScale = Vector3.Lerp(
                _newStartScale * 1.1f, 
                _newStartScale, 
                elapsedTime / durationTime);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        var _audioSource = newCell.CellView.GetComponent<AudioSource>();
        
        _audioSource.Play();
    }
    
    /// <summary>
    /// Анимация появления ячейки.
    /// Ячейка появляется с нулевого масштаба с подпрыгиванием.
    /// </summary>
    public static IEnumerator CellGenerateAnim(Cell cell)
    {
        var _rectTransform = cell.CellView.GetComponent<RectTransform>();
        float shakeTime = 0.05f;
        float durationTime = 0.01f;
        float elapsedTime = 0f;
        
        Vector3 fromScale = Vector3.zero;
        Vector3 toScale = new Vector3(1, 1, 1);
    
        _rectTransform.localScale = fromScale;
    
        // Анимация появления ячейки.
        while (elapsedTime < durationTime)
        {
            _rectTransform.localScale = Vector3.Lerp(
                fromScale, 
                toScale, 
                elapsedTime / durationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // Гарантируем окончательный масштаб.
        _rectTransform.localScale = toScale;
    
        // Анимация подпрыгивания.
        elapsedTime = 0f;
        var shakeSizeScale = toScale * 1.05f;
        while (shakeTime > elapsedTime)
        {
            _rectTransform.localScale = Vector3.Lerp(toScale, 
                shakeSizeScale, 
                elapsedTime / shakeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // Анимация возврата к исходному масштабу после подпрыгивания.
        elapsedTime = 0f;
        while (shakeTime > elapsedTime)
        {
            _rectTransform.localScale = Vector3.Lerp(shakeSizeScale, 
                toScale, 
                elapsedTime / shakeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Гарантируем окончательный масштаб.
        _rectTransform.localScale = toScale;
    }
}
