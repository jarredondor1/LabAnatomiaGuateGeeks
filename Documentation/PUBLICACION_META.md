# Publicación de GuateGeeks Anatomía VR en Meta Horizon

## Actualización 0.1.7 — Alpha, 9 de octubre de 2026

- Se compiló `Builds/GuateGeeksAnatomiaVR-release-0.1.7.apk` y se cargó al canal privado **Alpha** con la opción de publicación inmediata de Meta VR CLI. Meta asignó el ID de compilación `1281575331715423`; Meta Quest Developer Hub la muestra como compilación actual de Alpha para sus dos usuarios.
- Incluye estatura personalizada, sistemas anatómicos adicionales, descripciones ampliadas y botones reorganizados. El paquete conserva `com.guategeeks.anatomylabvr`, versión `0.1.7` (código `8`) y el certificado de publicación SHA-256 `ec4bea24881778e008b03b64dff58fa6797efc7331a80a17c5e925f86474e9e6`. SHA-256 del APK: `c1feed8e25de662cdf0c0515c3a947f420183566aa1beb867aa94cbcb4a57f68`.
- Unity completó la compilación y la utilidad de Meta validó el paquete sin errores. Las pruebas de compatibilidad de Meta seguían en curso al cargarlo.
- El panel web de Meta todavía indica `Draft` para esta compilación. Su confirmación de edad 13+ devuelve el error `1891841` porque la solicitud pública `0.1.0` continúa `Under Review`. Se asignó explícitamente `TEENS_AND_ADULTS` a `0.1.7` en Alpha mediante la utilidad oficial, pero no se ha comprobado una actualización dentro de un Quest físico. No confundir la asignación al canal con una instalación confirmada en el visor.

## Actualización 0.1.5 — Alpha, 9 de octubre de 2026

- Se compiló y cargó `Builds/GuateGeeksAnatomiaVR-release-0.1.5.apk` al canal privado **Alpha** mediante el panel web de Meta. El canal muestra `0.1.5` (código `6`) como compilación actual para sus dos usuarios; ID de compilación `1281507365055553`.
- El paquete Android conserva `com.guategeeks.anatomylabvr` y el certificado de publicación. SHA-256: `9434c219b0e8ae1dd824d0d1e025f247755a7213a53fcea3797d4ec21d3c8adf`.
- **Pendiente de activación:** Meta marca la compilación como `Draft` porque falta confirmar el grupo de edad 13+. El botón de confirmación devuelve el error `1891841`: la entrega pública `0.1.0` está `Under Review` y no admite modificaciones. La presencia de `0.1.5` como compilación actual de Alpha no confirma que los visores puedan descargarla. Comprobar que cambie a un estado publicable antes de anunciarla como actualización disponible.

## Actualización 0.1.4 — Alpha, 9 de octubre de 2026

- Se generó `Builds/GuateGeeksAnatomiaVR-release-0.1.4.apk` con la distribución nueva de botones, ajuste de estatura, sistemas anatómicos adicionales y descripciones ampliadas.
- Identificador Android `com.guategeeks.anatomylabvr`; versión `0.1.4`, código `5`. El APK contiene `01_Bootstrap` y `02_AnatomyLab`, fue firmado con el mismo certificado que `0.1.3` y su SHA-256 es `36387f81ff412c925fedfd8312acfa5f51b56a9969d51112cede5a6d922f8a94`.
- Unity informó compilación correcta; la compilación directa de los scripts terminó sin errores. No se pudo comprobar esta actualización dentro de un Quest físico porque no había un visor conectado al finalizar la carga.
- Meta Quest Developer Hub muestra `0.1.4` (código `5`) como compilación actual del canal privado **Alpha**, con dos usuarios y soporte para Quest 2, Quest Pro, familia Quest 3 y dispositivos futuros. ID de compilación en Meta: `1281460098393613`.
- En el panel de Meta, las pruebas automáticas de malware y seguridad aún aparecían en curso al terminar esta sesión. El envío público original sigue **Under Review** y Production (Store) permanece en `0.1.0`.
- Por una limitación del selector de archivos de Developer Hub, el registro remoto muestra el nombre `GuateGeeksAnatomiaVR-release-0.1.3.apk`; se reemplazó temporalmente el contenido de esa ruta por el paquete `0.1.4` durante la carga y se restauró luego. El manifiesto y el panel de Meta confirman versión `0.1.4`, código `5`.
- Meta sigue mostrando la advertencia de confirmación de edad 13+ para las compilaciones Alpha. La misma advertencia y estado `Draft` aparecen en `0.1.3`, que ya se había probado en Quest. Al intentar confirmar `0.1.4`, Meta devolvió error `1891841` porque la solicitud pública está en revisión.

## Actualización 0.1.2 — cargada en Alpha el 7 de octubre de 2026

- Paquete firmado: `Builds/GuateGeeksAnatomiaVR-release-0.1.2.apk`.
- Identificador Android: `com.guategeeks.anatomylabvr`; versión `0.1.2`, código `3`.
- El paquete incluye las escenas `01_Bootstrap` y `02_AnatomyLab`, mantiene el certificado de firma de `0.1.1` y declara soporte OpenXR para Quest.
- SHA-256 del paquete: `3ca73ade3d611275a67c38e5f45f21a35be566a106d17889a7c3743599d84677`.
- Meta Quest Developer Hub muestra `0.1.2` (código `3`) en el canal privado **Alpha** de la aplicación `1277011645505125`, con dos usuarios y compatibilidad con Quest 2, Quest Pro, familia Quest 3 y dispositivos futuros. Tras la carga indicó «Build 0.1.2 is under test»; al consultar de nuevo ofreció «Open Build Page», señal de que terminó la etapa inicial de pruebas automáticas.
- Se corrigió la posición inicial del visor y la distribución de tarjetas para evitar que la interfaz aparezca como una pantalla próxima y distorsionada. En Unity pasó la prueba de disposición espacial `SpatialPanelsStayReadableInQuestWithoutCoveringTheBody`. Todavía no se verificó el resultado dentro de un Quest físico.
- Production (Store) permanece en `0.1.0` y la solicitud pública sigue en revisión.

