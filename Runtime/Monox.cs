using UnityEngine;

[System.Serializable]
public class Monox : MonoBehaviour
{
    public virtual void Awake()
    {
        //#if !UNITY_EDITOR
        RegistrerObject();
        DoAwake();
        //Assem
    }

    protected void RegistrerObject()
    {
        ExInjector.RegistrerInjectableObject(GetType(), this);
        ExInjector.RegistrerInjectableReceptorsInObject(this);
    }
    protected virtual void DoAwake() { }

    protected virtual void OnDestroy()
    {
        ExInjector.RemoveInjectableObject(GetType());
        ExInjector.RemoveInjectableReceptorsInObject(this);
        DoDestroy();
    }

    protected virtual void DoDestroy() { }
}
