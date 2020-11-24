using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject Target;
    public float lowLimit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = Target.transform.position;
        targetPosition.y = Mathf.Max(targetPosition.y, lowLimit);
        targetPosition.z = transform.position.z;
        transform.Translate(targetPosition - transform.position);
    }
}
