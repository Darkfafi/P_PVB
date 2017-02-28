// Created by | Ramses Di Perna | 30-09-2016

namespace Ramses.Confactory.Addons
{

	public interface IConStructUser
	{
		/// <summary>
		/// WARNING: Don't call this method!
		/// This Method will be called when the Confactory is called when he is created (Which happens when it is called for the first time)
		/// The IConfactory returned with this method will represent this class when the Confactory requests this class again.
		/// If 'null' or 'this' is given, this class will represent itself.
		/// </summary>
		IConfactory ConStruct(IConfactoryFinder confactoryFinder);
	}
}
