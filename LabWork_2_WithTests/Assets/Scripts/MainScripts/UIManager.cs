using UnityEngine;
using TMPro;

/// <summary>
/// Менеджер UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField, Header("Счеты")]
    private TextMeshProUGUI currScoreTxt;
    [SerializeField]
    private TextMeshProUGUI hightScoreTxt;

    /// <summary>
    /// Обновление текста с текущим счётом.
    /// </summary>
    public void UpdateScore(int score)
    {
        if (currScoreTxt != null)
        {
            currScoreTxt.text = "Текущий\n" + score;
        }
    }
    
    /// <summary>
    /// Обновление текста с рекордным счётом.
    /// </summary>
    public void UpdateHighScore(int highScore)
    {
        if (hightScoreTxt != null)
        {
            hightScoreTxt.text = "Лучший\n" + highScore;
        }
    }
}