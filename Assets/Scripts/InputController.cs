using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HeroicArcade.CC
{
    [Serializable]
    public class MoveInputEvent : UnityEvent<Vector2>
    {
    }

    public sealed class InputController : MonoBehaviour
    {
        [SerializeField] MoveInputEvent moveInputEvent;

        Controls controls;
        //private Animator animator;
        //private int isWalkingHash;
        //private int isRunningHash;
        //private int isJumpingHash;
        private void Awake()
        {
            controls = new Controls();
            //animator = GetComponent<Animator>();

            //isWalkingHash = Animator.StringToHash("isWalking");
            //isRunningHash = Animator.StringToHash("isRunning");
            //isJumpingHash = Animator.StringToHash("isJumping");

            controls.Gameplay.Move.started += OnMove;
            controls.Gameplay.Move.canceled += OnMove;
            controls.Gameplay.Move.performed += OnMove;

            controls.Gameplay.Jump.started += OnJump;
            controls.Gameplay.Jump.canceled += OnJump;

            controls.Gameplay.Shoot.started += OnShoot;
            controls.Gameplay.Shoot.canceled += OnShoot;
            controls.Gameplay.Shoot.performed += OnShoot;
        }

        private Vector2 moveInput;
        [HideInInspector] public bool IsMovePressed;
        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            IsMovePressed = moveInput != Vector2.zero;
            //Debug.Log($"IsMovePressed {IsMovePressed}");
            moveInputEvent.Invoke(moveInput);
        }

        [HideInInspector] public bool IsJumpPressed;
        private void OnJump(InputAction.CallbackContext context)
        {
            IsJumpPressed = context.ReadValueAsButton();
        }

        [HideInInspector] public bool IsShootPressed;
        private void OnShoot(InputAction.CallbackContext context)
        {
            IsShootPressed = context.ReadValueAsButton();
        }

        private void OnEnable()
        {
            controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            controls.Gameplay.Disable();
        }
    }
}