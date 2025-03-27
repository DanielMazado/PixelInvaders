using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct AudioEntry
{
    public string name;       // Nombre del audio
    public AudioClip clip;    // Clip de audio asociado
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private List<AudioEntry> audioEntries; // Lista configurable desde el inspector.

    private Dictionary<string, AudioClip> audioClips; // Diccionario para buscar clips rápidamente.
    [SerializeField] private AudioSource audioSource; // AudioSource dedicado a los sonidos del juego.
    [SerializeField] private AudioSource musicSource; // AudioSource dedicado a la música de fondo.

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        musicSource.loop = true; // Aseguramos que la música pueda loopearse.
        musicSource.playOnAwake = false;

        // Convertir la lista a diccionario para búsquedas rápidas
        audioClips = new Dictionary<string, AudioClip>();
        foreach (var entry in audioEntries)
        {
            if (!audioClips.ContainsKey(entry.name))
                audioClips[entry.name] = entry.clip;
            else
                Debug.LogWarning($"AudioManager: El nombre '{entry.name}' está duplicado en los audio entries.");
        }
    }

    private void Start() 
    {
        if(SceneManager.GetActiveScene().name == "Menu") 
        {
            StopBackgroundMusic();
            PlayBackgroundMusic("Menu");
        }
        else if(SceneManager.GetActiveScene().name == "Level1") 
        {
            StopBackgroundMusic();
            PlayBackgroundMusic("Gameplay");
        }
    }

    // Método para reproducir un sonido basado en su nombre.
    public void PlaySound(string clipName, float delay = 0.0f)
    {
        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            if(delay != 0.0f) 
            {
                StartCoroutine(WaitAndPlaySound(clip, delay));
            } 
            else 
            {
                audioSource.PlayOneShot(clip);
            }
            
        }
        else
        {
            Debug.LogWarning($"AudioManager: No se encontró un clip con el nombre '{clipName}'.");
        }
    }

    private IEnumerator WaitAndPlaySound(AudioClip clip, float delay) 
    {
        yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(clip);
    }

    // Método para comprobar si se reproduce la música de fondo.

    public bool isBackgroundMusicPlaying() 
    {
        return musicSource.isPlaying;
    }
    
    // Método para iniciar la música de fondo.
    public void PlayBackgroundMusic(string musicName)
    {
        if (audioClips.TryGetValue(musicName, out AudioClip musicClip))
        {
            if (musicSource.clip != musicClip || !musicSource.isPlaying)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"AudioManager: No se encontró un clip de música con el nombre '{musicName}'.");
        }
    }

    // Método para detener la música de fondo.
    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.clip = null; // Limpia el clip para evitar reinicios accidentales.
        }
    }

    // Método para pausar la música de fondo.
    public void PauseBackgroundMusic()
    {
        musicSource.Pause();
    }

    // Método para reanudar la música de fondo.
    public void ResumeBackgroundMusic()
    {
        musicSource.UnPause();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
