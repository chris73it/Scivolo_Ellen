using UnityEngine;

namespace HeroicArcade.CC.FSM
{
    public sealed class ShootingState : CharacterState
    {
        private void OnEnable()
        {
            Character.Animator.CrossFade("Base Layer.Shoot", 0.05f);
        }

        private void Update()
        {
            Character.Animator.SetFloat("MoveSpeed", new Vector3(Character.velocity.x, 0, Character.velocity.z).magnitude / Character.MoveSpeed);
        }
    }
}