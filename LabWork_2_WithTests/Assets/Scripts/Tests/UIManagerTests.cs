using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TMPro;

public class UIManagerTests
{
    private GameObject uiManagerObj;
    private UIManager uiManager;
    private GameObject scoreObj;
    private TextMeshProUGUI scoreTxt;
    private GameObject highScoreObj;
    private TextMeshProUGUI highScoreTxt;

    /// <summary>
    /// Подготавливает тестовую среду перед каждым тестом:
    /// - Создает объект UIManager с текстовыми полями для отображения очков
    /// - Настраивает связи между компонентами через Reflection
    /// </summary>
    [SetUp]
    public void Setup()
    {
        uiManagerObj = new GameObject("UIManagerTest");
        uiManager = uiManagerObj.AddComponent<UIManager>();

        scoreObj = new GameObject("ScoreText", typeof(RectTransform));
        scoreObj.transform.SetParent(uiManagerObj.transform, false);
        scoreTxt = scoreObj.AddComponent<TextMeshProUGUI>();

        highScoreObj = new GameObject("HighScoreText", typeof(RectTransform));
        highScoreObj.transform.SetParent(uiManagerObj.transform, false);
        highScoreTxt = highScoreObj.AddComponent<TextMeshProUGUI>();

        FieldInfo scoreField =
            typeof(UIManager).GetField("currScoreTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo highScoreField =
            typeof(UIManager).GetField("hightScoreTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        scoreField.SetValue(uiManager, scoreTxt);
        highScoreField.SetValue(uiManager, highScoreTxt);
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста:
    /// - Удаляет созданные игровые объекты
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(uiManagerObj);
    }

    /// <summary>
    /// Проверяет обновление текущего счета:
    /// - Убеждается, что текст правильно отображает переданное значение
    /// </summary>
    [Test]
    public void TestUpdateScore()
    {
        uiManager.UpdateScore(100);
        Assert.IsTrue(scoreTxt.text.Contains("100"));
    }

    /// <summary>
    /// Проверяет обновление рекордного счета:
    /// - Убеждается, что текст правильно отображает переданное значение
    /// </summary>
    [Test]
    public void TestUpdateHighScore()
    {
        uiManager.UpdateHighScore(200);
        Assert.IsTrue(highScoreTxt.text.Contains("200"));
    }
}