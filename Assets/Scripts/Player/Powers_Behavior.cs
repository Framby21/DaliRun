using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Powers_Behavior : MonoBehaviour
{
    public enum TypePowers{
        NULLL = 0, TIME = 1, FIRE = 2, INVISIBILITY = 3, UPGRADESTATS = 4,
        HEAL = 5, DESTROY = 6
    }

    [SerializeField] private Power_Scriptable[] _powersList;
    private PlayerController _playerController;

    [SerializeField] private GameObject _parentfire;
    [SerializeField] private GameObject[] _powerPrefab;

    Transform _fireTransform;

    [SerializeField] BoxCollider2D _invisibilityObj;

    [SerializeField] AudioClip[] _listAudioPowers;
    [SerializeField] private AudioSource[] _audioSource;

    [SerializeField] private EventAnimation _invisibleAnim;

    //LIST UPGRADE
    float upgradeTime = 0;
    float upgradeFire = 0;
    float upgradeInvisibility = 0;
    float upgradeHeal = 0;
    float upgradeDestroy = 0;

    public int lvlTime = 0;
    public int lvlFire = 0;
    public int lvlInvisibility = 0;
    public int lvlHeal = 0;
    public int lvlDestroy = 0;

    [SerializeField] private TextMeshProUGUI[] _textPowersLvl;

    private void Start(){
        _playerController = gameObject.GetComponent<PlayerController>();

        _playerController._callPowerScriptByIndex += UsePower;

        //POWERS INIT
        _fireTransform = _parentfire.transform;
    }

    private void UsePower(int powerIndex,  int value, bool isSame){
        print("urji");
        switch(value){
            case 0:
            break;
            case 1:
                SlowTime();
                _audioSource[powerIndex].clip = _listAudioPowers[0];
                _audioSource[powerIndex].Play();
            break;
            case 2:
                Fire();
                _audioSource[powerIndex].clip = _listAudioPowers[1];
                _audioSource[powerIndex].Play();
            break;
            case 3:
                Invisibility();
                _audioSource[powerIndex].clip = _listAudioPowers[2];
                _audioSource[powerIndex].Play();
            break;
            case 4:
                _textPowersLvl[powerIndex].text = "";
                _audioSource[powerIndex].clip = _listAudioPowers[3];
                _audioSource[powerIndex].Play();
                UpgradeStats();
            break;
            case 5:
                Heal();
                _audioSource[powerIndex].clip = _listAudioPowers[4];
                _audioSource[powerIndex].Play();
            break;
            case 6:
                DestroyEnemy();
                _audioSource[powerIndex].clip = _listAudioPowers[5];
                _audioSource[powerIndex].Play();
            break;
        }
    }

    private void SlowTime(){
        GlobalEvents._power._slowTime?.Invoke(_powersList[(int)TypePowers.TIME].value + upgradeTime);
        
    }
 
    private void Fire(){
        float value = _powersList[(int)TypePowers.FIRE].value + upgradeFire;


        StartCoroutine(DuringFire(value));
    }
    IEnumerator DuringFire(float value){
        value /= 2;
        GameObject obj = Instantiate(_powerPrefab[0], _fireTransform.position, quaternion.identity);
        obj.transform.SetParent(_fireTransform);
        obj.transform.localScale = new Vector3(value,value,value);
        yield return new WaitForSeconds(value);
        obj.transform.SetParent(null);
        Destroy(obj);
    }

    private void Invisibility(){
        float value = _powersList[(int)TypePowers.INVISIBILITY].value + upgradeInvisibility;
        GlobalEvents._power._invisibleTime?.Invoke(value);

        
        
        StartCoroutine(DuringInvisibility(value));
    }
    IEnumerator DuringInvisibility(float value)
    {
        LayerMask excludeEnemies = LayerMask.GetMask("Ennemies");
        _invisibilityObj.excludeLayers = excludeEnemies;

        _invisibleAnim.LaunchAnim();
        yield return new WaitForSeconds(value);
        _invisibleAnim.RevertAnim();
        LayerMask excludeNothing = LayerMask.GetMask("Nothing");
        _invisibilityObj.excludeLayers = excludeNothing;
    }
    private void UpgradeStats(){
        GlobalEvents._chest._openChest?.Invoke();
        _playerController.secureInput = true;
    }

    private void Heal(){
        _playerController.Healing(_powersList[(int)TypePowers.HEAL].value + upgradeHeal);

    }

    private void DestroyEnemy(){
        StartCoroutine(WaitTimeComeDevil());
    }
    IEnumerator WaitTimeComeDevil(){
        yield return new WaitForSeconds(1.5f);
        GlobalEvents._power._destroyEnemy?.Invoke(_powersList[(int)TypePowers.DESTROY].value + upgradeDestroy);
    }

    public void WriteLevel(int index, int value, bool samePower){
        switch(value){
            case 0:print("NOPE");
            break;
            case 1:
            upgradeTime+=0.15f;
            if(samePower){
                int currentValue = lvlTime-1;
                foreach(TextMeshProUGUI txt in _textPowersLvl){ txt.text = "Lvl " + currentValue.ToString();}
                return;
            }
            _textPowersLvl[index].text = "Lvl " + lvlTime.ToString();
            lvlTime+=1;
            break;
            case 2:
            upgradeFire += 0.25f;
            if(samePower){
                int currentValue = lvlFire-1;
                foreach(TextMeshProUGUI txt in _textPowersLvl){ txt.text = "Lvl " + currentValue.ToString();}
                return;
            }
            _textPowersLvl[index].text = "Lvl " + lvlFire.ToString();
            lvlFire+=1;
            break;
            case 3:
            upgradeInvisibility += 0.15f;
            if(samePower){
                int currentValue = lvlInvisibility-1;
                foreach(TextMeshProUGUI txt in _textPowersLvl){ txt.text = "Lvl " + currentValue.ToString();}
                return;
            }
            _textPowersLvl[index].text = "Lvl " + lvlInvisibility.ToString();
            lvlInvisibility+=1;
            
            break;
            case 4:
            _textPowersLvl[index].text = "";
            break;
            case 5:
            upgradeHeal += 3;
            if(samePower){
                int currentValue = lvlHeal-1;
                foreach(TextMeshProUGUI txt in _textPowersLvl){ txt.text = "Lvl " + currentValue.ToString();}
                return;
            }
            _textPowersLvl[index].text = "Lvl " + lvlHeal.ToString();
            lvlHeal+=1;
            
            break;
            case 6:
            upgradeDestroy += 1;
            if(samePower){
                int currentValue = lvlDestroy-1;
                foreach(TextMeshProUGUI txt in _textPowersLvl){ txt.text = "Lvl " + currentValue.ToString();}
                return;
            }
            _textPowersLvl[index].text = "Lvl " + lvlDestroy.ToString();
            lvlDestroy+=1;
            break;
        }
    }

}
