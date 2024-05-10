using UnityEngine;

namespace Game.Scripts.Utils
{
    public static class UtilsMath
    {
        public enum FlattenAxis3D
        {
            Y,
            Z,
        }

        public static Vector3 RandomInsideCircle(float radius = 1f, FlattenAxis3D axis = FlattenAxis3D.Z)
        {
            var randomPoint = Random.insideUnitCircle * radius;

            return randomPoint.Bulk(axis);
        }
        
        /// <summary>
        /// Convert a Vector3 to a Vector2 by discarding it's 'y' component.
        /// </summary>
        /// <param name="input">3D Vector to be flattened.</param>
        /// <returns>Resulting 2D Vector.</returns>
        public static Vector2 Flatten(this Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        public static Vector2 Flatten(this Vector3 input, UtilsMath.FlattenAxis3D axis)
        {
            if (axis == UtilsMath.FlattenAxis3D.Z)
            {
                input.z = input.y;
            }

            return input.Flatten();
        }

        public static Vector3 Bulk(this Vector2 input)
        {
            return new Vector3(input.x, 0, input.y);
        }
        
        public static Vector3 Bulk(this Vector2 input, UtilsMath.FlattenAxis3D axis)
        {
            return axis == UtilsMath.FlattenAxis3D.Y ? new Vector3(input.x, input.y) : new Vector3(input.x, 0, input.y);
        }

        /// <summary>
        /// Check if a given point is within a specified radius.
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool IsWithinCircle(this Vector2 displacement, float radius = 1f)
        {
            return displacement.sqrMagnitude < radius * radius;
        }

        public static bool IsWithinSphere(this Vector3 displacement, float radius = 1f)
        {
            return displacement.sqrMagnitude < radius * radius;
        }

        public static Vector3 ProjectOffset(this Transform transform, Vector3 offset)
        {
            return transform.rotation * offset + transform.position;
        }
        
        // Quaternion helper functions, courtesy of Toyful Games
        public static Quaternion ShortestRotation(this Quaternion to, Quaternion from)
        {
            if (Quaternion.Dot(to, from) < 0)
            {
                return to * Quaternion.Inverse(from.Multiply(-1));
            }

            return to * Quaternion.Inverse(from);
        }
        public static Quaternion Multiply(this Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
    }
}