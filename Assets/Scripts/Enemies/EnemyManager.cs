using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject[] _prefab;
    [SerializeField] private Camera _cam;

    [SerializeField] private Transform[] _spawnPositions;
    [SerializeField] private int _maxSpawn;

    private List<GameObject> _instance = new List<GameObject>();
    private List<Enemy> _instanceScript = new List<Enemy>();
    private int _indexCreate = -1;
    [SerializeField] float _spawnAmount;
    [SerializeField] float _delayBegin;

    // Start is called before the first frame update
    void OnEnable(){
        GlobalEvents._power._slowTime += Slow;
        GlobalEvents._power._invisibleTime += Invisible;
        GlobalEvents._power._destroyEnemy += KillEnemy;

         
    }

    void Start()
    {
       Spawn();
    }

    private void Spawn()
    {
        for(int i =0; i < _maxSpawn; i++){

            int randomPos = Random.Range(0, _spawnPositions.Length);
            int rndEnemy = Random.Range(0, _prefab.Length);

            GameObject enemy = Instantiate(_prefab[rndEnemy], _spawnPositions[randomPos].position, Quaternion.identity);
            _instance.Add(enemy);
            _instanceScript.Add(enemy.GetComponent<Enemy>());
            _instanceScript[i].Target = _playerTransform;
            _instanceScript[i].Cam = _cam;

            enemy.SetActive(false);
        }

        StartCoroutine(SpawnBeginDelay());
    }

    IEnumerator SpawnBeginDelay(){
        yield return new WaitForSeconds(_delayBegin);
        ActiveEnemy();
    }

    private void ActiveEnemy(){
        _indexCreate++;

        if(_indexCreate >= _instance.Count){_indexCreate = 0;}

        if(_instance[_indexCreate].activeInHierarchy){_indexCreate++; StartCoroutine(ActivePermantlyEnemy()); return;}

        int randomPos = Random.Range(0, _spawnPositions.Length);
        _instance[_indexCreate].transform.position = _spawnPositions[randomPos].position;
        _instance[_indexCreate].SetActive(true);
        StartCoroutine(ActivePermantlyEnemy());
    }
    IEnumerator ActivePermantlyEnemy(){

        yield return new WaitForSeconds(_spawnAmount);
        ActiveEnemy();
    }

    private void Slow(float value){

        foreach(Enemy en in _instanceScript){
            if(en.gameObject.activeInHierarchy){
                en.Slow(value);
            }
        }
    }

    private void Invisible(float value){
        StartCoroutine(DuringInvisibility(value));
        print(value);
    }

    IEnumerator DuringInvisibility(float value)
    {
        
        foreach(Enemy en in _instanceScript){
            if(en.gameObject.activeInHierarchy){
                en.CanFollow = false;
            }
        }
        yield return new WaitForSeconds(value);
        
        foreach(Enemy en in _instanceScript){
            if(en.gameObject.activeInHierarchy){
                en.CanFollow = true;
            }
        }
    }

    private void KillEnemy(float value){
        List<Enemy> currentList = new List<Enemy>();
        for(int  i = 0; i < _instanceScript.Count; i++){
            if(_instanceScript[i].gameObject.activeInHierarchy){
                currentList.Add(_instanceScript[i]);
            }
        }

        float diff;
        if (currentList.Count < value){
            diff = currentList.Count;
        }
        else{
            diff = value;
        }

        
        for(int  i = 0; i < diff; i++){
            int rnd = Random.Range(0, currentList.Count);
            Enemy item = currentList[rnd];
            item.Killed();
            currentList.Remove(item);
        }
            
    }

    private void OnDisable(){
        GlobalEvents._power._slowTime -= Slow;
        GlobalEvents._power._invisibleTime -= Invisible;
        GlobalEvents._power._destroyEnemy -= KillEnemy;
    }
}