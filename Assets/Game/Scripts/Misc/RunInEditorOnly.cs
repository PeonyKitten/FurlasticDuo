using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FD.Misc
{
    public class RunInEditorOnly : MonoBehaviour
    {
        [SerializeField] private bool disableGameObjectIfNotEditor;
        [SerializeField] private bool disableIfNotOnlyScene = true;
        [SerializeField] private bool destroyIfDisabled = true;
        public UnityEvent onEditorStart;

        private void Awake()
        {
            var disabled = false;

#if !UNITY_EDITOR
            if (disableGameObjectIfNotEditor) {
                gameObject.SetActive(false);
                disabled = true;
            }
#endif
            if (disableIfNotOnlyScene && SceneManager.sceneCount > 1)
            {
                gameObject.SetActive(false);
                disabled = true;
            }

            if (destroyIfDisabled && disabled)
            {
                Destroy(gameObject);
                return;
            }

#if UNITY_EDITOR
            onEditorStart?.Invoke();
#endif
        }
    }
}
