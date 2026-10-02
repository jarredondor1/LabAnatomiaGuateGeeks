# MASTER PROMPT — GUATEGEEKS ANATOMY LAB VR

Quiero que construyas de principio a fin un proyecto educativo de anatomía humana en realidad virtual llamado:

# GuateGeeks Anatomy Lab VR

La experiencia estará inspirada funcionalmente en proyectos web como Human Atlas / BodyParts3D Explorer, pero NO debe limitarse a copiar una interfaz web.

La meta es crear una experiencia VR nativa para Meta Quest donde el usuario pueda:

- observar un cuerpo humano 3D a escala real;
- activar y desactivar sistemas anatómicos;
- buscar estructuras por nombre;
- señalar y seleccionar estructuras;
- aislarlas;
- ocultarlas;
- hacerlas transparentes;
- extraer partes del cuerpo;
- manipularlas con las manos/controladores;
- realizar un "Exploded View";
- devolver las estructuras a su posición original;
- explorar información educativa;
- realizar actividades y retos;
- funcionar completamente offline en su versión inicial.

---

# 1. PRINCIPIO GENERAL

No quiero solamente documentación ni una prueba conceptual.

Quiero que trabajes directamente sobre el proyecto y construyas cada fase.

Debes:

1. inspeccionar el entorno disponible;
2. crear la estructura del proyecto;
3. implementar;
4. compilar;
5. probar;
6. corregir errores;
7. documentar;
8. continuar con la siguiente fase.

No me pidas aprobación después de cada pequeño cambio.

Si encuentras decisiones técnicas menores, toma la decisión más razonable y documenta lo decidido.

Solo considera un bloqueo cuando realmente sea imposible continuar sin algo externo, como un dataset que requiera descarga manual o aceptación de licencia.

En ese caso:

- deja todo el pipeline preparado;
- documenta exactamente qué archivo falta;
- especifica dónde debe colocarse;
- continúa construyendo todo lo que no dependa de ese archivo.

---

# 2. PROYECTO INDEPENDIENTE

Este proyecto NO debe modificar proyectos existentes de GuateGeeks.

Crear un proyecto/repository independiente llamado aproximadamente:

GuateGeeksAnatomyLabVR

No modificar:

- GuateGeeksCloudForgeVR
- CloudForgeBootstrap
- otros proyectos Unity existentes.

---

# 3. STACK OBJETIVO

Usar preferentemente:

- Unity 6000.6.1f1 o la versión Unity 6 compatible instalada.
- Universal Render Pipeline.
- OpenXR.
- XR Interaction Toolkit 3.6.x.
- Unity Input System.
- Meta Quest 2 como dispositivo mínimo objetivo.
- Meta Quest 3 compatible posteriormente.
- Android ARM64.
- Vulkan cuando sea apropiado.
- OpenXR como capa XR principal.

Evitar dependencias propietarias innecesarias.

No utilizar:

- AWS
- OpenAI API
- APIs externas de pago
- servidores
- servicios cloud
- bases de datos pagadas

para el MVP.

Todo el MVP debe ser offline.

---

# 4. FUENTE DE DATOS ANATÓMICOS

La fuente principal debe ser:

BodyParts3D

El proyecto debe considerar miles de estructuras anatómicas independientes.

Como referencia funcional y técnica puedes analizar el proyecto open source Human Atlas basado en BodyParts3D, pero NO debes copiar código ciegamente.

Antes de incorporar código, modelos o metadata:

1. revisa las licencias;
2. identifica qué puede reutilizarse;
3. conserva atribuciones;
4. crea:

LICENSE_ATTRIBUTION.md

Documenta:

- dataset utilizado;
- repositorios utilizados como referencia;
- licencias;
- modificaciones;
- atribuciones requeridas.

Nunca elimines atribuciones del dataset original.

---

# 5. OBJETIVO DE ESCALA

Diseñar la arquitectura pensando en aproximadamente:

