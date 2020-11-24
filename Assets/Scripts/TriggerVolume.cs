using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVolume : MonoBehaviour
{
    public enum Type
    {
        Death,
        Win,
        Gun,
        BulletBundle
    }
    public interface GameRule
    {
        void OnEnter(TriggerVolume triggerVolume, Collider2D collision);
    }
    static public GameRule gameRule;

    public Type type;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableTriggerAfterSeconds(float seconds)
    {
        StartCoroutine(DelayedEnableColliderCoroutine(seconds));
    }

    IEnumerator DelayedEnableColliderCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameRule != null)
            gameRule.OnEnter(this, collision);
    }
}
