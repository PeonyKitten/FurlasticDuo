using UnityEngine.UIElements;

namespace Game.Scripts.Utils
{
    // Based off of Matthew J. Spencer's approach outlined here - https://www.youtube.com/watch?v=I1JcytXwXM4
    internal static class UIExtensions
    {
        public static VisualElement Create(this VisualElement parent, params string[] classNames)
        {
            return parent.Create<VisualElement>(classNames);
        }
    
        public static T Create<T>(this VisualElement parent, params string[] classNames) where T: VisualElement, new()
        {
            var element = new T();
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }
            parent.Add(element);
            return element;
        }
    }
}