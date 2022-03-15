using UnityEngine;
using Animancer.FSM;

namespace HeroicArcade.CC.FSM
{
    public abstract class CharacterState : StateBehaviour
    {
        [SerializeField] Character character;
        public Character Character { get => character; }
    }
}