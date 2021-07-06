using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExceptionSoftware.Injector
{
    [InitializeOnLoad]
    public class ExInjectorUtilityEditor
    {
        static InjectorSettingsAsset _settings = null;
        public static InjectorSettingsAsset Settings => LoadAsset();

        public const string INJECTOR_PATH = ExConstants.SETTINGS_PATH + "Injector/";
        public const string INJECTOR_PATH_RESOURCES = INJECTOR_PATH + "Resources/";
        public const string INJECTOR_MENU_ITEM = "Tools/Injector/";

        public const string INJECTOR_SETTINGS_FILENAME = "ExInjectorSettings";

        static ExInjectorUtilityEditor() => LoadAsset();

        internal static InjectorSettingsAsset LoadAsset()
        {
            if (!System.IO.Directory.Exists(INJECTOR_PATH))
                System.IO.Directory.CreateDirectory(INJECTOR_PATH);

            if (!System.IO.Directory.Exists(INJECTOR_PATH_RESOURCES))
                System.IO.Directory.CreateDirectory(INJECTOR_PATH_RESOURCES);

            if (_settings == null)
            {
                _settings = ExAssets.FindAssetsByType<InjectorSettingsAsset>().First();
            }

            if (_settings == null)
            {
                _settings = Resources.FindObjectsOfTypeAll<InjectorSettingsAsset>().FirstOrDefault();
            }

            if (_settings == null)
            {
                _settings = ExAssets.CreateAsset<InjectorSettingsAsset>(INJECTOR_PATH_RESOURCES, INJECTOR_SETTINGS_FILENAME, true, true);
            }

            return _settings;
        }

        [MenuItem(INJECTOR_MENU_ITEM + "Settings", priority = ExConstants.MENU_ITEM_PRIORITY)]
        static void SelectAsset()
        {
            LoadAsset();
            Selection.activeObject = _settings;
        }
    }
}
