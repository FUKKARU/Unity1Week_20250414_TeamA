using System;
using UnityEngine;
using NGeneral;
using AC = UnityEngine.AudioClip;

namespace NScriptableObject
{
    [CreateAssetMenu(fileName = "SSound", menuName = "ScriptableObject/SSound")]
    public sealed class SSound : AResourceScriptableObject<SSound>
    {
        [Serializable]
        public struct Bgm
        {
            [SerializeField] private AC main;
            public AC Main => main;
        }

        [Serializable]
        public struct Se
        {
            [SerializeField] private AC died;
            public AC Died => died;

            [SerializeField] private AC cleared;
            public AC Cleared => cleared;

            [SerializeField] private AC clicked;
            public AC Clicked => clicked;
        }

        [SerializeField] private Bgm bgm;
        public Bgm BGM => bgm;

        [SerializeField] private Se se;
        public Se SE => se;
    }
}