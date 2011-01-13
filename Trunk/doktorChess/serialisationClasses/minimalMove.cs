namespace doktorChess
{
    /// <summary>
    /// This class is used by the JSON code to express a move intended. It is then used to
    /// construct a proper move class when given a Board.
    /// </summary>
// ReSharper disable ClassNeverInstantiated.Global
    public class minimalMove
// ReSharper restore ClassNeverInstantiated.Global
    {
// ReSharper disable UnassignedField.Global
        public squarePos srcSquarePos;
        public squarePos dstSquarePos;
// ReSharper restore UnassignedField.Global
    }
}