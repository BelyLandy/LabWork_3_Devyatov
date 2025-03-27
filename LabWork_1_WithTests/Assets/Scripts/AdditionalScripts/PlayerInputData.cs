using UnityEngine;

public struct PlayerInputData
{
    private Vector2 movementInput;
    private bool isFullscreenedOrWindowed;

    public PlayerInputData(
        Vector2 _movementInput,
        bool _isFullscreenedOrWindowed = false)
    {
        movementInput = _movementInput; ;
        isFullscreenedOrWindowed = _isFullscreenedOrWindowed;
    }
    
    public Vector2 MovementInput => movementInput;
    public bool IsFullscreenedOrWindowed => isFullscreenedOrWindowed;
}