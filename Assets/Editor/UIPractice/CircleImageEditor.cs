using UIPractice;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.UIPractice
{
    [CustomEditor(typeof(CircleImage), true)]
    public class CircleImageEditor : ImageEditor
    {
        private const int UI_LAYER = 5;
        private SerializedProperty percent;
        private SerializedProperty chunkCount;
        
        [MenuItem("GameObject/UI/CircleImage", priority = 0)]
        private static void AddCircleImage() {
            var image = new GameObject("CircleImage") {layer = UI_LAYER};
            image.AddComponent<RectTransform>();
            image.AddComponent<CircleImage>();

            if (Selection.activeGameObject != null && Selection.activeGameObject.layer == UI_LAYER) {
                image.transform.SetParent(Selection.activeGameObject.transform);
            }
            else {
                var canvas = GetCanvas();
                image.transform.SetParent(canvas.transform);
            }
            image.transform.localPosition = Vector3.zero;
        }

        private static Canvas GetCanvas() {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null) {
                return canvas;
            }

            var gameObject = new GameObject("Canvas") {layer = UI_LAYER};
            gameObject.AddComponent<RectTransform>();
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        protected override void OnEnable() {
            base.OnEnable();
            percent = serializedObject.FindProperty("percent");
            chunkCount = serializedObject.FindProperty("chunkCount");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EditorGUILayout.Slider(percent, 0, 1, new GUIContent("Percent"));
            EditorGUILayout.PropertyField(chunkCount);
            serializedObject.ApplyModifiedProperties();
        }
    }
}