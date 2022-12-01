using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionInWorld : MonoBehaviour
{
    // вернуть координаты точки в мировом пространстве
    public static Vector3 GetWorldPoint(Vector3 screenPoint, float height)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Plane plane = new Plane(Vector2.up, new Vector3(0, height, 0));

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
