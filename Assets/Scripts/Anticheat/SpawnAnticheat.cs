using UnityEngine;

public class SpawnAnticheat : MonoBehaviour
{
    public GameObject anticheatPrefab;
    public Transform spawnPoint;
    [SerializeField] private AudioClip spawnSFX;
    [SerializeField] private float volume = 0.2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameObject.FindWithTag("Anticheat")==null) 
            {
                PathRecord.recordedSnapshots.Clear();
                Instantiate(anticheatPrefab, spawnPoint.position, Quaternion.identity);
                AudioManager.instance.PlaySFX(spawnSFX, volume);
            }
        }
    }
}
