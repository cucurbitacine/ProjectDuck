using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public class AssembliesPyramidWindow : EditorWindow
    {
        private static readonly List<List<AssemblyData>> Pyramid = new List<List<AssemblyData>>();
        
        private Vector2 _scrollView;
        
        [MenuItem("Tools/Assemblies/Pyramid")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AssembliesPyramidWindow));
            window.titleContent = new GUIContent("Assemblies Pyramid");
            
            Refresh();
        }

        private static void Refresh()
        {
            AssemblyData.Refresh();
            
            Pyramid.Clear();
            
            foreach (var data in AssemblyData.Data)
            {
                Pyramid.Add(new List<AssemblyData>() { data });
            }
        }
        
        private void OnGUI()
        {
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }

            _scrollView = GUILayout.BeginScrollView(_scrollView);
            
            foreach (var data in Pyramid)
            {
                GUILayout.Box(string.Join(", ", data.Select(d => d.assembly.name)));
            }
            
            GUILayout.EndScrollView();
        }
    }
}