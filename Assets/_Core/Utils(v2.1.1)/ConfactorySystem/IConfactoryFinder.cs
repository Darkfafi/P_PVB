// Created by | Ramses Di Perna | 27-03-2016

using UnityEngine;
using System.Collections;
using System;
namespace Ramses.Confactory
{
	public interface IConfactoryFinder
	{

		T Get<T>() where T : IConfactory;
		void Delete<T>() where T : IConfactory;
    }
}