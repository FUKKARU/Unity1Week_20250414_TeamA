using UnityEngine;

namespace NInGame
{
    public abstract class AStageManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Enemy[] enemies;

        // nullでも良い、はめ込む先のTransform
        [SerializeField] private Transform walk;
        [SerializeField] private Transform run;
        [SerializeField] private Transform jump;

        [SerializeField] private Word wordI;
        [SerializeField] private Word wordU;

        [SerializeField] private ResultManager resultManager;

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
        }

        private void Update()
        {
            CheckEnemiesDied();
        }

        // 重なっているなら、はめ込める
        // はめ込む座標（無理ならnull）、はめ込み先の種類、既にはめ込んであって無理だったか
        private (Vector3?, CharacterState, bool) CheckPutOnPointerUp(Transform src)
        {
            if (src == null) return default;

            Transform[] dstTrasforms = { walk, run, jump };
            CharacterState[] dstTypes = { CharacterState.Walk, CharacterState.Run, CharacterState.Jump };

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
                        if ((Vector2)word.Position == (Vector2)dst.position)
                            return (null, CharacterState.None, true);
                    }

                    return (dstPos, dstTypes[i], false);
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