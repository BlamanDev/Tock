using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressDictionnary : Dictionary<Pawn, int> {
    

    public int Add(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int position =  18 * (int)target.PlayerColor;
        base.Add(target, position);

        return position;
    }

    public int Move(string pawnTarget, int nbCell)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int newPosition = this.TestPosition(this[target]+nbCell);
        base.Remove(target);
        this[target] = newPosition;
        return newPosition;
    }

    public void Remove(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        base.Remove(target);
    }

    public Pawn GetPawn(int position)
    {
        Pawn pawnReturned = null;
        if (this.ContainsValue(position))
        {
            foreach (Pawn item in this.Keys)
            {
                if (this[item] == position)
                {
                    pawnReturned = item;
                }
            }
        }
        return pawnReturned;
    }

    public List<Pawn> GetPawnsInRange(int startIndex, int lastIndex)
    {
        List<Pawn> returnList = new List<Pawn>();
        foreach (Pawn item in this.Keys)
        {
            if (this[item] > startIndex && this[item] <= lastIndex)
            {
                returnList.Add(item);
            }
        }
        return returnList;
    }

    public int TestPosition(int position)
    {
        int rPosition = position;

        if (position > 72) rPosition -= 72;
        if (position < 0) rPosition += 72;

        return rPosition;
    }

    public int[] ExchangeCompute(Pawn target1, Pawn target2)
    {
        int[] nbMoves = new int[2];

        nbMoves[0] = (TestPosition(this[target2] - (int)target1.PlayerColor * 18)) - target1.Progress;
        nbMoves[1] = (TestPosition(this[target1] - (int)target2.PlayerColor * 18)) - target2.Progress;

        return nbMoves;
    }
}
 