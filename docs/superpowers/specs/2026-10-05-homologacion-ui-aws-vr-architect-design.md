# Homologación visual de BodyVR con AWS VR Architect

Fecha: 2026-10-05. Diseño aprobado por el usuario: «Solo apariencia; conservar distribución» y «Sí, aplicar este diseño».

## Objetivo

Hacer que la interfaz de BodyVR comparta el lenguaje visual de [GuateGeeks/aws-vr-architect](https://github.com/GuateGeeks/aws-vr-architect/tree/main), conservando la disposición y los controles anatómicos que BodyVR ya tiene en Unity y Meta Quest.

## Referencia verificada

Se examinó el commit e3a9614ba8f4ea2493fb33cc57cdf07b80169d73 del repositorio indicado, en particular LabVisuals.cs, HoloPanelGraphic.cs, LabMenu.cs y Validation/01-lab-overview.png. La referencia utiliza cian #54DCEC, ámbar #FFAD4F, azul grisáceo #91A9BC, blanco #E6F6FF, vidrio azul oscuro, esquinas recortadas, bordes finos, líneas de escaneo y marcas pequeñas de registro.

## Alcance

- Un componente gráfico propio de BodyVR dibuja el vidrio recortado, el borde, los acentos cian y ámbar y las líneas de escaneo con geometría UI, sin texturas ni dependencias nuevas.
- Una paleta central fija los colores de paneles, texto, botones, estados y superficie.
- El escritorio conserva su cabecera, búsqueda y sistemas a la izquierda, ficha a la derecha, cuerpo en el centro y mesa de exploración abajo. Se actualizan los estilos de paneles y botones, así como los estados de foco y selección. Los botones y las acciones existentes permanecen.
- El panel de Quest conserva su posición, tamaño, navegación, búsqueda y acciones. Sus fondos y botones adoptan los mismos acabados gráficos. La legibilidad se revisa a la distancia de uso existente. Los mandos y los gestos no cambian.
- La escena mantiene el cuerpo, plataforma y retícula. Se ajustan únicamente los colores y los detalles de entorno para acompañar el nuevo acabado de la UI, sin sustituir la sala ni mover el cuerpo.
- La interfaz sigue en español. «MASCULINO / FEMENINO», ficha, extracción, devolución, separación y restablecimiento conservan su comportamiento.

## Diseño técnico

AnatomyHoloTheme define los colores y la configuración visual. AnatomyHoloGraphic : MaskableGraphic genera un único mesh de panel con recortes, borde, marcas y líneas; usa raycastTarget=false como superficie decorativa y permite que los botones existentes reciban los clics. ExplorationWorkbench aplica el gráfico a cajas y botones sin alterar coordenadas ni callbacks. LabPanel aplica la paleta a los elementos VR generados; AnatomyProjectBuilder utiliza el mismo gráfico y colores al regenerar escenas, mientras LabPanel actualiza la escena ya existente en tiempo de ejecución. AnatomyStageVisuals comparte los colores cian/ámbar en detalles del fondo.

La tipografía se armoniza mediante tamaño, peso, alineación y contraste sobre la fuente UI ya disponible. Se evita añadir un paquete de fuentes que pueda alterar el build de Quest. El texto anatómico sigue siendo dinámico y puede ocupar varias líneas.

## Validación

- Pruebas PlayMode comprueban que el escritorio conserva los botones y el slider de la mesa, que la búsqueda y la extracción siguen funcionando y que las superficies decorativas no bloquean raycasts.
- Pruebas EditMode comprueban la geometría del panel y los valores compartidos de la paleta.
- Captura desde Unity compara la jerarquía visual de BodyVR con la referencia, sin tratar la imagen como copia de su funcionalidad AWS.
- Se ejecutan las suites existentes. Una prueba física en Quest y un APK nuevo se informarán como pendientes si no se realizan.

## Límites

No se trasladan servicios de AWS, voz ATLAS, paneles móviles, la mesa de arquitectura ni los controles de Quest 3. El cambio se limita a la apariencia de la UI de BodyVR.
