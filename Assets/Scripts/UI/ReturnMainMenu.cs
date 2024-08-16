using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainMenu : MonoBehaviour
{
    [SerializeField] private EventAnimation _animFadeBlack;
    // Start is called before the first frame update
    void Start()
    {
        _animFadeBlack.LaunchAnim();
    }

    public void MainMenu(){
        StartCoroutine(WaitFade(0));
        
    }
    public void TryAgain(){
        StartCoroutine(WaitFade(1));
    }

    public void Quit(){
        StartCoroutine(WaitFade(2));
    }

    IEnumerator WaitFade(int value){
        _animFadeBlack.RevertAnim();
        yield return new WaitForSeconds(1);
        switch(value){
            case 0:
            SceneManager.LoadScene(0);
            break;
            case 1:
            SceneManager.LoadScene(1);
            break;
            case 2:
            Application.Quit();
            break;
        }

    }
}
