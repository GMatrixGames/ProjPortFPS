using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int ammoCurrent, ammoMax;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSounds;
    public float shootVolume = .5f;
}