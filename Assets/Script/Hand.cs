using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : List<Card>
{
    public EventHandler OnAdd;
    public EventHandler OnRemove;
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

    public new void Add(Card item)
    {
        if (null != OnAdd)
        {
            OnAdd(this, new HandEventArgs(item, this.Count));
        }
        base.Add(item);

    }

    public new void Remove(Card item)
    {
        if (null != OnRemove)
        {
            OnRemove(this, new HandEventArgs(item, IndexOf(item)));
        }
        
        base.Remove(item);
    }



    private int nextFree()
    {
        int nextIndexFree = 0;
        if (this.Count>0)
        {
            nextIndexFree = this.FindIndex(x => x = null);
        }
        return nextIndexFree;
    }

    public void PickACard()
    {
        if (this.Count < 5)
        {
            this.Add(Deck.DrawACard());
        }
    }

    public void MakeHand()
    {
        for (int i = 0; i < 5; i++)
        {
            this.PickACard();
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
