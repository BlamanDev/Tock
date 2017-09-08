using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : List<Card>
{
    public EventHandler OnAdd;
    public EventHandler OnRemoveAt;
    private Deck deck;

    public Deck Deck
    {
        get
        {
            if (deck == null)
            {
                deck = GameObject.FindObjectOfType<Deck>();
            }

            return deck;
        }

        set
        {
            deck = value;
        }
    }

    public PlayerHand()
    {

    }

    public void Add(Card item)
    {
        if (null != OnAdd)
        {
            OnAdd(this, new HandEventArgs(item, this.Count));
        }
        base.Add(item);
    }

    public void RemoveAt(int CardIndex)
    {
        Card item = this[CardIndex];
        if (null != OnRemoveAt)
        {
            OnRemoveAt(this, new HandEventArgs(item, CardIndex));
        }
        Deck.CardsInDeck.Add(item);
        base.Remove(item);
    }

    public int nextFree()
    {
        return this.FindIndex(x => x = null);
    }

    public void PickACard()
    {
        if (this.Count < 5)
        {
            this.Add(Deck.DrawACard());
        }
    }

}

public class HandEventArgs : EventArgs
{
    private Card card;
    private int cardPosition;

    public Card Card
    {
        get
        {
            return card;
        }

    }

    public int CardPosition
    {
        get
        {
            return cardPosition;
        }

    }

    public HandEventArgs(Card card, int cardPosition)
    {
        this.card = card;
        this.cardPosition = cardPosition;
    }
}
