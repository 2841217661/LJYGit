using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("������Ч");
            AudioManager.Instance.PlaySound(AudioPathGlobals.Bgm1, AudioType.SFX, _playPoint:transform.position);
        }
    }
}
