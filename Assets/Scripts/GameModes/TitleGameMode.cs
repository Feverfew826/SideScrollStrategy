using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleGameMode : MonoBehaviour
{
    public bool isOnToggle0 { get; set; }
    public bool isOnToggle1 { get; set; }
    public bool isOnToggle2 { get; set; }

    public AudioSource uiAudioSource;
    public AudioClip buttonSound;
    public AudioClip toggleSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickedStartButton()
    {
        uiAudioSource.clip = buttonSound;
        uiAudioSource.Play();

        if (isOnToggle0)
            MyGameInstance.instance.difficulty = MyGameInstance.Difficulty.Easy;
        if (isOnToggle1)
            MyGameInstance.instance.difficulty = MyGameInstance.Difficulty.Normal;
        if (isOnToggle2)
            MyGameInstance.instance.difficulty = MyGameInstance.Difficulty.Hard;

        StartCoroutine(LoadSceneCoroutine(uiAudioSource.clip.length));
    }
    
    public void OnValueChangedToggle(Toggle toggle)
    {
        if(toggle.isOn)
        {
            uiAudioSource.clip = toggleSound;
            uiAudioSource.Play();
        }
    }

    IEnumerator LoadSceneCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
    }
}
