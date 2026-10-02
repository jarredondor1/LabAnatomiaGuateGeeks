# GuateGeeks Anatomy Lab VR

Proyecto Unity independiente en **BodyVR**. Explorador de anatomía offline para Meta Quest 2/3, con escritorio para desarrollo. Incluye 2,234 mallas BodyParts3D reales y relaciones IS-A/PART-OF. No usa servidores ni APIs.

## Abrir

Unity Hub → Add project from disk → seleccionar esta carpeta → Unity **6000.6.1f1**. Abrir `Assets/AnatomyLab/Scenes/01_Bootstrap.unity` y Play. Para trabajar directamente, abrir `02_AnatomyLab`. Las escenas ya están generadas; `GuateGeeks > Anatomy Lab > Configure Project and Scenes` las regenera (sobrescribe las escenas generadas).

## Funciones

- Cuerpo de referencia de 1.75 m; 15 grupos visuales y carga asíncrona local por sistema.
- Buscar nombres originales ingleses, traducciones básicas españolas, sinónimos e identificadores FMA/FJ.
- Seleccionar, ocultar, aislar, transparencia, enfocar moviendo el cuerpo, rotar y escalar.
- Exploded View animado, extracción, agarre XRI y retorno con snap por posición/orientación.
- Panel VR con teclado, panel rápido, tutorial, retos y puzzle con resultados locales.
- URP con perfiles PERFORMANCE / BALANCED / QUALITY y HUD de desarrollo.

La clasificación de sistemas es heurística y necesita revisión anatómica. Buscar un órgano compuesto devuelve sus componentes; cada malla es una pieza independiente. Las fichas educativas son DEMO de identificación, sin explicaciones médicas especializadas validadas. La traducción al inglés del UI es parcial; los nombres originales están disponibles.

## Controles

**Escritorio:** clic selecciona; arrastrar con botón derecho gira el cuerpo; rueda cambia escala; E extrae; G activa/desactiva arrastre de la pieza seleccionada; Q/E giran la pieza durante arrastre; T devuelve; R reinicia; espacio abre panel rápido. En BUSCAR, escribir con teclado o pulsar teclado virtual; retroceso borra.

**Quest:** apuntar y gatillo de la mano principal seleccionan; ambos rayos operan botones; grip agarra piezas extraídas mediante direct interactors; soltar cerca de posición y orientación originales realiza snap. A izquierdo abre panel rápido; stick derecho gira voluntariamente 30°. Caminar físicamente alrededor del cuerpo según el espacio disponible. Sin hand tracking de manos desnudas en este MVP.

## Dataset

Ya está incluido en `Assets/AnatomyLab/Resources/Anatomy`. Para reconstruir: descargar archivos oficiales indicados en `DATASET_PIPELINE.md` dentro de `SourceData`, luego ejecutar `python3 Tools/AnatomyPipeline/import.py`. No requiere Blender para la importación actual.

## Compilar y probar

Menú `GuateGeeks > Anatomy Lab > Build Quest APK`, o `Tools/run_unity.sh build`. APK en `Builds/GuateGeeksAnatomyLabVR.apk` si la compilación termina correctamente. `Tools/run_unity.sh edit-tests`, `play-tests`, `preview` y `audit` ejecutan verificaciones reproducibles. Requiere Unity licenciado y soporte Android instalado.

Consultar `TESTING.md`, `QUEST_SETUP.md`, `PROJECT_STATUS.md` e `IMPLEMENTATION_REPORT.md` para evidencia y pendientes. Nunca inferir una prueba física de Quest a partir del build.

## Licencias y próximos pasos

BodyParts3D, © The Database Center for Life Science licensed under CC Attribution 4.0 International. Ver `LICENSE_ATTRIBUTION.md`. Próximos hitos: validación física de Quest, revisión anatómica de grupos, precisión de picking, LOD validados y batching de geometría para dataset completo.
