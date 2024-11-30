/*
 * Clase GameManager
 * -------------------
 * Esta clase actúa como el núcleo central del juego "Rebel Assault", gestionando todos los componentes
 * y la lógica principal del juego. Coordina la interacción entre el jugador, enemigos, sistema de combate,
 * y la interfaz de usuario.
 *
 * Características principales:
 * - Control del ciclo de juego y actualización de estados
 * - Gestión de componentes principales (jugador, enemigos, mapa)
 * - Sistema de puntuación y ranking
 * - Control de entrada del usuario
 * - Manejo de colisiones y combate
 * - Gestión de interfaz de usuario (barras de vida, puntuación, tiempo)
 *
 * Componentes gestionados:
 * - MainCharacter: Maneja el personaje principal y sus acciones
 * - EnemyPro: Control de enemigos y su comportamiento
 * - CombatSystem: Sistema de combate y colisiones
 * - ScoreManager: Gestión de puntuaciones y rankings
 * - SoundManager: Control del audio y efectos sonoros
 * - Mapa: Gestión del escenario y desplazamiento
 *
 * Funcionamiento:
 * El GameManager inicializa todos los componentes necesarios y mantiene un bucle de juego
 * que actualiza constantemente el estado de todos los elementos. Coordina el movimiento
 * del jugador, la actualización de enemigos, el sistema de combate y la interfaz visual.
 *
 * Sistema de control:
 * - Teclas de dirección: Movimiento del personaje
 * - Espacio: Salto
 * - Shift: Correr
 * - Z: Disparar
 * 
 * Nota:
 * Esta clase requiere una referencia al formulario principal del juego y una ruta base
 * donde se encuentren todos los recursos necesarios (imágenes, sonidos, etc.).
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Media;

namespace Rebel_AssaultTesting
{
    public class GameManager
    {
        #region Propiedades
        private MainCharacter player;
        private Mapa mapa;
        private CombatSystem combatSystem;
        private HealthBar playerHealthBar;
        private Dictionary<EnemyPro, HealthBar> enemyHealthBars;
        private Timer gameTimer;
        private Form gameForm;
        private List<EnemyPro> enemies;
        private EnemyPro enemyPro;
        private ScoreManager scoreManager;
        private Panel scorePanel;
        private ListBox scoreListBox;
        private SoundManager soundManager;
        private Label scoreLabel;
        private Label timerLabel;
        private string basePath;
        private int elapsedTime;
        private int viewportWidth;
        private int mapWidth;
        private const float MapScrollSpeedFactor = 16f;
        private const int EndMap = 10000;
        private bool gameEnded = false;
        private DateTime lastFrameTime;
        #endregion

        #region Constructor
        public GameManager(Form form, string basePath)
        {
            this.basePath = basePath;
            gameForm = form;
            viewportWidth = form.ClientSize.Width;
            enemies = new List<EnemyPro>();
            InitializeGameComponents(basePath);
            gameForm.Paint += OnGamePaint;
        }
        #endregion

        #region Inicializacion de Componentes

        /// <summary>
        /// Inicializa los componentes principales del juego, incluyendo el jugador, el mapa, los enemigos,
        /// el sistema de combate y el temporizador.
        /// </summary>
        /// <param name="basePath">La ruta base donde se encuentran los recursos del juego, como imágenes y animaciones.</param>
        private void InitializeGameComponents(string basePath)
        {
            InitializePlayer(basePath);
            InitializeSoundManager(basePath);
            InitializeMap(1500, 800);
            InitializeEnemies();
            InitializeCombatSystem();
            InitializeScore();
            InitializeTimer();
            InitializeScorePanel();
            InitializeScoreManager();
            SetupTimer();
        }

        /// <summary>
        /// Inicializa el sistema de combate del juego, configurando las interacciones entre el jugador, los enemigos y el mapa.
        /// </summary>
        private void InitializeCombatSystem() => combatSystem = new CombatSystem(gameForm, player, enemies, mapa);

        /// <summary>
        /// Inicializa al jugador del juego, configurando su sprite, posición inicial, y otros atributos como la velocidad,
        /// la gravedad y el nivel del suelo. También agrega el sprite del jugador al formulario y configura la barra de salud.
        /// </summary>
        /// <param name="basePath">La ruta base para cargar los recursos relacionados con el jugador, como las animaciones y sprites.</param>
        private void InitializePlayer(string basePath)
        {
            PictureBox playerSprite = new PictureBox { SizeMode = PictureBoxSizeMode.AutoSize, BackColor = Color.Transparent };
            player = new MainCharacter(
                caracterSprite: playerSprite,
                initialPosition: new Point(150, 400),
                speed: 11,
                runningSpeed: 30,
                jumpSpeed: 20,
                gravity: 3,
                groundLevel: 400,
                basePath: basePath
            );
            gameForm.Controls.Add(player.Sprite);
            InitializePlayerHealthBar();
        }

        /// <summary>
        /// Inicializa el gestor de puntuaciones (ScoreManager).
        /// Este método crea una nueva instancia de la clase <see cref="ScoreManager"/> 
        /// y la asigna a la variable <see cref="scoreManager"/> para manejar la puntuación del juego.
        /// </summary>
        private void InitializeScoreManager()
        {
            scoreManager = new ScoreManager();
        }

        /// <summary>
        /// Inicializa el componente de la etiqueta que muestra la puntuación del jugador.
        /// Este método crea una nueva instancia de <see cref="Label"/> y configura sus propiedades, 
        /// como la fuente, color, ubicación, tamaño y fondo, para mostrar la puntuación en la pantalla.
        /// La etiqueta es añadida a los controles del formulario del juego.
        /// </summary>
        private void InitializeScore()
        {
            scoreLabel = new Label();
            scoreLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.Location = new Point(20, 20);
            scoreLabel.Size = new Size(200, 30);
            scoreLabel.BackColor = Color.Transparent;

            gameForm.Controls.Add(scoreLabel);
        }

        /// <summary>
        /// Inicializa y configura el panel de puntuaciones del juego.
        /// Este método crea los componentes necesarios para mostrar la interfaz de puntuaciones, 
        /// como el panel principal, la etiqueta de título, la lista de puntuaciones y el botón 
        /// para jugar nuevamente, y los agrega al formulario del juego.
        /// </summary>
        private void InitializeScorePanel()
        {
            scorePanel = CreateScorePanel();
            var titleLabel = CreateTitleLabel();
            scoreListBox = CreateScoreListBox();
            var playAgainButton = CreatePlayAgainButton();

            scorePanel.Controls.AddRange(new Control[] { titleLabel, scoreListBox, playAgainButton });
            gameForm.Controls.Add(scorePanel);

            // Centrar el panel en la pantalla
            scorePanel.Location = new Point(600,50);
            
        }

        /// <summary>
        /// Inicializa el temporizador del juego y configura su etiqueta para mostrar el tiempo transcurrido.
        /// Este método crea un <see cref="Timer"/>, configura su intervalo y lo inicia. Además, crea una etiqueta 
        /// <see cref="Label"/> para mostrar el tiempo transcurrido en la pantalla, y la agrega al formulario del juego.
        /// </summary>
        private void InitializeTimer()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start(); // Iniciar el temporizador

            elapsedTime = 0;

            timerLabel = new Label();
            timerLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            timerLabel.ForeColor = Color.White;
            timerLabel.Location = new Point(10, 50); // Posición debajo de la puntuación
            timerLabel.Size = new Size(200, 30);
            timerLabel.BackColor = Color.Transparent;
            gameForm.Controls.Add(timerLabel);
        }

        /// <summary>
        /// Inicializa la barra de vida del personaje Principal
        /// </summary>
        private void InitializePlayerHealthBar()
        {
            playerHealthBar = new HealthBar(new Point(player.Position.X, player.Position.Y - 20), 100, 10, 100);
            gameForm.Controls.Add(playerHealthBar.Container);
        }

        /// <summary>
        /// Inicializa el mapa del juego, configurando su nombre, posición, y dimensiones (ancho y alto).
        /// </summary>
        /// <param name="width">El ancho del mapa.</param>
        /// <param name="height">El alto del mapa.</param>
        private void InitializeMap(int width, int height)
        {
            mapa = new Mapa("map", 0, 0, width, height);
            mapWidth = mapa.Width;
        }


        /// <summary>
        /// Inicializa a los enemigos del juego, creando un conjunto de enemigos en ubicaciones específicas.
        /// Se asigna una barra de salud a cada enemigo y se configura su posición en el mapa.
        /// </summary>
        private void InitializeEnemies()
        {
            enemyHealthBars = new Dictionary<EnemyPro, HealthBar>();
            for (int i = 900; i <= 10000; i += 600)
            {
                int Ground = 445;
                CreateEnemy(new Point(i, Ground), "EnemyPro", player);
            }

        }

        /// <summary>
        /// Inicializa los sonidos del Juego
        /// </summary>
        /// <param name="BasePath"></param>
        private void InitializeSoundManager(string BasePath) //Pendiente documentacion XML
        {
            soundManager = new SoundManager(BasePath);
            soundManager.PlayInitialSoundAndBackground();
        }

        #endregion

        #region Creacion del ScorePanel
        /// <summary>
        /// Crea y configura el panel de puntuaciones.
        /// </summary>
        /// <returns>Un nuevo objeto Panel configurado.</returns>
        private Panel CreateScorePanel()
        {
            var panel = new Panel
            {
                Size = new Size(400, 315),
                BackColor = Color.FromArgb(200, Color.Black),
                Visible = false
            };
            return panel;
        }

        /// <summary>
        /// Crea y configura la etiqueta de título para las puntuaciones.
        /// </summary>
        /// <returns>Un nuevo objeto Label configurado.</returns>
        private Label CreateTitleLabel()
        {
            return new Label
            {
                Text = "HIGH SCORES",
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 37
            };
        }

        /// <summary>
        /// Crea y configura la lista de puntuaciones.
        /// </summary>
        /// <returns>Un nuevo objeto ListBox configurado.</returns>
        private ListBox CreateScoreListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                Font = new Font("Arial", 12),
                BorderStyle = BorderStyle.None
            };
        }

        /// <summary>
        /// Crea y configura el botón de "Jugar de nuevo".
        /// </summary>
        /// <returns>Un nuevo objeto Button configurado.</returns>
        private Button CreatePlayAgainButton()
        {
            var button = new Button
            {
                Text = "Jugar de nuevo",
                Dock = DockStyle.Bottom,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, Color.Green),
                ForeColor = Color.White
            };

            button.Click += (s, e) => Application.Restart();
            return button;
        }

        #endregion

        #region HighScore

        /// <summary>
        /// Muestra las puntuaciones más altas en un <see cref="ListBox"/> dentro del panel de puntuaciones.
        /// Este método recibe una lista de <see cref="ScoreEntry"/> y muestra las 10 mejores puntuaciones ordenadas en el 
        /// <see cref="scoreListBox"/>. El panel de puntuaciones se hace visible y se coloca al frente de la interfaz.
        /// </summary>
        /// <param name="scores">Una lista de objetos <see cref="ScoreEntry"/> que contiene las puntuaciones a mostrar.</param>
        public void ShowHighScores(List<ScoreEntry> scores)
        {
            scoreListBox.Items.Clear();

            scoreListBox.Items.Add("");
            scoreListBox.Items.Add("");
            scoreListBox.Items.Add("Rank            Name                       Score");
            scoreListBox.Items.Add("------------------------------------------------------");
            // Agregar scores ordenados
            int rank = 1;
            foreach (var score in scores.OrderByDescending(s => s.Score).Take(10))
            {
                scoreListBox.Items.Add($"{rank}\t{score.PlayerName}\t\t{score.Score:N0}");
                rank++;
            }

            scoreListBox.Invalidate();
            scorePanel.Visible = true;
            scorePanel.BringToFront();

        }

        /// <summary>
        /// Finaliza el juego y guarda la puntuación final del jugador.
        /// Este método recibe la puntuación final y el nombre del jugador, guarda la puntuación utilizando el 
        /// <see cref="scoreManager"/> y luego muestra las puntuaciones más altas llamando a <see cref="DisplayTopScores"/>.
        /// </summary>
        /// <param name="finalScore">La puntuación final obtenida por el jugador.</param>
        /// <param name="playerName">El nombre del jugador que obtuvo la puntuación.</param>
        private void EndGame(int finalScore, string playerName)
        {
            scoreManager.SaveScore(playerName, finalScore);
            DisplayTopScores();
        }

        /// <summary>
        /// Muestra las puntuaciones más altas almacenadas.
        /// Este método carga las puntuaciones utilizando el <see cref="scoreManager"/> y luego 
        /// las pasa al método <see cref="ShowHighScores"/> para que sean mostradas en la interfaz.
        /// </summary>
        private void DisplayTopScores()
        {
            List<ScoreEntry> scores = scoreManager.LoadScores();
            ShowHighScores(scores);
        }

        /// <summary>
        /// Controla el evento <see cref="Timer.Tick"/> para actualizar el tiempo transcurrido.
        /// Este método se ejecuta cada vez que el temporizador alcanza su intervalo. Incrementa el tiempo transcurrido,
        /// calcula los minutos y segundos y actualiza la etiqueta <see cref="timerLabel"/> para mostrar el tiempo en formato "mm:ss".
        /// </summary>
        /// <param name="sender">El objeto que genera el evento (en este caso, el temporizador).</param>
        /// <param name="e">Los argumentos del evento.</param>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;

            int minutes = elapsedTime / 60; // Calcular minutos
            int seconds = elapsedTime % 60; // Calcular segundos
            timerLabel.Text = $"Time: {minutes:D2}:{seconds:D2}"; // Formato 00:00
        }

        #endregion

        #region Creacion del Enemigo

        /// <summary>
        /// Crea un nuevo enemigo en una posición específica del mapa y lo agrega a la lista de enemigos.
        /// Además, crea y asigna una barra de salud al enemigo.
        /// </summary>
        /// <param name="position">La posición en el mapa donde se debe crear el enemigo.</param>
        /// <param name="tag">El tipo o etiqueta del enemigo (por ejemplo, "EnemyPro").</param>
        /// <param name="player">El jugador que interactúa con el enemigo.</param>
        public void CreateEnemy(Point position, string tag, MainCharacter player)
        {
            var enemySprite = new PictureBox { }; 
            var enemy = CreateEnemyInstance(position, tag, enemySprite, player);
            enemies.Add(enemy);
            CreateAndAddHealthBar(position, enemy);
        }

        /// <summary>
        /// Crea e instancia un nuevo objeto de tipo <see cref="EnemyPro"/> con los parámetros proporcionados.
        /// </summary>
        /// <param name="position">La posición en el mapa donde se debe colocar al enemigo.</param>
        /// <param name="tag">La etiqueta o tipo del enemigo (por ejemplo, "EnemyPro").</param>
        /// <param name="enemySprite">El sprite visual del enemigo, representado por un <see cref="PictureBox"/>.</param>
        /// <param name="player">El jugador que es el objetivo del enemigo (se utiliza para la interacción y el movimiento del enemigo).</param>
        /// <returns>Un objeto de tipo <see cref="EnemyPro"/> completamente configurado.</returns>
        private EnemyPro CreateEnemyInstance(Point position, string tag, PictureBox enemySprite, MainCharacter player)
        {
            return new EnemyPro(
                sprite: enemySprite,
                position: position,
                tag: tag,
                health: 100,
                speed: 5,
                currentAnimation: 0,
                groundLevel: 445,
                isAttacking: false,
                isActive: true,
                enemyType: "EnemyPro1",
                spawnPosition: position.X,
                basePath: basePath,
                targetPlayer: player,
                gameF: gameForm
            );
        }

        /// <summary>
        /// Crea una barra de vida para un enemigo y la agrega al formulario del juego.
        /// </summary>
        /// <param name="position">La posición donde se debe colocar la barra de vida, generalmente ajustada respecto a la posición del enemigo.</param>
        /// <param name="enemy">El enemigo al que se le creará la barra de vida, utilizado para obtener la salud del enemigo.</param>
        private void CreateAndAddHealthBar(Point position, EnemyPro enemy)
        {
            var enemyHealthBar = new HealthBar(new Point(position.X, position.Y - 20), 80, 10, enemy.Health);
            enemyHealthBars[enemy] = enemyHealthBar;
            gameForm.Controls.Add(enemyHealthBar.Container);
        }
        #endregion

        #region Actualizacion Barras de Vida
        /// <summary>
        /// Actualiza las posiciones de las barras de vida del jugador y de todos los enemigos.
        /// La posición de cada barra de vida se ajusta en función de la posición de su personaje respectivo.
        /// </summary>
        private void UpdateHealthBarsPosition()
        {
            // Actualizar la posición de la barra de vida del jugador
            playerHealthBar.UpdatePosition(new Point(player.Position.X - mapa.OffsetX, player.Position.Y - 20));

            // Actualizar la posición de las barras de vida de los enemigos
            foreach (var enemy in enemies)
            {
                if (enemyHealthBars.ContainsKey(enemy))
                {
                    // Posiciona la barra de vida del enemigo justo encima de su sprite
                    Point newHealthBarPosition = new Point(enemy.GetPosition().X - mapa.OffsetX, enemy.GetPosition().Y - 20);
                    enemyHealthBars[enemy].UpdatePosition(newHealthBarPosition);
                }
            }
        }
        #endregion

        #region Logica del dibujado

        /// <summary>
        /// Maneja el evento de pintura del juego, donde se dibujan el mapa, el jugador y los enemigos en la pantalla.
        /// Este método se llama en cada ciclo de renderizado para actualizar la visualización del juego.
        /// </summary>
        /// <param name="sender">El objeto que genera el evento de pintura (en este caso, el formulario de juego).</param>
        /// <param name="e">Argumentos del evento que contienen la información de gráficos para realizar la pintura.</param>
        private void OnGamePaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Dibujar el mapa y el personaje
            mapa.Dibujar(g);
            player.Sprite.Location = new Point(player.Position.X - mapa.OffsetX, player.Position.Y);
            g.DrawImage(player.Sprite.Image, player.Sprite.Location);
            // Dibujar los enemigos
            foreach (var enemy in enemies)
            {
                if (enemy.Sprite.Visible && enemy.Sprite.Image != null)
                {
                    int visualX = enemy.GetPosition().X - mapa.OffsetX;
                    g.DrawImage(enemy.Sprite.Image,
                              new Point(visualX, enemy.GetPosition().Y));
                }
            }
        }

        /// <summary>
        /// Configura el temporizador del juego para controlar el ritmo de la actualización del juego.
        /// Establece un intervalo de 50 milisegundos (20 FPS) y asocia el evento Tick a la función de bucle de juego (GameLoop).
        /// </summary>
        private void SetupTimer()
        {
            gameTimer = new Timer { Interval = 50 }; // 20 fps
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        /// <summary>
        /// El bucle principal del juego que se ejecuta en cada intervalo del temporizador.
        /// Calcula el tiempo transcurrido entre frames (deltaTime), actualiza la lógica del juego y fuerza el redibujado de la interfaz.
        /// </summary>
        /// <param name="sender">El objeto que genera el evento (en este caso, el temporizador).</param>
        /// <param name="e">Los argumentos del evento (en este caso, los eventos generados por el temporizador).</param>
        private void GameLoop(object sender, EventArgs e)
        {
            TimeSpan deltaTime = DateTime.Now - lastFrameTime;
            lastFrameTime = DateTime.Now;

            UpdateGame(deltaTime);
            gameForm.Invalidate(); // Forzar el redibujado del formulario
        }
        #endregion

        #region Metodos Update

        /// <summary>
        /// Actualiza los componentes del juego en cada frame. Este método maneja el movimiento del jugador, la actualización de las barras de vida, 
        /// la lógica de los enemigos, la animación del jugador y la actualización de sus posiciones visuales.
        ///
        /// <para>Se calcula la cantidad de desplazamiento del mapa, se actualiza la posición de los enemigos, se anima al jugador y se actualizan 
        /// las posiciones de las barras de vida de los personajes.</para>
        ///
        /// <param name="deltaTime">El tiempo transcurrido desde el último frame. Se usa para hacer que el movimiento sea dependiente del tiempo.</param>
        /// </summary>
        private void UpdateGame(TimeSpan deltaTime)
        {
            if (gameEnded) return;
            //velocidad del mapa para que coincida con el movimiento del personaje
            float scrollAmount = (float)(player.Speed * deltaTime.TotalSeconds * MapScrollSpeedFactor);

            if (player.MovingRight || player.MovingLeft) HandleMovement(scrollAmount);

            if (IsPlayerNearMapEnd()) EndGameAndShowScores();
            UpdateLogic();
        }

        /// <summary>
        /// Actualiza la lógica del juego en cada ciclo.
        /// Este método actualiza varios aspectos del juego en cada cuadro, incluyendo la salud del jugador, 
        /// el estado de los enemigos, la animación del jugador, y otros elementos visuales como la posición y las barras de salud.
        /// </summary>
        private void UpdateLogic()
        {
            playerHealthBar.CurrentHealth = player.Health; //Actualiza la vida del personaje principal
            UpdateEnemies();
            UpdateEnemiesPosition();
            player.MoveAndAnimate();
            UpdatePlayerVisualPosition();
            UpdateHealthBarsPosition();
            UpdateScore();
        }

        /// <summary>
        /// Actualiza la puntuación mostrada en la interfaz.
        /// Este método actualiza el texto de la etiqueta de puntuación para reflejar la puntuación actual del sistema de combate.
        /// </summary>
        private void UpdateScore()
        {
            scoreLabel.Text = "Score: " + combatSystem.Score.ToString();
        }


        /// <summary>
        /// Actualiza la lógica de los enemigos, maneja la eliminación de enemigos cuando su salud es baja 
        /// y actualiza las barras de vida de los enemigos restantes.
        ///
        /// <para>Este método recorre la lista de enemigos, actualiza la lógica de cada uno, verifica si su salud 
        /// es inferior o igual a 10 y, en ese caso, elimina al enemigo y su barra de salud del juego. Si la salud del 
        /// enemigo es mayor a 10, se actualiza su barra de vida.</para>
        ///
        /// </summary>
        private void UpdateEnemies()
        {
            foreach (var enemy in enemies.ToList())
            {
                enemy.UpdateEnemyLogic();

                if (enemy.Health <= 10)
                {
                    if (enemyHealthBars.ContainsKey(enemy))
                    {
                        gameForm.Controls.Remove(enemyHealthBars[enemy].Container);
                        enemyHealthBars.Remove(enemy);
                    }

                    gameForm.Controls.Remove(enemy.Sprite); //Eliminar Sprite del enemigo

                    enemies.Remove(enemy); //Eliminar el enemigo de la lista

                }
                else 
                {
                    if (enemyHealthBars.ContainsKey(enemy)) enemyHealthBars[enemy].CurrentHealth = enemy.Health; // Actualizar barra de vida
                }
            }
        }


        /// <summary>
        /// Actualiza la posición visual de cada enemigo en la pantalla considerando el desplazamiento del mapa.
        /// <para>Este método recorre todos los enemigos en la lista y ajusta su posición visual sobre el formulario. 
        /// La posición en el eje X del enemigo se ajusta restando el desplazamiento del mapa, lo que permite que el mapa 
        /// y los enemigos se desplacen juntos, proporcionando una experiencia de desplazamiento más fluida.</para>
        /// </summary>
        private void UpdateEnemiesPosition()
        {
            foreach (var enemy in enemies)
            {
                // Calcular la posición visual del enemigo considerando el desplazamiento del mapa
                int visualX = enemy.GetPosition().X - mapa.OffsetX;


                enemy.Sprite.Location = new Point(visualX, enemy.Sprite.Top);
                enemy.Sprite.Visible = true;
            }
        }

        /// <summary>
        /// Actualiza la posición visual del personaje en la pantalla considerando el desplazamiento del mapa.
        /// <para>Este método ajusta la ubicación del sprite del personaje, desplazándolo en la pantalla en función de su posición global 
        /// y el desplazamiento actual del mapa, asegurando que el personaje siempre se dibuje en la ubicación correcta.</para>
        /// </summary>
        private void UpdatePlayerVisualPosition()
        {
            int visualX = player.Position.X - mapa.OffsetX;
            player.Sprite.Location = new Point(visualX, player.Position.Y);
        }

        /// <summary>
        /// Maneja el movimiento del personaje y el desplazamiento del mapa según la dirección de movimiento del jugador.
        /// <para>Este método ajusta la posición del personaje en la pantalla, moviéndolo a la derecha o a la izquierda dependiendo 
        /// de la dirección en que se está moviendo. Si el jugador supera ciertos umbrales de posición, también desplazará el mapa 
        /// para simular el avance del jugador en el mundo del juego.</para>
        /// </summary>
        /// <param name="scrollAmount">La cantidad de desplazamiento calculada para el movimiento, que se utiliza tanto para el 
        /// movimiento del personaje como para el desplazamiento del mapa.</param>
        private void HandleMovement(float scrollAmount)
        {
            int scrollZone = 500;
            if (player.MovingRight)
            {
                if (player.Position.X >= 500)
                {
                    mapa.DesplazarX((int)(scrollAmount)); // Mover el mapa despues de superada la posicion 500
                }
                else
                {
                    player.MoveRight((int)scrollAmount); //Mover solo el personaje
                }
            }
            else if (player.MovingLeft)
            {
                int visualX = player.Position.X - mapa.OffsetX;
                if (visualX <= scrollZone && mapa.OffsetX > 0)
                {
                    mapa.DesplazarX(-(int)(scrollAmount));
                }
                else if (player.Position.X > 0)
                {
                    player.MoveLeft((int)scrollAmount);
                }
            }
        }

        #endregion

        #region Juego Completado

        /// <summary>
        /// Verifica si el jugador está cerca del final del mapa.
        /// Este método evalúa la posición del jugador y determina si su posición en el eje X está cerca del borde derecho del mapa.
        /// </summary>
        /// <returns>Retorna <c>true</c> si el jugador está cerca del final del mapa, de lo contrario retorna <c>false</c>.</returns>
        private bool IsPlayerNearMapEnd()
        {
            //verifica si el jugador esta cerca del final del mapa
            return (player.Position.X >= mapWidth - 110);
        }

        /// <summary>
        /// Finaliza el juego y muestra la pantalla de puntuaciones.
        /// Este método detiene el temporizador del juego, calcula la puntuación final basada en el puntaje y el tiempo restante,
        /// muestra un cuadro de diálogo para que el jugador ingrese su nombre y luego finaliza el juego y muestra las mejores puntuaciones.
        /// </summary>
        private void EndGameAndShowScores()
        {
            if (gameEnded) return;

            gameEnded = true;
            gameTimer.Stop(); // Detener el timer del juego

            // Calcular puntuación final basada en tiempo y score
            int timeBonus = Math.Max(0, 300 - elapsedTime) * 10; // Bonus por tiempo
            int finalScore = combatSystem.Score + timeBonus;

            // Mostrar diálogo para ingresar nombre
            using (var inputDialog = new Form())
            {
                inputDialog.Text = "¡Felicitaciones!";
                inputDialog.Size = new Size(300, 150);
                inputDialog.StartPosition = FormStartPosition.CenterParent;
                inputDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputDialog.MaximizeBox = false;
                inputDialog.MinimizeBox = false;

                var label = new Label()
                {
                    Text = "Ingresa tu nombre:",
                    Location = new Point(20, 20),
                    Size = new Size(200, 20)
                };

                var textBox = new TextBox()
                {
                    Location = new Point(20, 50),
                    Size = new Size(200, 20)
                };

                var button = new Button()
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(20, 80)
                };

                inputDialog.Controls.AddRange(new Control[] { label, textBox, button });
                inputDialog.AcceptButton = button;

                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string playerName = string.IsNullOrWhiteSpace(textBox.Text) ? "Anonymous" : textBox.Text;
                    EndGame(finalScore, playerName);

                    DisplayTopScores();
                    scorePanel.BringToFront();
                    scorePanel.Refresh();
                }
            }
        }

        #endregion

        #region Manejador de Teclas
        private void KeysZ()
        {
            combatSystem.PlayerShoot(player.FacingRight); soundManager.PlayShootPlayerSound();
        }

        /// <summary>
        /// Maneja la acción del jugador al presionar una tecla específica durante el juego.
        /// <para>Este método recibe el código de la tecla presionada y ejecuta la acción correspondiente sobre el jugador, 
        /// como moverse, saltar, agacharse, correr o disparar.</para>
        /// </summary>
        /// <param name="keyCode">El código de la tecla que fue presionada por el jugador.</param>
        public void HandleKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left: player.StartMovingLeft(); break;
                case Keys.Right: player.StartMovingRight(); break;
                case Keys.Space: player.Jump(); break;
                case Keys.Down: player.StartCrouching(); break;
                case Keys.ShiftKey: player.StartRunning(); break;
                case Keys.Z: KeysZ(); break;
            }
        }

        /// <summary>
        /// Maneja las acciones del jugador cuando una tecla es liberada.
        /// <para>Este método recibe el código de la tecla que fue liberada y ejecuta la acción correspondiente para detener la acción en curso del jugador, 
        /// como detener el movimiento, dejar de agacharse o dejar de correr.</para>
        /// </summary>
        /// <param name="keyCode">El código de la tecla que fue liberada por el jugador.</param>
        public void HandleKeyUp(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left: player.StopMovingLeft(); break;
                case Keys.Right: player.StopMovingRight(); break;
                case Keys.Down: player.StopCrouching(); break;
                case Keys.ShiftKey: player.StopRunning(); break;
            }
        }

        #endregion
    }

    // Clase GameForm
    public partial class GameForm : Form
    {
        private GameManager gameManager;
        public GameForm()
        {
            InitializeGameForm();
        }

        private void InitializeGameForm()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                gameManager = new GameManager(this, basePath);
                InitializeFormProperties(basePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing GameForm: {ex.Message}");
            }
        }

        private void InitializeFormProperties(string basePath)
        {
            string iconPath = Path.Combine(basePath, "Resources", "hacha.ico");
            this.Icon = new Icon(iconPath);
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
            this.ClientSize = new Size(1500, 600);
            this.Name = "Rebel Assault";
            this.Text = "Rebel Assault";
            this.DoubleBuffered = true; // Evitar parpadeos en la pantalla
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            gameManager.HandleKeyDown(e.KeyCode);
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            gameManager.HandleKeyUp(e.KeyCode);
        }
    }
}
