using UnityEngine;

namespace HeroicArcade.CC.FSM
{
    public sealed class AmbulationState : CharacterState
    {
        private void OnEnable()
        {
            Character.Animator.CrossFade("Base Layer.Move", 0.25f);
        }

        private void Update()
        {
            Character.Animator.SetFloat("MoveSpeed", new Vector3(Character.velocity.x, 0, Character.velocity.z).magnitude / Character.MoveSpeed);
        }
    }
}