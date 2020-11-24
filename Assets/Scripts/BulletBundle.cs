using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBundle : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int num;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = bulletPrefab.GetComponent<SpriteRenderer>().sprite;
        GetComponent<BoxCollider2D>().size = bulletPrefab.GetComponent<BoxCollider2D>().size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
