using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HandPinchLocomotionBB : MonoBehaviour
{
    [SerializeField, Interface(typeof(IHand))]
    private UnityEngine.Object _rightHand;
    public IHand RightHand;

    public Transform handDirection; 
    public float moveSpeed = 1.5f;
    public bool invert = false;

    protected virtual void Awake()
    {
        RightHand = _rightHand as IHand;
    }

    void Update()
    {
        if (RightHand == null || handDirection == null) return;

        // Vérifie si la main pince (Pouce + Index)
        if (RightHand.GetFingerIsPinching(HandFinger.Index))
        {
            float yaw = handDirection.eulerAngles.y;
            Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;

            if (invert) forward = -forward;

            transform.position += forward * moveSpeed * Time.deltaTime;

            Debug.DrawRay(transform.position, forward, Color.cyan, 0.1f);
        }
    }
}