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
        InjectorCore.RegistrerInjectableObject(GetType(), this);
        InjectorCore.RegistrerInjectableReceptorsInObject(this);
    }
    protected virtual void DoAwake() { }

    protected virtual void OnDestroy()
    {
        InjectorCore.RemoveInjectableObject(GetType());
        InjectorCore.RemoveInjectableReceptorsInObject(this);
        DoDestroy();
    }

    protected virtual void DoDestroy() { }
}
