# Ajustes de anclaje y experiencia del laboratorio

**Objetivo:** Mantener el cuerpo sobre la mesa, integrar el nuevo logotipo en el frente de su base y facilitar la lectura y las acciones de las tarjetas.

**Diseño:** La selección y el enfoque cambiarán el resaltado y la ficha, sin trasladar el cuerpo. La rueda del ratón acercará la vista de escritorio en lugar de escalar el modelo. La mesa tendrá un faldón frontal continuo con una insignia enrasada que use el archivo `guategeeks.png` suministrado. Las tarjetas se acercarán al usuario, mostrarán estados claros para la pieza seleccionada o extraída y ofrecerán mejores indicaciones cuando la búsqueda no encuentre resultados.

**Comprobación:** Primero se agregan pruebas de regresión para anclaje, origen del logotipo y estados de extracción; se observa su fallo. Después se implementa el cambio, se regeneran las escenas y se ejecutan las suites de Unity, revisión visual y compilación Android.

- [x] Reproducir con una prueba el desplazamiento del cuerpo al seleccionar una estructura.
- [x] Reproducir con una prueba el logotipo antiguo y los estados ambiguos de extracción.
- [x] Fijar el cuerpo, cambiar el acercamiento de escritorio y mantener la extracción de piezas.
- [ ] Integrar `guategeeks.png` en la base y regenerar las escenas con el tamaño final.
- [x] Mejorar legibilidad, posición y estados de las tarjetas.
- [ ] Verificar pruebas, captura visual y APK para Quest.
