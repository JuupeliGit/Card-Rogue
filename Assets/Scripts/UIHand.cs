using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHand : MonoBehaviour
{
    public static UIHand instance;

    public Transform handTransform;
    public Transform reserveTransform;

    public GameObject cardPrefab;
    public Card inStack;
    public List<GameObject> cardsInReserve = new List<GameObject>();
    public List<GameObject> cardsInHand = new List<GameObject>();

    private int currentId;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        for(int i = 0; i < 10; i++)
        {
            GameObject newCard = (GameObject)Instantiate(cardPrefab, reserveTransform);
            cardsInReserve.Add(newCard);
        }
    }

    public void AddCardToHand(Card card)
    {
        currentId++;

        GameObject cardUI = cardsInReserve[cardsInReserve.Count - 1];
        cardUI.GetComponent<UICard>().interactable = true;
        cardUI.GetComponent<UICard>().SetCardInfo(card, Deck.instance.hand.Count - 1);

        cardsInHand.Add(cardUI);
        cardsInReserve.Remove(cardUI);

        cardUI.transform.SetParent(handTransform);
        cardUI.transform.SetAsLastSibling();
        cardUI.SetActive(true);
    }

    public void RemoveCardFromhand(UICard handCard, bool discard)
    {
        GameObject cardUI = cardsInHand[FindCardInHand(handCard)];

        cardsInReserve.Add(cardUI);
        cardsInHand.Remove(cardUI);

        cardUI.transform.SetParent(reserveTransform);

        if (discard)
            StartCoroutine(DiscardAnimation(cardUI));
        else
            StartCoroutine(ConsumeAnimation(cardUI));
    }

    public void RemoveAllCards()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            UICard handCard = cardsInHand[i].GetComponent<UICard>();
            RemoveCardFromhand(handCard, true);
        }
    }

    public int FindCardInHand(UICard card)
    {
        if (cardsInHand.IndexOf(card.gameObject) != -1)
            return cardsInHand.IndexOf(card.gameObject);

        return 0;
    }

    private void LateUpdate()
    {
        for(int i = 0; i < handTransform.childCount; i++)
        {
            RectTransform cardRect = handTransform.GetChild(i).GetComponent<RectTransform>();
            UICard cardUI = cardRect.GetComponent<UICard>();

            float posX = cardRect.anchoredPosition.x;
            float posY = cardRect.anchoredPosition.y;

            if (!cardRect.GetComponent<UICard>().isDragging)
            {
                float offsetX = -32f * handTransform.childCount + i * 64f - 48f;
                float offsetY = Mathf.Sin((float)i / handTransform.childCount * 4) * 10 - 16;
                offsetY += cardUI.isHovered ? 32 : 0;

                if (ActionManager.instance.cardBeingPlayed)
                    offsetY = -50f;

                posX = Mathf.Lerp(cardRect.anchoredPosition.x, 640 / 2 + offsetX, Time.deltaTime * 4);
                posY = Mathf.Lerp(cardRect.anchoredPosition.y, 40 + offsetY, Time.deltaTime * 4);
            }

            cardRect.anchoredPosition = new Vector2(posX, posY);
        }
    }

    private IEnumerator DiscardAnimation(GameObject cardUI)
    {
        RectTransform cardRect = cardUI.GetComponent<RectTransform>();

        Vector2 startPos = cardRect.anchoredPosition;
        Vector2 endPos = new Vector2(80, -64);

        float elapsed = 0f;
        float time = 0.25f;

        while (elapsed < time)
        {
            Vector2 curPos = Vector2.Lerp(startPos, endPos, elapsed / time);
            cardRect.anchoredPosition = curPos;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cardUI.SetActive(false);

        cardUI.transform.position = cardUI.transform.parent.position;
    }

    private IEnumerator ConsumeAnimation(GameObject cardUI)
    {
        RectTransform cardRect = cardUI.GetComponent<RectTransform>();

        Vector2 startPos = cardRect.anchoredPosition;
        Vector2 endPos = new Vector2(cardRect.anchoredPosition.x, 400);

        float elapsed = 0f;
        float time = 0.5f;

        while (elapsed < time)
        {
            Vector2 curPos = Vector2.Lerp(startPos, endPos, elapsed / time);
            cardRect.anchoredPosition = curPos;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cardUI.SetActive(false);

        cardUI.transform.position = cardUI.transform.parent.position;
    }
}
