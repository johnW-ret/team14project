namespace TeamFourteen.CoreGame
{
    public interface IOwnedPickupable : IPickupable
    {
        bool CanPickup { get; }
        bool RequestRelease(ObjectHolder holder);
    }
}
