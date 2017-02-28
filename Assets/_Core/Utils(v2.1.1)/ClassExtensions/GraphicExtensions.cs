using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class GraphicExtensions {

	public static Color SetAlpha(this Graphic graphic, float alpha)
	{
        graphic.color = graphic.color.AlphaVersion(alpha);
		return graphic.color;
	}
}
