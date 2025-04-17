using System;
using UnityEngine;

namespace NInGame
{
    public abstract class AStageManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Enemy[] enemies;

        // はめ込む先のTransform
        [SerializeField] private Transform[] walks;
        [SerializeField] private Transform[] runs;
        [SerializeField] private Transform[] jumps;

        [SerializeField] private Word wordI;
        [SerializeField] private Word wordU;

        [SerializeField] private ResultManager resultManager;

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
            }

            InitDstData();
        }

        private void Update()
        {
            CheckEnemiesDied();
        }

        private void InitDstData()
        {
            int walkLen = walks.Length;
            int runLen = runs.Length;
            int jumpLen = jumps.Length;

            CharacterState[] walkStates = new CharacterState[walkLen];
            for (int i = 0; i < walkLen; i++)
                walkStates[i] = CharacterState.Walk;
            CharacterState[] runStates = new CharacterState[runLen];
            for (int i = 0; i < runLen; i++)
                runStates[i] = CharacterState.Run;
            CharacterState[] jumpStates = new CharacterState[jumpLen];
            for (int i = 0; i < jumpLen; i++)
                jumpStates[i] = CharacterState.Jump;

            dstTrasforms = new Transform[walkLen + runLen + jumpLen];
            Array.Copy(walks, 0, dstTrasforms, 0, walkLen);
            Array.Copy(runs, 0, dstTrasforms, walkLen, runLen);
            Array.Copy(jumps, 0, dstTrasforms, walkLen + runLen, jumpLen);

            dstTypes = new CharacterState[walkLen + runLen + jumpLen];
            Array.Copy(walkStates, 0, dstTypes, 0, walkLen);
            Array.Copy(runStates, 0, dstTypes, walkLen, runLen);
            Array.Copy(jumpStates, 0, dstTypes, walkLen + runLen, jumpLen);
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
            OnGameEnded(false);
        }

        private void OnGameEnded(bool cleared)
        {
            if (resultManager != null)
                resultManager.Show(cleared);
        }
    }
}