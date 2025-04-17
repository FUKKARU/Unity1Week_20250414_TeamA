using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommentDataLoop
{
    [TextArea]
    public string message;
    public int weight = 1; // 出やすさ（任意）
}

public class FloatingCommentLooper : MonoBehaviour
{
    public GameObject commentPrefab;
    public RectTransform parentCanvas;

    public List<CommentDataLoop> commentList = new List<CommentDataLoop>();

    public float minY = 50f;
    public float maxY = 400f;
    public float minSpeed = 80f;
    public float maxSpeed = 150f;

    public float spawnIntervalMin = 0.5f;
    public float spawnIntervalMax = 2.0f;

    private bool isRunning = true;

    void Start()
    {
        StartCoroutine(LoopSpawn());
    }

    private IEnumerator LoopSpawn()
    {
        while (isRunning)
        {
            SpawnRandomComment();
            float delay = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnRandomComment()
    {
        if (commentList.Count == 0) return;

        string message = GetRandomComment();

        GameObject go = Instantiate(commentPrefab, parentCanvas);
        var comment = go.GetComponent<FloatingComment>();

        float y = Random.Range(minY, maxY);
        float speed = Random.Range(minSpeed, maxSpeed);

        comment.Init(message, speed, y);
    }

    private string GetRandomComment()
    {
        // シンプルなランダム選出（重みなし）
        int index = Random.Range(0, commentList.Count);
        return commentList[index].message;
    }
}
