using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using MelonLoader;
using UnityEngine;

namespace AcceleratorThings
{
    [RegisterTypeInIl2Cpp]
    public class PlayerAccelerator : SRBehaviour
    {
        public PlayerAccelerator(IntPtr ptr) : base(ptr) { }

        public static SECTR_AudioCue launchedCue;

        public double timeBetweenLaunches = 1;

        private double nextLaunch = 0.0;
        private Collider currPlayerCol;

        public void Update()
        {
            if (nextLaunch > 0.0)
            {
                nextLaunch -= Time.deltaTime;
                if (nextLaunch <= 0.0)
                    Physics.IgnoreCollision(currPlayerCol, GetComponentInParent<MeshCollider>(), false);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (nextLaunch > 0.0)
                return;

            IdentifiableActor ident = other.GetComponent<IdentifiableActor>();
            if (ident == null || !ident.identType.IsPlayer)
                return;

            if (currPlayerCol)
                Physics.IgnoreCollision(currPlayerCol, GetComponentInParent<MeshCollider>(), false);
            currPlayerCol = other;
            Physics.IgnoreCollision(other, GetComponentInParent<MeshCollider>());

            ident.GetComponent<SRCharacterController>().BaseVelocity = transform.forward * 600;
            nextLaunch = timeBetweenLaunches;

            SECTR_AudioSystem.Play(launchedCue, transform.position, false);
        }
    }
}
