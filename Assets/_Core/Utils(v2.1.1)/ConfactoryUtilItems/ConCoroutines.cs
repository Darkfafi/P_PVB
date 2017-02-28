using UnityEngine;
using System.Collections;
using Ramses.Confactory;
using Ramses.Confactory.Addons;
using System.Collections.Generic;
using System;

public class ConCoroutines : IConfactory, IConStructUser
{
	private CoroutineObject coroutineObject;
	private const string NAME_OBJECT = "<Coroutine>:";

	private Dictionary<object, CoroutineVisualObject> allRunningRoutines = new Dictionary<object, CoroutineVisualObject>();

	public IConfactory ConStruct(IConfactoryFinder confactoryFinder)
	{
		coroutineObject = ConfactoryTools.CreateConGameObject(NAME_OBJECT).AddComponent<CoroutineObject>();
		UpdateName();
		return null;
	}

	public void ConClear()
	{
		GameObject.Destroy(coroutineObject.gameObject);
	}

	public Coroutine StartCoroutine(IEnumerator method, object context)
	{
		Coroutine coroutine = null;
        if (context != null)
		{
			if (!allRunningRoutines.ContainsKey(context))
			{
				CoroutineVisualObject routineObject = new GameObject().AddComponent<CoroutineVisualObject>();
				allRunningRoutines.Add(context, routineObject);
				routineObject.gameObject.name = method.ToString();
				routineObject.transform.SetParent(coroutineObject.transform);
				coroutine = coroutineObject.StartCoroutine(method);
				routineObject.SetCoroutine(context, coroutine);
			}
			else
			{
				Debug.LogError("There is already a method running on this context");
			}
			UpdateName();
        }
		return coroutine;
    }
	public bool HasContext(object context)
	{
		return allRunningRoutines.ContainsKey(context);
	}
	public void StopContext(object context)
	{
		if(coroutineObject != null && allRunningRoutines.ContainsKey(context))
		{
			coroutineObject.StopCoroutine(allRunningRoutines[context].CoroutineActive);
			GameObject.Destroy(allRunningRoutines[context].gameObject);
			allRunningRoutines.Remove(context);
			UpdateName();
        }
	}

	private void UpdateName()
	{
		coroutineObject.name = NAME_OBJECT + "("+ allRunningRoutines.Count +")";
	}

	internal class CoroutineObject : MonoBehaviour{}
	internal class CoroutineVisualObject : MonoBehaviour
	{
		public object Context { get; private set; }
		public Coroutine CoroutineActive { get; private set; }

		public void SetCoroutine(object context, Coroutine coroutine)
		{
			Context = context;
			CoroutineActive = coroutine;
		}
	}
}
