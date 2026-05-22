# 🎮 Sistema Modular de Trampas - Kill The Boss

## ✅ Setup Automático

He creado un generador completo que prepara todas las trampas y items en < 1 minuto.

### Paso 1: Generar Assets desde Unity

```
Tools > [KILL THE BOSS] Generar Sistema de Trampas Completo
```

Esto crea automáticamente:
- **Items** (`Assets/ScriptableObjects/Items/`)
  - Veneno, AnguilaElectrica, Fregona, Bala, Polvora, PolvoraXXL, Cuchillo, Cepo
- **TrapData** (`Assets/ScriptableObjects/Traps/`)
  - Trap_BotellaRon, Trap_CuboDucha, Trap_Barriles, Trap_Canon, Trap_Farolillo, Trap_CepoOsos
- **Prefabs Configurados** (`Assets/ScriptableObjects/TrapConfigs/`)
  - Trap_*_Ready.prefab (listos para usar en la escena)

---

## 🎯 Cómo Montar en la Escena

### Opción A: Usar Prefabs Listos (RECOMENDADO - 30 segundos)

1. Abre tu escena en Unity
2. Ve a `Assets/ScriptableObjects/TrapConfigs/`
3. Arrastra cualquier prefab `Trap_*_Ready.prefab` a la escena
4. ✅ La trampa está lista. El boss la detectará automáticamente.

### Opción B: Configurar desde Cero (Avanzado)

Si quieres montar una trampa manualmente desde los prefabs base:

1. Arrastra un prefab de trampa a la escena (ej: `Assets/Prefabs/Trampas/trampa osos_0.prefab`)
2. Agrega el componente `TrapInteractable`:
   - Inspector > Add Component > `TrapInteractable`
3. Arrastra el `TrapData` correspondiente:
   - Campo `trapData` ← Arrastra `Assets/ScriptableObjects/Traps/Trap_CepoOsos.asset`
4. Agrega `BossTriggerTrap` (si es STATIC_PREPARED o PLACEABLE):
   - Inspector > Add Component > `BossTriggerTrap`
   - Campo `trapInteractable` ← Arrastra el componente `TrapInteractable` del mismo objeto
5. Configura referencias opcionales (Animator, AudioSource, etc.)

---

## 🏹 Ejemplos de Uso

### Botella de Ron (STATIC_PREPARED)

**Flujo:**
1. Jugador recoge `Veneno`
2. Acerca a la `Botella de Ron` → muestra "E para interactuar"
3. Pulsa E → prepara la botella
4. Boss se acerca al trigger → recibe 1 daño
5. Trampa se destruye

**Montaje en escena:**
1. Arrastra `Trap_BotellaRon_Ready.prefab`
2. Listo. Sin configuración adicional.

---

### Cepo para Osos (PLACEABLE)

**Flujo:**
1. Jugador recoge `Cepo` (combina Chatarra + Mandíbula de Tiburón automáticamente)
2. Acerca a cualquier zona del mapa → muestra "E para colocar"
3. Pulsa E → entra en modo colocación
4. Click izquierdo para colocar → el cepo aparece en el mapa
5. Boss pisa el cepo → recibe 1 daño + stun 0.5s
6. Trampa se destruye

**Montaje en escena:**
1. Arrastra `Trap_CepoOsos_Ready.prefab`
2. Asegúrate de que el Cepo esté en el inventario
3. Pulsa E cerca de cualquier zona válida
4. Listo.

---

### Cañón (ACTIONABLE)

**Flujo:**
- **Con Bala + Pólvora**: daño 1
- **Con Bala + Pólvora XXL**: daño 2 (tiene prioridad)

**Montaje en escena:**
1. Arrastra `Trap_Canon_Ready.prefab`
2. Jugador recoge Bala y Pólvora XXL
3. Acerca al Cañón → "E para disparar"
4. Pulsa E → cañón dispara, daño 2, trampa se destruye
5. Si solo tiene Bala + Pólvora → daño 1

---

## 📋 Mapeo de Prefabs → TrapData → Items

| Trampa | Prefab | TrapData | Item Requerido | Tipo |
|--------|--------|----------|---------------|------|
| Botella Ron | `Botella de ron_0` | `Trap_BotellaRon` | Veneno | STATIC_PREPARED |
| Cubo Ducha | `Cubo de ducha_0` | `Trap_CuboDucha` | AnguilaElectrica | STATIC_PREPARED |
| Barriles | `Barril_0` | `Trap_Barriles` | Fregona | STATIC_PREPARED |
| Cañón | `cañon_0` | `Trap_Canon` | Bala + Pólvora/XXL | ACTIONABLE |
| Farolillo | `Farolillo_0` | `Trap_Farolillo` | Cuchillo | ACTIONABLE |
| Cepo Osos | `trampa osos_0` | `Trap_CepoOsos` | Cepo | PLACEABLE |

---

## 🔧 Personalización (Opcional)

Si quieres modificar el daño, cooldown o comportamiento:

1. Abre el `TrapData` en el Inspector (ej: `Trap_BotellaRon.asset`)
2. Modifica campos:
   - `damage`: daño al boss
   - `reusable`: se puede usar múltiples veces
   - `cooldown`: segundos antes de poder usar de nuevo
   - `consumeItems`: si consume el item al usar
   - `destroyAfterUse`: si se destruye la trampa
   - `animatorTrigger`: nombre del trigger en el Animator
3. Guarda (Ctrl+S)
4. Los cambios aplican automáticamente a los prefabs que usen ese TrapData

---

## 📺 Cómo Conectar Animaciones

Si quieres que la trampa tenga animación:

1. El prefab debe tener un componente `Animator`
2. En `TrapInteractable.animator`, arrastra el Animator del mismo objeto
3. En `TrapData.animatorTrigger`, escribe el nombre del trigger (ej: "Activate")
4. El trigger se ejecutará automáticamente al activar la trampa

---

## 🔊 Cómo Conectar Sonidos

1. El prefab debe tener un componente `AudioSource`
2. En `TrapInteractable.audioSource`, arrastra el AudioSource
3. En `TrapData.audioClips`, agrega los AudioClips que quieras
4. Se reproduce aleatoriamente uno de ellos al activar

---

## 🌟 Cómo Conectar Partículas

1. El prefab debe tener un componente `ParticleSystem`
2. En `TrapInteractable.effectParticles`, arrastra el ParticleSystem
3. Se ejecutará automáticamente al activar
4. Alternativamente, en `TrapData.vfxPrefab` arrastra un prefab de partículas (se instancia)

---

## 🎨 Estados Visuales Automáticos

La trampa cambia color según estado:
- **Blanco**: Idle (no preparada)
- **Amarillo**: Prepared (lista)
- **Naranja**: Active (en uso)
- **Gris**: Cooldown o Used

---

## 📚 Referencias en Inspector

Todos los campos clave están documentados con `[Tooltip]`. Pasa el ratón para ver descripciones.

---

## ✨ Cheat Sheet: Montaje Rápido

```
1. Tools > [KILL THE BOSS] Generar Sistema de Trampas Completo
   ↓
2. Arrastra Trap_*_Ready.prefab a la escena
   ↓
3. ¡Listo! La trampa funciona automáticamente.
```

**Eso es todo.**
