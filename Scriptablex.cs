using UnityEngine;

[System.Serializable]
public class Scriptablex : ScriptableObject
{

    public void Awake()
    {
        //#if !UNITY_EDITOR
        InjectorCore.RegistrerInjectableObject(GetType(), this);
        InjectorCore.RegistrerInjectableReceptorsInObject(this);
        //#endif
        DoAwake();
    }

    protected virtual void DoAwake()
    {
    }
}
