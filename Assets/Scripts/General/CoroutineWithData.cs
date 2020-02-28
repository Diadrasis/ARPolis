using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://answers.unity.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html

public class CoroutineWithData
{
    public Coroutine Coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.Coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
    }
}

public class CoroutineWithData<T>
{
    private IEnumerator _target;
    public T result;
    public Coroutine Coroutine { get; private set; }

    public CoroutineWithData(MonoBehaviour owner_, IEnumerator target_)
    {
        _target = target_;
        this.Coroutine = owner_.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            result = (T)_target.Current;
            yield return result;
        }
    }

}
