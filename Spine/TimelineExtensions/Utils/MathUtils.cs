using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils.Math
{
    public class MathUtils : MonoBehaviour
    {
        public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1 - t;
            float uu = u * u;
            float uuu = u * u * u;
            float tt = t * t;
            float ttt = t * t * t;
            Vector3 p = p0 * uuu;
            p += 3 * p1 * t * uu;
            p += 3 * p2 * tt * u;
            p += p3 * ttt;
            return p;
        }

        public static Vector3 BezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1 - t;
            float uu = u * u;
            float tu = t * u;
            float tt = t * t;

            Vector3 p = p0 * 3 * uu * (-1.0f);
            p += p1 * 3 * (uu - 2 * tu);
            p += p2 * 3 * (2 * tu - tt);
            p += p3 * 3 * tt;

            return p.normalized;
        }


    }
}
