using System;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    /// <summary>
    /// Holds objects of type <typeparamref name="T"/> and calls events that we have subscribed to.
    /// </summary>
    /// <typeparam name="T">Type of object that is being selected.</typeparam>
    public class ActionObjectContainer<T> : ObjectContainer<T> where T : class
    {
        // make private?
        public OnSelectEvent<T> OnSelect;
        public OnSelectEvent<T> OnDeselect;

        public ActionObjectContainer(OnSelectEvent<T> OnSelect, OnSelectEvent<T> OnDeselect, T initialSelected = null)
        {
            this.OnSelect += OnSelect;
            this.OnDeselect += OnDeselect;
        }

        public bool Subscribe(ISelectionHandler<T> handler)
        {
            try
            {
                OnSelect += handler.OnSelect;
                OnDeselect += handler.OnDeselect;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not add events to handler:\n {e}");
                return false;
            }
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
