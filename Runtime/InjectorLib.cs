using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class ExInjectableVariable
{
    public object obj = null;
    public Type typeRequired = null;
    public Type classOwner = null;

    public ExInjectableVariable(object obj, Type typeRequired, Type classOwner)
    {
        this.obj = obj;
        this.typeRequired = typeRequired;
        this.classOwner = classOwner;
    }

    public virtual void SetValue(object obj)
    {
        //injected = true;
        ExInjector.Log($"\tInjecting on: " + ToString());
    }

    public override string ToString() => $"{typeRequired.Name} -> {classOwner.Name}";
}

public class ExInjectableProperty : ExInjectableVariable
{
    public PropertyInfo variable = null;

    public ExInjectableProperty(object obj, PropertyInfo variable, Type classOwner) : base(obj, variable.PropertyType, classOwner)
    {
        this.variable = variable;
    }

    public override void SetValue(object val)
    {
        variable.SetValue(obj, val, null);
    }
}

public class ExInjectableField : ExInjectableVariable
{
    public FieldInfo variable = null;

    public ExInjectableField(object obj, FieldInfo variable, Type classOwner) : base(obj, variable.FieldType, classOwner)
    {
        this.variable = variable;
    }

    public override void SetValue(object val)
    {
        variable.SetValue(obj, val);
    }
}

public class ExInjertorUtils
{
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
