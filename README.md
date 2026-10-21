# GuateGeeks Anatomy Lab VR

Proyecto Unity independiente en **BodyVR**. Explorador de anatomía offline para Meta Quest 2/3, con escritorio para desarrollo. Incluye 2,234 mallas BodyParts3D reales y relaciones IS-A/PART-OF. No usa servidores ni APIs.

## Abrir

Unity Hub → Add project from disk → seleccionar esta carpeta → Unity **6000.6.1f1**. Abrir `Assets/AnatomyLab/Scenes/01_Bootstrap.unity` y Play. Para trabajar directamente, abrir `02_AnatomyLab`. Las escenas ya están generadas; `GuateGeeks > Anatomy Lab > Configure Project and Scenes` las regenera (sobrescribe las escenas generadas).

## Funciones

- Cuerpo de referencia de 1.75 m; 15 grupos visuales y carga asíncrona local por sistema.
- Buscar nombres originales ingleses, traducciones básicas españolas, sinónimos e identificadores FMA/FJ.
- Seleccionar, ocultar, aislar y aplicar transparencia sin desplazar el cuerpo de la plataforma. La vista de escritorio se acerca al modelo sin cambiar su tamaño.
- Exploded View animado, extracción, agarre XRI y retorno con snap por posición/orientación.
- Mesa de proyección con `GuateGeeksFull.png` transparente, centrado bajo el cuerpo y extendido sobre casi todo el ancho de la superficie. Las tarjetas holográficas de catálogo, evaluación, exploración y sistemas son movibles y llevan los ojos animados `GeekEyes.png` en las esquinas.
- Sala envolvente basada en [AWS VR Architect de GuateGeeks](https://github.com/GuateGeeks/aws-vr-architect), con su panorama de Guatemala, retícula, costillas luminosas, techo hexagonal y proyector. El cuerpo y las piezas siguen anclados a la plataforma.
- Teclado espacial de 27 letras, sugerencias de búsqueda, retos y ensamblaje con resultados locales.
- URP con perfiles PERFORMANCE / BALANCED / QUALITY y HUD de desarrollo.

La clasificación de sistemas es heurística y necesita revisión anatómica. Buscar un órgano compuesto devuelve sus componentes; cada malla es una pieza independiente. La evaluación ofrece explicaciones breves para órganos frecuentes y una función general del tipo o sistema cuando falta una descripción específica revisada. La interfaz está en español; los nombres originales en inglés siguen disponibles para buscar.

## Controles

**Escritorio / Unity Game:** clic en el cuerpo o en una sugerencia para seleccionar. La selección mantiene el cuerpo sobre la superficie al ras del piso. El catálogo y los sistemas se apilan a la izquierda; la evaluación y la exploración, a la derecha. En **Mesa de exploración**, ajusta **Tu estatura** entre 130 y 210 cm con la barra o los botones de 1 cm; el cuerpo adopta esa altura y la conserva al restablecerlo o cambiar de referencia. Pulsa **Extraer y examinar**, mueve el ratón y haz clic fuera de las tarjetas para soltar; **Devolver al cuerpo** repone la pieza. Estas acciones se habilitan según el estado de la pieza. **Sistemas anatómicos** muestra seis botones grandes por página; **Siguientes sistemas** da acceso a todas las capas disponibles para la referencia elegida, incluidas las reproductora, respiratoria y urinaria. El catálogo también permite filtrar por esos grupos. Arrastra la barra **Mover** de cualquier tarjeta para reubicarla. **M** abre el menú rápido: permite mostrar u ocultar tarjetas y usar **Traer aquí**. WASD desplaza la vista; Shift acelera. La rueda acerca la vista al cuerpo o ajusta la distancia de una pieza tomada; el botón derecho gira el cuerpo sobre su posición. Consulta `DESKTOP_EXPLORATION.md`.

**Quest:** las tarjetas se colocan alrededor de la vista inicial a distancia de interacción. El rayo del mando izquierdo elige letras y grupos del catálogo; el gatillo selecciona botones y piezas. El botón de menú izquierdo abre un acceso compacto a catálogo, evaluación, exploración, sistemas y **Traer aquí**. En la tarjeta **Mesa de exploración** se puede indicar la estatura con precisión de 1 cm. El agarre mueve una tarjeta, incluido el bloque de sistemas, desde su barra de puntos; la tarjeta no atraviesa el suelo. Apunta a una pieza y mantén el **agarre derecho** para extraerla sin que salte de sitio; el rayo la mueve con el mando. Mientras la sostienes, la **palanca derecha vertical** acerca o aleja la pieza y la **horizontal** la gira; **A/B** la amplía o reduce. También puedes mantener el **agarre izquierdo** y separar o acercar ambos mandos para cambiar su tamaño. Suelta el agarre izquierdo para conservar el tamaño elegido y el derecho para dejar la pieza o encajarla cerca de su origen. La palanca izquierda desplaza al usuario; cuando no sujetas una pieza, la derecha gira la vista en pasos de 30°. Al devolver la pieza, recupera su tamaño anatómico. La ergonomía y los mandos aún requieren validación física en Meta Quest 2.

Interfaz en español. En evaluación, cada selección explica «Qué es» y «Función principal» con contexto sobre su ubicación, relación anatómica y papel. Las piezas identificadas tienen una descripción específica; las demás muestran el contexto de su tipo o sistema sin mensajes de revisión. Los términos sin traducción revisada usan un rótulo de tipo o grupo sin mostrar el identificador; los nombres originales siguen siendo buscables. Las descripciones anatómicas se contrastaron con [OpenStax Anatomy and Physiology](https://openstax.org/books/anatomy-and-physiology-2e/pages/1-introduction).

## Dataset

Ya está incluido en `Assets/AnatomyLab/Resources/Anatomy`. Para reconstruir: descargar archivos oficiales indicados en `DATASET_PIPELINE.md` dentro de `SourceData`, luego ejecutar `python3 Tools/AnatomyPipeline/import.py`. No requiere Blender para la importación actual.

## Compilar y probar

Menú `GuateGeeks > Anatomy Lab > Build Quest APK`, o `Tools/run_unity.sh build`. El APK está en `Builds/GuateGeeksAnatomyLabVR.apk`. `Tools/run_unity.sh edit-tests`, `play-tests`, `preview` y `audit` ejecutan verificaciones reproducibles. Requiere Unity licenciado y soporte Android instalado.

Consultar `TESTING.md`, `QUEST_SETUP.md`, `PROJECT_STATUS.md` e `IMPLEMENTATION_REPORT.md` para evidencia y pendientes. Nunca inferir una prueba física de Quest a partir del build.

## Licencias y próximos pasos

BodyParts3D, © The Database Center for Life Science licensed under CC Attribution 4.0 International. Ver `LICENSE_ATTRIBUTION.md`. Próximos hitos: validación física de Quest, revisión anatómica de grupos, precisión de picking, LOD validados y batching de geometría para dataset completo.

## Referencia femenina

En Play/Game, elige **FEMENINO** en la cabecera. Incluye 888 piezas HRA con selección, búsqueda en español, extracción y devolución. Conserva el selector **MASCULINO** para volver a BodyParts3D. Consulta [FEMALE_REFERENCE.md](FEMALE_REFERENCE.md) para el alcance y las fuentes.
