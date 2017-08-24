using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : List<Card> {
    public EventHandler OnAdd;
    public EventHandler OnRemove;

    public void Add(Card item)
    {
        if (null != OnAdd)
        {
            OnAdd(this, new HandEventArgs(item,this.Count));
        }
        base.Add(item);
    }

    public void Remove(Card item)
    {
        if (null != OnRemove)
        {
            OnRemove(this, new HandEventArgs(item,this.IndexOf(item)));
        }
        base.Remove(item);
    }

    public int nextFree()
    {
     return  this.FindIndex(x => x = null);
    }

}

public class HandEventArgs:EventArgs
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
