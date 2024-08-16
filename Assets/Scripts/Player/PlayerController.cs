using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")]
    private Transform _playerTr;
    private SpriteRenderer _playerRender;
    [SerializeField] private GameObject _cam;
    private Transform _camTr;
    private Powers_Behavior _powerBehaviorScript;

    //INPUTS
    private InputController _inputController;
    bool _l; bool _r; bool _t; bool _b;
    bool _p1; bool _p2;

    public bool secureInput = false;

    //STATISTIQUES
    [Header("SPEED")]
    //Speed
    [SerializeField] float _initSpeed = 0.25f;
    [SerializeField] private float _coefSpeed = 0.1f;
    public float CoefSpeed { get{ return _coefSpeed; } set { _coefSpeed = value; } }
    private bool _isDead;

    //Cooldown
    [Header("COOLDOWN")]
    [SerializeField] float _initCooldownP1 = 1f;
    [SerializeField] float _initCooldownP2 = 1f;
    [SerializeField] private float _coefCooldown = 1f;
    public float CoefCooldown { get{return _coefCooldown;} set{_coefCooldown = value;}}
    bool _p1InCooldown = false; bool _p2InCooldown = false;
    [SerializeField] Image[] _radialsPower;

    //POWERS
    [Header("POWERS")]
    [SerializeField] private Power_Scriptable[] _powerScript;
    [SerializeField] Image[] _powerRender;
    [SerializeField] private int _power1Index;
    [SerializeField] private int _power2Index;
    public UnityAction<int, int, bool> _callPowerScriptByIndex;

    //Life
    [Header("LIFE")]
    [SerializeField] private Image _barLife;
    private float _currentLife;
    [SerializeField] private float _regenLife;
    public float RegenLife{get{return _regenLife;} set{_regenLife = value;}}
    [SerializeField] private float _maxLife;
    public float MaxLife{get{return _maxLife;} set{_maxLife = value;}}
    [SerializeField] private TextMeshProUGUI _currentLifeText;
    [SerializeField] private TextMeshProUGUI _maxLifeText;

    [Header("CHEST")]
    [SerializeField] private GameObject _choices;
    private List<EventAnimation> _animsChoices = new List<EventAnimation>();
    private bool _timeIsStopped;

    [Header("STATS")]
    [SerializeField] private Stats_Scriptable[] _statsScript;

    public int lvlCooldown = 1;
    public int lvlMoveSpeed = 1;
    public int lvlRegen = 1;
    public int lvlMaxHealth = 1;


    [Header("ANIM")]
    [SerializeField] private EventAnimation _animDamage;
    [SerializeField] private EventAnimation[] _animPowers;

    [SerializeField] private EventAnimation _animFadeBlack;

    [Header("AUDIO")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioPlayer;

    void Start()
    {
        //INIT
        _playerTr = gameObject.transform;
        _camTr = _cam.transform;
        _inputController = gameObject.GetComponent<InputController>();
        _playerRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _powerBehaviorScript = gameObject.GetComponent<Powers_Behavior>();
        _camTr.position = _playerTr.position;

        foreach(Image radial in _radialsPower){radial.fillAmount = 1;}

        _animFadeBlack.LaunchAnim();
        _isDead = false;

        //EVENTS
        _inputController._goLeft += LEFT;
        _inputController._goRight += RIGHT;
        _inputController._goTop += TOP;
        _inputController._goBot += BOT;

        _inputController._goPower1 += POWER1;
        _inputController._goPower2 += POWER2;

        GlobalEvents._power._addPower += Addpower;
        GlobalEvents._power._upgradeStats += UpdateStats;
        GlobalEvents._enemy._damage += TakeDamage;
        GlobalEvents._chest._openChest += OpeningChest;
        
        _initSpeed = 0.15f;
        _coefSpeed = 0.15f;

        //LIFE
        _currentLife = MaxLife;
        _barLife.fillAmount = Utility.Remap(_currentLife, 0, MaxLife, 0, 1);
        _currentLifeText.text = _currentLife.ToString();
        _maxLifeText.text = MaxLife.ToString();

        //POWERS

        for(int i = 0; i < _choices.transform.childCount-1; i++){
            _animsChoices.Add(_choices.transform.GetChild(i).GetChild(0).GetComponent<EventAnimation>());
        }

        /*
        int randomValue = UnityEngine.Random.Range(1,6);

        _power1Index = randomValue;
        //_power2Index = 2;
        
        CheckPower(0,randomValue);
        //_powerBehaviorScript.RandomPower(randomValue);
        /*CheckPower(1,_power2Index);
        */
    }

    void Update()
    {
        if(_timeIsStopped){return;}

        //INPUTS MOVEMENT
        if(_l){
            _playerTr.position = new Vector2(_playerTr.position.x - _initSpeed * CoefSpeed, _playerTr.position.y);
            _playerRender.flipX = false;
        }

        if(_r){
            _playerTr.position = new Vector2(_playerTr.position.x + _initSpeed * CoefSpeed, _playerTr.position.y);
            _playerRender.flipX = true;
        }

        if(_t){
            _playerTr.position = new Vector2(_playerTr.position.x, _playerTr.position.y + _initSpeed * CoefSpeed);
        }

        if(_b){
            _playerTr.position = new Vector2(_playerTr.position.x, _playerTr.position.y - _initSpeed * CoefSpeed);
        }

        //INPUTS POWERS
        if(_p1){
            if(_power1Index == 0){return;}
            if(_p1InCooldown){return;}
            if(secureInput){return;}
            print("Power 1");
            _callPowerScriptByIndex?.Invoke(0,_power1Index, false);
            _animPowers[0].LaunchAnim();
            PlayCooldownP1();
        }
        if(_p2){
            if(_power2Index == 0){return;}
            if(_p2InCooldown){return;}
            if(secureInput){return;}
            print("Power 2");
            _callPowerScriptByIndex?.Invoke(1,_power2Index, false);
            _animPowers[1].LaunchAnim();
            PlayCooldownP2();
        }

        //LERP CAMERA TO PLAYER
        Vector3 tagertPos = new Vector3( _playerTr.position.x, _playerTr.position.y, -10);
        Vector3 targetPosition = Vector3.Lerp(_camTr.position, tagertPos, Time.deltaTime * 2);
        _camTr.position = targetPosition;
       
       Regeneration();
    }

    //INPUTS
    void LEFT(bool value){
        _l = value;
    }
    void RIGHT(bool value){
        _r = value;
    }
    void TOP(bool value){
        _t = value;
    }
    void BOT(bool value){
        _b = value;
    }

    //POWERS
    void Addpower(int indexAbility, int indexPower){
        switch(indexAbility){
            case 0: 
            if(_power1Index == indexPower){
                _powerBehaviorScript.WriteLevel(indexAbility, indexPower,false);
            }
            else{
                _powerBehaviorScript.WriteLevel(indexAbility, indexPower,false);
            }
            _power1Index = indexPower;
            break;
            case 1: 
            if(_power2Index == indexPower){
                _powerBehaviorScript.WriteLevel(indexAbility, indexPower, false);
            }
            else{
                _powerBehaviorScript.WriteLevel(indexAbility, indexPower, false);
            }
            _power2Index = indexPower;
            break;
        }

        if(_power1Index == _power2Index){
            _powerBehaviorScript.WriteLevel(indexAbility, indexPower,true);
        }

        CheckPower(indexAbility, indexPower);
        StartCoroutine(ClosingChest());
    }
    void POWER1(bool value){
        _p1 = value;
    }

    void POWER2(bool value){
        _p2 = value;
    }

    public void CheckPower(int idPower, int idPowerRender){
        switch(idPower){
            case 0: _initCooldownP1 = _powerScript[idPowerRender].cooldown;
            break;
            case 1: _initCooldownP2 = _powerScript[idPowerRender].cooldown;
            break;
        }
        _powerRender[idPower].sprite = _powerScript[idPowerRender].powerSprite;
    }

    //COOLDOWN
    void PlayCooldownP1(){
        StartCoroutine(CooldownPower1());
    }
    IEnumerator CooldownPower1(){
        _p1InCooldown = true;

        float l_elapsedTime = 0;
        float l_duration = _initCooldownP1 * CoefCooldown;
        _radialsPower[0].fillAmount = 0;

        while (l_elapsedTime <= l_duration)
        {
            l_elapsedTime += Time.deltaTime;
            float l_ratio = Mathf.Clamp(l_elapsedTime / l_duration, 0, 1);
            _radialsPower[0].fillAmount = l_ratio;

            yield return null;
        }
        _radialsPower[0].fillAmount = 1;
        
        _p1InCooldown = false;
    }

    void PlayCooldownP2(){
        StartCoroutine(CooldownPower2());
    }
    IEnumerator CooldownPower2(){
        _p2InCooldown = true;

        float l_elapsedTime = 0;
        float l_duration = _initCooldownP2 * CoefCooldown;
        _radialsPower[1].fillAmount = 0;

        while (l_elapsedTime <= l_duration)
        {
            l_elapsedTime += Time.deltaTime;
            float l_ratio = Mathf.Clamp(l_elapsedTime / l_duration, 0, 1);
            _radialsPower[1].fillAmount = l_ratio;

            yield return null;
        }
        _radialsPower[1].fillAmount = 1;
        _p2InCooldown = false;
    }

    //LIFE
    private void TakeDamage(float value){
        if(_isDead){return;}

        _currentLife -= value;
        float truncate = Mathf.Floor(_currentLife);
        _barLife.fillAmount = Utility.Remap(truncate, 0, MaxLife, 0, 1);
        
        _currentLifeText.text = truncate.ToString();

        if(_audioSource.clip != _audioPlayer[0]){
            _audioSource.clip = _audioPlayer[0];
        }
        
        _audioSource.Play();
        _animDamage.LaunchAnim();

        if(_currentLife <= 1){
            print("PlayerIsDead");
            
            _isDead = true;
            _currentLifeText.text = "0";

            _audioSource.clip = _audioPlayer[1];
            _audioSource.Play();

            _animFadeBlack.RevertAnim();

            StartCoroutine(GoToDeadScene());
        }
    }

    private void Regeneration(){
        if(_isDead){return;}
        if(_currentLife >= MaxLife){return;}

        _currentLife += Time.deltaTime * RegenLife; 
        float truncate = Mathf.Floor(_currentLife);
        _barLife.fillAmount = Utility.Remap(truncate, 0, MaxLife, 0, 1);

        if(_currentLife <= 0){
            _currentLifeText.text = "0";
        }
        else{
            _currentLifeText.text = truncate.ToString();
        }
    }

    public void Healing(float value){
        if(_isDead){return;}
        _currentLife+=value;
        float truncate = Mathf.Floor(_currentLife);

        if(_currentLife < MaxLife){
            _barLife.fillAmount = Utility.Remap(truncate, 0, MaxLife, 0, 1);
            _currentLifeText.text = truncate.ToString();
        }
        else{
            _currentLife = MaxLife;
            _barLife.fillAmount = 1;
            _currentLifeText.text = MaxLife.ToString();
        }
    }


    //OPENING CHEST
    private void OpeningChest(){
        Time.timeScale = 0;
        print("OpeningChest");

        _timeIsStopped = true;
        secureInput = true;
        
        _choices.SetActive(true);
    }

    IEnumerator ClosingChest(){
        foreach(EventAnimation anim in _animsChoices){
            anim.RevertAnim();
        }
        yield return new WaitForSecondsRealtime(0.25f);

        Time.timeScale = 1;
        _timeIsStopped = false;
        _choices.SetActive(false);

        yield return new WaitForSeconds(0.25f);
        secureInput = false;
    }

    

    public void Skip(){
        StartCoroutine(ClosingChest());
    }

    //STATS
    private void UpdateStats(int value){
        switch(value){
            case 0:
                CoefSpeed += _statsScript[value].value;
                lvlMoveSpeed++;
            break;
            case 1:
                CoefCooldown -= _statsScript[value].value;
                if(CoefCooldown <= 0.1f){
                    CoefCooldown = 0.1f;
                }
                lvlCooldown++;
            break;
            case 2:
                MaxLife += _statsScript[value].value;
                _currentLife += _statsScript[value].value;

                _maxLifeText.text = MaxLife.ToString();
                _currentLifeText.text = Math.Floor(_currentLife).ToString();
                lvlMaxHealth++;
            break;
            case 3:
                RegenLife += _statsScript[value].value;
                lvlRegen++;
            break;
            case 4:
                if(_power1Index == 0 && _power2Index == 0){
                    return;
                }

                if(_power1Index == 0 && _power2Index != 0){
                    _powerBehaviorScript.WriteLevel(1, _power2Index, false);
                    _animPowers[1].LaunchAnim();
                }
                else if(_power1Index != 0 && _power2Index == 0){
                    _powerBehaviorScript.WriteLevel(0, _power1Index, false);
                    _animPowers[0].LaunchAnim();
                }
                else{
                    int rnd = UnityEngine.Random.Range(0,2);
               
                    if(rnd ==0){
                            _powerBehaviorScript.WriteLevel(rnd, _power1Index, false);
                            _animPowers[0].LaunchAnim();
                    }
                    else{
                            _powerBehaviorScript.WriteLevel(rnd, _power2Index, false);
                            _animPowers[1].LaunchAnim();
                    }
                    
                    if(_power1Index == _power2Index){
                        _powerBehaviorScript.WriteLevel(0, _power1Index,true);
                        _powerBehaviorScript.WriteLevel(1, _power2Index,true);
                    }

                    print("SORT " + rnd);
                }

                
                
            break;
        }

        StartCoroutine(ClosingChest());
    }

    IEnumerator GoToDeadScene(){
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(3);
    }
    private void OnDisable(){
        _inputController._goLeft -= LEFT;
        _inputController._goRight -= RIGHT;
        _inputController._goTop -= TOP;
        _inputController._goBot -= BOT;

        _inputController._goPower1 -= POWER1;
        _inputController._goPower2 -= POWER2;

        GlobalEvents._power._addPower -= Addpower;
        GlobalEvents._power._upgradeStats -= UpdateStats;
        GlobalEvents._enemy._damage -= TakeDamage;
        GlobalEvents._chest._openChest -= OpeningChest;
    }
}
