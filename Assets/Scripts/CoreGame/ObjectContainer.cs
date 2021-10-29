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

            public virtual void Select(T _object)
            {
                selected = _object;
            }

            public virtual void Deselect()
            {
                selected = null;
            }
        }

        public class ActionObjectContainer<T> : ObjectContainer<T> where T : class
        {
            public delegate void OnSelectEvent(T _object);
            public readonly OnSelectEvent OnSelect;
            public readonly OnSelectEvent OnDeselect;

            public ActionObjectContainer(OnSelectEvent OnSelect, OnSelectEvent OnDeselect, T initialSelected = null)
            {
                this.OnSelect += OnSelect;
                this.OnDeselect += OnDeselect;
            }

            public override void Select(T _object)
            {
                OnSelect.Invoke(_object);

                base.Select(_object);
            }

            public override void Deselect()
            {
                OnDeselect.Invoke(Selected);

                base.Deselect();
            }
        }
    }
}
