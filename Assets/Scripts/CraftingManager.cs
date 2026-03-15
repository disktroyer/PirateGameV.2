using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventory;

    [Header("UI")]
    public GameObject craftingBarUI;
    public Slider craftingSlider;
    private Animator animator;


    [Header("Config (fallback)")]
    public float craftingTime = 2f; // tiempo por defecto si la receta no define uno

    private float craftingTimer = 0f;
    public bool isCrafting = false;

    [Header("Recetas (puedes asignar aquí o usar las del InventoryManager)")]
    public CraftingRecipe[] recipes;  // lista de recetas

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            TryCraft();
        }
        else if (isCrafting && Input.GetKeyUp(KeyCode.F))
        {
            CancelCraft();
        }
    }

    void TryCraft()
    {


        if (inventory == null) return;

        CraftingRecipe recipe = GetValidRecipe();
        if (recipe == null)
        {
            Debug.Log("No se encontró receta. ¿Faltan ingredientes?");
            animator.SetBool("IsCrafting", false);
            return;
        }

        isCrafting = true;
        animator.SetBool("IsCrafting", true);
        Debug.Log("animCrafteo");

        craftingTimer += Time.deltaTime;

        float targetTime = recipe != null ? recipe.craftingTime : craftingTime;

        if (craftingBarUI != null)
            craftingBarUI.SetActive(true);
        if (craftingSlider != null)
            craftingSlider.value = Mathf.Clamp01(craftingTimer / targetTime);

        if (craftingTimer >= targetTime)
        {
            // Consumir ingredientes (usando el método existente en InventoryManager)
            if (recipe.ingredientes != null)
            {
                foreach (var ing in recipe.ingredientes)
                {
                    if (ing != null && !string.IsNullOrEmpty(ing.itemName))
                        inventory.EliminarItem(ing.itemName);
                }
            }

            // Crear resultado
            if (recipe.resultado != null)
            {
                inventory.AddItem(recipe.resultado);
            }

            StopCraft();
        }
    }

    void StopCraft()
    {
        craftingTimer = 0f;
        if (craftingSlider != null) craftingSlider.value = 0f;
        if (craftingBarUI != null) craftingBarUI.SetActive(false);
        isCrafting = false;

        animator.SetBool("IsCrafting", false);
    }

    void CancelCraft()
    {
        StopCraft();
    }

    CraftingRecipe GetValidRecipe()
    {
        var source = (recipes != null && recipes.Length > 0) ? recipes : inventory.recetas;
        if (source == null) return null;

        foreach (var r in source)
        {
            if (r == null) continue;
            bool allOk = true;

            if (r.ingredientes == null || r.ingredientes.Length == 0)
                continue;

            foreach (var ing in r.ingredientes)
            {
                if (ing == null || !inventory.ContieneItem(ing.itemName))
                {
                    allOk = false;
                    break;
                }
            }

            if (allOk) return r;
        }

        return null;
    }
}
