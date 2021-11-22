// Written by John Whatley for personal project (Retruate). Used with permission (from self).
using System;

namespace TeamFourteen.Core
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(IStateTransition transition) : base("Invalid transition: " + transition.ToString()) { }
    }
}