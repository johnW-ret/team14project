using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternPlacement : MonoBehaviour
{
    //public bool lanternTrigger;

    private void OnTriggerEnter(Collider other)
    {
        //If Lantern is Triggered by the PlatformCollider
        if(other.gameObject.GetComponent<PlatformCollider>())
        {
            other.gameObject.GetComponent<PlatformCollider>().platformState = true;

            //Score Update
            if (!other.gameObject.GetComponent<PlatformCollider>().scoreState)
            {
                //Increase Score

                //Doesn't allow the score to be increased by this platform
                other.gameObject.GetComponent<PlatformCollider>().scoreState = true;
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        //If Lantern is Triggered by the PlatformCollider
        if (other.gameObject.GetComponent<PlatformCollider>())
        {
            //If the platform has multiple use
            if(!other.gameObject.GetComponent<PlatformCollider>().oneTimeUse)
            {
                other.gameObject.GetComponent<PlatformCollider>().platformState = false;
            }
        }
    }
}
