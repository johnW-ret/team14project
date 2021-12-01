using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Column : ObjectHolder
    {
        [SerializeField] [HideInInspector] private ParticleSystem flame;
        [SerializeField] [HideInInspector] private Checkpoint checkpoint;

        [Header("Values")]
        [SerializeField] private float pickupRadius = 2.5f;
        private bool lit;
        private static int idCounter = -1;
        private static Dictionary<int, Column> columns = new Dictionary<int, Column>();
        // bad but oh well
        public static int NumberOfColumnsRemaining => columns.Values.Where((Column c) => c.lit).Count();
        public static int NumberOfColumns => columns.Count;

        protected override void Awake()
        {
            base.Awake();

            if (columns.ContainsKey(idCounter + 1))
                Debug.LogWarning($"Dictionary already contains column {gameObject.name}");
            else
                columns.Add(++idCounter, this);
        }

        // context menu doesn't work for some reason. even on base class. inheritance problems with serialization?
        [ContextMenu("Set References")]
        protected override void SetReferences()
        {
            base.SetReferences();

            flame = transform.Find("Object Holder").GetComponentInChildren<ParticleSystem>();

            if (!checkpoint)
                if (!transform.Find("Checkpoint").TryGetComponent(out checkpoint))
                    Debug.LogWarning($"Checkpoint cannot be found on {gameObject.name}. Will not be used as checkpoint position.");
        }

        int hits;
        Collider[] colliderHits = new Collider[8];
        private void Update()
        {
            // if we are not holding anything
            if (pickupableContainer.Selected == null)
            {
                hits = Physics.OverlapSphereNonAlloc(transform.position, pickupRadius, colliderHits, ~LayerMask.NameToLayer("Items"));

                // if we hit something
                for (int h = 0; h < hits; h++)
                {
                    // if said thing is a pickupable
                    if (colliderHits[h].transform.TryGetComponent(out IPickupable pickupable))
                    {
                        TrySelect(pickupable);
                    }
                }
            }
        }

        private void LightFlame()
        {
            flame.Play();
            lit = true;

            if (checkpoint)
                GameManager.SetCheckpoint(checkpoint);
        }

        protected override void OnPickupComplete()
        {
            base.OnPickupComplete();

            LightFlame();
        }
    }
}
