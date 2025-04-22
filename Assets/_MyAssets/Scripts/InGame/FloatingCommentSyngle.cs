using UnityEngine;
using TMPro;

public class FloatingCommentSyngle : MonoBehaviour
{
    public string message = "Hello, world!";
    public float speed = 100f;
    public bool playOnStart = true;

    private RectTransform rectTransform;
    private float screenWidth;

    private bool isPlaying = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        screenWidth = Screen.width;
        GetComponent<TextMeshProUGUI>().text = message;
        // レイキャスト無効化
        var text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.raycastTarget = false;
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            StartComment();
        }
    }

    public void StartComment()
    {
        // 画面外右に初期化
        rectTransform.anchoredPosition = new Vector2(screenWidth , rectTransform.anchoredPosition.y);
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying) return;

        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        if (rectTransform.anchoredPosition.x < -screenWidth - 100)
        {
            Destroy(gameObject); // 画面外に出たら削除
        }
    }
}
