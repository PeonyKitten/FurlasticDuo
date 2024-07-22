using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Misc
{
    public class CurvedHallwayTrigger : MonoBehaviour
    {
        public FD.Toys.Door hamsterWheelDoor;
        public Material curvedHallwayMaterial;
        public float maxCurviness = 1f;
        public bool resetCurvature = false;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UpdateShaderCurviness();
            }

            Debug.Log("Player Detected");
        }

        private void UpdateShaderCurviness()
        {
            if (hamsterWheelDoor != null && curvedHallwayMaterial != null)
            {
                float openness = hamsterWheelDoor.Openness;
                float curviness;

                if (resetCurvature)
                {
                    curviness = (1 - openness) * maxCurviness;
                }
                else
                {
                    curviness = openness * maxCurviness;
                }

                curvedHallwayMaterial.SetFloat("_Skew_Multiplier", curviness);
            }
        }
    }
}
