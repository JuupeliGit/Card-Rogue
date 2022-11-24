using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCursor : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float posX = (Input.mousePosition.x + 8) / Screen.width * 480f;
        float posY = (Input.mousePosition.y + 12) / Screen.height * 320f;

        posX = Mathf.Clamp(posX, rectTransform.rect.width + 4, 476);
        posY = Mathf.Clamp(posY, 4, 316 - rectTransform.rect.height);

        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }
}
