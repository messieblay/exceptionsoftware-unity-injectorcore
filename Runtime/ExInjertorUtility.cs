using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ExceptionSoftware.Injector
{
    public class ExInjertorUtility
    {
        static InjectorSettingsAsset _settings = null;
        public static InjectorSettingsAsset Settings => LoadAsset();
        internal static InjectorSettingsAsset LoadAsset()
        {
            if (_settings == null)
            {
                _settings = Resources.FindObjectsOfTypeAll<InjectorSettingsAsset>().FirstOrDefault();
            }

            return _settings;
        }
        /// <summary>
        /// Reflects the class and returns injects Receptors.
        /// </summary>
        /// <returns>The class.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="types">Types.</param>
        public static IEnumerable<ExInjectableVariable> ReflectClass(object obj, params Type[] types)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            if (obj == null)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }
            string log = "";
            for (int x = 0; x < types.Length; x++)
            {
                log = "";
                foreach (PropertyInfo p in types[x].GetProperties(flags))
                {
                    //int c0 = p.GetCustomAttributes(typeof(Injectx), true).Length;
                    //int c1 = p.GetCustomAttributes(typeof(Injectx), false).Length;

                    if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                    {
                        if (!p.CanWrite)
                            continue;

                        if (obj == null && p.GetSetMethod() != null && !p.GetSetMethod().IsStatic)
                            continue;

                        yield return new ExInjectableProperty(obj, p, types[x]);
                    }
                }

                foreach (FieldInfo p in types[x].GetFields(flags))
                {
                    //int c0 = p.GetCustomAttributes(typeof(Injectx), true).Length;
                    //int c1 = p.GetCustomAttributes(typeof(Injectx), false).Length;

                    if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                    {
                        if (obj == null && !p.IsStatic)
                            continue;

                        yield return new ExInjectableField(obj, p, types[x]);
                    }
                }

                if (log != "")
                {
                    //              Log (types [x].Name + log);
                }
            }


        }

    }
}
