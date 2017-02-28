// Created by | Ramses Di Perna | 08-10-2016

namespace Ramses.StateMachine
{
	public struct StateInfo
	{
		private StateInfoParameter[] parameters;

		public StateInfo(params StateInfoParameter[] parameters)
		{
			this.parameters = parameters;
		}

		public T Get<T>() where T : StateInfoParameter
		{
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i].GetType() == typeof(T))
					{
						return (T)parameters[i];
					}
				}
			}
			return null;
		}
	}


	public abstract class StateInfoParameter
	{

	}
}