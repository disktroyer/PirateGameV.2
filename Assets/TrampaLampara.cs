using UnityEngine;
using System.Collections;

public class TrampaLampara : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform sombraLamparaTransform;
    [SerializeField] private SpriteRenderer sombraLamparaSprite;
    [SerializeField] private Transform lamparaTechoTransform;
    [SerializeField] private SpriteRenderer lamparaTechoSprite;

    [Header("Secuencia")]
    [SerializeField] private float sombraEscalaFinalMultiplicador = 0.5f;
    [SerializeField] private float tiempoReducirSombra = 0.35f;
    [SerializeField] private float alturaInicialLampara = 3f;
    [SerializeField] private float tiempoCaidaLampara = 0.5f;
    [SerializeField] private bool sombraVisibleAlInicio = true;

    [Header("Efecto de Fuego al Impacto")]
    [Tooltip("ParticleSystem de fuego/chispas. Se activa al tocar el suelo.")]
    [SerializeField] private ParticleSystem particulasFuego;
    [Tooltip("Si está activo, copia Sorting Layer y Order in Layer de la lámpara para que las partículas se vean por delante en 2D.")]
    [SerializeField] private bool sincronizarSortingParticulasConLampara = true;
    [Tooltip("Suma adicional al Order in Layer al sincronizar partículas.")]
    [SerializeField] private int offsetOrdenParticulas = 2;
    [Tooltip("GameObject con el Animator de flames.gif. Se activa al impacto y se escala automáticamente.")]
    [SerializeField] private GameObject flamesAnimado;
    [Tooltip("Escala del sprite de llamas sobre la lámpara (se puede agrandar para compensar que es pequeño).")]
    [SerializeField] private Vector3 flamesEscala = new Vector3(3f, 3f, 1f);
    [Tooltip("Cuántos segundos permanecen las llamas visibles tras el impacto.")]
    [SerializeField] private float tiempoLlamas = 1.2f;
    [Tooltip("La lámpara se tiñe de negro en este tiempo (segundos) tras el impacto.")]
    [SerializeField] private float tiempoOscurecerLampara = 0.6f;
    [Tooltip("Sonido de fuego/impacto al caer la lámpara.")]
    [SerializeField] private AudioSource audioFuego;
    [Tooltip("Clip de audio. Si audioFuego ya tiene un clip asignado en el Inspector, deja esto vacío.")]
    [SerializeField] private AudioClip clipImpacto;

    [Header("Trigger")]
    [SerializeField] private bool activarConEnemy = true;
    [SerializeField] private bool activarSoloConJefe = true;
    [SerializeField] private string enemyTag = "Enemy";

    private bool secuenciaActiva;
    private Vector3 sombraEscalaNormal = Vector3.one;
    private Vector3 sombraPosicionObjetivo;
    private BossController trappedBoss; // cachear jefe durante stun

    [Header("Daño al Jefe")]
    [SerializeField] private float tiempoStunLampara = 2f;
    [SerializeField] private float corazonesAPerdidos = 3f;
    [SerializeField] private string animacionArdiendo = "Burn"; // nombre del trigger en Animator

    void Start()
    {
        if (sombraLamparaTransform == null && sombraLamparaSprite != null)
            sombraLamparaTransform = sombraLamparaSprite.transform;

        if (lamparaTechoTransform == null && lamparaTechoSprite != null)
            lamparaTechoTransform = lamparaTechoSprite.transform;

        if (sombraLamparaSprite == null && sombraLamparaTransform != null)
            sombraLamparaSprite = sombraLamparaTransform.GetComponent<SpriteRenderer>();

        if (lamparaTechoSprite == null && lamparaTechoTransform != null)
            lamparaTechoSprite = lamparaTechoTransform.GetComponent<SpriteRenderer>();

        if (sombraLamparaTransform != null)
        {
            sombraEscalaNormal = sombraLamparaTransform.localScale;
            sombraPosicionObjetivo = sombraLamparaTransform.position;
        }

        if (lamparaTechoSprite != null)
            lamparaTechoSprite.enabled = false;

        if (sombraLamparaSprite != null)
            sombraLamparaSprite.enabled = sombraVisibleAlInicio;

        // Auto-asignación opcional si no se enlazó manualmente en Inspector
        if (particulasFuego == null)
            particulasFuego = GetComponentInChildren<ParticleSystem>(true);

        if (audioFuego == null)
            audioFuego = GetComponent<AudioSource>();

        // Ocultar llamas al inicio
        if (flamesAnimado != null)
            flamesAnimado.SetActive(false);

        // Asegurarse de que las partículas no estén emitiendo
        if (particulasFuego != null)
        {
            var particulasGO = particulasFuego.gameObject;
            if (!particulasGO.activeSelf)
                particulasGO.SetActive(true);

            ConfigurarSortingParticulas();

            particulasFuego.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particulasFuego.Clear(true);
        }
    }

    private void ConfigurarSortingParticulas()
    {
        if (!sincronizarSortingParticulasConLampara || particulasFuego == null || lamparaTechoSprite == null)
            return;

        ParticleSystemRenderer psRenderer = particulasFuego.GetComponent<ParticleSystemRenderer>();
        if (psRenderer == null)
            return;

        psRenderer.sortingLayerID = lamparaTechoSprite.sortingLayerID;
        psRenderer.sortingOrder = lamparaTechoSprite.sortingOrder + offsetOrdenParticulas;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activarConEnemy)
            return;

        bool esJefe = other.GetComponent<BossController>() != null;
        bool tagValida = other.CompareTag(enemyTag);

        if (activarSoloConJefe)
        {
            if (!esJefe)
                return;
        }
        else if (!esJefe && !tagValida)
        {
            return;
        }

        if (esJefe || tagValida)
        {
            // cachear jefe para luego aplicarle daño
            trappedBoss = other.GetComponent<BossController>();
            ActivarTrampa();
        }
    }

    public void ActivarTrampa()
    {
        if (secuenciaActiva)
            return;

        StartCoroutine(SecuenciaLampara());
    }

    private IEnumerator SecuenciaLampara()
    {
        secuenciaActiva = true;

        if (sombraLamparaTransform != null)
        {
            if (sombraLamparaSprite != null)
                sombraLamparaSprite.enabled = true;

            Vector3 escalaInicio = sombraLamparaTransform.localScale;
            Vector3 escalaFinal = sombraEscalaNormal * Mathf.Max(0.05f, sombraEscalaFinalMultiplicador);

            float t = 0f;
            while (t < tiempoReducirSombra)
            {
                t += Time.deltaTime;
                float factor = Mathf.Clamp01(t / Mathf.Max(0.01f, tiempoReducirSombra));
                sombraLamparaTransform.localScale = Vector3.Lerp(
                    escalaInicio,
                    escalaFinal,
                    factor
                );
                yield return null;
            }

            sombraLamparaTransform.localScale = escalaFinal;
        }

        if (lamparaTechoTransform != null)
        {
            Vector3 inicioCaida = sombraPosicionObjetivo + Vector3.up * alturaInicialLampara;
            Vector3 finCaida = sombraPosicionObjetivo;

            lamparaTechoTransform.position = inicioCaida;

            if (lamparaTechoSprite != null)
                lamparaTechoSprite.enabled = true;

            float t = 0f;
            while (t < tiempoCaidaLampara)
            {
                t += Time.deltaTime;
                float factor = Mathf.Clamp01(t / Mathf.Max(0.01f, tiempoCaidaLampara));
                float ease = factor * factor;
                lamparaTechoTransform.position = Vector3.Lerp(inicioCaida, finCaida, ease);
                yield return null;
            }

            lamparaTechoTransform.position = finCaida;
        }

        // ── IMPACTO: fuego, sonido y oscurecer lámpara ──────────────────────

        // 1. Reproducir sonido de impacto
        if (audioFuego != null)
        {
            if (clipImpacto != null)
                audioFuego.PlayOneShot(clipImpacto);
            else
                audioFuego.Play();
        }

        // 2. Activar sistema de partículas
        if (particulasFuego != null)
        {
            if (!particulasFuego.gameObject.activeSelf)
                particulasFuego.gameObject.SetActive(true);

            particulasFuego.transform.position = sombraPosicionObjetivo;

            ConfigurarSortingParticulas();

            particulasFuego.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particulasFuego.Clear(true);
            particulasFuego.Play();
        }

        // 3. Activar sprite/animación de llamas y escalar para compensar tamaño pequeño
        if (flamesAnimado != null)
        {
            flamesAnimado.transform.position = sombraPosicionObjetivo;
            flamesAnimado.transform.localScale = flamesEscala;
            flamesAnimado.SetActive(true);
        }

        // 4. Oscurecer lámpara de su color actual a negro, mientras las llamas la tapan
        if (lamparaTechoSprite != null && tiempoOscurecerLampara > 0f)
        {
            Color colorInicio = lamparaTechoSprite.color;
            Color colorNegro = Color.black;
            float t = 0f;
            while (t < tiempoOscurecerLampara)
            {
                t += Time.deltaTime;
                float factor = Mathf.Clamp01(t / tiempoOscurecerLampara);
                lamparaTechoSprite.color = Color.Lerp(colorInicio, colorNegro, factor);
                yield return null;
            }
            lamparaTechoSprite.color = colorNegro;
        }

        // 5. Esperar a que terminen las llamas y luego apagarlas
        float tiempoEsperaLlamas = Mathf.Max(0f, tiempoLlamas - tiempoOscurecerLampara);
        if (tiempoEsperaLlamas > 0f)
            yield return new WaitForSeconds(tiempoEsperaLlamas);

        if (flamesAnimado != null)
            flamesAnimado.SetActive(false);

        if (particulasFuego != null)
            particulasFuego.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 6. APLICAR DAÑO Y STUN AL JEFE ─────────────────────────────────
        if (trappedBoss != null)
        {
            // Restar 3 corazones (cada corazón = 20 de vida si maxHealth=100)
            float damageAmount = (trappedBoss.maxHealth / 5f) * corazonesAPerdidos;
            trappedBoss.RecibirDaño(damageAmount);

            // Reproducir animación de ardiendo
            if (trappedBoss.animator != null && !string.IsNullOrEmpty(animacionArdiendo))
                trappedBoss.animator.SetTrigger(animacionArdiendo);

            // Stunear al jefe (queda quiero y sin poder hacer nada)
            trappedBoss.Trap_Stun(tiempoStunLampara);

            trappedBoss = null; // limpiar referencia
        }

        secuenciaActiva = false;
    }

}
