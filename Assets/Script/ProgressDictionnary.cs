using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Special Dictionnary used to keep track of the pawns
/// </summary>
public class ProgressDictionnary : Dictionary<Pawn, int>, IProgressDictionnary
{
    public Dictionary<Pawn, string> Houses = new Dictionary<Pawn, string>();
    /// <summary>
    /// Compute the position of the target according to its color and add it 
    /// </summary>
    /// <param name="pawnTarget">string - name of the target</param>
    /// <returns>the new position</returns>
    public int Add(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int position =  18 * target.OwningPlayerIndex;
        base.Add(target, position);
        Debug.Log("Added : " + pawnTarget + "- to ProgressDico at position : " + position);

        return position;
    }


    /// <summary>
    /// Move the target in the dictionnary
    /// </summary>
    /// <param name="pawnTarget">string - name of the target</param>
    /// <param name="nbCell">int - number of cell</param>
    /// <returns>the new position</returns>
    public int Move(string pawnTarget, int nbCell)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        int newPosition = target.ProgressInDictionnary + nbCell;

        if (this.ContainsKey(target))
        {
            base.Remove(target);
        }

        if (target.Progress < 71)
        {
            this[target] = TestPosition(newPosition);
            Debug.Log("Moved : " + target + " for " + nbCell + " cells in ProgressDico, new position : " + newPosition);

        }
        else
        {
            if (Houses.ContainsKey(target))
            {
                Houses.Remove(target);
            }
            Houses[target] = target.OwningPlayerIndex.ToString()+ "_" + target.Progress;
            newPosition = 75 - target.Progress;
            Debug.Log("Moved : " + target + " for " + nbCell + " cells in ProgressDico, entering House : " + target.PlayerColorE.ToString() + target.Progress + " , new Position : " + newPosition);

        }
        return newPosition;
    }

    /// <summary>
    /// Remove the target in the dictionnary
    /// </summary>
    /// <param name="pawnTarget">string - name of the target</param>
    public void Remove(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        base.Remove(target);
    }

    /// <summary>
    /// Return the pawn corresponding to a position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Return all the pawns between to position
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="lastIndex"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Fix the position if it is out of bound
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int TestPosition(int position)
    {
        int rPosition = position;

        if (position > 75) rPosition -= 75;
        if (position < 0) rPosition += 75;

        return rPosition;
    }

    /// <summary>
    /// Compute the number of cell between to pawns
    /// </summary>
    /// <param name="target1"></param>
    /// <param name="target2"></param>
    /// <returns></returns>
    public int[] ExchangeCompute(Pawn target1, Pawn target2)
    {
        int[] nbMoves = new int[2];

        nbMoves[0] = (TestPosition(this[target2] - (int)target1.PlayerColorE * 18)) - target1.Progress;
        nbMoves[1] = (TestPosition(this[target1] - (int)target2.PlayerColorE * 18)) - target2.Progress;

        return nbMoves;
    }

    public bool TestHouseFree(string name,int nbMoves)
    {
        Pawn target = GameObject.Find(name).GetComponent<Pawn>();
        return Houses.ContainsValue(target.PlayerColorE.ToString() + (target.Progress + nbMoves).ToString());
    }

    public bool HasPawn(Pawn pawnTested)
    {
        return this.ContainsKey(pawnTested);
    }

    public bool HasValue(int progress)
    {
        throw new System.NotImplementedException();
    }
}
 