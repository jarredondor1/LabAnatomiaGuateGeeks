# ADR-003: selección central de bounds y física lazy

Rayos XRI manejan UI y grabs; las piezas no extraídas se seleccionan con ray/bounds centralizado. No se crean 2,234 colliders o scripts. Solo cada pieza extraída recibe BoxCollider + Rigidbody kinematic + XRGrabInteractable. Compromiso: picking aproximado ante superposición; aislado/buscador permiten seleccionar piezas internas. Mejorar con BVH y triangle picking si pruebas de usabilidad lo requieren. El collider complejo no es necesario para el primer grab.
