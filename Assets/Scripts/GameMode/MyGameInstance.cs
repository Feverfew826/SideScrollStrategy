using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameInstance : MonoBehaviour
{
    public static MyGameInstance instance;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    };

    public Difficulty difficulty;

    public bool hasCleared = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
