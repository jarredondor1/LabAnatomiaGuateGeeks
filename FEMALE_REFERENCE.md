# Referencia femenina en BodyVR

En la escena `02_AnatomyLab`, pulsa **Play**, abre **Game** y elige **FEMENINO** en la cabecera. En Quest, abre **ANATOMÍA → FEMENINO**. El botón **MASCULINO** permite regresar al catálogo original.

La elección se conserva entre sesiones. Al cambiar se recarga la escena para limpiar agarres, selección, piezas extraídas, separación y retos. Las capas visibles se guardan por separado para cada referencia. Las estadísticas educativas generales se conservan.

## Explorar

- Busca **útero**, **ovario**, **trompa uterina**, **mama** o un identificador de origen.
- Selecciona una pieza y usa **AMPLIAR INFORMACIÓN**, **EXTRAER PIEZA**, **DEVOLVER AL CUERPO** o **AISLAR**.
- La piel comienza translúcida y permite seleccionar los órganos a través de ella. Para seleccionar la piel, búscala por su nombre; puedes alternar su transparencia. **RESTABLECER CUERPO** recupera su transparencia inicial.
- **SEPARAR PIEZAS** y **RESTABLECER CUERPO** funcionan en ambos modelos.
- **FUENTES** muestra la atribución y el alcance del modelo activo.

## Alcance real

Se importan directamente 888 mallas del Human Reference Atlas femenino v1.5, con 38 piezas en el grupo reproductivo. Las ocho estructuras de embarazo se conservan en una capa independiente y desactivada al inicio. Se mantienen las proporciones y posiciones relativas originales; no se adapta ni se reutiliza geometría masculina.

La referencia ofrece superficie corporal y órganos seleccionados: el esqueleto y los músculos son parciales. Las traducciones de las estructuras reproductivas, mamarias y principales están incluidas; otras muestran un identificador en español hasta disponer de traducción específica revisada. Los nombres originales continúan siendo buscables.

La geometría conserva sus 5,557,599 triángulos; se carga por sistemas. El rendimiento del modelo femenino en Meta Quest 2 físico está pendiente de medición. Esta actualización no recompila el APK anterior.

## Reproducir la importación

Descarga el GLB oficial indicado en `LICENSE_ATTRIBUTION.md` en `SourceData/3d-vh-f-united-v1.5.glb` y ejecuta `Tools/AnatomyPipeline/import_female.py` con Python y NumPy. El importador verifica la versión, las transformaciones, el número de piezas y los índices; registra el checksum y las adaptaciones. Los recursos derivados quedan en `Assets/AnatomyLab/Resources/Anatomy/Female`.

## Validación

11 pruebas de catálogo (EditMode) y 16 pruebas de interacción (PlayMode) aprobadas. Se verifican las 888 mallas, índices y límites; nombres y búsqueda reproductiva; cambio de referencia con una pieza agarrada, limpieza de retos y separación, extracción y devolución del útero, transparencia inicial y regreso al modelo masculino durante una carga. También se verificaron el selector, la extracción y los créditos con clics en Unity.
