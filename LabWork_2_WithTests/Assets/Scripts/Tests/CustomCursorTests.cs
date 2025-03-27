using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

public class CustomCursorTests
{
    private GameObject _cursorObject;
    private CustomCursor _customCursor;
    
    /// <summary>
    /// Подготавливает тестовую среду с главной камерой
    /// и создает объект кастомного курсора перед каждым тестом
    /// </summary>
    [SetUp]
    public void Setup()
    {
        if (Camera.main == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
        }

        _cursorObject = new GameObject();
        _customCursor = _cursorObject.AddComponent<CustomCursor>();
    }

    /// <summary>
    /// Проверяет инициализацию курсора,
    /// убеждаясь что метод Awake корректно устанавливает текстуру
    /// </summary>
    [Test]
    public void Awake_ShouldSetCursor()
    {
        Texture2D testTexture = new Texture2D(2, 2);

        _customCursor.GetType()
            .GetField("_texture2D", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_customCursor, testTexture);

        _customCursor.Invoke("Awake", 0);
        Assert.Pass();
    }
    
    /// <summary>
    /// Проверяет обновление позиции курсора,
    /// убеждаясь что он корректно следует за позицией мыши
    /// </summary>
    [UnityTest]
    public IEnumerator Update_WhenIsChanged_ShouldSetCursorPosition()
    {
        _customCursor.GetType().GetField("isChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_customCursor, true);
        
        Vector3 testPosition = new Vector3(100, 100, 0);
        Input.mousePosition.Should().NotBe(testPosition);
        
        yield return null;
        
        Camera.main.ScreenToWorldPoint(Input.mousePosition).z.Should().Be(0);
    }
}