2,000+ estructuras anatómicas.

No diseñar una solución que funcione únicamente con 20 objetos.

El sistema debe soportar conceptualmente:

- huesos;
- músculos;
- órganos;
- arterias;
- venas;
- sistema nervioso;
- sistema respiratorio;
- sistema digestivo;
- sistema urinario;
- sistema reproductivo;
- sistema endocrino;
- sistema linfático;
- otras categorías existentes en el dataset.

La implementación puede comenzar con subconjuntos de datos, pero la arquitectura final debe soportar el catálogo completo.

---

# 6. ARQUITECTURA DATA-DRIVEN

Crear una arquitectura basada en datos.

Cada estructura debe poseer un identificador único.

Crear un modelo equivalente a:

AnatomyStructure

Campos mínimos:

- id
- canonicalName
- displayName
- latinName si está disponible
- synonyms
- systemId
- category
- description
- meshReference
- materialReference
- originalPosition
- originalRotation
- originalScale
- bounds
- parentStructure
- childStructures
- searchableTerms

Crear:

AnatomySystem

con información como:

- id
- name
- displayName
- defaultColor
- structures
- defaultVisibility

Separar:

DATA

de

PRESENTATION

de

INTERACTION.

---

# 7. ESTRUCTURA DEL PROYECTO

Organizar Assets aproximadamente así:

Assets/
  AnatomyLab/
    Art/
      Anatomy/
      Materials/
      UI/
      Environment/
    Audio/
    Data/
      Catalog/
      Systems/
      Localization/
    Prefabs/
      Anatomy/
      Interaction/
      UI/
      Environment/
    Scenes/
    Scripts/
      Core/
      Anatomy/
      Interaction/
      Search/
      ExplodedView/
      UI/
      XR/
      Education/
      Performance/
      Utilities/
    ScriptableObjects/
    Addressables/
    Editor/
    Tests/
      EditMode/
      PlayMode/
    Documentation/

Mantener namespaces coherentes.

Ejemplo:

GuateGeeks.AnatomyLab.Core
GuateGeeks.AnatomyLab.Anatomy
GuateGeeks.AnatomyLab.Interaction
GuateGeeks.AnatomyLab.XR

---

# 8. ESCENAS

Crear como mínimo:

01_Bootstrap
02_AnatomyLab
03_PerformanceTest

Bootstrap debe encargarse de:

- inicialización;
- carga de configuración;
- catálogo anatómico;
- XR;
- transición de escena.

AnatomyLab será la experiencia principal.

PerformanceTest permitirá probar:

- 100 estructuras;
- 500 estructuras;
- 1,000 estructuras;
- dataset completo disponible.

---

# 9. ENTORNO VISUAL

Crear un laboratorio futurista educativo.

No quiero un ambiente excesivamente complejo.

Debe sentirse:

- limpio;
- moderno;
- tecnológico;
- educativo;
- profesional.

Debe existir:

- plataforma central;
- cuerpo humano sobre la plataforma;
- iluminación suave;
- fondo oscuro/neutro;
- panel holográfico;
- espacio alrededor del cuerpo para caminar;
- indicadores visuales discretos.

Mantener rendimiento como prioridad.

---

# 10. ESCALA HUMANA

El cuerpo debe aparecer aproximadamente a escala humana real.

Altura objetivo aproximada:

1.70–1.80 metros.

Crear mecanismos para:

- cambiar escala;
- ajustar posición;
- rotar todo el modelo;
- centrarlo frente al usuario.

---

# 11. SISTEMAS ANATÓMICOS

Crear un AnatomySystemManager.

Debe permitir:

ShowSystem(systemId)
HideSystem(systemId)
ToggleSystem(systemId)
IsolateSystem(systemId)
ShowAll()
HideAll()
ResetVisibility()

La UI debe ofrecer botones para activar/desactivar sistemas.

Cada sistema debe tener un color visual diferenciable, pero evitar una apariencia infantil.

---

