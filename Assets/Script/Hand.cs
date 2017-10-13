using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special list for handling the player's hand
/// </summary>
public class PlayerHand : List<Card>
{
    public EventHandler OnAdd;
    public EventHandler OnRemove;

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
}

/// <summary>
/// Special EventArgs used with the PlayerHand
/// </summary>
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
