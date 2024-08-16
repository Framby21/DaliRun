using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Power_Scriptable : ScriptableObject
{
    public int idType;
    public string powerName;
    public int powerId;
    public Sprite powerSprite;
    public float cooldown;
    public float value;
    public Color color;
}

