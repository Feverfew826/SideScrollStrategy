using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVolume : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        RuleManager.Rule?.OnEnter(this, collision);
    }

    public enum Type
    {
        Death,
        Win,
        Gun,
        BulletBundle,
        EnemyBarrack,
        OurBarrack
    }
    public interface IGameRule
    {
        void OnEnter(TriggerVolume triggerVolume, Collider2D collision);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    public Type type;

    public void EnableTriggerAfterSeconds(float seconds)
    {
        StartCoroutine(DelayedEnableColliderCoroutine(seconds));
    }

    IEnumerator DelayedEnableColliderCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GetComponent<BoxCollider2D>().enabled = true;
    }
}
