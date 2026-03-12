using UnityEngine;

public class IgniteWood : MonoBehaviour
{
    public ParticleSystem fire;

    void Start()
    {
        fire = GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "CampfireASmallEmissive")
        {
            fire.Play();
        }
    }
}