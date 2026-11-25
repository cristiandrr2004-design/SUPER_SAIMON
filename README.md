# Súper Saimón

Videojuego educativo 2D de plataformas sobre los principios y valores del **SER UNISIMÓN**.
Súper Saimón recorre espacios de la universidad, supera obstáculos y evita a **Monsai**, el antagonista que intenta sabotear su camino.

**Prototipo actual: Nivel 1 (salón de clases).** Súper Saimón sube por los muebles del salón para recuperar los **5 libros perdidos** mientras evita a Monsai.

## Integrantes

| Integrante | Usuario de GitHub | Rol |
|---|---|---|
| Leonardo Rubén Islas Escamilla | `@usuario` | |
| Mahonri Isaac Delgado Alcalá | `@usuario` | |
| Angie Marcela Pérez Fontalvo | `@usuario` | |
| Cristian David Ruiz Ríos | `@cristiandrr2004-design` | |

## Cómo ejecutarlo

1. Instalar **Unity 6.6 (6000.6.2f1)** desde Unity Hub.
2. Clonar el repositorio: `git clone https://github.com/cristiandrr2004-design/SUPER_SAIMON.git`
3. En Unity Hub: **Add > Add project from disk** y elegir la carpeta `SUPER_SAIMON`.
4. Abrir la escena `Assets/Scenes/Nivel1` y presionar **Play**.

## Controles

| Tecla | Acción |
|---|---|
| Flechas o A / D | Caminar |
| Espacio, W o flecha arriba | Saltar |
| T | Correr las pruebas automáticas de mecánicas y físicas |
| R | Reiniciar el nivel después de las pruebas |

## Mecánicas implementadas

- **Moverse:** desplazamiento a izquierda y derecha con `Rigidbody2D`.
- **Saltar y caer:** salto solo desde el suelo, gravedad aumentada (escala 3) y detección de suelo por la normal del contacto.
- **Plataformas de un solo lado:** los muebles del salón usan `PlatformEffector2D`; se atraviesan desde abajo y se pisan desde arriba.
- **Interactuar / recoger:** 5 libros con collider *trigger*; el contador muestra `Libros: x / 5`.
- **Evitar:** Monsai patrulla el piso; al tocarlo, Saimón pierde una vida, sale empujado y queda invencible 1,5 s.
- **Vidas:** 3 vidas. Caer del escenario quita una vida y devuelve al inicio.
- **Victoria y derrota:** al recoger todos los libros aparece el slogan *"¡Aquí gané con libertad y disciplina!"*; sin vidas aparece *"¡Monsai te detuvo!"* con botón para reintentar.
- **Cámara:** sigue a Saimón con suavizado y no se sale de los bordes del salón.

## Pruebas (testing)

Durante el juego, la tecla **T** ejecuta `PruebasFisicas.cs`, que mueve a Saimón automáticamente y verifica:

1. Gravedad: cae y aterriza en el piso.
2. Movimiento horizontal.
3. Altura del salto (> 2 unidades).
4. Pararse sobre un mueble.
5. Atravesar una plataforma de un solo lado desde abajo.
6. Recoger un libro (el contador sube).
7. Daño de Monsai (pierde una vida).
8. Invencibilidad después del golpe.
9. Caída al vacío (vuelve al inicio y pierde una vida).

Los resultados aparecen en pantalla y en la **Console** como `APROBADA` / `FALLIDA`.

## Estructura del proyecto

```
Assets/
├── Animaciones/     Clips y Animator Controller de Súper Saimón
├── Audio/           Efectos de sonido y música del nivel
├── Editor/          Herramientas del editor (menú "Super Saimon")
├── Materiales/      Material de física sin fricción
├── Scenes/          Escenas (Inicio, Nivel1)
├── Scripts/         Código del juego en C#
└── Sprites/
    ├── Escenario/   Fondos de los niveles
    ├── Generados/   Libro y chispa
    └── Personajes/  Súper Saimón, Monsai y personajes de fondo
```

### Scripts

| Script | Función |
|---|---|
| `SuperSaimon.cs` | Movimiento, salto, animaciones, vidas, daño y sonidos del protagonista |
| `Monsai.cs` | Patrulla del antagonista, animación de caminar y daño al tocarlo |
| `Libro.cs` | Libro coleccionable: flota, suena y suelta chispas al recogerlo |
| `ContadorLibros.cs` | Interfaz: libros, vidas, pantallas de victoria y derrota |
| `Destello.cs` | Efecto visual de chispas |
| `Mov_camara.cs` | Cámara que sigue al jugador dentro de los límites del escenario |
| `PruebasFisicas.cs` | Pruebas automáticas de mecánicas y físicas |

## Niveles planeados (GDD)

| Mundo | Principio | Escenario |
|---|---|---|
| Dignitown | Dignidad | Aula de clase |
| Autonomitown | Autonomía | Biblioteca José Martí |
| Sinergitown | Sinergia | Aulas / auditorio |
| Equitown | Equidad | Casa Blanca |
