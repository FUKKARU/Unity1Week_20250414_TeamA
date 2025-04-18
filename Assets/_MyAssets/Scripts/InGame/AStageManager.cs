using System;
using UnityEngine;

namespace NInGame
{
    public abstract class AStageManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Enemy[] enemies;

        // はめ込む先のSentence
        [SerializeField] private Sentence[] walkLefts;
        [SerializeField] private Sentence[] walkRights;
        [SerializeField] private Sentence[] runLefts;
        [SerializeField] private Sentence[] runRights;
        [SerializeField] private Sentence[] jumps;

        [SerializeField] private Word wordI;
        [SerializeField] private Word wordU;

        [SerializeField] private Canvas[] worldSpaceCanvases;

        [SerializeField] private ResultManager resultManager;

        [SerializeField] private bool doCameraFollowPlayer;
        [SerializeField] private bool doClearWhenAllEnemiesDied;

        // ゲーム開始時に、配列を結合して正規化 (はめ込めるかチェックするときに使う)
        private Transform[] dstTrasforms;
        private CharacterState[] dstTypes;

        private bool hasAllEnemiesDied = false;

        private void Awake()
        {
            if (wordI != null)
            {
                wordI.CheckPutOnPointerUp = CheckPutOnPointerUp;
                wordI.OnPut = (state) =>
                {
                    if (player != null)
                        player.NowState = state;
                };
            }

            if (wordU != null)
            {
                wordU.CheckPutOnPointerUp = CheckPutOnPointerUp;
                wordU.OnPut = (state) =>
                {
                    foreach (Enemy enemy in enemies)
                        if (enemy != null)
                            enemy.NowState = state;
                };
            }

            if (player != null)
            {
                player.OnPlayerFailed = () => OnGameEnded(false);
                player.OnPlayerCleared = () => OnGameEnded(true);

                if (doCameraFollowPlayer)
                {
                    Camera.main.transform.SetParent(player.transform);

                    foreach (Canvas canvas in worldSpaceCanvases)
                    {
                        if (canvas != null)
                            canvas.transform.SetParent(player.transform);
                    }
                }
            }

            InitDstData();
        }

        private void Update()
        {
            if (doClearWhenAllEnemiesDied)
                CheckEnemiesDied();
        }

        private void InitDstData()
        {
            // 最初の子供がパネルになっているはず
            Func<Sentence, Transform> GetPanelTransform = (sentence) => sentence.transform.GetChild(0);

            CharacterState[] walkLeftsStates = MakeArray(walkLefts.Length, CharacterState.WalkLeft);
            CharacterState[] walkRightsStates = MakeArray(walkRights.Length, CharacterState.WalkRight);
            CharacterState[] runLeftsStates = MakeArray(runLefts.Length, CharacterState.RunLeft);
            CharacterState[] runRightsStates = MakeArray(runRights.Length, CharacterState.RunRight);
            CharacterState[] jumpsStates = MakeArray(jumps.Length, CharacterState.Jump);

            dstTrasforms = JoinArray(
                FuncArray(walkLefts, GetPanelTransform),
                FuncArray(walkRights, GetPanelTransform),
                FuncArray(runLefts, GetPanelTransform),
                FuncArray(runRights, GetPanelTransform),
                FuncArray(jumps, GetPanelTransform)
            );
            dstTypes = JoinArray(walkLeftsStates, walkRightsStates, runLeftsStates, runRightsStates, jumpsStates);

            static T[] MakeArray<T>(int length, T defaultValue = default)
            {
                T[] array = new T[length];
                for (int i = 0; i < length; i++)
                    array[i] = defaultValue;
                return array;
            }

            static T2[] FuncArray<T1, T2>(T1[] array, Func<T1, T2> func)
            {
                T2[] result = new T2[array.Length];
                for (int i = 0; i < array.Length; i++)
                    result[i] = func(array[i]);
                return result;
            }

            static T[] JoinArray<T>(params T[][] arrays)
            {
                int totalLength = 0;
                foreach (T[] array in arrays)
                    totalLength += array.Length;

                T[] result = new T[totalLength];
                int offset = 0;
                foreach (T[] array in arrays)
                {
                    Array.Copy(array, 0, result, offset, array.Length);
                    offset += array.Length;
                }
                return result;
            }
        }

        // 重なっているなら、はめ込める
        // はめ込む座標（無理ならnull）、はめ込み先の種類、既にはめ込んであって無理だったか、はめ込むSentence(パネルの親である想定) を返す
        private (Vector3?, CharacterState, bool, Sentence) CheckPutOnPointerUp(Transform src)
        {
            if (src == null) return default;

            for (int i = 0; i < dstTrasforms.Length; i++)
            {
                Transform dst = dstTrasforms[i];
                if (dst == null) continue;

                Vector3 srcPos = src.position;
                Vector3 srcScl = src.lossyScale;
                Vector3 srcTop = srcPos + Vector3.up * (srcScl.y * 0.5f);
                Vector3 srcBottom = srcPos - Vector3.up * (srcScl.y * 0.5f);
                Vector3 srcLeft = srcPos - Vector3.right * (srcScl.x * 0.5f);
                Vector3 srcRight = srcPos + Vector3.right * (srcScl.x * 0.5f);

                Vector3 dstPos = dst.position;
                Vector3 dstScl = dst.lossyScale;
                Vector3 dstTop = dstPos + Vector3.up * (dstScl.y * 0.5f);
                Vector3 dstBottom = dstPos - Vector3.up * (dstScl.y * 0.5f);
                Vector3 dstLeft = dstPos - Vector3.right * (dstScl.x * 0.5f);
                Vector3 dstRight = dstPos + Vector3.right * (dstScl.x * 0.5f);

                bool canPut = srcTop.y > dstBottom.y && srcBottom.y < dstTop.y &&
                              srcLeft.x < dstRight.x && srcRight.x > dstLeft.x;

                if (canPut)
                {
                    // すでにはめ込まれている (＝dstの座標に、wordsのいずれかがある) なら、はめ込めない
                    Word[] words = new Word[] { wordI, wordU };
                    foreach (Word word in words)
                    {
                        if (word == null) continue;
                        if (word.Position == (Vector2)dst.position)
                            return (null, CharacterState.None, true, null);
                    }

                    return (dstPos, dstTypes[i], false, dstTrasforms[i].parent.GetComponent<Sentence>());
                }
            }

            return default;
        }

        private void CheckEnemiesDied()
        {
            if (hasAllEnemiesDied) return;

            foreach (Enemy enemy in enemies)
            {
                if (enemy == null) continue;
                if (!enemy.Died) return;
            }

            hasAllEnemiesDied = true;
            OnGameEnded(true);
        }

        private void OnGameEnded(bool cleared)
        {
            if (resultManager != null)
                resultManager.Show(cleared);
        }
    }
}