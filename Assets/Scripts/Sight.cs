using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public interface ISubscriber
    {
        void OnEnter(GameObject enteringObject, IReadOnlyList<GameObject> existingObject);
        void OnExit(GameObject exitingObject, IReadOnlyList<GameObject> existingObject);
    }

    private List<GameObject> _objectsInSight = new List<GameObject>();
    public IReadOnlyList<GameObject> objectsInSight { get { return _objectsInSight; } }

    private List<ISubscriber> subscribers = new List<ISubscriber>();

    public void Subscribe(ISubscriber subscriber)
    {
        if ( ! subscribers.Contains(subscriber))
            subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( ! _objectsInSight.Contains(collision.gameObject))
        {
            subscribers.ForEach(s => s.OnEnter(collision.gameObject, objectsInSight));
            _objectsInSight.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_objectsInSight.Contains(collision.gameObject))
        {
            _objectsInSight.Remove(collision.gameObject);
            subscribers.ForEach(s => s.OnExit(collision.gameObject, objectsInSight));
        }
    }
}
