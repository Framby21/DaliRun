using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Enemy_Scriptable _enemyStats;
    [SerializeField] private SpriteRenderer _spriteRender;
    private Transform _transform;
    private Camera _cam;
    public Camera Cam {get{return _cam;} set{_cam = value;}}
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _barLife;
    private Transform _target;
    public Transform Target {get{return _target;} set{_target = value;}}
    private Transform _currentTarget;

    private float _originDurationLife;
    private float _durationLife;
    private bool _isDead;
    private float _originSpeedFollow;
    private float _speedFollow;

    private bool _canDamage = true;
    private bool _canFollow = true;
    public bool CanFollow {get{return _canFollow;} set{_canFollow = value;}}

    [SerializeField] private bool _isGigaChad;
    [SerializeField] private Image _slowImage;
    [SerializeField] private EventAnimation _anim;
    [SerializeField] private EventAnimation _animDeadPower;

    private AudioSource _audioSource;

    private void Start(){
       _transform = gameObject.transform;
       _canvas.worldCamera = _cam;

       _spriteRender.sprite = _enemyStats.sprite;

       _originDurationLife = _enemyStats.maxHealth;
       _durationLife = _originDurationLife;

       _originSpeedFollow = _enemyStats.speed;
       _speedFollow = _originSpeedFollow;

       _audioSource = gameObject.GetComponent<AudioSource>();
       _audioSource.volume = 0.35f;
    }

    void Update()
    {
        if(_isDead){return;}

        if(!_canFollow){return;}

        if(_currentTarget != null){
            Vector3 follow = Vector3.Lerp(_transform.position, Target.position, Time.deltaTime * _speedFollow);

            float maxDistanceThisFrame = _speedFollow * Time.deltaTime;

            _transform.position = Vector3.MoveTowards(transform.position, follow, maxDistanceThisFrame);
        }

        _durationLife -= Time.deltaTime;
        _barLife.fillAmount = Utility.Remap(_durationLife, 0, _originDurationLife, 0, 1);

        if(_durationLife <= 0){
            Killed();
        }
    }

    //SLOW
    public void Slow(float value){
        StartCoroutine(SlowPending(value));
        StartCoroutine(RenderSlow(value));
    }
    IEnumerator SlowPending(float value){
        float reduce = _originSpeedFollow/(value*2);
        if(reduce <= 0.05f){reduce = 0.05f;}
        _speedFollow = reduce;
        yield return new WaitForSeconds(value);
        _speedFollow = _originSpeedFollow;
    }

    IEnumerator RenderSlow(float value){
        float l_elapsedTime = 0;
        float l_duration = value;
        _slowImage.fillAmount = 1;

        while (l_elapsedTime <= l_duration)
        {
            l_elapsedTime += Time.deltaTime;
            float l_ratio = Mathf.Clamp(l_elapsedTime / l_duration, 0,1);
            _slowImage.fillAmount = 1 - l_ratio;

            yield return null;
        }

        _slowImage.fillAmount = 0;
    }

    //LIFE
    private void TakeDamage(float value){
        _durationLife -= value;
        _barLife.fillAmount = Utility.Remap(_durationLife, 0, _originDurationLife, 0, 1);
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.tag == "Power"){
            TakeDamage(1);
        }
    }
    

    public void Killed(){

        if(_isGigaChad){return;}
        
        _isDead = true;

        if(gameObject.activeInHierarchy){
            _audioSource.Play();
            StartCoroutine(KillBeast());  
        }

    }

    IEnumerator KillBeast(){
        _animDeadPower.LaunchAnim();
        yield return new WaitForSeconds(0.25f);
        _anim.RevertAnim();
        yield return new WaitForSeconds(2);
        _originDurationLife = _enemyStats.maxHealth;
       _durationLife = _originDurationLife;

       _originSpeedFollow = _enemyStats.speed;
       _speedFollow = _originSpeedFollow;
        gameObject.SetActive(false);
    }

    //ATTACK
    void OnTriggerStay2D(Collider2D col)
    {
        if(_isDead){return;}
        if(!_canDamage){return;}
        
        if(col.tag == "Player"){
            GlobalEvents._enemy._damage?.Invoke(_enemyStats.dmg);
            StartCoroutine(DelayDamage());
        }
    }

    IEnumerator DelayDamage(){
        _canDamage = false;
        yield return new WaitForSeconds(0.5f);
        _canDamage = true;
    }

    private void OnEnable(){
        _isDead = false;
        _canFollow = true;

        _originDurationLife = _enemyStats.maxHealth;
       _durationLife = _originDurationLife;

       _originSpeedFollow = _enemyStats.speed;
       _speedFollow = _originSpeedFollow;

       _currentTarget = Target;

       gameObject.transform.localScale = Vector3.zero;

       _slowImage.fillAmount = 0;

       _anim.LaunchAnim();
    }
    private void OnDisable(){
        Killed();
    }
}
