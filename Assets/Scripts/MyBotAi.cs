using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBotAi : MonoBehaviour
{
    public VirtualInputSource input;
    public GameObject target;
    public float attackRange;

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {

            if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
            {
                if (target.transform.position.x < transform.position.x)
                    input.SetHorizontalAxisLeftStick(-1);
                else
                    input.SetHorizontalAxisLeftStick(1);
            }
            else
            {
                input.SetButtonDown(InputSource.Button.Square);
            }


        }

    }
}
