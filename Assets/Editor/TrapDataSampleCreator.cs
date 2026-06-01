using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TrapDataSampleCreator
{
    private const string itemsFolder = "Assets/ScriptableObjects/Items";
    private const string trapsFolder = "Assets/ScriptableObjects/Traps";
    private const string prefabsFolder = "Assets/Prefabs/Trampas";
    private const string configFolder = "Assets/ScriptableObjects/TrapConfigs";

    [MenuItem("Tools/[KILL THE BOSS] Generar Sistema de Trampas Completo")]
    public static void CreateCompleteSystem()
    {
        CreateFolders();
        CreateAllItemData();
        CreateAllTrapData();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        CreateTrapPrefabs();

        EditorUtility.DisplayDialog("Sistema de Trampas", 
            "✅ Sistema completamente generado en:\n" +
            "- Items: " + itemsFolder + "\n" +
            "- Traps: " + trapsFolder + "\n" +
            "- Configs: " + configFolder + "\n\n" +
            "Abre la escena y arrastra los prefabs de trampas al inspector.",
            "OK");
    }

    private static void CreateFolders()
    {
        foreach (var folder in new[] { itemsFolder, trapsFolder, configFolder })
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.ImportAsset(folder);
            }
        }
    }

    // ============================
    // ITEMS
    // ============================

    private static void CreateAllItemData()
    {
        // Items básicos
        CreateItemDataIfNotExists("Veneno", ItemType.Veneno, "pocion veneno_0");
        CreateItemDataIfNotExists("AnguilaElectrica", ItemType.AnguilaElectrica, "anguila_0");
        CreateItemDataIfNotExists("Fregona", ItemType.Fregona, "fregona_0");
        CreateItemDataIfNotExists("Bala", ItemType.Bala, "bola de cañon_0");
        CreateItemDataIfNotExists("Polvora", ItemType.Polvora, "polvora_0");
        CreateItemDataIfNotExists("PolvoraXXL", ItemType.PolvoraXXL, "polvora xxl_0");
        CreateItemDataIfNotExists("Cuchillo", ItemType.Cuchillo, "cuchillo cristal_0");
        CreateItemDataIfNotExists("Cepo", ItemType.Cepo, "trampa osos_0");
        
        // Items de combinación para cepo
        CreateItemDataIfNotExists("Chatarra", ItemType.Generic, "Chatarra_0");
        CreateItemDataIfNotExists("MandibulaTiburon", ItemType.Generic, "mandibula tiburon_0");
    }

    private static ItemData CreateItemDataIfNotExists(string name, ItemType itemType, string prefabName)
    {
        string path = Path.Combine(itemsFolder, name + ".asset");
        ItemData existingItem = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (existingItem != null)
            return existingItem;

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = name;
        item.itemType = itemType;
        item.icon = null;
        
        // Cargar el prefab asociado si existe
        string prefabPath = Path.Combine(prefabsFolder, prefabName + ".prefab");
        item.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        item.useAmount = 1;
        
        AssetDatabase.CreateAsset(item, path);
        return item;
    }

    // ============================
    // TRAPS
    // ============================

    private static void CreateAllTrapData()
    {
        CreateTrapData_BotellaRon();
        CreateTrapData_CuboDucha();
        CreateTrapData_Barriles();
        CreateTrapData_Canon();
        CreateTrapData_Farolillo();
        CreateTrapData_CepoOsos();
    }

    private static void CreateTrapData_BotellaRon()
    {
        string path = Path.Combine(trapsFolder, "Trap_BotellaRon.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Botella de Ron";
        data.description = "Trampa preparada. Requiere veneno. El boss recibe daño al beber.";
        data.trapType = TrapType.STATIC_PREPARED;
        data.damage = 1f;
        data.reusable = false;
        data.needsPreparation = true;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("Veneno");
        data.consumeItems = true;
        data.animatorTrigger = "Activate";
        data.interactionText = "Usa veneno en la botella";
        data.prepareText = "Botella envenenada";
        data.activeText = "El boss bebe la botella";
        
        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateTrapData_CuboDucha()
    {
        string path = Path.Combine(trapsFolder, "Trap_CuboDucha.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Cubo de Ducha";
        data.description = "Trampa preparada. Requiere anguila eléctrica. Electrocuta al boss.";
        data.trapType = TrapType.STATIC_PREPARED;
        data.damage = 1f;
        data.reusable = false;
        data.needsPreparation = true;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("AnguilaElectrica");
        data.consumeItems = true;
        data.stunDuration = 2f;
        data.animatorTrigger = "Activate";
        data.interactionText = "Coloca la anguila en el cubo";
        data.prepareText = "Cubo cargado";
        data.activeText = "Descarga eléctrica";
        
        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateTrapData_Barriles()
    {
        string path = Path.Combine(trapsFolder, "Trap_Barriles.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Barriles";
        data.description = "Trampa preparada. Requiere fregona. El boss resbala.";
        data.trapType = TrapType.STATIC_PREPARED;
        data.damage = 1f;
        data.reusable = false;
        data.needsPreparation = true;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("Fregona");
        data.consumeItems = true;
        data.stunDuration = 2.5f;
        data.animatorTrigger = "Activate";
        data.interactionText = "Friega los barriles";
        data.prepareText = "Barriles mojados y resbaladizos";
        data.activeText = "El boss resbala";
        
        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateTrapData_Canon()
    {
        string path = Path.Combine(trapsFolder, "Trap_Canon.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Cañón";
        data.description = "Trampa accionable. Bala + Pólvora = daño 1. Bala + Pólvora XXL = daño 2.";
        data.trapType = TrapType.ACTIONABLE;
        data.damage = 1f;
        data.reusable = false;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("Bala");
        data.secondaryItem = LoadItem("Polvora");
        data.consumeItems = true;
        data.animatorTrigger = "Activate";
        data.interactionText = "Dispara el cañón";
        data.activeText = "¡BOOM!";
        data.itemRequirements = new System.Collections.Generic.List<TrapData.TrapItemRequirement>();

        data.itemRequirements.Add(new TrapData.TrapItemRequirement
        {
            label = "Bala + Pólvora XXL (daño 2)",
            primaryItem = LoadItem("Bala"),
            secondaryItem = LoadItem("PolvoraXXL"),
            damage = 2f,
            consumePrimary = true,
            consumeSecondary = true
        });

        data.itemRequirements.Add(new TrapData.TrapItemRequirement
        {
            label = "Bala + Pólvora (daño 1)",
            primaryItem = LoadItem("Bala"),
            secondaryItem = LoadItem("Polvora"),
            damage = 1f,
            consumePrimary = true,
            consumeSecondary = true
        });

        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateTrapData_Farolillo()
    {
        string path = Path.Combine(trapsFolder, "Trap_Farolillo.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Farolillo / Cuerda";
        data.description = "Trampa accionable. Requiere cuchillo. Cae fuego.";
        data.trapType = TrapType.ACTIONABLE;
        data.damage = 1f;
        data.reusable = false;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("Cuchillo");
        data.consumeItems = true;
        data.animatorTrigger = "Activate";
        data.interactionText = "Corta la cuerda";
        data.activeText = "¡Fuego!";
        
        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateTrapData_CepoOsos()
    {
        string path = Path.Combine(trapsFolder, "Trap_CepoOsos.asset");
        if (AssetDatabase.LoadAssetAtPath<TrapData>(path) != null)
            return;

        TrapData data = ScriptableObject.CreateInstance<TrapData>();
        data.trapName = "Cepo para Osos";
        data.description = "Trampa placeable. Combina Chatarra + Mandíbula de Tiburón para crear el cepo.";
        data.trapType = TrapType.PLACEABLE;
        data.damage = 1f;
        data.stunDuration = 0.5f;
        data.reusable = false;
        data.autoActivate = true;
        data.destroyAfterUse = true;
        data.requiredItem = LoadItem("Cepo");
        data.consumeItems = true;
        data.animatorTrigger = "Activate";
        data.interactionText = "Coloca el cepo";
        data.prepareText = "Cepo listo";
        data.activeText = "¡El boss pisa el cepo!";
        
        AssetDatabase.CreateAsset(data, path);
    }

    // ============================
    // PREFABS DE TRAMPAS
    // ============================

    private static void CreateTrapPrefabs()
    {
        Debug.Log("[TrapSystem] Creando prefabs de trampas...");
        CreateTrapPrefabVariant("BotellaRon", "Botella de ron_0", LoadTrap("Trap_BotellaRon"));
        CreateTrapPrefabVariant("CuboDucha", "Cubo de ducha_0", LoadTrap("Trap_CuboDucha"));
        CreateTrapPrefabVariant("Barriles", "Barril_0", LoadTrap("Trap_Barriles"));
        CreateTrapPrefabVariant("Canon", "cañon_0", LoadTrap("Trap_Canon"));
        CreateTrapPrefabVariant("Farolillo", "Farolillo_0", LoadTrap("Trap_Farolillo"));
        CreateTrapPrefabVariant("CepoOsos", "trampa osos_0", LoadTrap("Trap_CepoOsos"));
        Debug.Log("[TrapSystem] Prefabs de trampas generados.");
    }

    private static void CreateTrapPrefabVariant(string name, string sourcePrefabName, TrapData trapData)
    {

        if (trapData == null)
        {
            Debug.LogError($"[TrapSystem] ERROR: TrapData es null para {name}. Verifica que el asset TrapData se creó correctamente en Traps.\nIntentando forzar creación...");
            // Intentar forzar creación
            switch(name)
            {
                case "BotellaRon": CreateTrapData_BotellaRon(); break;
                case "CuboDucha": CreateTrapData_CuboDucha(); break;
                case "Barriles": CreateTrapData_Barriles(); break;
                case "Canon": CreateTrapData_Canon(); break;
                case "Farolillo": CreateTrapData_Farolillo(); break;
                case "CepoOsos": CreateTrapData_CepoOsos(); break;
            }
            trapData = LoadTrap($"Trap_{name}");
        }

        // Defensive: If still null, abort and log error
        if (trapData == null)
        {
            Debug.LogError($"[TrapSystem] ERROR: TrapData sigue siendo null para {name}. Abortando prefab para evitar NullReferenceException.");
            return;
        }

        string prefabPath = Path.Combine(prefabsFolder, sourcePrefabName + ".prefab");
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (sourcePrefab == null)
        {
            Debug.LogError($"[TrapSystem] No se encontró prefab base: {prefabPath}");
            return;
        }

        // Crear instancia temporal para modificar
        GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        if (tempInstance == null)
        {
            Debug.LogError($"[TrapSystem] No se pudo instanciar el prefab base: {prefabPath}");
            return;
        }

        // Agregar TrapInteractable si no existe
        TrapInteractable trapInteractable = tempInstance.GetComponent<TrapInteractable>();
        if (trapInteractable == null)
            trapInteractable = tempInstance.AddComponent<TrapInteractable>();

        trapInteractable.trapData = trapData;
        trapInteractable.debugLogs = true;

        // Agregar BossTriggerTrap para STATIC y PLACEABLE
        if (trapData.trapType == TrapType.STATIC_PREPARED || trapData.trapType == TrapType.PLACEABLE)
        {
            BossTriggerTrap bossTrigger = tempInstance.GetComponent<BossTriggerTrap>();
            if (bossTrigger == null)
                bossTrigger = tempInstance.AddComponent<BossTriggerTrap>();

            bossTrigger.trapInteractable = trapInteractable;
            bossTrigger.bossTag = "Boss";
        }

        // Encontrar componentes útiles
        Animator animator = tempInstance.GetComponent<Animator>();
        if (animator != null)
            trapInteractable.animator = animator;

        // Guardar como prefab
        string newPrefabPath = Path.Combine(configFolder, "Trap_" + name + "_Ready.prefab");
        PrefabUtility.SaveAsPrefabAsset(tempInstance, newPrefabPath);

        // Limpiar
        Object.DestroyImmediate(tempInstance);

        Debug.Log($"✅ Prefab creado: {newPrefabPath}");
    }

    // ============================
    // HELPERS
    // ============================

    private static ItemData LoadItem(string itemName)
    {
        string path = Path.Combine(itemsFolder, itemName + ".asset");
        return AssetDatabase.LoadAssetAtPath<ItemData>(path);
    }

    private static TrapData LoadTrap(string trapName)
    {
        string path = Path.Combine(trapsFolder, trapName + ".asset");
        return AssetDatabase.LoadAssetAtPath<TrapData>(path);
    }
}
