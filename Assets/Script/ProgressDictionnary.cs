using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressDictionnary : Dictionary<Pawn, int> {
    

    public void Add(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        base.Add(target, 18 * (int)target.PlayerColor);
    }

    public void Move(string pawnTarget, int nbCell)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int key = this[target]+nbCell;
        if (key > 70) key -= 70;
        base.Remove(target);
        this[target] = key;
    }

    public void Remove(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        base.Remove(target);
    }

    public Pawn GetPawn(int progress)
    {
        Pawn pawnReturned = null;
        if (this.ContainsValue(progress))
        {
            foreach (Pawn item in this.Keys)
            {
                if (this[item] == progress)
                {
                    pawnReturned = item;
                }
            }
        }
        return pawnReturned;
    }
}
 