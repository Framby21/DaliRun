using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool _isConsumed;
    private SpriteRenderer _spriteRender;
    [SerializeField] private Sprite _spriteOpen;
    [SerializeField] private EventAnimation _anim;

    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRender = gameObject.GetComponent<SpriteRenderer>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col){
        if(_isConsumed){return;}

        if(col.tag == "Player"){
           _spriteRender.sprite = _spriteOpen;
           _audioSource.Play();
           _anim.LaunchAnim();
          StartCoroutine(Opened());
        }
    }

    IEnumerator Opened(){
        yield return new WaitForSeconds(0.25f);
         GlobalEvents._chest._openChest?.Invoke();
          _isConsumed = true;
    }

}