## Actualización 0.1.1 — cargada en Alpha el 7 de octubre de 2026

- Paquete de actualización firmado: `Builds/GuateGeeksAnatomiaVR-release.apk`.
- Identificador Android: `com.guategeeks.anatomylabvr`; versión `0.1.1`, código `2`.
- El certificado de firma coincide con el de la versión `0.1.0` y el paquete contiene bibliotecas ARM64/OpenXR.
- SHA-256 del paquete: `3bd3dd09c09b5491d674a9e3f6d4f536e4ad8e1304f8510189dad6f7174c6eb6`.
- Cargada con Meta Quest Developer Hub al canal privado **Alpha** de la aplicación `1277011645505125` el 7 de octubre de 2026. Meta muestra `0.1.1` (código `2`) como compilación actual para Quest 2, Quest Pro, familia Quest 3 y dispositivos futuros; el canal conserva sus dos usuarios.
- Tras la carga, Meta indicó «Build 0.1.1 is under test»; después Developer Hub dejó de mostrar ese aviso y ofreció «Open Build Page». La compilación de Production (Store) continúa en `0.1.0` y la solicitud pública sigue «Under Review». No se ha verificado la ejecución en un Quest físico desde esta sesión.
- Captura de comprobación del canal: `Documentation/Meta/alpha-0.1.1.png`.

## Solicitud en Meta — enviada a revisión el 6 de octubre de 2026

- Equipo: GuateGeeks.
- Aplicación: GuateGeeks Anatomía VR (`1277011645505125`).
- Cuenta de desarrollador: `fs.joaquin.arredondo@gmail.com`.
- Perfil de Meta Horizon: `jarredondo`.
- Precio: gratis.
- Edad mínima autocertificada: adolescentes y adultos (13 años o edad aplicable en la región).
- Ficha: https://developers.meta.com/vr/manage/applications/1277011645505125/submissions/1277011658838457/
- Política de privacidad pública: https://github.com/GuateGeeks/bodyvr-privacidad/blob/main/README.md
- Canal de compilación: Production (Store). La compilación `0.1.0` está cargada, procesada y enlazada a esta solicitud.
- Ficha: nombre, categoría, compatibilidad, detalles, imágenes e ícono completos en Meta.
- Contenido: autocertificación de 13+ completada. Certificado IARC `0737f2aa-d31a-85f5-85f0-3fa41c8ba8ba` emitido y guardado en la ficha. La clasificación de ESRB para las Américas es Teen; las otras regiones muestran sus propias clasificaciones.
- El titular aceptó el Developer Distribution Agreement y las condiciones para casting, grabación y emisión en directo; las tres funciones quedaron activadas.
- Meta confirmó: «Submission successful! Your app has been submitted for review.» La aprobación y salida pública siguen pendientes.

## Prueba en Meta Quest antes de la publicación

La misma compilación `0.1.0` se copió al canal privado Alpha; Meta muestra la prueba de compilación como `Complete` y compatibilidad de entrega para Quest 2, Quest Pro, familia Quest 3 y dispositivos futuros. El enlace de acceso por URL quedó guardado con aprobación automática:

https://www.meta.com/s/4rM2HcGZp

El propietario debe abrirlo con su cuenta Meta Horizon `jarredondo`, declarar que es mayor de 18 años, aceptar los términos de la invitación y elegir `Join Release Channel`. Después podrá buscar **GuateGeeks Anatomía VR** en la biblioteca del visor o en **My Preview Apps** de la tienda e instalarla desde Meta. Esta prueba es privada; no equivale a la publicación pública. No se ha instalado ni probado físicamente en un visor desde esta sesión.

## Archivos preparados

- Versión firmada para publicación: `Builds/GuateGeeksAnatomiaVR-release.apk`.
- Cinco capturas reales de la aplicación: `Documentation/Store/01-vista-masculina.png` a `05-utero.png` (2560 × 1440).
- Portada horizontal: `Documentation/Store/portada-horizontal.png` (2560 × 1440).
- Banner panorámico: `Documentation/Store/portada-hero.png` (3000 × 900; arte promocional basado en la experiencia).
- Ícono: `Documentation/Store/icono-guategeeks.png` (512 × 512; logotipo proporcionado por GuateGeeks).

El paquete Android de producción usa `com.guategeeks.anatomylabvr`, versión `0.1.0` (código `1`), ARM64 y firma de publicación. El canal Alpha tiene asignada la versión `0.1.7` (código `8`). La clave de firma se conserva fuera del proyecto en `~/Documents/GuateGeeks/BodyVR-signing/bodyvr-release.keystore`; la contraseña está en el llavero de macOS, servicio `com.guategeeks.anatomylabvr.signing`, cuenta `bodyvr-release`. Hay que conservar una copia segura de ambos para futuras actualizaciones. No subir la clave al repositorio.

## Pendiente tras el envío

- Probar instalación, arranque, mandos, lectura de tarjetas y restauración de piezas en un Meta Quest físico. No había un visor conectado durante la preparación.
- Esperar la aprobación de Meta. Después, configurar la salida pública desde el panel; la solicitud en revisión todavía no hace buscable la aplicación para el público.

La APK firmada es el paquete que Meta distribuye tras aprobar la aplicación; entregarla de forma aislada no la convierte en una app pública y buscable en Horizon.
