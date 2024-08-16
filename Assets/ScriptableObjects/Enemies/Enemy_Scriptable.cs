using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Enemy_Scriptable : ScriptableObject
{
    public int maxHealth;
    public float dmg;
    public float cooldown;
    public float speed;
    
    public Sprite sprite;
}
