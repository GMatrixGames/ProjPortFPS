using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public string guid;
    public GameObject gunModel;
    public Quaternion gunRotation;
    public int minDamage, maxDamage;
    public float shootRate;
    public int shootDist;
    public int maxShots;
    public float shootCooldown;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSounds;
    public AudioClip hitSound;
    public float shootVolume = .5f;
    public bool displayHeat = true;
    public bool hasDropoff = true;

    public float shotCount;
}