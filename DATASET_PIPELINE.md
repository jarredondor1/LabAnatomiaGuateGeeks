# Pipeline BodyParts3D

Fuente oficial: https://dbarchive.biosciencedbc.jp/en/bodyparts3d/download.html
Licencia: https://dbarchive.biosciencedbc.jp/en/bodyparts3d/lic.html

Colocar en `SourceData/`:

- isa_BP3D_4.0_obj_99.zip
- isa_parts_list_e.txt
- isa_element_parts.txt
- isa_inclusion_relation_list.txt
- partof_parts_list_e.txt
- partof_element_parts.txt
- partof_inclusion_relation_list.txt

Base de descarga: `https://dbarchive.biosciencedbc.jp/data/bodyparts3d/LATEST/`. Los archivos ya fueron descargados para esta implementación; SourceData está excluida de Git para evitar duplicar el archivo original. Las mallas derivadas sí se versionan.

Ejecutar `python3 Tools/AnatomyPipeline/import.py` desde cualquier directorio. También disponible en el menú Import Dataset del Editor. Usa Python estándar, no requiere instalar bibliotecas. El proceso conserva el archivo original sin cambios y registra su SHA256.

Pasos: unir conceptos y relaciones → leer OBJ desde ZIP → convertir `(x,y,z)` a `(x,z,y)` e invertir caras → normalizar uniformemente la altura global a 1.75 m → centrar pivotes individuales → escribir geometría binaria por sistema → catálogo JSON → reporte. No se modifica la relación espacial entre piezas. Índices negativos OBJ y triangulación por abanico soportados. El importador elimina únicamente binarios generados obsoletos y sus metas.

Formato de malla little-endian: int32 vertexCount, int32 triangleCount, float32 XYZ por vértice, int32 por índice. Normales recalculadas en Unity. Tamaños verificados en tests. `bounds` usa la representación de JsonUtility (`m_Center`, `m_Extent`). FMA es un concepto; FJ es una malla. No deben confundirse al contar estructuras.

Sistema asignado por coincidencia determinista de conceptos de origen. Esto no constituye clasificación anatómica revisada. Las traducciones cubren unos términos frecuentes; resto conserva nombre inglés oficial. Los conceptos completos apuntan a varias piezas, no crean mallas duplicadas.

`optimize.py` genera auditoría antes/después sin alterar forma; `generate_lod.py` es una herramienta opcional para Blender con ratio explícito y validación visual obligatoria. No usar LOD experimental para material educativo sin revisión. `export.py` valida encabezados y copia artefactos empaquetados a destino local solicitado.
