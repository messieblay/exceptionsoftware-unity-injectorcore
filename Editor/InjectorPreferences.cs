using UnityEditor;

namespace ExceptionSoftware.Injector
{
    public static class InjectorPreferences
    {
        private static string logInjectionPath = "Project/Ex Injector";


        private static string logInjectionKey = "exinjectorLog";
        private static string logInjectionEditorKey = "exinjectorEditorLog";




        [SettingsProvider]
        public static SettingsProvider CreateMultiplayerBuildLocationSettingsProvider()
        {
            InjectorSettingsAsset asset = ExInjertorUtils.Settings;

            var provider = new SettingsProvider(logInjectionPath, SettingsScope.Project)
            {
                label = "Ex Injector",
                guiHandler = (searchContext) =>
                {
                    // Lets actually draw our settings window!
                    // First off were going to draw a textbox for the build path.
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        bool res = EditorGUILayout.Toggle("Log message", EditorPrefs.GetBool(logInjectionKey));

                        if (check.changed)
                        {
                            // if the path string was saved, lets store it off into editor prefrences, so we can easily 
                            // query for it in the build process.
                            EditorPrefs.SetBool(logInjectionKey, res);
                            asset.logs = res;

                            Unityx.SetDirty(asset);
                        }
                    }

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        bool res = EditorGUILayout.Toggle("Log Editor message", EditorPrefs.GetBool(logInjectionEditorKey));

                        if (check.changed)
                        {
                            // if the path string was saved, lets store it off into editor prefrences, so we can easily 
                            // query for it in the build process.
                            EditorPrefs.SetBool(logInjectionEditorKey, res);
                            asset.logsEditor = res;

                            Unityx.SetDirty(asset);
                        }
                    }

                }
            };

            return provider;
        }
    }
}