# 12. SELECCIÓN DE ESTRUCTURAS

Crear un AnatomySelectionManager.

Interacción mediante:

- XR Ray Interactor;
- direct interaction cuando corresponda.

Al apuntar una estructura:

HOVER:

- resaltar suavemente;
- mostrar nombre.

SELECT:

- seleccionar;
- mantener resaltado;
- abrir panel de información.

Evitar materiales duplicados innecesariamente.

Usar:

- MaterialPropertyBlock;
- shader parameter;
- mecanismo equivalente eficiente.

---

# 13. PANEL DE INFORMACIÓN

Al seleccionar una estructura mostrar:

Nombre

Sistema

Categoría

Descripción

Acciones:

AISLAR

OCULTAR

EXTRAER

TRANSPARENCIA

VOLVER A POSICIÓN

ENFOCAR

Debe poder cerrarse fácilmente.

---

# 14. MODO TRANSPARENCIA

Implementar transparencia por estructura y por sistema.

Ejemplos:

hacer músculos semitransparentes para observar huesos;

hacer órganos transparentes para observar estructuras internas.

Crear estados como:

Opaque
Transparent
Hidden

Evitar crear un material nuevo para cada estructura.

---

# 15. BÚSQUEDA

Crear un buscador anatómico.

Debe encontrar estructuras utilizando:

- nombre;
- nombre parcial;
- sinónimos;
- variantes;
- términos indexados.

Ejemplo:

heart
corazón
cardiac

si la metadata disponible permite esas asociaciones.

Crear un índice de búsqueda que NO recorra miles de GameObjects en cada tecla.

Construir el índice una vez.

Implementar búsqueda incremental.

Resultados:

máximo 10–20 visibles simultáneamente.

Cuando el usuario selecciona un resultado:

1. hacer visible su sistema;
2. seleccionar estructura;
3. enfocar cámara/cuerpo;
4. resaltarla;
5. mostrar panel de información.

---

# 16. LOCALIZACIÓN

Preparar arquitectura para:

- Español
- Inglés

Español será idioma prioritario.

No es obligatorio traducir manualmente miles de estructuras durante la primera versión.

Separar localization data de los scripts.

---

# 17. EXPLODED VIEW

Esta es una característica fundamental.

Crear:

ExplodedViewController

El usuario debe poder desplazar las estructuras hacia afuera manteniendo una relación con su posición anatómica.

Calcular aproximadamente:

bodyCenter

structureCenter

direction =
(structureCenter - bodyCenter).normalized

explodedPosition =
originalPosition +
direction * explosionFactor

No modificar permanentemente la posición original.

Mantener:

originalTransform

separado de:

currentVisualTransform.

Crear slider:

0%
25%
50%
75%
100%

La transición debe ser animada.

Utilizar interpolación suave.

El usuario debe poder seleccionar estructuras incluso estando explotadas.

---

# 18. EXTRACCIÓN MANUAL

Quiero poder literalmente sacar una estructura del cuerpo.

Flujo:

1. usuario apunta;
2. selecciona;
3. pulsa "Extraer" o realiza una acción directa;
4. estructura deja su posición anatómica;
5. se vuelve XR Grab Interactable;
6. usuario puede agarrarla;
7. moverla;
8. rotarla;
9. observarla.

No colocar permanentemente XRGrabInteractable + Rigidbody + collider complejo en las 2,000+ piezas si eso perjudica rendimiento.

Diseñar interacción dinámica.

Ejemplo:

StructureInteractionAdapter

Solo la estructura activa puede recibir temporalmente:

- collider detallado;
- Rigidbody;
- XRGrabInteractable.

Al regresar a posición:

eliminar/desactivar componentes costosos.

---

# 19. REENSAMBLADO

Crear:

ReturnToBody()

La pieza debe:

- dejar de ser manipulable;
- animarse hacia originalPosition;
- animarse hacia originalRotation;
- recuperar originalScale;
- reintegrarse al sistema.

