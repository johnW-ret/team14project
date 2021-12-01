namespace TeamFourteen.CoreGame
{
    public delegate void OnValueUpdate<T>(T value);

    public interface IValuePublisher<T>
    {
        OnValueUpdate<T> ValueUpdateEvent { get; set; }
    }
}