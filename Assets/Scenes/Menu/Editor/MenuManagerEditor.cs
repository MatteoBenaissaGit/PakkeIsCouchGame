namespace MatteoBenaissaLibrary.Menu.Editor
{
    
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseMenuManager))]
    public class MenuManagerEditor : UnityEditor.Editor
    {
        private BaseMenuManager _baseMenuManagerScript;

        private void OnEnable()
        {
            _baseMenuManagerScript = (BaseMenuManager)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            //buttons
            if (GUILayout.Button("Set Menu Type Simple"))
            {
                _baseMenuManagerScript.SetMenuTypeSimple();
            }

            if (GUILayout.Button("Set Menu Type Side Slide"))
            {
                _baseMenuManagerScript.SetMenuTypeSideSlide();
            }

            EditorUtility.SetDirty(_baseMenuManagerScript);
            
            #region Warnings

            //references check
            MenuReferences simpleMenuReferences = _baseMenuManagerScript.SimpleMenuReferences;
            MenuReferences sideSlideMenuReferences = _baseMenuManagerScript.SideSlideMenuReferences;

            if (simpleMenuReferences.MenuGameObject == null ||
                simpleMenuReferences.PlayButton == null ||
                simpleMenuReferences.CreditsButton == null ||
                simpleMenuReferences.QuitButton == null ||
                sideSlideMenuReferences.MenuGameObject == null ||
                sideSlideMenuReferences.PlayButton == null ||
                sideSlideMenuReferences.CreditsButton == null ||
                sideSlideMenuReferences.QuitButton == null)
            {
                EditorGUILayout.HelpBox("References missing", MessageType.Warning, true);
            }

            if (string.IsNullOrEmpty(_baseMenuManagerScript.PlaySceneName) ||
                string.IsNullOrEmpty(_baseMenuManagerScript.CreditSceneName))
            {
                EditorGUILayout.HelpBox("Scenes names missing", MessageType.Warning, true);
            }

            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}