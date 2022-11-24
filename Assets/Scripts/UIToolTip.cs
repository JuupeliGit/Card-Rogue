using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string toolTipText;

    private Transform toolTipUI;

    private void Start()
    {
        toolTipUI = FindObjectOfType<UIFollowCursor>().transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toolTipUI == null)
            return;

        toolTipUI.GetChild(0).gameObject.SetActive(true);
        toolTipUI.GetComponentInChildren<TMP_Text>().text = toolTipText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTipUI == null)
            return;

        toolTipUI.GetChild(0).gameObject.SetActive(false);
    }
}
