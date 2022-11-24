using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool interactable;

    public Card card;
    public int id;

    public Image cardImage;
    public TMP_Text cardNameText;
    public TMP_Text cardAbilityText;

    public bool isDragging;
    public bool isHovered;

    private Vector2 offset;
    public int oldSiblingIndex;
    private Transform oldParent;

    private RectTransform rectTransform;

    public void SetCardInfo(Card card, int id)
    {
        this.card = card;
        this.id = id;

        cardImage.sprite = card.cardSprite;
        cardNameText.text = card.cardName;

        cardAbilityText.text = "";
        for(int i = 0; i < card.abilities.Length; i++)
        {
            CardAbility a = card.abilities[i];
            cardAbilityText.text += a.ability.name + (a.showAmount ? " " + a.amount : "");
            if (i < card.abilities.Length - 1)
                cardAbilityText.text += "," + '\n';
        }

        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable || ActionManager.instance.cardBeingPlayed)
            return;

        Vector2 mousePos = Input.mousePosition;
        offset = (Vector2)transform.position - mousePos;

        oldSiblingIndex = transform.GetSiblingIndex();

        oldParent = transform.parent;
        transform.SetParent(oldParent.parent);

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable || !isDragging)
            return;

        Vector2 mousePos = Input.mousePosition;
        transform.position = mousePos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!interactable || !isDragging)
            return;

        Vector2 mousePos = Input.mousePosition;
        transform.position = mousePos + offset;

        isDragging = false;

        if(rectTransform.anchoredPosition.y < 128)
        {
            transform.SetParent(oldParent);
            transform.SetSiblingIndex(oldSiblingIndex);
        }
        else
        {
            isHovered = false;

            interactable = false;
            StartCoroutine(MoveToCorner());

            Deck.instance.SetToStack(oldSiblingIndex);

            ActionManager.instance.PlayCard(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.1f;

        oldSiblingIndex = transform.GetSiblingIndex();
        //transform.SetAsLastSibling();

        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;

        transform.SetSiblingIndex(oldSiblingIndex);

        isHovered = false;
    }

    private IEnumerator MoveToCorner()
    {
        float elapsed = 0f;
        float time = 0.1f;

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(472 - 64 / 2, 290 - 96 / 2);

        while (elapsed < time)
        {
            Vector2 curPos = Vector2.Lerp(startPos, endPos, elapsed / time);
            rectTransform.anchoredPosition = curPos;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.anchoredPosition = endPos;
    }
}
