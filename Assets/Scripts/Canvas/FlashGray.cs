using UnityEngine;
using TMPro;

public class FlashGray : MonoBehaviour
{
    private TextMeshProUGUI textElement;
    
    [Header("Settings")]
    public float speed = 1.5f;
    public Color darkGray = new Color(0.2f, 0.2f, 0.2f,0.5f);
    public Color lightGray = new Color(0.8f, 0.8f, 0.8f,0.5f);

    void Start()
    {
        textElement = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Generates a value between 0 and 1 that bounces back and forth
        float t = Mathf.PingPong(Time.time * speed, 1.0f);
        
        // Applies the color transition
        textElement.color = Color.Lerp(darkGray, lightGray, t);
    }
}
