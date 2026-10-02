# ADR-004: transforms originales inmutables

La dirección sale del centro corporal hacia el pivot de cada pieza, con fallback vertical si coincide. El factor 0..1 desplaza hasta 0.65 m. Un manager interpola con factor exponencial independiente del framerate solo estados dirty. Extraer suspende interpolación de esa pieza. Retorno visita el transform original y se reintegra al factor vigente; reset lleva factor a cero. La cámara nunca se mueve automáticamente para focus.
