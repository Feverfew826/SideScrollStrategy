using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldKilledByDistance && Vector3.Distance(startPosition, transform.position) > distanceLimit)
            Destroy(gameObject);

        if (shouldKilledByTime && Time.time - startTime > timeLimit)
            Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        RuleManager.Rule?.OnHit(this, collision);
    }

    private void OnDestroy()
    {
        RuleManager.Rule?.OnDestroy(this);
        RuleManager.overridedGameRule = null;
    }

    public Rigidbody2D RigidBody { private set; get; }

    public interface IGameRule
    {
        void OnHit(Bullet bullet, Collision2D collision);
        void OnDestroy(Bullet bullet);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    public enum BulletName
    {
        NormalBullet
    }
    public BulletName bulletName;

    public bool shouldKilledByDistance = true;
    public bool shouldKilledByTime = true;

    private Vector3 startPosition;
    private float startTime;

    public float distanceLimit = 100;
    public float timeLimit = 5f;

    public static Bullet Shoot(GameObject bulletPrefab, Vector3 position, Quaternion rotation, float power)
    {
        GameObject bulletGo = Instantiate<GameObject>(bulletPrefab, position, rotation);
        Bullet bullet = bulletGo.GetComponent<Bullet>();
        bullet.BeShotOnNextFrame(new Vector3(power, 0, 0));

        return bullet;
    }

    private void BeShotOnNextFrame(Vector3 power)
    {
        startPosition = transform.position;
        startTime = Time.time;
        StartCoroutine(DelayedShot(power));
    }

    private IEnumerator DelayedShot(Vector3 power)
    {
        yield return new WaitForEndOfFrame();
        RigidBody.gravityScale = 0;
        RigidBody.AddForce(power);
    }

}
