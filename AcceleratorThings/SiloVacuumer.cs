using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AcceleratorThings
{
    [RegisterTypeInIl2Cpp]
    public class SiloVacuumer : SRBehaviour
    {
        public SiloVacuumer(IntPtr ptr) : base(ptr) { }

        private TrackCollisions tracker;
        private Collider collider;

        private void Awake()
        {
            tracker = GetComponent<TrackCollisions>();
            collider = GetComponent<Collider>();
        }

        public void Update()
        {
            foreach (GameObject g in tracker.CurrColliders())
            {
                SiloCatcher c = g.GetComponent<SiloCatcher>();
                if (c)
                    c.OnTriggerStay(collider);
            }
        }
    }
}
