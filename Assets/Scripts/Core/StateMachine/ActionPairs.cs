// Written by John Whatley for personal project (Retruate). Used with permission (from self).
using System;
using System.Collections.Generic;

namespace TeamFourteen.Core
{
    public class ActionPairs<T>
    {
        private Dictionary<T, Action> actionPairs = new Dictionary<T, Action>();
        public void SetAction(Action action, T value)
        {
            if (actionPairs.TryGetValue(value, out _))
                actionPairs.Remove(value);
            else
                actionPairs.Add(value, action);
        }

        public void TryInvoke(T value)
        {
            if (actionPairs.TryGetValue(value, out Action action))
                action.Invoke();
        }
    }
}