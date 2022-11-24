using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    public static Deck instance;

    public Card inStack;
    public List<Card> library = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    public TMP_Text deckSizeText;
    public TMP_Text discardSizeText;

    // Events
    public delegate void OnResolve();
    public event OnResolve event_OnResolve;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        UpdateUI();

        ShuffleCards(library);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            DrawCard();
    }

    public void SetToStack(int index)
    {
        inStack = hand[index];
        hand.RemoveAt(index);
    }

    public void DrawCard()
    {
        if(hand.Count >= 10)
        {
            ActionManager.instance.actionFinished = true;
            return;
        }

        if (library.Count <= 0)
            DiscardIntoDeck();

        if (library.Count <= 0)
        {
            ActionManager.instance.actionFinished = true;
            return;
        }

        Card card = library[library.Count - 1];

        hand.Add(card);
        library.RemoveAt(library.Count - 1);

        UIHand.instance.AddCardToHand(card);

        UpdateUI();

        SoundManager.instance.PlaySound(5, 1f, 1f);

        ActionManager.instance.actionFinished = true;
    }

    public void DiscardAt(int index)
    {
        if (hand.Count <= 0 || index > hand.Count - 1)
            return;

        Card card = hand[index];

        discardPile.Add(card);
        hand.Remove(card);

        UICard handCard = UIHand.instance.cardsInHand[index].GetComponent<UICard>();
        UIHand.instance.RemoveCardFromhand(handCard, true);

        UpdateUI();

        SoundManager.instance.PlaySound(6, 1f, 1f);
    }

    public void DiscardRandom()
    {
        if (hand.Count <= 0)
        {
            ActionManager.instance.actionFinished = true;
            return;
        }

        int r = Random.Range(0, hand.Count);

        DiscardAt(r);

        ActionManager.instance.actionFinished = true;
    }

    public void Resolve(UICard handCard)
    {
        if (inStack == null)
            return;

        discardPile.Add(inStack);

        inStack = null;

        UIHand.instance.RemoveCardFromhand(handCard, true);

        UpdateUI();

        SoundManager.instance.PlaySound(6, 1f, 0.5f);

        event_OnResolve?.Invoke();
    }

    public void Consume(UICard handCard)
    {
        UIHand.instance.RemoveCardFromhand(handCard, false);
        inStack = null;

        UpdateUI();
    }

    public void DiscardAll()
    {
        discardPile.AddRange(hand);
        hand.Clear();

        UIHand.instance.RemoveAllCards();

        UpdateUI();

        SoundManager.instance.PlaySound(6, 1f, 1f);
    }

    public void DiscardIntoDeck()
    {
        library.AddRange(discardPile);
        discardPile.Clear();

        ShuffleCards(library);

        UpdateUI();
    }

    public void ResetDeck()
    {
        library.AddRange(discardPile);
        discardPile.Clear();

        library.AddRange(hand);
        hand.Clear();

        ShuffleCards(library);

        UIHand.instance.RemoveAllCards();

        UpdateUI();
    }

    public void AddNewCard(Card card)
    {
        discardPile.Add(card);

        UpdateUI();
    }

    private void UpdateUI()
    {
        deckSizeText.text = library.Count + "/" + TotalCards;
        discardSizeText.text = "" + discardPile.Count;
    }

    private void ShuffleCards(List<Card> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int r = Random.Range(i, cards.Count);
            cards[i] = cards[r];
            cards[r] = temp;
        }
    }

    private int TotalCards
    {
        get { return library.Count + hand.Count + discardPile.Count; }
    }
}
