using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choices : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] GameObject _buttonStats;
    [SerializeField] GameObject _buttonPower;
    [SerializeField] private Power_Scriptable[] _powers;
    [SerializeField] private Stats_Scriptable[] _stats;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _spriteRender;

    private int _indexPower;
    [SerializeField] private EventAnimation _anim;
    [SerializeField] private TextMeshProUGUI[] _textsPowers;

    [SerializeField] private TextMeshProUGUI _textStatsLvl;
    [SerializeField] private TextMeshProUGUI _textPowerLvl;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Powers_Behavior _powerBehavior;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable(){
        _name.text = "";
        _spriteRender.sprite = null;
        _background.color = Color.white;

        _buttonStats.SetActive(false);
        _buttonPower.SetActive(false);

        float rnd = Random.Range(0,2); 

        gameObject.transform.localScale = Vector3.zero;
        _anim.LaunchAnim();

        switch(rnd){
            case 0:
            RandomStats();
            break;
            case 1:
            RandomPower();
            break;
        }
    }

    private void RandomStats(){
        _buttonStats.SetActive(true);

        _name.color = Color.black;

        int rnd = Random.Range(0, _stats.Length);
        Stats_Scriptable item = _stats[rnd];

        _background.color = Color.white;
        _name.text = item.nameStat;
        _spriteRender.sprite = item.sprite;
        _indexPower = rnd;

        int valueLvl = 0;
        switch(rnd){
            case 0:
            valueLvl = _playerController.lvlMoveSpeed;
            break;
            case 1:
            valueLvl = _playerController.lvlCooldown;
            break;
            case 2:
            valueLvl = _playerController.lvlMaxHealth;
            break;
            case 3:
            valueLvl = _playerController.lvlRegen;
            break;
            case 4:
            valueLvl = 1;
            break;
        }
        _textStatsLvl.text = "Lvl " + valueLvl.ToString();

        if(rnd == 4){
            _textStatsLvl.text = " ";
        }
    }

    private void RandomPower(){
        _buttonPower.SetActive(true);

        _name.color = Color.white;

        int rnd = Random.Range(1, _powers.Length);
        Power_Scriptable item = _powers[rnd];

        _background.color = item.color;
        _name.text = item.powerName;
        _spriteRender.sprite = item.powerSprite;
        _indexPower = rnd;

        foreach(TextMeshProUGUI txt in _textsPowers){
            if(rnd == 3){
                txt.color = Color.white;
            }
            else{
                txt.color = Color.black;
            }
        }

        int valueLvl = 0;
        switch(rnd){
            case 1:
            valueLvl = _powerBehavior.lvlTime;
            break;
            case 2:
            valueLvl = _powerBehavior.lvlFire;
            break;
            case 3:
            valueLvl = _powerBehavior.lvlInvisibility;
            break;
            case 4:
            valueLvl = 1;
            break;
            case 5:
            valueLvl = _powerBehavior.lvlHeal;
            break;
            case 6:
            valueLvl = _powerBehavior.lvlDestroy;
            break;
        }
        _textPowerLvl.text = "Lvl " + valueLvl.ToString();
    }

    private void OnDisable(){
        _name.text = "";
        _spriteRender.sprite = null;
        _background.color = Color.white;

        _buttonStats.SetActive(false);
        _buttonPower.SetActive(false);
    }

    public void StatsClick(){
        GlobalEvents._power._upgradeStats?.Invoke(_indexPower);
        _audioSource.Play();
    }

    public void P1Click(){
        GlobalEvents._power._addPower?.Invoke(0, _indexPower);
        _audioSource.Play();
    }

    public void P2Click(){
        GlobalEvents._power._addPower?.Invoke(1, _indexPower);
        _audioSource.Play();
    }
}
