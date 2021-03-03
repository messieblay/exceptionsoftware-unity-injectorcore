using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class InjectorCoreEditor
{
    static Dictionary<System.Type, object> _injectables = new Dictionary<System.Type, object>();
    static Dictionary<System.Type, List<InjectableVariable>> _injectsReceptors = new Dictionary<System.Type, List<InjectableVariable>>();
    static List<InjectableVariable> _injectsStaticReceptors = new List<InjectableVariable>();

    static List<InjectableVariable> _auxList = null;

    static bool _initialized = false;

    static void Initialize()
    {
        _injectsReceptors.Clear();
        _injectables.Clear();
    }
    public static void ReStartService()
    {
        _initialized = false;
        StartService();
    }

    public static void StartService()
    {
        if (_initialized) return;

        InjectorCore.onNewInjectable -= OnNewInjectableNoEditor;
        InjectorCore.onNewInjectable += OnNewInjectableNoEditor;

        Logx.LogTitle("InjectorCoreEDITOR STARTING!", LogxEnum.InjectorCoreEditor);
        Initialize();
        ColletAllStaticInjectReceptor();
        InjectDependences();
        Logx.LogTitle("InjectorCoreEDITOR STARTING DONE!", LogxEnum.InjectorCoreEditor);
        _initialized = true;
    }

    static void OnNewInjectableNoEditor(Type typex, object obj)
    {
        RegistrerInjectableObject(typex, obj);
    }
    #region Registrer Injectable Objects

    /// <summary>
    /// Registrers the injectable object only if has Injectablex attribute.
    /// </summary>
    /// <param name="typex">Typex.</param>
    /// <param name="obj">Object.</param>
    public static void RegistrerInjectableObject(object obj)
    {
        //[FIX] Nuse, hace tiempo que no entro por aqui. A veces recibe un object null
        if (obj == null) return;
        RegistrerInjectableObject(obj.GetType(), obj);
    }

    public static void RegistrerInjectableObject(System.Type typex, object obj)
    {
        if (_injectables.ContainsKey(typex))
            return;

        if (typex.GetCustomAttributes(typeof(Injectablex), false).Length > 0)
        {
            Log("Registred: [{0}]", typex.Name);
            _injectables.Add(typex, obj);

            InjectDependencesByNewInjectable(typex);
            return;
        }
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
        Log("Registred: {0}", type.Name);

        foreach (InjectableVariable inject in ReflectClass(obj, obj.GetType()))
        {
            GetInjectableReceptor(inject.typeRequired).Add(inject);
            InjectDependency(inject);
        }
    }

    #endregion

    /// <summary>
    /// Injects all injetable objects y all receptors if not injected yet.
    /// </summary>
    public static void InjectDependences()
    {
        object injectableObject = null;
        foreach (List<InjectableVariable> inj in _injectsReceptors.Values)
        {
            if (inj.Count == 0)
                continue;

            if (_injectables.TryGetValue(inj.First().typeRequired, out injectableObject))
            {
                inj/*.Where(i => !i.injected).ToList()*/.ForEach(i =>
                {
                    i.SetValue(injectableObject);
                    Log("Injected: {0} -> [{1}]", i.typeRequired.Name, i.classOwner.Name);
                });
            }
        }
    }

    public static void InjectDependencesByNewInjectable(Type typex)
    {
        object injectableObject = null;
        if (_injectables.TryGetValue(typex, out injectableObject))
        {
            foreach (InjectableVariable inj in _injectsReceptors.Values.SelectMany(s => s.Where(s2 => s2.typeRequired.Equals(typex))/*.Where(s3 => !s3.injected)*/))
            {
                try
                {
                    inj.SetValue(injectableObject);
                    Log("Injected: {0} -> [{1}]", inj.typeRequired.Name, inj.classOwner.Name);
                }
                catch (System.Exception ex) { Debug.LogException(ex); }
            }
        }
    }

    /// <summary>
    /// Injects the dependences on a single objects
    /// </summary>
    /// <param name="inj">Inj.</param>
    static void InjectDependency(InjectableVariable inj)
    {
        object injectableObject = null;
        if (_injectables.TryGetValue(inj.typeRequired, out injectableObject))
        {
            inj.SetValue(injectableObject);
        }
    }

    static List<InjectableVariable> GetInjectableReceptor(System.Type typex)
    {
        _auxList = null;
        if (!_injectsReceptors.TryGetValue(typex, out _auxList))
        {
            _auxList = new List<InjectableVariable>();
            _injectsReceptors.Add(typex, _auxList);
        }
        return _auxList;
    }

    /// <summary>
    /// Collets all static inject receptor.
    /// </summary>
    static void ColletAllStaticInjectReceptor()
    {
        foreach (InjectableVariable inject in ReflectClass(null, Assembly.GetExecutingAssembly().GetTypes()))
        {
            _injectsStaticReceptors.Add(inject);
            GetInjectableReceptor(inject.typeRequired).Add(inject);
            Log("Static receptor collected: [{0}] <- {1}", inject.classOwner.Name, inject.typeRequired.Name);
        }
    }


    /// <summary>
    /// Reflects the class and returns injects Receptors.
    /// </summary>
    /// <returns>The class.</returns>
    /// <param name="obj">Object.</param>
    /// <param name="types">Types.</param>
    static IEnumerable<InjectableVariable> ReflectClass(object obj, params Type[] types)
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public;

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
                if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                {
                    if (!p.CanWrite)
                        continue;

                    if (obj == null && p.GetSetMethod() != null && !p.GetSetMethod().IsStatic)
                        continue;

                    yield return new InjectableProperty(obj, p, types[x]);
                }
            }

            foreach (FieldInfo p in types[x].GetFields(flags))
            {
                if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                {
                    if (obj == null && !p.IsStatic)
                        continue;

                    yield return new InjectableField(obj, p, types[x]);
                }
            }

            if (log != "")
            {
                //              Log (types [x].Name + log);
            }
        }


    }


    #region DEBUG

    static bool DEBUG = true;
    public static void Log(string s, params string[] sparams) { Log(string.Format(s, sparams)); }
    public static void Log(string s)
    {
        if (!DEBUG)
            return;
        Logx.Log(s, LogxEnum.InjectorCoreEditor);
    }

    #endregion
}
