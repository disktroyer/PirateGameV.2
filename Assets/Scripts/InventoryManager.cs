using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Slots")]
    public Image slot1Image;
    public Image slot2Image;
    public TextMeshProUGUI mensajeTMP;

    [Header("Mensajes")]
    public float mensajeDuracion = 2f;
    private bool cercaDeBarriles = false;
    private bool cercaDeCubo = false;
    private bool cercaDeRon = false;



    [Header("Crafteo")]
    public CraftingRecipe[] recetas;
    public CraftingProgressUI craftingUI;
    private Animator animator;


    [Header("Drop Settings")]
    public Transform dropPoint;      // Lugar donde soltar los objetos
    public float dropOffset = 0.5f;  // Por si quieres moverlos un poco

    // --- Inventario Interno ---
    public ItemData[] items = new ItemData[2];

    // --- Crafteo interno ---
    private CraftingRecipe recetaActual = null;
    private bool isCrafting = false;
    private float craftingTimer = 0f;

    void Start()
    {
        ActualizarUI();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Q → soltar slot 0
        if (Input.GetKeyDown(KeyCode.Q))
            DropSlot0();

        //F → mantener para craftear
        if (Input.GetKey(KeyCode.F))
            ProcesarCrafteo();

        if (Input.GetKeyUp(KeyCode.F))
            CancelarCrafteo();


        if (Input.GetKeyDown(KeyCode.R))
        {
            if (cercaDeBarriles)
            {
                if (ContieneItem("fregona"))
                {
                    animator.SetTrigger("FregarTrigger");
                    MostrarMensaje("Limpiando los barriles...");
                }
                else
                {
                    MostrarMensaje("Necesitas la fregona para limpiar esto");
                }
            }

            if (cercaDeCubo)
            {
                if (ContieneItem("anguila electrica"))
                {
                    animator.SetTrigger("AnguilaTrigger");
                    MostrarMensaje("Colocando Anguila...");
                }
                else
                {
                    MostrarMensaje("Necesitas la Anguila para electrocutar la ducha");
                }
            }

            if (cercaDeRon)
            {
                if (ContieneItem("veneno"))
                {
                    animator.SetTrigger("VenenoTrigger");
                    MostrarMensaje("Envenenando Botella...");
                }
                else
                {
                    MostrarMensaje("Necesitas el veneno para colocar la trampa");
                }
            }
        }
    }

    // ================================================================
    // ------------------------ INVENTARIO -----------------------------
    // ================================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Esto imprimirá el nombre de CUALQUIER objeto que toques
        Debug.Log("¡He tocado el objeto: " + collision.gameObject.name + "!");

        if (collision.CompareTag("Barriles"))
        {
            cercaDeBarriles = true;
            MostrarMensaje("Pulsa R para fregar");
        }

        if (collision.CompareTag("CuboDucha"))
        {
            cercaDeCubo = true;
            MostrarMensaje("Pulsa R para Colocar Anguila");
        }

        if (collision.CompareTag("BotellaRon"))
        {
            cercaDeRon = true;
            MostrarMensaje("Pulsa R para Enevenenar Botella");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Barriles"))
        {
            cercaDeBarriles = false;
            LimpiarMensaje();
        }

        if (other.CompareTag("CuboDucha"))
        {
            cercaDeCubo = false;
            LimpiarMensaje();
        }

        if (other.CompareTag("BotellaRon"))
        {
            cercaDeRon = false;
            LimpiarMensaje();
        }
    }
    public bool AddItem(ItemData newItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                ActualizarUI();
                MostrarMensaje("Recogiste " + newItem.itemName);
                return true;
            }
        }

        MostrarMensaje("Inventario lleno");
        return false;
    }

    //public bool ContieneItem(ItemData item)
    //{
    //    foreach (var i in items)
    //    {
    //        if (i == item)
    //            return true;
    //    }
    //    return false;
    //}

    public bool ContieneItem(string itemName)
    {
        foreach (var i in items)
        {
            if (i != null && i.itemName == itemName)
                return true;
        }
        return false;
    }

    void ActualizarUI()
    {
        slot1Image.sprite = items[0] != null ? items[0].icon : null;
        slot1Image.color = items[0] != null ? Color.white : new Color(1, 1, 1, .3f);

        slot2Image.sprite = items[1] != null ? items[1].icon : null;
        slot2Image.color = items[1] != null ? Color.white : new Color(1, 1, 1, .3f);
    }

    // ================================================================
    // ------------------------ DROP OBJETO ----------------------------
    // ================================================================

    public void DropSlot0()
    {
        if (items[0] == null)
        {
            MostrarMensaje("Nada que soltar");
            Debug.Log("[Drop] No hay item en el slot 0");
            return;
        }

        ItemData item = items[0];

        // ⬇⬇⬇ AQUI CAMBIAMOS: soltar donde está el jugador ⬇⬇⬇
        Vector3 dropPos = transform.position;     // ← posición del objeto con InventoryManager
        dropPos.z = 0f;                            // opcional para evitar z raro

        if (item.prefab != null)
        {
            Instantiate(item.prefab, dropPos, Quaternion.identity);
            Debug.Log($"[Drop] Soltado {item.itemName} en {dropPos}");
        }
        else
        {
            Debug.LogWarning($"[Drop] El item {item.itemName} no tiene prefab");
        }

        // Mover slot 1 → slot 0
        items[0] = items[1];
        items[1] = null;

        ActualizarUI();
        MostrarMensaje($"Soltaste {item.itemName}");
    }


    // ================================================================
    // ------------------------ CRAFTEO --------------------------------
    // ================================================================

    void ProcesarCrafteo()
    {
        if (!isCrafting)
        {
            recetaActual = BuscarRecetaValida();

            if (recetaActual == null)
            {
                MostrarMensaje("No puedes craftear nada");
                animator.SetBool("IsCrafting", false);

                return;
            }
            
            isCrafting = true;
            animator.SetBool("IsCrafting", true);

            craftingTimer = 0f;

            if (craftingUI != null)
                craftingUI.Show();
        }

        craftingTimer += Time.deltaTime;

        if (craftingUI != null)
            craftingUI.SetProgress(craftingTimer / recetaActual.craftingTime);

        if (craftingTimer >= recetaActual.craftingTime)
            CompletarCrafteo();
    }

    CraftingRecipe BuscarRecetaValida()
    {
        foreach (var receta in recetas)
        {
            if (TieneIngredientes(receta))
                return receta;
        }
        return null;
    }

    bool TieneIngredientes(CraftingRecipe receta)
    {
        foreach (var ingrediente in receta.ingredientes)
        {
            if (!ContieneItem(ingrediente.itemName))
                return false;
        }
        return true;
    }

    void CompletarCrafteo() 
    {
        MostrarMensaje("Fabricaste " + recetaActual.resultado.itemName);

        // Eliminar ingredientes
        foreach (var ingrediente in recetaActual.ingredientes)
        {
            EliminarItem(ingrediente.itemName);
        }

        // Añadir resultado
        AddItem(recetaActual.resultado);

        CancelarCrafteo();
    }

    void CancelarCrafteo()
    {
        isCrafting = false;
        recetaActual = null;
        craftingTimer = 0f;

        if (craftingUI != null)
            craftingUI.Hide();

        animator.SetBool("IsCrafting", false);

    }

    public void EliminarItem(string itemName)
    {
        if (items[0] != null && items[0].itemName == itemName)
        {
            items[0] = items[1];
            items[1] = null;
            ActualizarUI();
            return;
        }

        if (items[1] != null && items[1].itemName == itemName)
        {
            items[1] = null;
            ActualizarUI();
        }
    }

    // ================================================================
    // ------------------------ MENSAJES -------------------------------
    // ================================================================

    private void MostrarMensaje(string msg)
    {
        if (mensajeTMP == null) return;

        mensajeTMP.text = msg;
        CancelInvoke(nameof(LimpiarMensaje));
        Invoke(nameof(LimpiarMensaje), mensajeDuracion);
    }

    void LimpiarMensaje()
    {
        if (mensajeTMP != null)
            mensajeTMP.text = "";
    }
}
