using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Column : ObjectHolder
    {
        [SerializeField] [HideInInspector] private ParticleSystem flame;

        [Header("Values")]
        [SerializeField] private float pickupRadius = 2.5f;
        private bool lit;

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            flame = transform.Find("Object Holder").GetComponentInChildren<ParticleSystem>();
        }

        private void Reset() => SetReferences();

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
        }

        protected override void OnPickupComplete()
        {
            base.OnPickupComplete();

            LightFlame();
        }
    }
}
