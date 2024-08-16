using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapSwitcher : MonoBehaviour
{
    [SerializeField] private int _id;
    private AudioSource _audioSource;
    private EventAnimation _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = gameObject.GetComponent<EventAnimation>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player"){
            _anim.LaunchAnim();
            _audioSource.Play();
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait(){
        yield return new WaitForSeconds(0.5f);
        GlobalEvents._map._changeMapId?.Invoke(_id);
    }
}
