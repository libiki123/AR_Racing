using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

	public static bool IsPointOverUIObject(this Vector2 pos)
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return false;
		}

		PointerEventData eventPos = new PointerEventData(EventSystem.current);
		eventPos.position = new Vector2(pos.x, pos.y);

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventPos, results);

		return results.Count > 0;
	}
}
