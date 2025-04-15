using UnityEngine;

namespace NInGame
{
    public sealed class Enemy : ACharacter
    {
        protected override Vector3 Forward => -transform.right;
    }
}