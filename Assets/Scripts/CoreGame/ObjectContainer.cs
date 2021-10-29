namespace TeamFourteen.CoreGame
{
    public partial class HoldObject
    {
        public class ObjectContainer<T> where T : class
        {
            public ObjectContainer(T initialSelected = null)
            {
                selected = initialSelected;
            }

            private T selected;
            public T Selected => selected;

            public void Select(T _object)
            {
                selected = _object;
            }

            public void Deselect()
            {
                selected = null;
            }
        }
    }
}
