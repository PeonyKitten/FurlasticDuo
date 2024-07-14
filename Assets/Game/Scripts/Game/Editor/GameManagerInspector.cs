using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Scripts.Game.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty _gameScene;
        
        private void OnEnable()
        {
            _gameScene = serializedObject.FindProperty("gameScene");
        }

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            var myInspector = new VisualElement();
            
            var sceneNames = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))
                .Where(sceneName => sceneName != "GUI").ToList();
            
            var sceneDropdown = new DropdownField("Game Scene")
            {
                choices = sceneNames,
                value = _gameScene.stringValue
            };

            sceneDropdown.BindProperty(_gameScene);
            
            myInspector.Add(sceneDropdown);
            return myInspector;
        }
    }
}
