# Laboratorio espacial de anatomía — plan de implementación

**Objetivo:** Adaptar la experiencia espacial del repositorio GuateGeeks AWS VR Architect a BodyVR, manteniendo datos anatómicos reales y la interacción en español.

**Arquitectura:** La escena aporta una mesa de proyección y el logotipo original. Una interfaz de tarjetas en espacio 3D usa los servicios existentes de catálogo, selección, extracción, educación y vista explotada. El controlador XR separa navegación, búsqueda por letras y manipulación de piezas.

**Tecnología:** Unity 6.6, URP, uGUI, XR Interaction Toolkit, OpenXR Quest 2, Input System.

## Pasos

- [x] Inspeccionar el proyecto actual y la referencia local del repositorio.
- [x] Crear una prueba que falle si faltan mesa, logotipo, tarjetas movibles o alfabeto.
- [x] Construir mesa, anillos, ambiente y placa con logotipo.
- [x] Construir tarjetas espaciales de catálogo, evaluación y exploración en español.
- [x] Conectar teclado A–Z y Ñ del mando izquierdo con resultados y selección.
- [x] Permitir mover tarjetas con su asa; conservar clic y arrastre en Unity.
- [x] Facilitar extraer, sujetar, evaluar y devolver piezas.
- [x] Añadir desplazamiento con palanca izquierda y giro cómodo con la derecha.
- [x] Regenerar escenas, ejecutar pruebas de editor y de juego y revisar captura visual.
- [x] Compilar un APK firmado para Quest; queda pendiente la prueba física con el visor.
