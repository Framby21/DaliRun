using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _maps;
    [SerializeField] private GameObject[] _initPlayerPos;
    [SerializeField] private GameObject _player;
    private Transform _transformPlayer;
    private List<Transform> _transformInitPlayerPos = new List<Transform>();
    private int oldMap = 0;

    [SerializeField] private AudioSource[] _musics;

    // Start is called before the first frame update
    private void Start()
    {
        _transformPlayer = _player.transform;

        foreach(GameObject obj in _initPlayerPos){
            _transformInitPlayerPos.Add(obj.transform);
        }

        for(int i = 0; i < _maps.Length; i++){
            _maps[i].SetActive(false);
        }
        

        GlobalEvents._map._changeMapId += ChangeMap;

        StartCoroutine(LaunchMap(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeMap(int value){
        if(value == 3){StartCoroutine(FadeMusic());}
        if(value == 4){
            SceneManager.LoadScene(2);
            return;
        }
        StartCoroutine(LaunchMap(value));
    }

    IEnumerator LaunchMap(int value){
        yield return new WaitForSeconds(0.05f);
        _maps[oldMap].SetActive(false);
        _transformPlayer.transform.position = _transformInitPlayerPos[value].position;
        oldMap = value;
        _maps[value].SetActive(true);
    }

    public void ReturnMainMenu(){
        SceneManager.LoadScene(0);
    }

    IEnumerator FadeMusic(){
         float l_elapsedTime = 0;
        float l_duration = 2;

        _musics[1].Play();

        while (l_elapsedTime <= l_duration)
        {
            l_elapsedTime += Time.deltaTime;
            float l_ratio = Mathf.Clamp(l_elapsedTime / l_duration, 0, 1);

            float lerpV1 = Mathf.Lerp(0.3f, 0, l_ratio);
            _musics[0].volume = lerpV1;

            float lerpV2 = Mathf.Lerp(0, 0.3f, l_ratio);
            _musics[1].volume = lerpV2;
            yield return null;
        }
    }

    private void OnDisable(){
        GlobalEvents._map._changeMapId -= ChangeMap;
    }
}
