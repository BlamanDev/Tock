using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressDictionnary : Dictionary<Pawn, int> {
    

    public int Add(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int newProgress =  18 * (int)target.PlayerColor;
        base.Add(target, newProgress);

        return newProgress;
    }

    public int Move(string pawnTarget, int nbCell)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int key = this[target]+nbCell;
        if (key > 70) key -= 70;
        base.Remove(target);
        this[target] = key;
        return key;
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
 