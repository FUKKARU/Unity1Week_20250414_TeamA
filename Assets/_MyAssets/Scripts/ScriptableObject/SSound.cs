using UnityEngine;
using NGeneral;

namespace NScriptableObject
{
    [CreateAssetMenu(fileName = "SSound", menuName = "ScriptableObject/SSound")]
    public sealed class SSound : AResourceScriptableObject<SSound>
    {
        [SerializeField] private AudioClip bgm;
        public AudioClip BGM => bgm;
    }
}