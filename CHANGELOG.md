# Changelog

## 0.5.6 — Icono para Meta Quest · 2026-10-06

- Se configuró `guategeeks.png` del escritorio como icono adaptable de la aplicación Android/Quest.
- El nombre visible de la aplicación se cambió a «GuateGeeks Anatomía VR».
- Se generó y comprobó el APK para instalación desde Meta Quest Developer Hub.

## 0.5.5 — Evaluación anatómica ampliada · 2026-10-06

- Se quitó el mensaje «La función exacta de esta pieza requiere revisión» y se ampliaron las explicaciones de qué es y cuál es la función principal.
- Se añadieron descripciones específicas de estructuras como la vagina, el cuello uterino, la retina y la ampolla de la trompa uterina. La tarjeta de evaluación dispone de más espacio para leerlas completas.
- Se agregaron pruebas para evitar que vuelvan a mostrarse mensajes de revisión y para comprobar que la descripción cabe en la tarjeta.

## 0.5.4 — Función principal de cada selección · 2026-10-06

- La tarjeta de evaluación separa «Qué es» de «Función principal» y explica con lenguaje sencillo la ubicación o tipo de la pieza y su papel en el cuerpo.
- Se ampliaron las descripciones de órganos habituales y de estructuras como el uréter, la cápsula renal y el árbol bronquial. Las piezas sin función individual verificada muestran el papel del sistema sin atribuírselo indebidamente.
- Se comprobó que el texto cabe en la tarjeta durante la ejecución en Unity. Pasaron las 17 pruebas de edición y las 27 de juego.

## 0.5.3 — Evaluación breve de piezas · 2026-10-06

- La tarjeta «02 / EVALUACIÓN DE PIEZA» muestra el nombre sin identificador y una explicación breve de la estructura y su función. Hay descripciones específicas para órganos frecuentes y una explicación del tipo o grupo anatómico cuando la fuente no ofrece una identificación revisada.
- Se quitó la «Ficha de estudio» y el acceso rápido ahora se llama «Evaluación».
- Pasaron las 16 pruebas de edición y las 27 de juego.

## 0.5.2 — Manipulación de piezas en Quest · 2026-10-06

- El agarre derecho toma una pieza señalada con el rayo y la extrae sin desplazarla bruscamente ni mover el cuerpo de la plataforma.
- Mientras se sostiene, el mando mueve y orienta la pieza; la palanca derecha ajusta alcance y giro, A/B cambia su tamaño y el agarre de ambos mandos permite ampliarla o reducirla separándolos.
- El agarre de contacto de XRI se suspende durante la manipulación con rayo para evitar que dos controles disputen la misma pieza. Al soltar cerca de su posición y orientación originales se ensambla; al devolverla recupera el tamaño anatómico.
- Se añadieron pruebas de regresión para extracción sin salto, movimiento, giro, escala y conservación de la posición del cuerpo.
- Pasaron las 15 pruebas de edición y las 26 de juego. El APK de Quest se generó y verificó; falta comprobar la ergonomía con los mandos físicos.

## 0.5.1 — Movimiento seguro de tarjetas · 2026-10-06

- El arrastre con ratón y el agarre de Quest mantienen todas las esquinas de las tarjetas sobre el suelo y fuera del pedestal, incluso al girarlas.
- Un movimiento amplio del mando se comprueba por tramos para impedir que una tarjeta salte de un lado al otro atravesando la base.
- Se añadieron pruebas de regresión para ambos controles; pasaron las 15 pruebas de edición y las 24 de juego. Se generó un nuevo APK para Quest.

## 0.5 — Sala envolvente GuateGeeks · 2026-10-06

- Se integraron el panorama y los shaders de suelo, techo y proyección del laboratorio AWS VR Architect en una sala anatómica amplia.
- El logotipo completo y transparente ahora se lee sobre la plataforma desde el puesto del visitante.
- Las tarjetas y los paneles de sistemas incorporan los ojos GuateGeeks animados; conservan las acciones de catálogo, extracción, evaluación y traslado.
- El logotipo ocupa casi todo el ancho de la cubierta bajo el cuerpo; las seis opciones de sistemas ahora comparten un bloque movible, visible junto al catálogo y accesible desde el menú rápido.
- Las 15 pruebas de edición y 23 de juego terminaron sin fallos. Se generó un nuevo APK para Quest.

## 0.3.0 — 2026-10-02

- Laboratorio holográfico con retícula, arcos y tarjetas iluminadas.
- Sin geometría de mandos en escritorio; selección, extracción y liberación con clics.
- Pie simplificado a separación de piezas y restablecimiento.
- Interfaz en español; nombres frecuentes traducidos y términos pendientes identificados explícitamente.
- Giro y escala del cuerpo con palanca derecha de Quest; pendiente prueba física.
- 15 pruebas PlayMode aprobadas y verificación manual de clics. APK anterior sin recompilar.

## 0.2.0 — 2026-10-02 (Editor / escritorio)

- Estación de exploración con interfaz oscura/cian, búsqueda, capas, información contextual y controles del modelo.
- Ficha ampliada y arrastrable con I, con acciones de extracción.
- Dos mandos simulados con agarre XRI, cambio de mano, giro de piezas y ajuste de profundidad.
- Atajos suspendidos durante escritura y selección del cuerpo bloqueada al operar botones.
- 14 pruebas PlayMode aprobadas; nueva captura en Documentation/Previews/desktop-workbench.png.
- El APK de la entrega anterior no se recompiló con esta actualización de escritorio.

## 0.1.0 — 2026-10-02

- Proyecto independiente BodyVR, Unity 6, URP y OpenXR/Quest.
- BodyParts3D oficial: 2,234 mallas, importación reproducible y atribución.
- Sistemas, búsqueda, selección, aislamiento, transparencia y focus.
- Exploded view, extracción XRI, retorno, snap, reset y escritorio.
- UI world-space, teclado, tutorial, retos, puzzle, estadísticas locales.
- Tests EditMode/PlayMode, escena performance, herramientas de auditoría y build Android.

Limitaciones y evidencia verificable en IMPLEMENTATION_REPORT.md.

## 0.4 — Referencia femenina

- Catálogo independiente HRA femenino v1.5: 888 mallas, piel translúcida y capa de embarazo separada.
- Selector Masculino / Femenino en escritorio y panel de VR; capas guardadas por referencia.
- Nombres reproductivos y mamarios en español, búsqueda y siete fichas de identificación.
- Cambio de modelo limpia agarres, selección, retos y separación; fuentes y alcance accesibles en la interfaz.
