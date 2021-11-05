using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    public bool platformState;
    public bool scoreState;
    public bool oneTimeUse = true;

    private bool Usable;

    private void Start()
    {
        platformState = false;
        scoreState = false;
        Usable = true;
    }

    private void FixedUpdate()
    {
        //This Platform is only usable once
        if (oneTimeUse)
        {
            //If it is one time use, has it been used
            if (Usable)
            {
                if (platformState == true)
                {
                    //Interacts with level
                }
                //While platformStat is false
                else
                {
                    //Interacts with level
                }
                Usable = false;
            }
            else
            {
                if (platformState == true)
                {
                    //Interacts with level
                }
                //While platformState is false
                else
                {
                    //Interacts with level
                }
            }
        }
        //If the Platform has more usage
        else
        {
            if(platformState == true)
            {
                //Interacts with level
            }
            //While false what would it do
            else
            {
                //Interacts with level
            }
        }
    }
}
