using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityStandardAssets.CinematicEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bloom_FPS))]
    public class BloomEditor_FPS : Editor
    {
        [NonSerialized]
        private List<SerializedProperty> m_Properties = new List<SerializedProperty>();

        BloomGraphDrawer_FPS _graph;

        bool CheckHdr(Bloom_FPS target)
        {
            var camera = target.GetComponent<Camera>();
#if UNITY_5_6_OR_NEWER
            return camera != null && camera.allowHDR;
#else
            return camera != null && camera.hdr;
#endif
        }

        void OnEnable()
        {
            var settings = FieldFinder_FPS<Bloom_FPS>.GetField(x => x.settings);
            foreach (var setting in settings.FieldType.GetFields())
            {
                var prop = settings.Name + "." + setting.Name;
                m_Properties.Add(serializedObject.FindProperty(prop));
            }

            _graph = new BloomGraphDrawer_FPS();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.Space();
                var bloom = (Bloom_FPS)target;
                _graph.Prepare(bloom.settings, CheckHdr(bloom));
                _graph.DrawGraph();
                EditorGUILayout.Space();
            }

            foreach (var property in m_Properties)
                EditorGUILayout.PropertyField(property);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
