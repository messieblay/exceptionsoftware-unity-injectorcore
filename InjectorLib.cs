using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class InjectableVariable
{
    public object obj = null;
    public Type typeRequired = null;
    public Type classOwner = null;


    //public bool injected = false;


    public InjectableVariable(object obj, Type typeRequired, Type classOwner)
    {
        this.obj = obj;
        this.typeRequired = typeRequired;
        this.classOwner = classOwner;
    }

    public virtual void SetValue(object obj)
    {
        //injected = true;
        Logx.Log("\tInjecting on: " + ToString(), LogxEnum.InjectorCore);
    }

    public override string ToString()
    {
        return typeRequired.Name + " -> " + classOwner.Name;
    }
}

public class InjectableProperty : InjectableVariable
{
    public PropertyInfo variable = null;

    public InjectableProperty(object obj, PropertyInfo variable, Type classOwner) : base(obj, variable.PropertyType, classOwner)
    {
        this.variable = variable;
    }

    public override void SetValue(object val)
    {
        variable.SetValue(obj, val, null);
        //base.SetValue(val);
    }
}

public class InjectableField : InjectableVariable
{
    public FieldInfo variable = null;

    public InjectableField(object obj, FieldInfo variable, Type classOwner) : base(obj, variable.FieldType, classOwner)
    {
        this.variable = variable;
    }

    public override void SetValue(object val)
    {
        variable.SetValue(obj, val);
        //base.SetValue(val);
    }
}

public class InjertorCoreUtils
{


    /// <summary>
    /// Reflects the class and returns injects Receptors.
    /// </summary>
    /// <returns>The class.</returns>
    /// <param name="obj">Object.</param>
    /// <param name="types">Types.</param>
    public static IEnumerable<InjectableVariable> ReflectClass(object obj, params Type[] types)
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
                int c0 = p.GetCustomAttributes(typeof(Injectx), true).Length;
                int c1 = p.GetCustomAttributes(typeof(Injectx), false).Length;

                if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                {
                    //                  Logx.Log ("Prop: " + p.Name, LogOptions.Delayed);
                    //                  log += "Prop: " + p.Name + "\n";
                    //Log(types[x].FullName + "-> Reflecting property: " + p.Name);
                    if (!p.CanWrite)
                        continue;

                    if (obj == null && p.GetSetMethod() != null && !p.GetSetMethod().IsStatic)
                        continue;

                    yield return new InjectableProperty(obj, p, types[x]);
                }
            }

            foreach (FieldInfo p in types[x].GetFields(flags))
            {
                int c0 = p.GetCustomAttributes(typeof(Injectx), true).Length;
                int c1 = p.GetCustomAttributes(typeof(Injectx), false).Length;

                if (p.GetCustomAttributes(typeof(Injectx), true).Length > 0)
                {
                    //                  Logx.Log ("Field: " + p.Name, LogOptions.Delayed);
                    //                  log += "Field: " + p.Name + "\n";
                    //Log(types[x].FullName + "-> Reflecting field: " + p.Name);
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

}
