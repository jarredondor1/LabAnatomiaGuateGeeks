# Licencias y atribuciones

## BodyParts3D 4.0

BodyParts3D, © The Database Center for Life Science licensed under CC Attribution 4.0 International.

- Fuente: https://dbarchive.biosciencedbc.jp/en/bodyparts3d/download.html
- Licencia oficial (actualizada 2025-02-27; revisada 2026-10-02): https://dbarchive.biosciencedbc.jp/en/bodyparts3d/lic.html
- Términos: https://creativecommons.org/licenses/by/4.0/
- Archivos: isa_BP3D_4.0_obj_99.zip, isa_parts_list_e.txt, isa_element_parts.txt, isa_inclusion_relation_list.txt, partof_parts_list_e.txt, partof_element_parts.txt, partof_inclusion_relation_list.txt.
- Las geometrías y metadata derivadas conservan CC BY 4.0. Mantener esta atribución en redistribuciones y dentro de la aplicación.
- Adaptaciones: conversión OBJ a geometría binaria; cambio de ejes, inversión de winding, escala uniforme a 1.75 m, pivotes locales centrados, agrupación heurística por sistemas, términos de búsqueda y traducciones básicas al español. No se ha realizado una nueva simplificación sobre la reducción oficial.
- Los comentarios originales de algunos OBJ citan una licencia histórica; los archivos fuente se conservan intactos. Este proyecto se basa en la licencia vigente publicada por el titular.
- Archivo original y checksum registrados en DATASET_IMPORT_REPORT.md.

## Human Reference Atlas: femenino v1.5

Kristen Browne; Heidi Schlehlein. 2023. *3D Reference Organ Set for Female, v1.5*. Human Reference Atlas / HuBMAP. CC BY 4.0.

- Ficha y licencia verificadas el 2026-10-02: https://lod.humanatlas.io/ref-organ/united-female/v1.5/
- DOI: https://doi.org/10.48539/HBM352.BTSQ.586
- Fuente directa: https://cdn.humanatlas.io/digital-objects/ref-organ/united-female/v1.5/assets/3d-vh-f-united.glb
- Licencia: https://creativecommons.org/licenses/by/4.0/
- 888 mallas originales; 5,557,599 triángulos. Se mantienen todas las piezas, sin combinar con el cuerpo masculino.
- Adaptaciones: reflexión del eje Z e inversión de caras para Unity; traslación común para apoyar los pies en el escenario; pivotes locales; soldadura de vértices coincidentes; normales recalculadas; colores por sistema, traducciones y términos de búsqueda. No se deformó ni simplificó la geometría.
- Conjunto de referencia con superficie corporal y órganos seleccionados. Su esqueleto y musculatura son parciales. Incluye ocho piezas de embarazo en una capa independiente, oculta por defecto.
- Procedencia y checksum: `Tools/AnatomyPipeline/female-import-report.json`. Importador: `Tools/AnatomyPipeline/import_female.py` (Python + NumPy).

## Referencia funcional

Human Atlas: https://github.com/ashemag/human-atlas — consultado como referencia de funciones y atribución. No se incorporó código ni geometría adaptada de ese repositorio. Se usa directamente el dataset oficial. El proyecto de referencia declara MIT para código y CC BY 4.0 para datos; aquí no se necesita redistribuir su código.

## Dependencias

Unity 6000.6.1f1 y paquetes oficiales URP, OpenXR, XR Interaction Toolkit, Input System, Unity UI y Test Framework: mantienen sus licencias incluidas en Packages/PackageCache y distribución de Unity. Consultar las condiciones de Unity para distribuir builds. Sin APIs pagadas ni servicios cloud en runtime.

## Recursos propios

La interfaz y la geometría anatómica son de este proyecto. El panorama, la retícula, el techo, el haz de proyección, el shader `LabLogo.shader` y el recurso `GuateGeeksSDF.png` proceden del repositorio [GuateGeeks AWS VR Architect](https://github.com/GuateGeeks/aws-vr-architect), adaptados a la mesa de anatomía. `GuateGeeksFull.png` y `GeekEyes.png` son los archivos de marca proporcionados por el titular y conservan su transparencia; los ojos reciben animación en este proyecto. No se incorporaron marcas de terceros. El código original permanece bajo los derechos del titular del proyecto; este documento no lo publica ni le asigna una licencia abierta.
