using UnityEngine;

public class LagSimulator : MonoBehaviour 
{
	public int t = 3000000;

	private void Update() 
	{
		float k = 0;
		for (int i = 0; i < t; i++) 
		{
			k += Mathf.Sqrt(i);
		}
	}
}
