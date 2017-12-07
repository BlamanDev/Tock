using System.Collections.Generic;

public interface IProgressDictionnary
{
    int Add(string pawnTarget);
    int[] ExchangeCompute(Pawn target1, Pawn target2);
    Pawn GetPawn(int position);
    List<Pawn> GetPawnsInRange(int startIndex, int lastIndex);
    int Move(string pawnTarget, int nbCell);
    void Remove(string pawnTarget);
    bool TestHouseFree(string name,int nbMoves);
    bool HasPawn(Pawn pawnTested);
    bool HasValue(int progress);
    int TestPosition(int position);
}