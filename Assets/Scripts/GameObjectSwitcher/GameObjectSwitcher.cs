using System.Collections.Generic;
using UnityEngine;

public class GameObjectDropdownSwitcher : MonoBehaviour
{
    [Header("List panel yang akan di-show sesuai index (Show(0) = Show1)")]
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    private void Start()
    {
        // One-way strict → default Show1
        Show(0);
    }

    public void Show(int index)
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogWarning("[DropdownSwitcher] List panel kosong!");
            return;
        }

        if (index < 0 || index >= objects.Count)
        {
            Debug.LogError($"[DropdownSwitcher] Index {index} di luar range list! (Count: {objects.Count})");
            return;
        }

        GameObject activated = null;

        for (int i = 0; i < objects.Count; i++)
        {
            bool active = (i == index);
            objects[i].SetActive(active);

            if (active)
                activated = objects[i];
        }

        if (activated != null)
        {
            Debug.Log($"[DropdownSwitcher] Show → {activated.name} (index: {index}) AKTIF");
        }
        else
        {
            Debug.LogWarning("[DropdownSwitcher] Tidak ada GameObject yang aktif setelah Show()");
        }
    }
}