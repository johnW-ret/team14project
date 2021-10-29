using System;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public interface IPickupable
    {
        void Pickup(Transform holder, Action OnComplete);
        void Release();
    }
}
