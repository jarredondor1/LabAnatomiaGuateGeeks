# Plan de implementación: homologación visual de BodyVR

**Objetivo:** Aplicar el vidrio, el cian y ámbar, y las líneas de escaneo de AWS VR Architect a BodyVR sin mover paneles ni cambiar controles anatómicos.

**Arquitectura:** Una paleta compartida y una malla decorativa dibujan el acabado. Las imágenes existentes siguen recibiendo clics; el escritorio y el panel de Quest comparten el estilo sin cambiar posiciones ni acciones.

**Tecnología:** Unity 6.6, C#, uGUI y pruebas de Unity.

## 1. Sistema visual compartido

- [x] Escribir primero pruebas de paleta, recorte de esquinas y ausencia de raycasts en la decoración.
- [ ] Verificar el fallo inicial en la copia temporal: Unity no completó la inicialización de licencia en modo de lote.
- [x] Crear AnatomyHoloTheme y AnatomyHoloGraphic con colores de referencia y geometría UI sin texturas nuevas.
- [x] Confirmar que las 13 pruebas de edición pasan en el editor.

## 2. Interfaz de escritorio

- [x] Añadir una prueba que comprueba las dimensiones actuales de cabecera, navegación, ficha y mesa, además de raycasts de botones.
- [x] Aplicar el vidrio a cajas, búsqueda, botones y control de separación; conservar coordenadas y acciones existentes.
- [x] Ejecutar las 17 pruebas de uso, incluidas las de clic, búsqueda, extracción y cambio de referencia.

## 3. Quest y escena

- [x] Probar que el panel de Quest cargado tiene el gráfico compartido y botones interactivos.
- [x] Estilizar la escena existente en tiempo de ejecución y las escenas futuras en el constructor.
- [x] Ajustar solo los colores de la retícula y los anillos del entorno.
- [x] Ejecutar ambas suites, revisar la compilación y capturar la vista actual de Unity.

La validación física con Meta Quest 2 y una APK nueva se informarán por separado si no pueden ejecutarse aquí.
