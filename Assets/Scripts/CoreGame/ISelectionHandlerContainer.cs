namespace TeamFourteen.CoreGame
{
    /// <summary>
    /// Represents a container that holds references to <see cref="ISelectionHandler{T}"/> and calls <see cref="OnSelectEvent{T}"/> events.
    /// </summary>
    /// <typeparam name="T">Type of object that is being selected.</typeparam>
    public interface ISelectionHandlerContainer<T>
    {
        bool Subscribe(ISelectionHandler<T> handler);
        
        // maybe Unsubcribe(ISelectionHandler<T> handler) ??? lol
        // would need dictionary maybe idk. not really needed now.
    }
}