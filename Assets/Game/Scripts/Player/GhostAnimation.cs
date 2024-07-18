// <GhostAnimation.cs>
// <Kai Yiu Shum>, <Alvin Philips>
// <04 July 2024>
// <Script-based animation for ghost>

using FD.Elastic;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class GhostAnimation : MonoBehaviour
    {
        [Header("Elastic Force")]
        [SerializeField] private ElasticForce elasticForce;

        [Header("Body Setting")]
        [SerializeField] private GameObject ghostBodyParent;
        [SerializeField] private float jointPositionOffset;
        [SerializeField] private AnimationCurve bodyScaleFactorCurve;

        [Header("Effect Line Setting")]
        [SerializeField] private GameObject ghostEffectParent;
        [SerializeField] private float maxRotation = 90f;
        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private AnimationCurve effectScaleFactorCurve;

        [Header("Ghost Mouth")]
        [SerializeField] private Animator mouthAnimator;

        [Header("Ghost Material and Effect")]
        [SerializeField] private GameObject effectLine;
        [SerializeField] private GameObject body;
        [SerializeField] private GameObject maxEffect;

        [Header("Debug Settings")]
        [SerializeField] private bool debugJointDirections;

        private List<Transform> ghostBodyJoints = new();
        private List<Transform> ghostEffectJoints = new();
        private float origGhostBodyLength;

        private Transform player1;
        private Transform player2;
        private float maxDistance;
        private float forceAppliedThreshold;
        private float snapBackThreshold;

        private Renderer effectLineRenderer;
        private Renderer bodyRenderer;

        void Start()
        {
            // Get the Elastic Force components
            player1 = elasticForce.Player1;
            player2 = elasticForce.Player2;
            maxDistance = elasticForce.MaxDistance;
            forceAppliedThreshold = elasticForce.ForceAppliedDistance / maxDistance;
            snapBackThreshold = elasticForce.SnapbackDistance / maxDistance;

            // Get the materials
            effectLineRenderer = effectLine.GetComponent<Renderer>();
            bodyRenderer = body.GetComponent<Renderer>();

            // Get the joints from the joint parent from hierachy
            if (ghostBodyParent is not null)
            {
                Transform parentTransform = ghostBodyParent.transform;
                ghostBodyJoints = GetAllChildrenUnder(parentTransform);
            }

            if (ghostEffectParent is not null)
            {
                Transform parentTransform = ghostEffectParent.transform;
                ghostEffectJoints = GetAllChildrenUnder(parentTransform);
            }

            Vector3 ghostLengthVec = ghostBodyJoints[ghostBodyJoints.Count - 1].position - ghostBodyJoints[0].position;
            origGhostBodyLength = ghostLengthVec.magnitude;
        }

        private void Update()
        {
            // calculate the vector between players
            Vector3 playerVec = player2.position - player1.position;

            // Set the start joint position
            Vector3 playerVecNormalized = playerVec.normalized;
            Vector3 offsetPos = playerVecNormalized * jointPositionOffset;
            ghostBodyJoints[0].position = player1.position - offsetPos;

            // Set the position of the in-between joints
            for (int i = 0; i < ghostBodyJoints.Count; i++)
            {
                float moveFactor = (float) i / (ghostBodyJoints.Count - 1);
                Vector3 totalDist = AddLengthToVector(playerVec, jointPositionOffset*2);

                // Handle Body joints
                ghostBodyJoints[i].position = ghostBodyJoints[0].position + totalDist * moveFactor;

                // Handle Effect joints
                ghostEffectJoints[i].position = ghostEffectJoints[0].position + totalDist * moveFactor;
            }

            // calculate the current ghost length
            Vector3 ghostLengthVec = ghostBodyJoints[ghostBodyJoints.Count - 1].position - ghostBodyJoints[0].position;
            float currentGhostBodyLength = ghostLengthVec.magnitude;

            // Calculate the scale factor for the in-between joints
            float ghostLength = Map(currentGhostBodyLength, origGhostBodyLength, maxDistance, 0, 1);
            float bodyScaleFactor = bodyScaleFactorCurve.Evaluate(ghostLength);
            float effectScaleFactor = effectScaleFactorCurve.Evaluate(ghostLength);

            // Set the scale of the in-between Body joints
            for (int i = 1; i < ghostBodyJoints.Count - 1; i++)
            {
                    Vector3 targetScale = new Vector3(1, bodyScaleFactor, bodyScaleFactor);
                    ghostBodyJoints[i].localScale = targetScale;
            }

            CalculateNormalAndBitangent(playerVec.normalized, out var forward, out var up);
            var right = Vector3.Cross(up, forward);

            // Align the rotation of the Body joints
            foreach (Transform childTransform in ghostBodyJoints)
            {
                childTransform.rotation = Quaternion.LookRotation(-forward, up);

                if (debugJointDirections)
                {
                    Debug.DrawRay(childTransform.position, up, Color.green);
                    Debug.DrawRay(childTransform.position, right, Color.red);
                    Debug.DrawRay(childTransform.position, forward, Color.blue);
                }
            }

            // Calculate the rotation for the in-between Effect joints
            float targetRotation = rotationCurve.Evaluate(ghostLength) * maxRotation;
            Quaternion rootRotation = Quaternion.Euler(-targetRotation * 2, 0, 0);
            ghostEffectJoints[0].localRotation = rootRotation;

            // Set the rotation of the in-between Effect joints
            for (int i = 1; i < ghostEffectJoints.Count; i++)
            {
                Quaternion newRotation = Quaternion.Euler(targetRotation, 0, 0);
                ghostEffectJoints[i].localRotation = newRotation;
            }

            ghostEffectJoints[1].localScale = new Vector3(1, effectScaleFactor, effectScaleFactor);
            ghostEffectJoints[2].localScale = new Vector3(1, effectScaleFactor, effectScaleFactor);
            ghostEffectJoints[3].localScale = new Vector3(1, 1 / effectScaleFactor, 1 / effectScaleFactor);
            ghostEffectJoints[4].localScale = new Vector3(1, 1 / effectScaleFactor, 1 / effectScaleFactor);

            // Set the stages for effects
            if (effectLineRenderer != null)
            {
                effectLineRenderer.material.SetFloat("_Stages", ghostLength);

                // Set the effect line opacity
                float opa = Map(ghostLength, 0, forceAppliedThreshold/2, 0, 1);
                effectLineRenderer.material.SetFloat("_OPA", opa);

                if (ghostLength < snapBackThreshold) maxEffect.SetActive(false);
                else maxEffect.SetActive(true);
            }

            if (bodyRenderer != null)
            {
                bodyRenderer.material.SetFloat("_Stages", ghostLength);
            }

            // Set the ghost mouth deformation
            if (mouthAnimator != null)
            {
                mouthAnimator.SetFloat("MouthShapeBlend", ghostLength);
            }
        }

        private void CalculateNormalAndBitangent(Vector3 tangent, out Vector3 normal, out Vector3 bitangent)
        {
            // Choose an arbitrary vector that is not parallel to the tangent
            Vector3 up = Vector3.up;

            // Ensure the arbitrary vector is not parallel to the tangent
            if (Vector3.Dot(tangent, up) > 0.99f)
            {
                up = Vector3.forward;
            }

            // Calculate the normal vector using the cross product
            normal = Vector3.Normalize(Vector3.Cross(tangent, up));

            // Calculate the bitangent vector using the cross product
            bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));

            if (Vector3.Dot(bitangent, up) < 0)
            {
                bitangent = -bitangent;
            }
        }

        // Function to map a value from one range to another
        private float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
        {
            // Ensure the ranges are valid
            if (originalMin == originalMax)
            {
                Debug.LogError("Original min and max values must be different.");
                return newMin; // or any default value
            }

            if (newMin == newMax)
            {
                Debug.LogError("New min and max values must be different.");
                return newMin; // or any default value
            }

            // Map the value
            float mappedValue = newMin + ((value - originalMin) / (originalMax - originalMin)) * (newMax - newMin);
            return mappedValue;
        }

        // Function to add length to a vector
        private Vector3 AddLengthToVector(Vector3 vector, float lengthToAdd)
        {
            // Calculate the unit vector (direction) of the original vector
            Vector3 direction = vector.normalized;

            // Calculate the new length of the vector
            float newLength = vector.magnitude + lengthToAdd;

            // Scale the direction vector by the new length to get the new vector
            return direction * newLength;
        }

        // Function to get all children under a parent Transform
        private List<Transform> GetAllChildrenUnder(Transform parent)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform child in parent)
            {
                children.Add(child);
                // Recursively add the children's children
                children.AddRange(GetAllChildrenUnder(child));
            }

            return children;
        }
    }
}
