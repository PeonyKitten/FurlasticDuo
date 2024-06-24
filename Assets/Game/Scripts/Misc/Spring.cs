using System;

namespace Game.Scripts.Misc
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