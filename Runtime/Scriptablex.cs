using UnityEngine;

[System.Serializable]
public class Scriptablex : ScriptableObject
{

    public void Awake()
    {
        //#if !UNITY_EDITOR
        ExInjector.RegistrerInjectableObject(GetType(), this);
        ExInjector.RegistrerInjectableReceptorsInObject(this);
        //#endif
        DoAwake();
    }

    protected virtual void DoAwake()
    {
    }
}