Añadir opción:

RESET BODY

que restaure:

- posiciones;
- rotaciones;
- escala;
- transparencia;
- visibilidad;
- exploded factor.

---

# 20. SNAP ANATÓMICO

Cuando una estructura extraída se acerque suficientemente a su posición original:

mostrar indicador.

Ejemplo:

verde = posición válida.

Al soltar dentro del rango:

hacer snap automático.

Tolerancia configurable.

---

# 21. MODO AISLAR

Seleccionando una estructura:

ISOLATE

debe ocultar o volver transparentes todas las estructuras excepto:

- pieza seleccionada;
- opcionalmente estructuras relacionadas.

Botón:

EXIT ISOLATION.

---

# 22. FOCUS

Crear función:

FocusStructure(structureId)

El cuerpo o viewport debe ajustarse para colocar la estructura en una posición fácilmente observable.

Evitar mover agresivamente la cámara XR del usuario.

Preferiblemente:

mover/rotar el modelo anatómico,

NO la cabeza/cámara VR.

---

# 23. INTERFAZ VR

Crear panel holográfico principal.

Secciones:

ANATOMÍA

SISTEMAS

BUSCAR

ESTRUCTURA

EXPLORAR

APRENDER

AJUSTES

Mantener textos legibles en Quest.

Evitar paneles gigantes.

Aplicar buenas prácticas VR:

- buen tamaño de fuente;
- contraste;
- botones amplios;
- distancia cómoda;
- evitar UI demasiado cercana a la cámara.

---

# 24. PANEL RÁPIDO

Crear un panel rápido accesible desde controlador izquierdo o botón equivalente con:

Skeleton
Muscles
Organs
Nerves
Vessels
Exploded View
Reset

según los sistemas realmente disponibles.

---

# 25. EDUCATIONAL MODE

Después del explorer básico, crear un modo educativo.

Pantalla:

MODO EXPLORAR

MODO APRENDER

MODO RETO

---

# 26. MODO APRENDER

Seleccionar una estructura y mostrar información educativa sencilla.

Ejemplo:

CORAZÓN

Sistema:
Cardiovascular

Función:
...

Ubicación:
...

Relacionado con:
...

Mantener la información separada de la lógica.

Crear:

EducationalContentDatabase

No inventar información médica especializada que no esté validada.

Para el MVP utilizar contenido básico y marcar claramente fixtures/demo cuando corresponda.

---

# 27. MODO RETO

Crear un QuizManager.

Ejemplos:

"Encuentra el corazón."

"Selecciona el fémur."

"Encuentra un órgano perteneciente al sistema respiratorio."

Flujo:

pregunta

↓

usuario busca/señala

↓

selección

↓

correcto/incorrecto

↓

feedback

↓

siguiente reto

Crear estadísticas locales:

- preguntas;
- correctas;
- incorrectas;
- porcentaje;
- tiempo aproximado.

No requiere cuenta.

---

# 28. MODO ENSAMBLAJE

Crear una actividad:

ANATOMY PUZZLE

Ejemplo:

extraer:

- corazón;
- pulmones;
- hígado;
- estómago;
- cerebro.

El usuario debe devolverlos correctamente.

Registrar:

correctPlacements
wrongPlacements
completionTime

---

# 29. PERFORMANCE — PRIORIDAD CRÍTICA

Meta Quest 2 es una plataforma móvil.

Objetivo:

72 FPS siempre que sea razonablemente posible.

No debemos asumir que mantener más de 2,000 GameObjects complejos activos es aceptable.

Perfilar:

- CPU
- GPU
- memoria
- draw calls
- batches
- SetPass
- triángulos
- colliders
- scripts activos

Crear optimizaciones reales.

---

# 30. LOADING POR SISTEMA

No cargar innecesariamente todos los sistemas con máximo detalle al iniciar.

Crear:

AnatomyStreamingManager

Permitir:

LoadSystemAsync()
UnloadSystemAsync()

