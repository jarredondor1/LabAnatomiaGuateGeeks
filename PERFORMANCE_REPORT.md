# Performance report — 2026-10-02

## Datos medidos estáticamente

- Dataset: 2,234 mallas, 6,681,030 triángulos, 4,209,773 vértices.
- Geometría binaria en disco: 130,707,508 bytes (~124.7 MiB).
- Catálogo inicial: 457 piezas de cuatro sistemas; 1,006,672 triángulos y 618,788 vértices.
- Memoria geométrica teórica total: 181,206,912 bytes para posiciones + normales + índices uint32, sin overhead. No es medición de memoria del dispositivo.
- Auditoría exacta de posiciones duplicadas: potencial ahorro 20,677,224 bytes; welding no aplicado para evitar cambios de shading no revisados.

## Optimizaciones implementadas

Carga asíncrona local por sistema; descarga explícita; liberación de copia CPU de meshes con UploadMeshData(true); 2 materiales anatómicos compartidos; colores con MaterialPropertyBlock; colliders y XRI solo en piezas extraídas; selección central de bounds a ~15 Hz; animación central limitada a estados dirty; sin scripts Update por estructura; sin sombras; MSAA 2x por defecto, render scale 1.0.

SRP Batcher se configura, pero MPB puede impedir la ruta de batching de esos renderers. No se afirma una reducción medida de draw calls. La transparencia añade overdraw. Renderizar las 2,234 piezas a la vez no está aprobado aún para Quest 2.

## Instrumentación

PerformanceHUD (solo desarrollo): FPS, piezas visibles/cargadas, triángulos, vértices, grabs activos, memoria reservada, Draw Calls, Batches y SetPass mediante ProfilerRecorder. Physics collider por pieza extraída: 1 BoxCollider. Hay además 2 triggers de interacción directa en el rig.

Escena 03_PerformanceTest: escenarios de 100, 500, 1000 y todas las piezas visibles, 10 s después de 2 s de estabilización. Carga primero todo; la memoria de esos escenarios no representa streaming parcial. CSV local en Application.persistentDataPath. CPU/GPU frame time y thermal throttling requieren Unity Profiler conectado al Quest; el HUD no los sustituye.

## Resultados y límites

No hay Quest conectado (adb devices sin dispositivos). **72 FPS no verificados**. Pruebas funcionales en Editor y captura revisada; no equivalen a benchmark. No se midió aún GPU, CPU, temperatura o duración de batería en Quest.

## Pendiente

Medir perfiles PERFORMANCE (0.8 / 2x MSAA), BALANCED (1.0 / 2x), QUALITY (1.15 / 4x) en Quest 2. Revisar LOD experimental antes de usarlo; todavía no se importan LOD secundarios. Considerar batches por sistema con picking independiente si CPU/draw calls lo exige. Mantener estructuras pequeñas y detalles docentes críticos.
