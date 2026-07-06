using UnityEngine;
using WeaponData = Data.WeaponData;

namespace Visual
{
    public class WeaponVisual : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private ParticleSystem fireEffect;
     
        private AudioSource _audioSource;
        private WeaponData _data;

        public void Init(WeaponData data)
        {
            _data = data;
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayFire()
        {
            fireEffect?.Play(withChildren: true);
            _audioSource?.PlayOneShot(_data.fireSound);
        }

        public void PlayReload()
        {
            _audioSource?.PlayOneShot(_data.reloadSound);
        }
        
        public Transform GetFirePoint => firePoint;
    }
}