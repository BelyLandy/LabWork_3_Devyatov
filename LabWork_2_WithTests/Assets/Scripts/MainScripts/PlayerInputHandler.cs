using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] 
    private bool toggleSwipeMovement;
    
    private GameField _gameField;
    private PlayerInput _playerInput;
    private Action<InputAction.CallbackContext> _moveCellUpHandler;
    private Action<InputAction.CallbackContext> _moveCellLeftHandler;
    private Action<InputAction.CallbackContext> _moveCellRightHandler;
    private Action<InputAction.CallbackContext> _moveCellDownHandler;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        //_playerInput.Gameplay = _playerInput.Gameplay; 
        _gameField = FindFirstObjectByType<GameField>();
    }
    
    private void OnEnable()
    {
        _playerInput.Gameplay.Enable();
        _moveCellUpHandler = 
            ctx => HandleInputAction(Vector2.up);
        _moveCellDownHandler = 
            ctx => HandleInputAction(Vector2.down);
        _moveCellLeftHandler = 
            ctx => HandleInputAction(Vector2.left);
        _moveCellRightHandler = 
            ctx => HandleInputAction(Vector2.right);
        
        _playerInput.Gameplay.MoveCellUp.performed += _moveCellUpHandler;
        _playerInput.Gameplay.MoveCellLeft.performed += _moveCellLeftHandler;
        _playerInput.Gameplay.MoveCellRight.performed += _moveCellRightHandler;
        _playerInput.Gameplay.MoveCellDown.performed += _moveCellDownHandler;
        _playerInput.Gameplay.MoveWithSwipe.performed += HandleSwipeAction;
    }

    private void OnDisable()
    {
        _playerInput.Gameplay.MoveCellUp.performed -= _moveCellUpHandler;
        _playerInput.Gameplay.MoveCellLeft.performed -= _moveCellLeftHandler;
        _playerInput.Gameplay.MoveCellRight.performed -= _moveCellRightHandler;
        _playerInput.Gameplay.MoveCellDown.performed -= _moveCellDownHandler;
        _playerInput.Gameplay.MoveWithSwipe.performed -= HandleSwipeAction;
        _playerInput.Gameplay.Disable();
    }
    
    private void HandleInputAction(Vector2 direction)
    {
        Debug.Log($"Ввод пользователя -> {direction}");
        if (!_gameField.IsDoingAnim)
        {
            StartCoroutine(_gameField.MoveCells(direction));
        }
        else if (_gameField.endGame)
        {
            _gameField.ResetGame();
        }
    }
    
    private void HandleSwipeAction(InputAction.CallbackContext context)
    {
        if (toggleSwipeMovement)
        {
            Vector2 delta = context.ReadValue<Vector2>();
            Vector2 direction = Vector2.zero;
            
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                direction = delta.x > 0 ? Vector2.right : Vector2.left;
            }
            else if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
            {
                direction = delta.y > 0 ? Vector2.up : Vector2.down;
            }

            if (direction != Vector2.zero)
            {
                HandleInputAction(direction);
            }
        }
    }
}
