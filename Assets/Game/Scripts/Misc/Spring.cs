using System;

namespace FD.Misc
{
    [Serializable]
    public struct Spring
    {
        public float strength;
        public float damping;

        public float CalculateSpringForce(float displacement, float velocity)
        {
            return displacement * strength - velocity * damping;
        }
    }
}