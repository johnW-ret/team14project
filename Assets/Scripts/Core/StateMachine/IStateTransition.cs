// Written by John Whatley for personal project (Retruate). Used with permission (from self).
using System;

namespace TeamFourteen.Core
{
    public interface IStateTransition
    {
        Type StateType { get; }
        Type CommandType { get; }
        string ToString();
        int GetHashCode();

        bool Equals(object obj);
    }
}