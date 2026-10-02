# Verificación reproducible

`Tools/run_unity.sh edit-tests`: valida catálogo completo, duplicados, sistemas, búsquedas acentuadas/bilingües, filtrado, visibilidad/reset, exploded position y todos los índices de las 2,234 mallas.

`Tools/run_unity.sh play-tests`: carga la escena real y prueba selección/aislamiento, extracción/retorno, umbral de snap, visibilidad/reset/transparencia, scoring y carga/focus desde búsqueda. Resultados NUnit XML en TestResults. Las pruebas usan temporalmente el sistema urinario para minimizar tiempo de carga y restauran preferencias de sistemas.

`Tools/run_unity.sh preview`: Play Mode y render de cámara real a Documentation/Previews/laboratory.png. `Tools/run_unity.sh audit`: validación estática. Una imagen no prueba controles XR físicos.

`03_PerformanceTest` carga todo y mide 100/500/1000/dataset completo visible durante 10 segundos por escenario, tras 2 segundos de estabilización. Escribe performance.csv en Application.persistentDataPath. Su métrica de memoria representa todo el dataset cargado en estos escenarios, no carga parcial. En APK usar adb logcat para ruta y exportar CSV desde almacenamiento de la aplicación.

Distinguir siempre: Editor verified, Android build verified, Quest physical device verified. Solo el último requiere hardware. Evidencia final en IMPLEMENTATION_REPORT y PROJECT_STATUS.
