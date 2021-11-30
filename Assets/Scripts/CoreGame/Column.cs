using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Column : ObjectHolder
    {
        [Header("Values")]
        [SerializeField] private float pickupRadius = 2.5f;
        
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
    }
}
