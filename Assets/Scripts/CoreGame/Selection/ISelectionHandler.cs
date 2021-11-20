namespace TeamFourteen.Selection
{
    /// <summary>
    /// Represents an object that can execute selection event methods of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of object that is being selected.</typeparam>
    public interface ISelectionHandler<T>
    {
        OnSelectEvent<T> OnSelect { get; }
        OnSelectEvent<T> OnDeselect { get; }
    }
}
