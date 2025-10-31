using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public float x, y, z;
    public int nivel;
    public int vida;
}

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void GuardarPartida()
    {
        PlayerData data = new PlayerData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform t = player.transform;
            data.x = t.position.x;
            data.y = t.position.y;
            data.z = t.position.z;

            // Ejemplo: si tienes un script PlayerStats con nivel y vida
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                data.nivel = stats.nivel;
                data.vida = stats.vida;
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Guardado en: " + path);
    }

    public static bool CargarPartida()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontró partida guardada.");
            return false;
        }

        string json = File.ReadAllText(path);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(data.x, data.y, data.z);

            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.nivel = data.nivel;
                stats.vida = data.vida;
            }
        }

        Debug.Log("✅ Partida cargada correctamente.");
        return true;
    }

    public static void NuevaPartida()
    {
        if (File.Exists(path))
            File.Delete(path);

        Debug.Log("🆕 Nueva partida iniciada. Archivo anterior eliminado.");
    }
}
