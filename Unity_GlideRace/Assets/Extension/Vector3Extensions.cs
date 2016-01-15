using UnityEngine;

public static class Vector3Extensions
{
    private static Vector3 temp;
    public static Vector3 Mul(this Vector3 v1, Vector3 v2)
    {
        temp = Vector3.zero;
        temp.x = v1.x * v2.x;
        temp.y = v1.y * v2.y;
        temp.z = v1.z * v2.z;
        return temp;
    }
    public static void SetMul(this Vector3 v1, Vector3 v2)
    {
        temp = Vector3.zero;
        temp.x = v1.x * v2.x;
        temp.y = v1.y * v2.y;
        temp.z = v1.z * v2.z;
        v1 = temp;
    }
}