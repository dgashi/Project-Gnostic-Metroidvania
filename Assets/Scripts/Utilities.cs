using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }


    public static IEnumerator WaitForUnscaledSeconds(float delay)
    {
        var timer = 0f;
        while (timer < delay)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
    }


    public static float Vector2DAngleFromZero(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }


    public static float DistanceBetweenVectorsSquared(Vector2 vectorA, Vector2 vectorB)
    {
        return Mathf.Pow((vectorA.x - vectorB.x), 2) + Mathf.Pow((vectorA.y - vectorB.y), 2);
    }


    public static bool IsTagInList(GameObject gameObject, string[] listOfTags)
    {
        foreach (string i in listOfTags)
        {
            if (gameObject.CompareTag(i))
            {
                return true;
            }
        }

        return false;
    }
}
