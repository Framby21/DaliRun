using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }
}
