namespace HeroicArcade.CC.FSM
{
    public sealed class JumpState : CharacterState
    {
        private void OnEnable()
        {
            //Character.Animator.CrossFade("Base Layer.Jump", 0.25f);
            Character.Animator.SetBool("IsJumpPressed", true);
        }

        private void OnDisable()
        {
            Character.Animator.SetBool("IsJumpPressed", false);
        }
    }
}