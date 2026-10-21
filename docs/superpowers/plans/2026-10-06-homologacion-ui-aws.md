# Homologación UI y UX con AWS VR Architect Implementation Plan

> **For agentic workers:** implement this plan task-by-task in the current workspace. Steps use checkbox syntax for tracking.

**Goal:** Dar a BodyVR la misma experiencia visual y de acceso a paneles que el laboratorio AWS de GuateGeeks, conservando anatomía, plataforma y controles Quest 2.

**Architecture:** `AnatomyHoloTheme` y `AnatomyHoloGraphic` dibujan un lenguaje visual común. `SpatialLabUI` construye las tres tarjetas anatómicas y el menú rápido, mientras una clase de disposición coloca esas tarjetas según editor o XR sin modificar el modelo. Los controles de selección y extracción se conservan.

**Tech Stack:** Unity 6000.6.1f1, C#, uGUI World Space, URP, XR Interaction Toolkit, NUnit PlayMode/EditMode.

---

### Task 1: Pruebas de homologación

**Files:** `Assets/AnatomyLab/Tests/PlayMode/InteractionTests.cs`, `Assets/AnatomyLab/Tests/EditMode/HoloVisualTests.cs`.

- [x] Añadir una prueba PlayMode que invoque la disposición cercana y compruebe tarjetas a menos de 0.8 m de la vista, menú rápido con acciones anatómicas y retorno a la disposición inicial.
- [x] Añadir una prueba EditMode que valide colores de marca, estados visuales de vidrio luminoso y tirador compacto.
- [x] Ejecutar ambas y confirmar fallos causados por la función aún ausente.

### Task 2: Apariencia y controles

**Files:** `Assets/AnatomyLab/Scripts/UI/AnatomyHoloTheme.cs`, `AnatomyHoloGraphic.cs`, `SpatialLabUI.cs`, un componente de respuesta visual del botón si hace falta.

- [x] Adoptar los colores y la malla de vidrio del repositorio: halo, borde, marcas, recortes y resaltado.
- [x] Dar estados visuales a botones primarios, secundarios y tarjetas de sistemas; sustituir el tirador «MOVER» repetido por una barra discreta.
- [x] Conservar texto y acciones de anatomía en español.
- [x] Ejecutar pruebas y corregir incompatibilidades.

### Task 3: Disposición y acceso Quest

**Files:** `Assets/AnatomyLab/Scripts/UI/SpatialLabUI.cs`, nueva clase de disposición si simplifica responsabilidades, `DESKTOP_EXPLORATION.md`.

- [x] Colocar las tres tarjetas en escritorio sin tapar el cuerpo.
- [x] Colocarlas cerca de la vista inicial en XR; permitir «TRAER AQUÍ» sin desplazar el cuerpo.
- [x] Crear menú rápido abrible con `M` o menú izquierdo y comprobar clic/rayo en sus botones.
- [x] Ejecutar PlayMode y revisar una captura de Game.

### Task 4: Entrega verificable

**Files:** `README.md`, `Documentation/Previews/laboratory.png`, `TestResults/`, `Builds/GuateGeeksAnatomyLabVR.apk`.

- [x] Ejecutar suites EditMode y PlayMode completas y guardar resultados.
- [x] Compilar APK Quest y verificar ZIP y firma Android.
- [x] Revisar captura y diferencias para confirmar diseño, anclaje y ausencia de contenido AWS.
