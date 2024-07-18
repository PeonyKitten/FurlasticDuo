using FMODUnity;
using UnityEngine;

namespace Game.Scripts.Audio
{
    public class PlayGenericOneShot: MonoBehaviour
    {
        [SerializeField] private EventReference soundEvent;

        public void PlaySoundEvent()
        {
            if (!soundEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(soundEvent);
            }
        }
    }
}