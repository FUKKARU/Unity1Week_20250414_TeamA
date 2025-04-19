using UnityEngine;
using TMPro;

public class FloatingComment : MonoBehaviour
{
    public float speed = 100f;

    private RectTransform rectTransform;
    private float screenWidth;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        screenWidth = Screen.width;
        // レイキャスト無効化
        var text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.raycastTarget = false;
        }
    }

    public void Init(string message, float speed, float yPosition)
    {
        this.speed = speed;
        GetComponent<TextMeshProUGUI>().text = message;
        rectTransform.anchoredPosition = new Vector2(screenWidth+50 , yPosition);
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        if (rectTransform.anchoredPosition.x < -screenWidth -100)
        {
            Destroy(gameObject);
        }
    }
}
