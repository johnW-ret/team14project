namespace TeamFourteen.Selection
{
    public delegate void OnSelectEvent<T>(T _object);

    /// <summary>
    /// Holds object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of object that is being selected.</typeparam>
    public class ObjectContainer<T> where T : class
    {
        public ObjectContainer(T initialSelected = null)
        {
            selected = initialSelected;
        }

        private T selected;
        public T Selected => selected;

        public virtual void Select(T _object)
        {
            selected = _object;
        }

        public virtual void Deselect()
        {
            selected = null;
        }
    }
}
