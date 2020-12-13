using System.Collections.Generic;
public abstract class SubscribeManager<Observer>
{
    public interface Observable
    {
        void OnSubscribe(Observer observer);
        void OnUnsubscribe(Observer observer);
    }

    public List<Observer> _subscribers;
    public IReadOnlyList<Observer> subscribers { get { return _subscribers; } }

    private Observable observable;

    public SubscribeManager(Observable observable)
    {
        _subscribers = new List<Observer>();
        this.observable = observable;
    }

    virtual public void Subscribe(Observer observer)
    {
        if (!_subscribers.Contains(observer))
        {
            _subscribers.Add(observer);
            observable.OnSubscribe(observer);
        }
    }

    virtual public void Unsubscribe(Observer observer)
    {
        if (_subscribers.Contains(observer))
        {
            _subscribers.Remove(observer);
            observable.OnUnsubscribe(observer);
        }
    } 
}