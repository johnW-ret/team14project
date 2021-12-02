namespace TeamFourteen.CoreGame
{
    public class Lantern : RigidObject
    {
        protected override bool CanReleaseTo(ObjectHolder holder)
        {
            return base.CanReleaseTo(holder) && holder.gameObject.layer == (int)LayerManager.Layers.Player;
        }
    }
}