using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ADreamOfHome
{
    public class AlignAndAllowRotate : AlignWithTargetBody
    {
        public float sidereal = 0;

        public override void UpdateRotation(Vector3 currentDirection, Vector3 targetDirection, float slerpRate, bool usePhysics)
        {
            if (usePhysics)
            {
                Vector3 vector = OWPhysics.FromToAngularVelocity(currentDirection, targetDirection);
                _owRigidbody._rigidbody.angularVelocity = new Vector3(0, 0, sidereal);
                _owRigidbody.AddAngularVelocityChange(vector * slerpRate);
            }
            else
            {
                Quaternion quaternion = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(currentDirection, targetDirection), slerpRate);
                _owRigidbody.GetRigidbody().rotation = quaternion * _owRigidbody.GetRotation();
            }
        }
    }
}
