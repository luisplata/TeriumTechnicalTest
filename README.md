# Prueba Tecnica Luis Plata
Hola a todos.
A continuación dejo los links de la prueba terminada.

Link de [Itch](https://peryloth.itch.io/technical-test-teriun-games) y
[GitHub](https://github.com/luisplata/TeriunTechnicalTest)

Aquí quiero resaltar el trabajo que se hizo con github Actions y el CI/CD

[Actions Windows 😎](https://github.com/luisplata/TeriunTechnicalTest/actions)

En esta página se ve como después de compilar se despliega automáticamente en la pagina de itch.io

Todo completamente funcional.

Explicación de la prueba.

Inspirado en la temática y los retos. Quise darme homenaje a un juego que me gusta mucho: Golden Eye 007 de la Nintendo 64
El juego está totalmente inspirado en ese juego. Adicionando mecánicas de aturdimiento y destello a algunas armas para hacerlas más divertidas.
El movimiento también está inspirado, por lo que solo hace falta WASD.
Se dispara con la tecla espacio.
Se puede ver la tabla de jugadores y sus vidas con shift.
El juego cuenta con un `gameloop` lo cual controla toda la instancia del juego.

Para hacer el juego más divertido coloque largos cooldown para cada arma para que tengan chances de ganar.
La vida de cada uno es de 100
1. GoldenEye (Amarillo) Cooldown 1 min, te mata luego de 5 seg.
2. Rocket (Gris) Cooldown 20 seg, te saca 50 puntos de vida y te aturde por 3 seg
3. Mina (Rojo) Cooldown 30 seg, te saca 70 puntos y te ciega por 5 seg
4. Pistola (Negro) Cooldown medio segundo te saca 10 de vida.

Todos los datos son configurables en editor.

Para la parte técnica, se utilizaron diversos patrones de diseño para encapsular varias partes del juego.

- Factory Method: Se utilizó para toda la parte de las armas.
- ServiceLocator: Para encapsular sonidos, música, GameLoop y eventos del juego.
- Template Method: Se usó para encapsular los efectos de cada arma.
- State Pattern: Para marcar el gameloop y sincronizar a todos los jugadores.

Se tiene un panel de "Debug" para mostrar información relevante.