Evaluar Unity Addressables.

Organizar contenido anatómico por grupos:

Skeleton
Muscular
Organs
Cardiovascular
Nervous
etc.

---

# 31. ADDRESSABLES

Utilizar Addressables si aportan ventajas reales.

Objetivo:

- separar sistemas;
- loading async;
- menor memoria inicial;
- permitir datasets adicionales en el futuro.

No convertir Addressables en una dependencia complicada sin necesidad.

Documentar la decisión.

---

# 32. COLLIDERS

No utilizar automáticamente MeshCollider convexo en miles de estructuras.

Diseñar niveles de interacción:

LEVEL 0:
render solamente

LEVEL 1:
bounds selection

LEVEL 2:
simple collider

LEVEL 3:
detailed collider cuando estructura está seleccionada/manipulada

Crear collider lazy-loading cuando sea viable.

---

# 33. LOD

Crear estrategia LOD.

Por ejemplo:

LOD0:
modelo original optimizado

LOD1:
modelo reducido

LOD2:
modelo muy reducido

No degradar estructuras pequeñas hasta desaparecer cuando sean importantes educativamente.

Crear herramientas Editor para detectar modelos sin LOD.

---

# 34. PIPELINE DE IMPORTACIÓN

Crear tooling de Editor.

Menu:

GuateGeeks
  Anatomy Lab
    Import Dataset
    Rebuild Catalog
    Validate Anatomy
    Generate Bounds
    Generate Preview
    Performance Audit

Crear:

AnatomyDatasetImporter

Debe intentar:

1. escanear archivos disponibles;
2. asociarlos con metadata;
3. crear AnatomyStructureData;
4. asignar sistema;
5. calcular bounds;
6. guardar posición original;
7. crear catálogo;
8. detectar errores.

---

# 35. PIPELINE EXTERNO

Si resulta conveniente usar Blender para optimización:

crear scripts reproducibles.

Por ejemplo:

Tools/
  AnatomyPipeline/
    import.py
    optimize.py
    generate_lod.py
    export.py

Debe poder ejecutarse nuevamente.

No hacer pasos manuales irreproducibles salvo que sean inevitables.

---

# 36. METADATA

Crear:

anatomy_catalog.json

o ScriptableObjects equivalentes.

No codificar nombres anatómicos directamente en scripts.

Ejemplo conceptual:

{
  "id": "FMA_7088",
  "name": "Heart",
  "system": "cardiovascular",
  "mesh": "...",
  "synonyms": [...]
}

---

# 37. FUTUROS DATASETS

La arquitectura debe admitir en el futuro:

MaleBodyDataset

FemaleBodyDataset

sin reescribir toda la aplicación.

Crear concepto:

IAnatomyDatasetProvider

o arquitectura equivalente.

No es necesario obtener un cuerpo femenino en esta primera fase.

---

# 38. OPTIMIZACIÓN DE MESHES

Analizar:

- polígonos;
- vértices;
- normales;
- materiales;
- texturas.

Reducir materiales diferentes.

Utilizar atlas cuando tenga sentido.

No destruir precisión visual crítica.

Registrar antes/después.

Crear documento:

PERFORMANCE_REPORT.md

---

# 39. PROFILING

Crear herramientas para medir:

Visible Structures
Loaded Structures
Triangles
Vertices
Active Colliders
Active Interactables
Memory
FPS

Crear PerformanceHUD disponible solo en desarrollo.

Debe poder ocultarse en producción.

---

# 40. INPUT VR

Configurar:

Left Controller

Right Controller

Ray Interactors

Direct Interactors

Grab

UI interaction

Teleport opcional

Turn

Evitar locomoción incómoda.

La experiencia principal debe permitir explorar caminando físicamente alrededor de la plataforma cuando sea posible.

---

# 41. DESKTOP FALLBACK

La escena debe poder probarse sin Quest.

Implementar modo Editor:

mouse + keyboard

