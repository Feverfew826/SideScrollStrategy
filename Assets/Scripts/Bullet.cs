using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public interface GameRule
    {
        void OnHit(Bullet bullet, Collision2D collision);
    }
    static public GameRule gameRule;

    public enum BulletName
    {
        NormalBullet
    }
    public BulletName bulletName;

    public Rigidbody2D rigidBody { private set; get; }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Shoot(GameObject bulletPrefab, Vector3 position, Quaternion rotation, float power)
    {
        GameObject bulletGo = Instantiate<GameObject>(bulletPrefab, position, rotation);
        Bullet bullet = bulletGo.GetComponent<Bullet>();
        bullet.BeShotOnNextFrame(new Vector3(power, 0, 0));
    }

    private void BeShotOnNextFrame(Vector3 power)
    {
        StartCoroutine(DelayedShot(power));
    }

    private IEnumerator DelayedShot(Vector3 power)
    {
        yield return new WaitForEndOfFrame();
        rigidBody.gravityScale = 0;
        rigidBody.AddForce(power);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameRule != null)
            gameRule.OnHit(this, collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("!!");
    }
}
