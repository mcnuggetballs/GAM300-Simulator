using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Dialogue;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI textBox;
    public Image PlayerIcon;
    public float targetY;
    public float bubbleUpSpeed = 1.0f;
    public void SetTargetY(float value)
    {
        targetY = value;
    }
    public void Snap()
    {
        Vector2 pos = transform.localPosition;
        pos.y = targetY;
        transform.localPosition = pos;
    }
    private void Update()
    {
        if (transform.localPosition.y < targetY)
        {
            Vector2 pos = transform.localPosition;
            pos.y += bubbleUpSpeed * Time.unscaledDeltaTime;
            transform.localPosition = pos;
        }
        else
        {
            targetY = transform.localPosition.y;
        }
    }
}
