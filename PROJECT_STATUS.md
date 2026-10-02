# Estado del proyecto — 2026-10-02

COMPLETE indica implementación del alcance MVP y verificación indicada. No significa validación física de Quest ni contenido docente de producción.

## Phase 0 — COMPLETE

- Implemented: Unity independiente, Git, paquetes, URP, OpenXR, escenas y ARM64.
- Verified: Compilación Unity y build Android.
- Pending: Validación física Quest.
- Known Issues: Sin error de compilación registrado en versión final.

## Phase 1 — COMPLETE

- Implemented: Laboratorio, plataforma, cámara XR, rayos, direct interactors y panel world-space.
- Verified: Render en Play Mode revisado; rig/configuración presentes.
- Pending: Prueba de ergonomía y controles en Quest; sample XR Simulator opcional.
- Known Issues: Editor utiliza fallback propio; no hand tracking.

## Phase 2 — COMPLETE

- Implemented: BodyParts3D oficial, 2,234 mallas y 3,432 conceptos, importador y atribución.
- Verified: Catálogo completo y todos los índices de geometría comprobados.
- Pending: Revisión anatómica de clasificación heurística.
- Known Issues: Conceptos compuestos se exploran como componentes.

## Phase 3 — COMPLETE

- Implemented: Escala 1.75 m, carga por sistemas, materiales, visibilidad.
- Verified: Pruebas de carga, filtros, sistemas y reset.
- Pending: Medición de memoria física.
- Known Issues: Todas las piezas simultáneas no tienen presupuesto Quest aprobado.

## Phase 4 — COMPLETE

- Implemented: Hover, selección, panel, ocultar, aislar, transparencia y focus.
- Verified: Pruebas de aislamiento y focus sin mover cabeza.
- Pending: Precisión fina de picking en superposiciones.
- Known Issues: Picking por bounds aproximados.

## Phase 5 — COMPLETE

- Implemented: Índice offline con términos ingleses/españoles, IDs, búsqueda y teclado VR.
- Verified: Búsqueda acentuada, filtros y carga de sistema oculto.
- Pending: Ampliar traducciones y ranking semántico.
- Known Issues: UI inglés parcial; órganos compuestos devuelven componentes.

## Phase 6 — COMPLETE

- Implemented: Exploded View radial, slider/presets, animación dirty y reset.
- Verified: Pruebas de posición y selección de piezas separadas.
- Pending: Evaluar legibilidad de dataset completo en Quest.
- Known Issues: No empaquetado espacial avanzado.

## Phase 7 — COMPLETE

- Implemented: Extracción dinámica, XRI grab, retorno, snap por posición/rotación.
- Verified: Pruebas de lifecycle XRI, extracción/retorno y tolerancia de snap.
- Pending: Comprobar grip real, hápticos y comodidad.
- Known Issues: Collider simple; agarre de piezas, no ensamblajes de órgano completos.

## Phase 8 — COMPLETE

- Implemented: Fichas DEMO de identificación, retos, puzzle de cinco piezas, estadísticas, tutorial.
- Verified: Pruebas de scoring y cinco colocaciones válidas del puzzle.
- Pending: Contenido docente revisado y traducción completa.
- Known Issues: No son explicaciones médicas especializadas validadas.

## Phase 9 — IN PROGRESS

- Implemented: Streaming local, meshes sin copia CPU, materiales compartidos, física lazy, HUD y escenarios.
- Verified: Auditorías estáticas y escenarios implementados; no benchmark físico.
- Pending: CPU/GPU, memoria, draw calls y 72 FPS en Quest; LOD y batching según medición.
- Known Issues: Sin Quest conectado; no se declara objetivo de FPS logrado.

## Phase 10 — COMPLETE

- Implemented: APK de desarrollo Android IL2CPP ARM64, OpenXR Quest.
- Verified: Build Android Succeeded; paquete ARM64/OpenXR inspeccionado.
- Pending: Instalar y probar físicamente en Quest 2/3.
- Known Issues: Build de desarrollo incluye permiso INTERNET del motor; app no requiere red.

