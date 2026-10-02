# Arquitectura

DATA: `AnatomyCatalog`, `AnatomyStructure`, `AnatomySystem`, `IAnatomyDatasetProvider`. JSON local contiene identidad FJ de malla, identidad FMA de concepto, nombres, términos, sistema, transform original y bounds. `Data/Catalog/concepts.json` conserva asociaciones compuestas IS-A/PART-OF. `LocalDatasetProvider` se puede sustituir por otro dataset sin cambiar interacción.

PRESENTATION: `AnatomyStreamingManager` lee bytes por pieza vía Resources.LoadAsync, genera mallas y libera copia CPU mediante UploadMeshData(true). Mantiene un GameObject con MeshFilter y MeshRenderer por pieza cargada. Dos materiales compartidos (opaco y transparente); color/highlight mediante MaterialPropertyBlock. Las piezas ocultas no renderizan; opción de descargar sistemas ocultos. Resources no carga todas las geometrías al iniciar.

INTERACTION: selección centralizada sobre bounds a ~15 Hz, sin collider por pieza. Los rayos XRI operan UI y objetos extraídos; `LabInput` conecta esos rayos a la selección de bounds. Al extraer se añaden BoxCollider, Rigidbody y XRGrabInteractable. Al volver se eliminan. Cada pieza usa estado original separado del transform visual. Animación central únicamente para estados marcados en movimiento.

ORCHESTRATION: `LabRuntime` coordina servicios, persistencia, acciones de UI y actividades. `AnatomySystemManager`, `AnatomySelectionManager`, `ExplodedViewController`, `AnatomySearchIndex`, `QuizManager` no necesitan escenas para su lógica básica. `LabPanel` presenta Canvas world-space; `DesktopRig` permite probar sin XR.

EDUCATION: JSON externo, demos de identificación. Estadísticas y preferencias locales mediante PlayerPrefs. Sin datos personales. Tutorial de cinco acciones, omisible. Puzzle usa cinco piezas disponibles relacionadas con órganos, no modelos completos compuestos.

PERFORMANCE: no Update por estructura. Meshes legibles solo durante conversión; bounds selection evita física masiva. SRP Batcher configurado, pero MaterialPropertyBlock puede excluir renderers del SRP batching: medir SetPass/draw calls, no asumir reducción por activar la opción. No hay batching de geometrías ni LOD secundarios todavía.

Limitaciones: selección de bounds puede elegir envolventes superpuestas. Conceptos compuestos se buscan como componentes, sin agarre grupal. Clasificación heurística debe revisarse. Sin locomoción continua ni teletransporte; giro voluntario y caminata física. Separación de IO en provider permite usar Addressables en el futuro.
