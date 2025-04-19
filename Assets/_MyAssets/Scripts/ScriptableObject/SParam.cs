using UnityEngine;
using NGeneral;
using System;

namespace NScriptableObject
{
    [CreateAssetMenu(fileName = "SParam", menuName = "ScriptableObject/SParam")]
    public sealed class SParam : AResourceScriptableObject<SParam>
    {
        [SerializeField] private CCharacter character;
        public CCharacter Character => character;



        [Serializable]
        public sealed class CCharacter
        {
            [SerializeField, Range(0.0f, 10.0f)] private float walkVelocity = 2.0f;
            [SerializeField, Range(0.0f, 20.0f)] private float runVelocity = 5.0f;
            [SerializeField, Range(0.0f, 20.0f)] private float jumpForce = 5.0f;
            [SerializeField, Range(0.0f, 5.0f)] private float jumpInterval = 1.5f;
            [SerializeField, Range(-50.0f, 50.0f)] private float killY = -5.0f;
            [SerializeField, Range(0.0f, 3.0f)] private float disableIntervalOnDied = 0.5f;
            [SerializeField, Range(0.0f, 3.0f)] private float resultIntervalOnCleared = 1.0f;

            [SerializeField] private Override[] overrides;

            public float WalkVelocity => FindOverride()?.WalkVelocity ?? walkVelocity;
            public float RunVelocity => FindOverride()?.RunVelocity ?? runVelocity;
            public float JumpForce => FindOverride()?.JumpForce ?? jumpForce;
            public float JumpInterval => FindOverride()?.JumpInterval ?? jumpInterval;
            public float KillY => FindOverride()?.KillY ?? killY;
            public float DisableIntervalOnDied => FindOverride()?.DisableIntervalOnDied ?? disableIntervalOnDied;
            public float ResultIntervalOnCleared => FindOverride()?.ResultIntervalOnCleared ?? resultIntervalOnCleared;

            // 重たそうなので、改良したい
            private Override FindOverride()
            {
                if (overrides == null || overrides.Length <= 0)
                    return null;

                Scene nowScene = SceneManager.NowScene();
                foreach (var @override in overrides)
                {
                    if (@override.TargetScene == nowScene)
                        return @override;
                }

                return null;
            }



            [Serializable]
            public sealed class Override
            {
                [SerializeField] private Scene targetScene;

                [SerializeField, Range(0.0f, 10.0f)] private float walkVelocity = 2.0f;
                [SerializeField, Range(0.0f, 20.0f)] private float runVelocity = 5.0f;
                [SerializeField, Range(0.0f, 20.0f)] private float jumpForce = 5.0f;
                [SerializeField, Range(0.0f, 5.0f)] private float jumpInterval = 1.5f;
                [SerializeField, Range(-50.0f, 50.0f)] private float killY = -5.0f;
                [SerializeField, Range(0.0f, 3.0f)] private float disableIntervalOnDied = 0.5f;
                [SerializeField, Range(0.0f, 3.0f)] private float resultIntervalOnCleared = 1.0f;

                public Scene TargetScene => targetScene;

                public float WalkVelocity => walkVelocity;
                public float RunVelocity => runVelocity;
                public float JumpForce => jumpForce;
                public float JumpInterval => jumpInterval;
                public float KillY => killY;
                public float DisableIntervalOnDied => disableIntervalOnDied;
                public float ResultIntervalOnCleared => resultIntervalOnCleared;
            }
        }
    }
}