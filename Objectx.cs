using UnityEngine;

public class Objectx
{
    public Objectx()
    {
        //#if !UNITY_EDITOR
        InjectorCore.RegistrerInjectableObject(GetType(), this);
        InjectorCore.RegistrerInjectableReceptorsInObject(this);
        //#endif
    }


    //protected virtual void DoAwake() { }
}
