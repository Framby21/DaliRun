using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GlobalEvents
{
  //CLASSES
  public static readonly PowersEvent _power = new PowersEvent();
  public static readonly EnemyBehavior _enemy = new EnemyBehavior();

  public static readonly ChestBehavior _chest = new ChestBehavior();

  public static readonly MapBehavior _map = new MapBehavior();

  //POWERS
  public class PowersEvent
  {
    public UnityAction<int, int>_addPower;
    public UnityAction<float> _slowTime;
    public UnityAction<float> _invisibleTime;
    public UnityAction<int> _upgradeStats;
    public UnityAction<float> _destroyEnemy;
  }

  //ENEMIES
  public class EnemyBehavior{
    public UnityAction<float> _damage;
  }

  public class ChestBehavior{
    public UnityAction _openChest;
  }

  public class MapBehavior{
    public UnityAction<int> _changeMapId;
  }

}
