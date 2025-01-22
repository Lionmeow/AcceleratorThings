using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AcceleratorThings
{
    [RegisterTypeInIl2Cpp]
    public class DoubleAccelerator : SRBehaviour
    {
        public DoubleAccelerator(IntPtr ptr) : base(ptr) { }

        private Accelerator accelOne;
        private Accelerator accelTwo;

        private bool swapping = false;

        public void Awake()
        {
            Accelerator[] accels = transform.parent.GetComponentsInChildren<Accelerator>();
            accelOne = accels[0];
            accelTwo = accels[1];
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null || Vacuumable.TryGetVacuumable(other.gameObject, out _))
                return;

            if (!(swapping ? accelTwo : accelOne).CanLaunchObject(other.gameObject))
                return;

            (swapping ? accelTwo : accelOne).OnTriggerEnter(other);
            swapping = !swapping;
        }
    }
}
