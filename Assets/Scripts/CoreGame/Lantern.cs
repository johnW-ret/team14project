using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Lantern : RigidObject
    {
        protected override bool CanReleaseTo(ObjectHolder holder)
        {
            return base.CanReleaseTo(holder) && holder.gameObject.layer == LayerMask.NameToLayer("Player");
        }
    }
}