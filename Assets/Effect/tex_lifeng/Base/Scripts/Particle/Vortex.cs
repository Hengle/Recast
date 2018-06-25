using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//作者微信公众号：特效基地
//本脚本只能作用于粒子系统！
//Jason 20170422
[AddComponentMenu("Base/Particle/Vortex")]
[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class Vortex : MonoBehaviour
{
	
	public Vector3 direction = Vector3.forward;
	public float strength = 1.0f;

	public bool useCurve = false;
	public AnimationCurve vortexOverLifetime;

	ParticleSystem m_ParticleSystem;
	ParticleSystem.Particle[] m_Particles;

	void Start()
	{
		m_ParticleSystem = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (m_ParticleSystem.isPlaying && strength != 0 )
		{
			if (m_Particles == null || m_Particles.Length < m_ParticleSystem.particleCount)
			{
				m_Particles = new ParticleSystem.Particle[m_ParticleSystem.particleCount];
			}

			int count = m_ParticleSystem.GetParticles(m_Particles);
			Vector3 direction = this.direction.normalized;
			for (var i = 0; i < count; ++i)
			{
				Vector3 pos = m_Particles[i].position;
				if (pos.sqrMagnitude > 0.0f)
				{
					float radius = pos.magnitude;
					Vector3 dir = pos / radius;
					Vector3 oldV = m_Particles[i].velocity;

					oldV = Vector3.Dot(oldV, direction) * direction;
					Vector3 v = Vector3.Cross(dir, direction).normalized * radius * strength;

					if(useCurve ){
						float lifePercent = 1.0f - m_Particles[i].remainingLifetime / m_Particles[i].startLifetime;
						v *= vortexOverLifetime.Evaluate(lifePercent);
					}

					m_Particles[i].velocity = v + oldV;
				}
			}
			m_ParticleSystem.SetParticles(m_Particles, count);
		}
	}
}
