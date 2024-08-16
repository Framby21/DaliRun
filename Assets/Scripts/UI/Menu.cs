using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private EventAnimation _animFadeBlack;
    // Start is called before the first frame update
    void Start()
    {
        _animFadeBlack.LaunchAnim();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Play(){
        StartCoroutine(WaitFade(1));
    }

    public void Quit(){
        StartCoroutine(WaitFade(2));
    }


    
    IEnumerator WaitFade(int value){
        _animFadeBlack.RevertAnim();
        yield return new WaitForSeconds(1);
        switch(value){
            case 1:
            SceneManager.LoadScene(1);
            break;
            case 2:
            Application.Quit();
            break;
        }

    }
}
