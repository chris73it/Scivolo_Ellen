using UnityEngine;
using MenteBacata.ScivoloCharacterController;
using HeroicArcade.CC.FSM;
using HeroicArcade.CC.Demo;

namespace HeroicArcade.CC
{
    [RequireComponent(typeof(Animator))]
    public class AvatarController : MonoBehaviour
    {
        //Character character;
        public Character Character { get; private set; }

        [SerializeField] CharacterState[] states;
        
        public static class FSMState
        {
            public static readonly int Ambulation = 0; //Combines Idle, Walk and Run animations using a blend state.
            public static readonly int Jump = 1;
            public static readonly int Shooting = 2; //Combines Idle, Walk and Run animations using a blend state.
        }

        const float minVerticalSpeed = -12f;
        const float timeBeforeUngrounded = 0.02f;

        float deltaTime;

        float nextUngroundedTime = -1f;

        private void Awake()
        {
            Character = GetComponent<Character>();
        }

        Transform cameraTransform;
        public Transform CameraTransform { get => cameraTransform; }
        private void Start()
        {
            cameraTransform = Camera.main.transform;
            Character.Mover.canClimbSteepSlope = true;
        }

        MovingPlatform movingPlatform;

        Vector3 movementInput;
        bool groundDetected;
        bool isOnMovingPlatform = false;
        private void Update()
        {
            //First things first: sample delta time once for this coming frame.
            deltaTime = Time.deltaTime;

            movementInput = GetMovementInput();

            Character.velocityXZ += Character.MoveAcceleration * deltaTime;
            if (Character.velocityXZ > Character.MoveSpeed) //FIXME: MaxMoveSpeed ???
                Character.velocityXZ = Character.MoveSpeed;

            Character.velocity = Character.velocityXZ * movementInput;
            //Debug.Log($"velocity {velocity}");

            groundDetected = DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo);

            SetGroundedIndicatorColor(isGrounded);

            isOnMovingPlatform = false;

            if (isGrounded && Character.InputController.IsJumpPressed)
            {
                Character.verticalSpeed = Character.JumpSpeed;
                nextUngroundedTime = -1f;
                isGrounded = false;
            }

            if (isGrounded)
            {
                Character.Mover.isInWalkMode = true;
                Character.verticalSpeed = 0f;

                if (groundDetected)
                {
                    isOnMovingPlatform = groundInfo.collider.TryGetComponent(out movingPlatform);
                }
            }
            else
            {
                Character.Mover.isInWalkMode = false;

                BounceDownIfTouchedCeiling();

                Character.verticalSpeed += Character.Gravity * deltaTime;

                //Limit the maximum downward speed
                if (Character.verticalSpeed < minVerticalSpeed)
                    Character.verticalSpeed = minVerticalSpeed;

                Character.velocity += Character.verticalSpeed * transform.up;
            }

            //Perform the right movement and play the corresponding animation, i.e. detect
            //      when to use the jumping/falling, idling/walking/running animations.
            //NOTE: the FSM states are mainly useful to start/stop (aka, OnEnabled/OnDisabled) subsequent animations.
            if (!isGrounded)
            {
                Character.StateMachine.TrySetState(states[FSMState.Jump]);
            }
            else
            {
                if (movementInput.sqrMagnitude < 1E-06f)
                {
                    Character.velocityXZ = 0f;
                }
                if (Character.InputController.IsShootPressed)
                {
                    Character.StateMachine.TrySetState(states[FSMState.Shooting]);
                }
                else
                {
                    Character.StateMachine.TrySetState(states[FSMState.Ambulation]);
                }
            }

            RotateTowards(Character.velocity);
            Character.Mover.Move(Character.velocity * deltaTime, Character.moveContacts, out Character.contactCount);
        }

        private void LateUpdate()
        {
            if (isOnMovingPlatform)
            {
                ApplyPlatformMovement(movingPlatform);
            }
        }

        private void RotateTowards(Vector3 direction)
        {
            switch (Character.cameraStyle)
            {
                case Character.CameraStyle.Adventure:
                    //Do nothing
                    break;

                case Character.CameraStyle.Combat:
                    direction = Character.AvatarController.CameraTransform.forward;
                    break;

                case Character.CameraStyle.EricWei:
                    if (direction.sqrMagnitude == 0)
                    {
                        goto case Character.CameraStyle.Adventure;
                    }
                    goto case Character.CameraStyle.Combat;

                default:
                    Debug.LogError($"Unexpected CameraStyle {Character.cameraStyle}");
                    return;
            }

            Vector3 flatDirection = Vector3.ProjectOnPlane(direction, transform.up);
            if (flatDirection.sqrMagnitude < 1E-06f)
                return;
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Character.TurnSpeed * Time.deltaTime);
        }

        Vector3 projectedCameraForward;
        Quaternion rotationToCamera;
        private Vector3 GetMovementInput()
        {
            projectedCameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up);
            rotationToCamera = Quaternion.LookRotation(projectedCameraForward, transform.up);
            return rotationToCamera * (currentMovement.x * Vector3.right + currentMovement.z * Vector3.forward);
        }

        private bool DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo)
        {
            bool groundDetected = Character.GroundDetector.DetectGround(out groundInfo);

            if (groundDetected)
            {
                if (groundInfo.isOnFloor && Character.verticalSpeed < 0.1f)
                    nextUngroundedTime = Time.time + timeBeforeUngrounded;
            }
            else
                nextUngroundedTime = -1f;

            isGrounded = Time.time < nextUngroundedTime;
            return groundDetected;
        }

        private void SetGroundedIndicatorColor(bool isGrounded)
        {
            if (Character.GroundedIndicator != null)
                Character.GroundedIndicator.material.color = isGrounded ? Color.green : Color.blue;
        }

        private void BounceDownIfTouchedCeiling()
        {
            for (int i = 0; i < Character.contactCount; i++)
            {
                if (Vector3.Dot(Character.moveContacts[i].normal, transform.up) < -0.7f)
                {
                    Character.verticalSpeed = -0.25f * Character.verticalSpeed;
                    break;
                }
            }
        }

        private void ApplyPlatformMovement(MovingPlatform movingPlatform)
        {
            GetMovementFromMovingPlatform(movingPlatform, out Vector3 movement, out float upRotation);

            transform.Translate(movement, Space.World);
            transform.Rotate(0f, upRotation, 0f, Space.Self);
        }

        private void GetMovementFromMovingPlatform(MovingPlatform movingPlatform, out Vector3 movement, out float deltaAngleUp)
        {
            movingPlatform.GetDeltaPositionAndRotation(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);
            Vector3 localPosition = transform.position - movingPlatform.transform.position;
            movement = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            platformDeltaRotation.ToAngleAxis(out float platformDeltaAngle, out Vector3 axis);
            float axisDotUp = Vector3.Dot(axis, transform.up);

            if (-0.1f < axisDotUp && axisDotUp < 0.1f)
                deltaAngleUp = 0f;
            else
                deltaAngleUp = platformDeltaAngle * Mathf.Sign(axisDotUp);
        }

        private Vector3 currentMovement;
        public void OnMoveInput(Vector2 moveInput)
        {
            //y needs to preserve its value from the previous Update.
            currentMovement.x = moveInput.x;
            currentMovement.z = moveInput.y;
        }
    }
}