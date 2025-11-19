using UnityEngine;

public class GameManage : MonoBehaviour
{
    // Singleton
    public static GameManage Instance { get; private set; }

    public GameObject[] uiObjects; // Mảng chứa các UI
    public bool[] actions;         // Mảng hành động, chỉ 1 cái là true
    public AudioSource UiSound;
    public AudioSource EffectSound;

    private void Awake()
    {
        // Kiểm tra singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có 1 instance tồn tại
            return;
        }
        Instance = this;

        // (Optional) Nếu muốn tồn tại giữa các scene
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetActiveByIndex(0);
    }

    public void PlayUiSound(AudioClip clip)
    {
        UiSound.PlayOneShot(clip);
    }

    public void PlayEffectSound(AudioClip clip)
    {
        EffectSound.PlayOneShot(clip);
    }

    void Update()
    {
        UpdateUIByAction();
    }

    void UpdateUIByAction()
    {
        if (uiObjects.Length != actions.Length)
        {
            Debug.LogWarning("Mảng uiObjects và actions không cùng kích thước!");
            return;
        }

        for (int i = 0; i < uiObjects.Length; i++)
        {
            if (uiObjects[i] != null)
                uiObjects[i].SetActive(actions[i]);
        }
    }

    public void SetActiveByIndex(int index)
    {
        if (index < 0 || index >= actions.Length)
        {
            Debug.LogWarning("Index vượt quá giới hạn mảng!");
            return;
        }

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = (i == index);
        }

        UpdateUIByAction();
    }
}
