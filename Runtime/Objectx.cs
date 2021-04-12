using UnityEngine;

public class Objectx
{
    public Objectx()
    {
        //#if !UNITY_EDITOR
        ExInjector.RegistrerInjectableObject(GetType(), this);
        ExInjector.RegistrerInjectableReceptorsInObject(this);
        //#endif
    }


    //protected virtual void DoAwake() { }
}
