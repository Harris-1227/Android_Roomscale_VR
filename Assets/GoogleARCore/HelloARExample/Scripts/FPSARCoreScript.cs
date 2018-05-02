using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSARCoreScript : MonoBehaviour {

	[SerializeField]
    private float m_updateInterval = 0.5f;

    private float m_accum;
    private int m_frames;
    private float m_timeleft;
    public float FPSstring;
	
	// Update is called once per frame
	void Update () 
	{
		m_timeleft -= Time.deltaTime;
		m_accum += Time.timeScale / Time.deltaTime;
		m_frames++;

		if ( 0 < m_timeleft ) return;

		FPSstring = m_accum / m_frames;
		m_timeleft = m_updateInterval;
		m_accum = 0;
		m_frames = 0;
	}
}