para:

- rotar modelo;
- seleccionar;
- buscar;
- exploded view;
- mostrar/ocultar sistemas.

Así podremos verificar gran parte del proyecto sin generar APK en cada cambio.

---

# 42. INPUT SIMULATOR

Configurar XR Device Simulator si está disponible y es compatible.

Documentar controles.

---

# 43. AUDIO

Añadir feedback ligero:

- hover;
- select;
- correct;
- incorrect;
- snap;
- system toggle.

No utilizar assets con licencia dudosa.

Si no existen assets:

crear placeholders claros y documentarlos.

---

# 44. HÁPTICOS

Usar vibración sutil cuando:

- estructura seleccionada;
- pieza agarrada;
- snap correcto;
- respuesta correcta.

Crear:

HapticFeedbackService.

---

# 45. GUARDADO LOCAL

Guardar preferencias mediante sistema local:

- idioma;
- volumen;
- handedness si aplica;
- sistema visible;
- tutorial completado.

No almacenar información personal.

---

# 46. TUTORIAL

Primera ejecución:

PASO 1
Apunta a una estructura.

PASO 2
Selecciona.

PASO 3
Extrae una pieza.

PASO 4
Devuélvela.

PASO 5
Prueba Exploded View.

Debe poder saltarse.

---

# 47. SEGURIDAD Y COMODIDAD VR

Evitar:

- movimiento de cámara inesperado;
- flashing;
- aceleraciones rápidas;
- UI fija a la cabeza;
- textos demasiado pequeños.

El cuerpo se mueve.

El usuario no debe ser desplazado artificialmente salvo locomoción voluntaria.

---

# 48. TESTS

Crear EditMode tests para:

Catalog

Search

Structure State

Exploded Position

Visibility

System Filtering

Reset

Crear PlayMode tests para:

Selection

Isolation

Extraction

ReturnToBody

Snap

Quiz

System Toggle

No marcar una fase como terminada si los tests asociados fallan.

---

# 49. VALIDATORS

Crear verificadores Editor.

Ejemplos:

AnatomyCatalogValidator
MissingMeshValidator
DuplicateIdValidator
MissingSystemValidator
ColliderAudit
PerformanceAudit

La salida debe indicar claramente:

PASS

WARNING

FAIL

---

# 50. LOGGING

Crear logging consistente.

No llenar consola cada frame.

Prefijos:

[AnatomyLab]
[Dataset]
[XR]
[Performance]
[Education]

---

# 51. DOCUMENTACIÓN

Mantener actualizados:

README.md

ARCHITECTURE.md

DATASET_PIPELINE.md

QUEST_SETUP.md

TESTING.md

PERFORMANCE_REPORT.md

LICENSE_ATTRIBUTION.md

CHANGELOG.md

---

# 52. README

El README debe explicar:

Qué es GuateGeeks Anatomy Lab VR

Características

Stack

Dataset

Cómo abrir en Unity

Cómo importar dataset

Cómo ejecutar en Editor

Cómo probar VR

Cómo generar APK

Arquitectura básica

Licencias

Roadmap

---

# 53. ROADMAP DE IMPLEMENTACIÓN

Trabajar internamente por fases.

No detenerte entre fases salvo bloqueo real.

---

## FASE 0 — AUDITORÍA Y SETUP

- verificar Unity;
- verificar paquetes;
- crear proyecto;
- configurar Git;
- configurar .gitignore;
- crear estructura de carpetas;
- configurar URP;
- configurar OpenXR;
- configurar XRI;
- Android/Quest;
- crear escenas;
- compilar.

Resultado:

proyecto limpio sin errores.

---

## FASE 1 — LABORATORIO VR

Crear:

- entorno;
- plataforma;
- XR Origin;
- iluminación;
- locomoción;
- manos/controladores;
- panel principal;
- cuerpo placeholder.

Verificar Editor y VR architecture.

---

## FASE 2 — DATASET PIPELINE

