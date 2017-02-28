// Created by | Ramses Di Perna | 02-10-2016

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// The behaviourStateInfo holds all the StateInfoParts. 
	/// This struct is used by the BehaviourStateHandler
	/// </summary>
	public struct BehaviourStateInfo
	{
		private IStateInfoPart[] stateInfoParts;

		public BehaviourStateInfo(params IStateInfoPart[] infoParts)
		{
			stateInfoParts = infoParts;
		}

		public T? Get<T>() where T : struct, IStateInfoPart
		{
			if (stateInfoParts != null)
			{
				for (int i = 0; i < stateInfoParts.Length; i++)
				{
					if (stateInfoParts[i].GetType() == typeof(T))
					{
						return (T)stateInfoParts[i];
					}
				}
			}
			return null;
		}
	}
}