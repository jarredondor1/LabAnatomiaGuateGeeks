# Quest / Android

Unity 6000.6.1f1; URP 17.6.0; XRI 3.6.1; OpenXR 1.18.0; XR Management 4.7.0; Input System 1.20.0. Android IL2CPP ARM64, min SDK 32 / target SDK 34. Vulkan primero y OpenGLES3 fallback. OpenXR Meta Quest y Oculus Touch activados, single-pass instanced, tracking floor. El escritorio usa altura simulada; el headset usa tracking real sin duplicar altura.

Identificador: `com.guategeeks.anatomylabvr`. Build de desarrollo, datos incluidos offline. No se solicita permiso INTERNET expresamente. El build de desarrollo de Unity puede incluir capacidades de depuración; revisar manifest final para distribución.

Generar desde menú o `Tools/run_unity.sh build`. Instalar manualmente con adb: `adb install -r Builds/GuateGeeksAnatomyLabVR.apk`. La instalación no se ejecuta automáticamente ni modifica apps ajenas. Conectar Quest en modo desarrollador y autorizar USB para pruebas físicas.

Checklist físico pendiente: altura y orientación; gatillo y panel; búsqueda con teclado; grips izquierdo/derecho; feedback háptico; snap por distancia/ángulo; comfort del giro; regreso/reset mientras existe agarre; 10 min de cada perfil; CSV de escena PerformanceTest. Registrar modelo, OS, refresh rate, temperatura y CPU/GPU frame time. No declarar 72 FPS basándose en Editor.

XRI XR Interaction Simulator: paquete compatible instalado; sample opcional aún no importado/configurado. El fallback de escritorio está implementado y documentado. Hand tracking sin controladores no está incluido.
