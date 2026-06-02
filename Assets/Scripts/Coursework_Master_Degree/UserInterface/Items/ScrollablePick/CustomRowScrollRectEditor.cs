using UnityEditor;
using UnityEditor.UI;

namespace Coursework_Master_Degree.UserInterface.Items.ScrollablePick
{
    [CustomEditor(typeof(CustomRowScrollRect))]
    public class CustomRowScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty _parentScrollRect;

        protected override void OnEnable()
        {
            base.OnEnable();
            _parentScrollRect = serializedObject.FindProperty("ParentScrollRect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_parentScrollRect);

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}