using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using MelonLoader;
using UnityEngine;

namespace AcceleratorThings
{
    [RegisterTypeInIl2Cpp]
    public class GenericVacuum : SRBehaviour
    {
        public GenericVacuum(IntPtr ptr) : base(ptr) { }

        public GameObject destroyOnVacFX;
        public GameObject vacJointPrefab;

        public Transform vacOrigin;

        public float maxVacDist = 6;
        public float minJointSpeed = 200;
        public float maxJointSpeed = 20;
        public float minSpringStrength = 3;
        public float maxSpringStrength = 50;
        public float accelDist = 2.8f;

        public int layerMask = -1677738245;

        private TrackCollisions tracker;
        private Accelerator accel;

        private List<Joint> joints = new List<Joint>();

        private void Start()
        {
            tracker = GetComponent<TrackCollisions>();
            accel = transform.parent.parent.GetComponentInChildren<Accelerator>();
            destroyOnVacFX = EntryPoint.destroyOnVacFX;
            vacJointPrefab = EntryPoint.vacJointPrefab;
            vacOrigin = transform.FindChild("vac point");
        }

        public void Update()
        {
            if (Time.timeScale == 0)
                return;

            ConsumeExistingJointed();
            foreach (GameObject inVac in tracker.CurrColliders())
                ConsumeVacItem(inVac);
        }

        public void OnDestroy() => ClearVac();

        public void ForceJoint(Vacuumable vacuumable)
        {
            vacuumable.Capture(CreateJoint(vacuumable));
        }

        public void ClearVac()
        {
            foreach (Joint joint in joints)
            {
                if (joint != null && joint.connectedBody != null)
                {
                    Vacuumable vacuumable = joint.connectedBody.GetComponent<Vacuumable>();
                    if (vacuumable)
                        vacuumable.Release();
                }
            }
        }

        public void ConsumeVacItem(GameObject vacItem)
        {
            Vacuumable vacuumable = vacItem.GetComponent<Vacuumable>();
            IdentifiableActor ident = vacItem.GetComponent<IdentifiableActor>();

            if (vacuumable && vacuumable.enabled)
            {
                if (Physics.Raycast(vacOrigin.position, vacItem.transform.position - vacOrigin.position, out RaycastHit hitInfo, maxVacDist, layerMask))
                {
                    if (hitInfo.rigidbody != null && hitInfo.rigidbody.gameObject == vacItem)
                    {
                        if (vacuumable.GetDestroyOnVac())
                        {
                            FXHelpers.SpawnAndPlayFX(destroyOnVacFX, vacItem.transform.position, vacItem.transform.rotation);
                            if (ident == null)
                                Destroyer.Destroy(vacItem, "GenericVacuum.ConsumeVacItem#1");
                            else
                                Destroyer.DestroyActor(vacItem, "GenericVacuum.ConsumeVacItem#2");
                        }
                        else
                        {
                            Rigidbody rb = vacItem.GetComponent<Rigidbody>();

                            if (vacuumable.IsCaptive() && vacuumable.IsTornadoed())
                                vacuumable.Release();

                            if (!vacuumable.IsCaptive())
                            {
                                if (rb.isKinematic)
                                    vacuumable.Pending = true;
                                else
                                    vacuumable.Capture(CreateJoint(vacuumable));
                            }
                        }
                    }
                }

                if (vacuumable.IsCaptive() && Vector3.Distance(vacOrigin.position, vacItem.transform.position) <= accelDist)
                {
                    vacuumable.Release();
                    accel.LaunchObject(vacuumable.GetComponent<Rigidbody>());
                }
            }
        }

        private void ConsumeExistingJointed()
        {
            joints.RemoveAll(x => x == null);
            foreach (Joint joint in joints)
            {
                SpringJoint springJoint = joint.Cast<SpringJoint>();
                if (springJoint.connectedBody && !joint.connectedBody.isKinematic)
                {
                    float magnitude = springJoint.transform.localPosition.magnitude;
                    float t = magnitude / maxVacDist;

                    if (magnitude > 0)
                        springJoint.transform.localPosition =
                            Mathf.Max(0.0f, magnitude - Mathf.Lerp(maxJointSpeed, minJointSpeed, t) * Time.deltaTime) / magnitude * springJoint.transform.localPosition;
                    //else accel.LaunchObject(springJoint.connectedBody);

                    springJoint.spring = Mathf.Lerp(maxSpringStrength, minSpringStrength, t);
                }
            }
        }

        private Joint CreateJoint(Vacuumable vacuumable)
        {
            GameObject jointPre = Instantiate(vacJointPrefab);
            jointPre.transform.position = vacuumable.transform.position;
            jointPre.transform.SetParent(vacOrigin);

            SpringJoint joint = jointPre.GetComponent<SpringJoint>();

            joint.spring = minSpringStrength;
            joints.Add(joint);

            return joint;
        }
    }
}
