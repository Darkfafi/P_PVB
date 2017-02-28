using UnityEngine;
using System.Collections;

public static class IntAndFloatExtensions
{

	public static int Clamp(this int s, int minInt, int maxInt)
	{
		if(s < minInt)
		{
			s = minInt;
		}
		if(s > maxInt)
		{
			s = maxInt;
		}
		return s;
	}

	public static float Clamp(this float s, float minFloat, float maxFloat)
	{
		if (s < minFloat)
		{
			s = minFloat;
		}
		if (s > maxFloat)
		{
			s = maxFloat;
		}
		return s;
	}
}