- investigar BodyParts3D;
- revisar licencia;
- preparar importación;
- metadata;
- catálogo;
- estructura por sistemas;
- tooling Editor;
- validación.

Si el dataset no puede descargarse automáticamente:

dejar instrucciones exactas.

---

## FASE 3 — BODY VIEWER

- cargar estructuras;
- escala;
- materiales;
- posición;
- sistemas;
- visibilidad;
- selección básica.

---

## FASE 4 — INTERACTION

- hover;
- select;
- panel;
- isolate;
- hide;
- transparency;
- focus.

---

## FASE 5 — SEARCH

- índice;
- UI;
- resultados;
- navegación;
- highlight;
- focus.

---

## FASE 6 — EXPLODED VIEW

Implementar completamente:

- slider;
- animación;
- reset;
- selección en modo exploded;
- performance.

---

## FASE 7 — EXTRACTION

- dynamic interaction;
- grab;
- physics;
- snap;
- ReturnToBody;
- ResetBody.

---

## FASE 8 — EDUCATION

- aprender;
- retos;
- puzzle anatómico;
- scoring local.

---

## FASE 9 — PERFORMANCE

Perfilar específicamente para Quest.

Optimizar:

- meshes;
- materiales;
- Addressables;
- colliders;
- scripts;
- draw calls;
- memoria.

---

## FASE 10 — QUEST BUILD

Configurar:

Android
ARM64
OpenXR
Quest 2

Generar:

Builds/GuateGeeksAnatomyLabVR.apk

Solo hacerlo cuando compile correctamente.

---

# 54. CRITERIOS MVP

El MVP NO está terminado hasta que pueda demostrarse:

1. abrir laboratorio;
2. observar cuerpo humano;
3. activar/desactivar mínimo varios sistemas;
4. seleccionar estructura;
5. ver nombre;
6. aislarla;
7. buscar estructura;
8. hacer exploded view;
9. extraer una pieza;
10. agarrarla;
11. devolverla;
12. resetear cuerpo.

Y:

- no hay errores de compilación;
- no hay NullReferenceExceptions en flujo normal;
- funciona en Editor;
- arquitectura Quest está configurada.

---

# 55. CRITERIOS PARA DATASET COMPLETO

Cuando BodyParts3D esté disponible:

verificar:

- cantidad total de estructuras importadas;
- cantidad con mesh;
- cantidad sin mesh;
- sistemas;
- duplicates;
- piezas sin metadata;
- errores de transformación.

Generar reporte:

DATASET_IMPORT_REPORT.md

Ejemplo:

Total structures:
Valid structures:
Missing meshes:
Duplicate IDs:
Systems:
Total triangles:
Disk size:
Runtime memory estimate:

---

# 56. PERFORMANCE TARGETS

Objetivo Quest 2:

72 FPS.

Crear tres perfiles:

QUALITY

BALANCED

PERFORMANCE

El perfil por defecto Quest 2 debe ser BALANCED o PERFORMANCE según resultados reales.

No afirmar que se alcanzan 72 FPS hasta medirlo físicamente.

---

# 57. QUALITY SETTINGS

Configurar de forma razonable para Quest:

- MSAA;
- render scale;
- shadows;
- texture resolution;
- anisotropic filtering;
- dynamic batching/SRP batching cuando corresponda.

No sacrificar legibilidad anatómica innecesariamente.

---

# 58. GIT

Crear commits pequeños y semánticos.

Ejemplo:

feat: create anatomy catalog
feat: add structure selection
feat: implement exploded view
perf: lazy load structure colliders
test: add anatomy catalog validation

No hacer un único commit gigante.

---

# 59. CALIDAD DE CÓDIGO

Evitar:

God Objects

FindObjectOfType repetitivo

GameObject.Find

strings mágicos

allocations por frame

LINQ en Update cuando pueda generar GC

duplicación de lógica

Separar responsabilidades.

Utilizar interfaces cuando aporten desacoplamiento real.

