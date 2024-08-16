using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LockEndZone : MonoBehaviour
{
    private float _currentTime;
    [SerializeField] private float _maxTime;
    [SerializeField] private TextMeshProUGUI _textValue;
    [SerializeField] private Image _imageRender;

    private SpriteRenderer _spriteCircle;
    private bool _isLaunched = false;

    [SerializeField] private EventAnimation _anim;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _clips;

    // Start is called before the first frame update
    void Start()
    {
        _currentTime = _maxTime;
        _spriteCircle = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isLaunched){return;}

        _currentTime -= Time.deltaTime;
        double floorValue = Math.Floor(_currentTime);

        _textValue.text = floorValue.ToString();
        float remap = Utility.Remap((float)floorValue, 0, _maxTime, 1, 0);

        _imageRender.fillAmount = remap;

        if(_currentTime <= 0){
            KillZone();
            _textValue.text = " ";
            _imageRender.fillAmount = 0;
        }
    }

    private void KillZone(){
        _isLaunched = false;
        
        _audioSource.clip = _clips[1];
        _audioSource.Play();

        StartCoroutine(WaitEndSound());
    }

    IEnumerator WaitEndSound(){
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player"){

            if(_isLaunched){return;}

            _audioSource.clip = _clips[0];
            _audioSource.Play();

            _anim.LaunchAnim();
            StartCoroutine(FadeCircle());
            _isLaunched = true;
        }
    }

    IEnumerator FadeCircle(){
         float l_elapsedTime = 0;
        float l_duration = 1;

        float alpha = _spriteCircle.color.a;

        while (l_elapsedTime <= l_duration)
        {
            l_elapsedTime += Time.deltaTime;
            float l_ratio = Mathf.Clamp(l_elapsedTime / l_duration, 0, 1);
            
            Vector4 colorLerp = Vector4.Lerp(new Vector4(1,1,1, alpha), new Vector4(1,1,1, 0), l_ratio);
            _spriteCircle.color = colorLerp;
            yield return null;
        }
    }
}
