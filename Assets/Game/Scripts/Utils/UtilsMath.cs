using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Utils
{
    public static class UtilsMath
    {
        public enum Axis3D
        {
            X,
            Y,
            Z,
        }

        public static Vector3 RandomInsideCircle(float radius = 1f, Axis3D axis = Axis3D.Z)
        {
            var randomPoint = Random.insideUnitCircle * radius;

            return randomPoint.Bulk(axis);
        }

        /// <summary>
        /// Convert a Vector3 to a Vector2 by discarding it's 'y' component.
        /// </summary>
        /// <param name="input">3D Vector to be flattened.</param>
        /// <param name="axis">Axis to 'Flatten' or ignore.</param>
        /// <returns>Resulting 2D Vector.</returns>
        public static Vector2 Flatten(this Vector3 input, Axis3D axis = Axis3D.Y)
        {
            return axis switch
            {
                Axis3D.X => new Vector2(input.y, input.z),
                Axis3D.Y => new Vector2(input.x, input.z),
                Axis3D.Z => new Vector2(input.x, input.y),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis")
            };
        }

        public static Vector3 Bulk(this Vector2 input, Axis3D zeroedAxis = Axis3D.Y)
        {
            return zeroedAxis switch
            {
                Axis3D.X => new Vector3(0, input.x, input.y),
                Axis3D.Y => new Vector3(input.x, 0, input.y),
                Axis3D.Z => new Vector3(input.x, input.y, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(zeroedAxis), zeroedAxis, "Invalid Axis")
            };
        }

        public static float Single(this Vector2 input, Axis3D axis)
        {
            return axis switch
            {
                Axis3D.X => input.x,
                Axis3D.Y => input.y,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis")
            };
        }

        public static float Single(this Vector3 input, Axis3D axis)
        {
            return axis switch
            {
                Axis3D.X => input.x,
                Axis3D.Y => input.y,
                Axis3D.Z => input.z,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis")
            };
        }

        /// <summary>
        /// Normalizes a Vector3 and outputs its magnitude prior to being normalized.
        /// </summary>
        /// <param name="value">Vector3 to Normalize</param>
        /// <param name="magnitude">Magnitude of the input Vector</param>
        public static void NormalizeWithMagnitude(this ref Vector3 value, out float magnitude)
        {
            magnitude = Vector3.Magnitude(value);

            value = magnitude > 0.00001 ? value / magnitude : Vector3.zero;
        }

        /// <summary>
        /// Given a Vector3, return a normalized vector (direction) and its magnitude.
        /// </summary>
        /// <param name="value">Input Vector3</param>
        /// <param name="magnitude">Magnitude of the input Vector</param>
        /// <returns></returns>
        public static Vector3 NormalizedWithMagnitude(this Vector3 value, out float magnitude)
        {
            magnitude = Vector3.Magnitude(value);

            return magnitude > 0.00001 ? value / magnitude : Vector3.zero;
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

        public static bool IsWithinCylinder(this Vector3 center, Vector3 target, float radius,
            Axis3D flattenedAxis = Axis3D.Y)
        {
            var center2D = center.Flatten(flattenedAxis);
            var target2D = target.Flatten(flattenedAxis);

            return (center2D - target2D).IsWithinCircle(radius);
        }

        public static bool IsWithinCylinder(this Vector3 center, Vector3 target, float radius, float height,
            Axis3D flattenedAxis = Axis3D.Y)
        {
            var isContainedVertical = Mathf.Abs(center.Single(flattenedAxis) - target.Single(flattenedAxis)) <= height / 2f;

            return isContainedVertical && center.IsWithinCylinder(target, radius, flattenedAxis);
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