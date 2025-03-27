using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] 
    private bool isChanged;

    [SerializeField] 
    private Texture2D _texture2D;

    private void Awake()
    {
        Cursor.SetCursor(_texture2D, Vector2.zero, CursorMode.Auto);
    }

    void Update() 
    {
        SetCustomCursor();
    }

    private void SetCustomCursor()
    {
        if (!isChanged || Camera.main == null) return;

        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(currentMousePosition);
        worldPosition.z = 0f;
        transform.position = worldPosition;
    }

}