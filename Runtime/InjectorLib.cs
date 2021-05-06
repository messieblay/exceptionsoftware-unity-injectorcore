using System;
using System.Reflection;

namespace ExceptionSoftware.Injector
{
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


}
