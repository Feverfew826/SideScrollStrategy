using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBotAi : MonoBehaviour, Sight.ISubscriber
{
    public VirtualInputSource input;
    public GameObject destroyOperationTarget;
    public GameObject currentDestroyTarget;

    public Sight sight;
    public float attackRange;

    void Sight.ISubscriber.OnEnter(GameObject enteringObject, IReadOnlyList<GameObject> existingObject)
    {
        MyBotAi ai = enteringObject.GetComponent<MyBotAi>();
        if(ai != null && ai.destroyOperationTarget != destroyOperationTarget)
        {
            if (currentDestroyTarget == null)
            {
                currentDestroyTarget = enteringObject;
            }
            else
            {
                float currentTargetDistance = Vector2.Distance(transform.position, currentDestroyTarget.transform.position);
                float enteringTargetDistance = Vector2.Distance(transform.position, enteringObject.transform.position);
                if (enteringTargetDistance < currentTargetDistance)
                {
                    currentDestroyTarget = enteringObject;
                }
            }
        }
    }

    void Sight.ISubscriber.OnExit(GameObject exitingObject, IReadOnlyList<GameObject> existingObject)
    {

    }

    private void Start()
    {
        sight.Subscribe(this);
    }

    private void OnDestroy()
    {
        sight.Unsubscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoveDirectionLeft = false;
        if(destroyOperationTarget != null)
            isMoveDirectionLeft = destroyOperationTarget.transform.position.x < transform.position.x;

        if(currentDestroyTarget != null)
        {
            input.SetButtonDown(InputSource.Button.Square);
        }
        else if(destroyOperationTarget != null)
        {
            if (Vector2.Distance(transform.position, destroyOperationTarget.transform.position) < attackRange)
            {
                input.SetButtonDown(InputSource.Button.Square);
            }
            else
            {
                if (isMoveDirectionLeft)
                    input.SetHorizontalAxisLeftStick(-1);
                else
                    input.SetHorizontalAxisLeftStick(1);
            }
        }
    }
}
