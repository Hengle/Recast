using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
	public interface IActor
	{
		void Move();

		void StopMove();

		void SetPosition(Vector3 position);

		void FaceTo(Vector3 dir);

		
		void CastSkill();

		bool isAlived{ get; set; }
	}
}

