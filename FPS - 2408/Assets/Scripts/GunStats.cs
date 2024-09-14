using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    public Quaternion gunRotation;
    public int shootDamage;
    public int minDamage, maxDamage;
    public float shootRate;
    public int shootDist;
    public int maxShots;
    public float shootCooldown;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSounds;
    public float shootVolume = .5f;
}