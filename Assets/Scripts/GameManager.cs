using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamFourteen.CoreGame;

namespace TeamFourteen
{
    public class GameManager : MonoBehaviour
    {
        private static GameObject player;
        private static Checkpoint latestCheckpoint;

        private void SetReferences()
        {
            if (!player)
                player = GameObject.Find("Player");
        }

        private void Awake()
        {
            SetReferences();
        }

        public static void SetCheckpoint(Checkpoint checkpoint)
        {
            latestCheckpoint = checkpoint;
            Debug.Log($"Checkpoint set at {checkpoint.transform.position}!");
        }

        public static void LoadLatestCheckpoint()
        {
            if (latestCheckpoint == null)
            {
                Debug.LogWarning("Cannot reset to latest checkpoint because there is no latest checkpoint.");
                return;
            }    

            // I like this, but maybe we should delegate this responsibility to player if we are calling member methods?
            player.GetComponent<Health>().ResetHealth();
            player.GetComponent<PlayerMovement>().Teleport(latestCheckpoint.transform.position);
        }
    }
}