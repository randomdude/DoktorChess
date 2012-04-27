namespace TestMateSituations
{
    public class mateSituation
    {
        public int movesToMate;
        public string FEN;

        public mateSituation(int newMovesToMate, string newFEN)
        {
            movesToMate = newMovesToMate;
            FEN = newFEN;
        }
    }
}