using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommentData
{
    [TextArea]
    public string message;
    public int spawnCount = 1; // èoÇ∑êî
}

public class FloatingCommentGroup : MonoBehaviour
{
    public GameObject commentPrefab;
    public RectTransform parentCanvas;

    public List<CommentData> commentGroup = new List<CommentData>();

    public float minY = 50f;
    public float maxY = 400f;
    public float minSpeed = 80f;
    public float maxSpeed = 150f;

    public float spawnDelayMin = 0f;
    public float spawnDelayMax = 1f;

    public void SpawnGroup()
    {
        foreach (var data in commentGroup)
        {
            for (int i = 0; i < data.spawnCount; i++)
            {
                float delay = Random.Range(spawnDelayMin, spawnDelayMax);
                StartCoroutine(SpawnOne(data.message, delay));
            }
        }
    }

    private IEnumerator SpawnOne(string message, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject go = Instantiate(commentPrefab, parentCanvas);
        var comment = go.GetComponent<FloatingComment>();

        float y = Random.Range(minY, maxY);
        float speed = Random.Range(minSpeed, maxSpeed);

        comment.Init(message, speed, y);
    }

    void Start()
    {
        SpawnGroup();
    }
}
