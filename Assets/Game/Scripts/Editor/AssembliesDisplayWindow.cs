using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;

namespace Game.Scripts.Editor
{
    public class AssembliesDisplayWindow : EditorWindow
    {
        private readonly List<AssemblyData> _filtered = new List<AssemblyData>();
        private readonly HashSet<Assembly> _depends = new HashSet<Assembly>();
        private readonly HashSet<Assembly> _refers = new HashSet<Assembly>();
        
        private Vector2 _mainScrollView;
        private Vector2 _dependScrollView;
        private Vector2 _referScrollView;
        
        private string _search;
        
        [MenuItem("Tools/Assemblies/Search")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AssembliesDisplayWindow));
            window.titleContent = new GUIContent("Assemblies Display");
            
            AssemblyData.Refresh();
        }
        
        private bool DrawAssembly(Assembly assembly)
        {
            if (GUILayout.Button($"{assembly.name}"))
            {
                _search = assembly.outputPath;
                
                SceneView.RepaintAll();
                
                return true;
            }
            
            return false;
        }

        private void DrawAssemblyList(string titleList, IReadOnlyCollection<Assembly> assemblies, ref Vector2 scrollView)
        {
            if (assemblies == null) return;
            
            GUILayout.BeginVertical();
            
            GUILayout.Box($"{titleList} ({assemblies.Count})");
            
            scrollView = GUILayout.BeginScrollView(scrollView);
            
            foreach (var assemblyData in assemblies)
            {
                DrawAssembly(assemblyData);
            }

            GUILayout.EndScrollView();
            
            GUILayout.EndVertical();
        }
        
        private void DrawAssemblyDataList(string titleList, IEnumerable<AssemblyData> data, ref Vector2 scrollView)
        {
            DrawAssemblyList(titleList, data.Select(d => d.assembly).ToList(), ref scrollView);
        }

        private void DrawUpdate()
        {
            if (GUILayout.Button("Refresh"))
            {
                AssemblyData.Refresh();
                
                _search = string.Empty;
            }
        }
        
        private void DrawSearchField()
        {
            if (string.IsNullOrWhiteSpace(_search))
            {
                _search = string.Empty;
            }
            
            _search = GUILayout.TextField(_search);
        }
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            DrawUpdate();
            DrawSearchField();
            
            GUILayout.EndHorizontal();
            
            _filtered.Clear();
            _filtered.AddRange(AssemblyData.Data.Where(data => data.assembly.outputPath.Contains(_search, StringComparison.OrdinalIgnoreCase)));
            
            _depends.Clear();
            _refers.Clear();
            foreach (var assemblyData in _filtered)
            {
                foreach (var depend in assemblyData.depends)
                {
                    _depends.Add(depend);
                }

                foreach (var refer in assemblyData.refers)
                {
                    _refers.Add(refer);
                }
            }
            
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            
            DrawAssemblyList("Is using", _refers, ref _referScrollView);
            
            DrawAssemblyDataList("Assembly", _filtered, ref _mainScrollView);
            
            DrawAssemblyList("Used by", _depends, ref _dependScrollView);
            
            GUILayout.FlexibleSpace();
            
            GUILayout.EndHorizontal();
        }
    }
}