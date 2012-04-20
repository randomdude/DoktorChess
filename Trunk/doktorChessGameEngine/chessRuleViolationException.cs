using System;

namespace doktorChessGameEngine
{
    public class chessRuleViolationException : Exception
    {
        public chessRuleViolationException() : base() { }
        public chessRuleViolationException(string why) : base(why) { }

    }
}