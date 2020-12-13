using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject target;
    public float lowLimit;
    public float followSpeed = 0.05f;
    public enum Mode
    {
        Teleport,
        Follow
    }
    public Mode mode;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            switch (mode)
            {
                case Mode.Teleport:
                    Teleport();
                    break;
                case Mode.Follow:
                    Follow();
                    break;
                default:
                    break;
            }
        }
    }

    void Teleport()
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = Mathf.Max(targetPosition.y, lowLimit);
        targetPosition.z = transform.position.z;

        Vector3 translation = targetPosition - transform.position;

        transform.Translate(translation);
    }

    void Follow()
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = Mathf.Max(targetPosition.y, lowLimit);
        targetPosition.z = transform.position.z;

        Vector3 translation = targetPosition - transform.position;

        if(Vector3.Magnitude(translation) > followSpeed)
            transform.Translate(translation.normalized * followSpeed);
        else
            transform.Translate(translation);
    }
}