No sobrearquitecturar cosas triviales.

---

# 60. UPDATE

No utilizar Update() para miles de AnatomyStructure.

Preferir:

eventos

managers

centralized updates

dirty flags

coroutines

async loading

según corresponda.

---

# 61. DOCUMENTAR DECISIONES IMPORTANTES

Crear:

Documentation/ADR/

Ejemplos:

ADR-001-dataset-format.md
ADR-002-addressables.md
ADR-003-selection-system.md
ADR-004-exploded-view.md

Documentar especialmente decisiones que afecten performance.

---

# 62. PLACEHOLDERS

Cuando falte un asset:

NO bloquear todo el proyecto.

Crear placeholder apropiado.

Marcar explícitamente:

PLACEHOLDER

y registrar pendiente.

No presentar placeholders como producción final.

---

# 63. NO INVENTAR RESULTADOS

No decir:

"Quest tested"

si solamente se ejecutó en Editor.

Diferenciar:

Editor verified

Android build verified

Quest physical device verified

Solo la última requiere prueba física real.

---

# 64. INFORME FINAL

Cuando completes el mayor alcance posible, crear:

IMPLEMENTATION_REPORT.md

Debe contener:

## Estado general

## Fases completadas

## Funcionalidades

## Arquitectura

## Dataset

## Performance

## Tests

## APK

## Pendientes

## Bloqueos

## Próximos pasos

---

# 65. STATUS DE CADA FASE

Mantener:

PROJECT_STATUS.md

Formato:

Phase 0 — COMPLETE
Phase 1 — COMPLETE
Phase 2 — IN PROGRESS
...

Para cada fase:

Implemented
Verified
Pending
Known Issues

---

# 66. PRIMERA ACCIÓN

Empieza ahora.

Primero:

1. inspecciona el directorio actual;
2. verifica herramientas instaladas;
3. confirma si existe un proyecto Unity o debemos crear el nuevo;
4. crea GuateGeeksAnatomyLabVR;
5. configura el proyecto base;
6. genera SPEC_ANATOMY_LAB.md;
7. genera PROJECT_STATUS.md;
8. comienza FASE 0;
9. continúa hacia las siguientes fases sin esperar confirmaciones innecesarias.

No te limites a explicarme qué habría que hacer.

Haz los cambios en archivos y código.

---

# 67. PRINCIPIO DE AUTONOMÍA

Tienes autorización para:

- crear carpetas;
- crear escenas;
- crear scripts;
- modificar ProjectSettings del proyecto nuevo;
- agregar paquetes gratuitos necesarios;
- escribir herramientas Editor;
- crear tests;
- generar documentación;
- ejecutar tests;
- corregir errores;
- generar builds de desarrollo;
- refactorizar código creado dentro de este proyecto.

No tienes autorización para:

- modificar otros proyectos;
- contratar servicios;
- crear recursos cloud con costo;
- utilizar APIs pagadas;
- comprar assets;
- eliminar proyectos externos;
- publicar repositorios públicamente;
- subir información privada.

---

# 68. DEFINICIÓN DE ÉXITO

El objetivo final es que GuateGeeks Anatomy Lab VR permita que un estudiante entre con Meta Quest, observe un cuerpo humano a escala real y pueda hacer algo parecido a:

"Sistema muscular OFF."

Los músculos desaparecen.

"Sistema óseo ON."

Observa el esqueleto.

Busca:

"Heart"

El corazón se ilumina.

Lo selecciona.

Pulsa:

EXTRAER.

Lo toma con la mano.

Lo observa desde cualquier ángulo.

Lo devuelve al cuerpo.

Activa:

EXPLODED VIEW.

Las estructuras se separan desde el centro del cuerpo mostrando las diferentes capas anatómicas.

Luego inicia:

RETO

y el sistema solicita:

"Encuentra el fémur."

Ese es el nivel de experiencia que debemos construir.

---

Comienza la implementación.