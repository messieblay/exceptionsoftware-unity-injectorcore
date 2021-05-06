using ExceptionSoftware.Injector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ExInjector
{

    static Dictionary<System.Type, object> _injectables = new Dictionary<System.Type, object>();
    static Dictionary<System.Type, List<ExInjectableVariable>> _injectsReceptors = new Dictionary<System.Type, List<ExInjectableVariable>>();
    static List<ExInjectableVariable> _injectsStaticReceptors = new List<ExInjectableVariable>();

    static List<ExInjectableVariable> _auxList = null;

    public static bool enableNormalLog = false;
    static bool _initialized = false;

    public static System.Action<Type, object> onNewInjectable = null;

    public static void ReStartService()
    {
        _initialized = false;
        StartService();
    }

    static void Initialize()
    {
        _injectsReceptors.Clear();
        _injectables.Clear();
    }

    public static void StartService()
    {
        if (_initialized) return;
        LogTitle("InjectorCore STARTING!");
        Initialize();
        ColletAllStaticInjectReceptor();
        InjectDependences();
        LogTitle("InjectorCore STARTING DONE!");
        _initialized = true;
    }

    #region Registrer Injectable Objects

    /// <summary>
    /// Registrers the injectable object only if has Injectablex attribute.
    /// </summary>
    /// <param name="typex">Typex.</param>
    /// <param name="obj">Object.</param>
    public static void RegistrerInjectableObject(object obj)
    {
        RegistrerInjectableObject(obj.GetType(), obj);
    }

    public static void RegistrerInjectableObject(System.Type typex, object obj)
    {
        if (onNewInjectable != null)
        {
            onNewInjectable(typex, obj);
        }

        if (_injectables.ContainsKey(typex))
        {
            //Eliminamos la referencia si el tipo existe pero el objeto ha sido borrado
            //Esto deberia ser solo por seguridad. El propio objeto al destruirse deberia borrar su propia referencia
            if (_injectables[typex] == null)
            {
                _injectables.Remove(typex);
            }
            else
            {
                return;
            }
        }

        if (typex.GetCustomAttributes(typeof(Injectablex), false).Length > 0)
        {
            Log($"Registred: {typex.Name}");
            _injectables.Add(typex, obj);

            InjectDependencesByNewInjectable(typex);
            return;
        }
    }
    public static void RemoveInjectableObject(System.Type typex)
    {
        Log($"Removing: {typex.Name}");
        _injectables.Remove(typex);
    }
    #endregion

    #region Registrer Inject Receptor

    /// <summary>
    /// Registrers the injectable receptors in object.
    /// </summary>
    /// <param name="obj">Object.</param>
    public static void RegistrerInjectableReceptorsInObject(object obj)
    {
        Type type = obj.GetType();
        Log($"Registred: {type.Name}");

        foreach (ExInjectableVariable inject in ExInjertorUtility.ReflectClass(obj, obj.GetType()))
        {
            GetInjectableReceptor(inject.typeRequired).Add(inject);
            InjectDependency(inject);
        }
    }

    public static void RemoveInjectableReceptorsInObject(object obj)
    {
        Type type = obj.GetType();
        Log($"Removing: [{ type.Name}]");

        List<Type> Keys = _injectsReceptors.Keys.ToList();
        List<ExInjectableVariable> values;

        foreach (Type key in Keys)
        {
            values = _injectsReceptors[key];
            values.RemoveAll(r => r == obj);
            if (values.Count == 0)
            {
                _injectsReceptors.Remove(key);
            }
        }
    }


    #endregion

    /// <summary>
    /// Injects all injetable objects y all receptors if not injected yet.
    /// </summary>
    public static void InjectDependences()
    {
        object injectableObject = null;
        foreach (List<ExInjectableVariable> inj in _injectsReceptors.Values)
        {
            if (inj.Count == 0)
                continue;

            if (_injectables.TryGetValue(inj.First().typeRequired, out injectableObject))
            {
                inj/*.Where(i => !i.injected).ToList()*/.ForEach(i =>
                {
                    i.SetValue(injectableObject);
                    Log($"Injected: {i.typeRequired.Name} -> [{ i.classOwner.Name}]");
                });
            }
        }
    }

    public static void InjectDependencesByNewInjectable(Type typex)
    {
        object injectableObject = null;
        //      
        if (_injectables.TryGetValue(typex, out injectableObject))
        {
            foreach (ExInjectableVariable inj in _injectsReceptors.Values.SelectMany(s => s.Where(s2 => s2.typeRequired.Equals(typex))/*.Where(s3 => !s3.injected)*/))
            {
                inj.SetValue(injectableObject);
                Log($"Injected: {inj.typeRequired.Name} -> [{ inj.classOwner.Name}]");
            }
        }
    }

    /// <summary>
    /// Injects the dependences on a single objects
    /// </summary>
    /// <param name="inj">Inj.</param>
    static void InjectDependency(ExInjectableVariable inj)
    {
        object injectableObject = null;
        if (_injectables.TryGetValue(inj.typeRequired, out injectableObject))
        {
            inj.SetValue(injectableObject);
        }
    }

    static List<ExInjectableVariable> GetInjectableReceptor(System.Type typex)
    {
        _auxList = null;
        if (!_injectsReceptors.TryGetValue(typex, out _auxList))
        {
            _auxList = new List<ExInjectableVariable>();
            _injectsReceptors.Add(typex, _auxList);
        }
        return _auxList;
    }

    /// <summary>
    /// Collets all static inject receptor.
    /// </summary>
    static void ColletAllStaticInjectReceptor()
    {
        foreach (ExInjectableVariable inject in ExInjertorUtility.ReflectClass(null, Assembly.GetExecutingAssembly().GetTypes()))
        {
            _injectsStaticReceptors.Add(inject);
            GetInjectableReceptor(inject.typeRequired).Add(inject);
            Log($"Static receptor collected: [{inject.classOwner.Name}] <- {inject.typeRequired.Name}");
        }
    }




    #region DEBUG

    public static void Log(string s)
    {
        if (!ExInjertorUtility.Settings.logs) return;

#if EXLOGS
        Logx.Log(s, LogxEnum.InjectorCore);
#else
        Debug.Log(s);
#endif
    }
    public static void LogTitle(string s)
    {
        if (!ExInjertorUtility.Settings.logs) return;

#if EXLOGS
        Logx.LogTitle(s, LogxEnum.InjectorCore);
#else
        Debug.Log(s);
#endif
    }
    #endregion
}
