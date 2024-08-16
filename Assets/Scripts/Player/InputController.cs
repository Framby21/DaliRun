using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    //MOVEMENT
    private KeyCode _left;
    public UnityAction<bool> _goLeft;
    private KeyCode _right;
    public UnityAction<bool> _goRight;
    private KeyCode _top;
    public UnityAction<bool> _goTop;
    private KeyCode _bot;
    public UnityAction<bool> _goBot;

    //POWERS
    [SerializeField] KeyCode _power1;
    public UnityAction<bool> _goPower1;
    [SerializeField] KeyCode _power2;
    public UnityAction<bool> _goPower2;

    void Start(){
        CheckKeyboard();
    }
    void Update()
    {
        //MOVEMENT
        if(Input.GetKey(_left)){
            _goLeft?.Invoke(true);
        }else{_goLeft?.Invoke(false);}

        if(Input.GetKey(_right)){
            _goRight?.Invoke(true);
        }else{_goRight?.Invoke(false);}

        if(Input.GetKey(_top)){
            _goTop?.Invoke(true);
        }else{_goTop?.Invoke(false);}

        if(Input.GetKey(_bot)){
            _goBot?.Invoke(true);
        }else{_goBot?.Invoke(false);}

        //POWERS
        if(Input.GetKeyDown(_power1)){
            _goPower1?.Invoke(true);
        }else{_goPower1?.Invoke(false);}

        if(Input.GetKeyDown(_power2)){
            _goPower2?.Invoke(true);
        }else{_goPower2?.Invoke(false);}
    }

    void CheckKeyboard(){
        string str = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        if(str == "fr"){
            print("AZERTY");
             _left = KeyCode.Q;
            _right = KeyCode.D;
            _top = KeyCode.Z;
            _bot = KeyCode.S;
        }
        else{
            print("QWERTY");
            _left = KeyCode.A;
            _right = KeyCode.D;
            _top = KeyCode.W;
            _bot = KeyCode.S;
        }
    }
}
