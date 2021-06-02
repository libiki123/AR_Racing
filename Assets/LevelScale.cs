using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScale : MonoBehaviour
{
    public void ScaleUpdate(float value)
	{
		transform.localScale = new Vector3(value, value, value);
	}
}
