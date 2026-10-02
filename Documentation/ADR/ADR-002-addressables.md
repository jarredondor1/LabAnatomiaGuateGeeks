# ADR-002: Resources asíncrono para MVP offline

No se añadió Addressables. Cada mesh se carga bajo demanda con Resources.LoadAsync y descarga explícita; no existe catálogo remoto, contenido descargable ni necesidad de un servicio. La APK incluye todos los bytes, pero no todos permanecen en memoria. Migrar el proveedor a Addressables cuando haga falta distribución de conjuntos adicionales, sin cambiar selección/estados. La carpeta Addressables queda reservada, no representa una integración activa.
