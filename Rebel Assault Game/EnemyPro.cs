/*
 * Clase EnemyPro
 * --------------
 * Representa un enemigo avanzado en el juego "Rebel Assault".
 * Esta clase extiende de `Character` y ofrece funcionalidades específicas para manejar enemigos con comportamientos dinámicos.
 *
 * Características principales:
 * - Comportamientos de patrullaje: El enemigo puede moverse a la derecha o izquierda, detenerse y reanudar su patrullaje.
 * - Sistema de ataque: Incluye disparos con proyectiles, animaciones de ataque, y cooldown entre ataques.
 * - Integración con el jugador principal: Permite identificar al jugador como objetivo y reaccionar según su posición.
 * - Gestión de sprites y animaciones: Incluye animaciones para caminar, atacar y disparar, con soporte para personalización mediante rutas de sprites.
 * - Timers: Usa múltiples temporizadores para coordinar animaciones, ataques, y cambios de estado.
 *
 * Uso de la clase:
 * La clase EnemyPro se instancia con parámetros que definen su posición, atributos de combate, y comportamiento en el mapa del juego. 
 * Los métodos proporcionan lógica para inicializar sistemas como patrullaje y ataques, lo que permite una fácil integración con otras partes del juego.
 *
 * Notas:
 * - Requiere una instancia de `SoundManager` para manejar los efectos de audio asociados al enemigo.
 * - La clase utiliza rutas para sprites organizados bajo un `basePath`, lo que facilita el manejo y carga de recursos visuales.
 * - Los enemigos se gestionan dentro de una lista, lo que permite un manejo eficiente de múltiples instancias en el mapa del juego.
 */



using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Rebel_AssaultTesting
{
    class EnemyPro : Character
    {
        #region Properties
        // Referencias principales
        private Form gameForm;
        private CombatSystem combatS;
        private List<EnemyPro> enemies;
        private Mapa Mapa;
        private MainCharacter targetPlayer;

        // Información básica del enemigo
        private string enemyType;
        private int spawnPosition;
        private string basePath;

        // Estado del enemigo
        private EnemyState currentState = EnemyState.PatrollingRight;

        // Configuración de patrullaje
        private bool movingRight = true;
        private float patrolSpeed = 5f;
        private int leftLimit;
        private int rightLimit;
        private int waitTime = 2000;

        // Timers
        private Timer animationTimer;
        private Timer waitTimer;

        // Animación y sprites
        private int currentFrame = 0;
        private int frameWidth = 35;
        private int frameHeight = 45;
        private float scaleFactor = 2.3f;
        private Bitmap spriteSheet;
        private Bitmap walkingRight;
        private Bitmap enemyAttack;
        private Bitmap shoot;

        // Propiedades del disparo
        private bool bulletFired = false;
        private int bulletX;
        private int bulletY;
        private int Damage = 17;
        private PictureBox bulletSprite;
        private SoundManager soundManager;

        // Sistema de ataque
        private const int attackRange = 500;
        private const float bulletSpeed = 50f;
        private const int attackCooldown = 5000;
        private Timer AttackCooldownTimer;
        private bool canAttack = true;
        private bool hasAttacked = false;

        // Paths de sprites
        private string walkingLeftSpritePath;
        private string walkingRightSpritePath;
        private string waitingSpritePath;
        private string attackSpritePath;
        private string shootSpritePath;


        // Enumeración del estado enemigo
        public enum EnemyState
        {
            PatrollingRight,
            Waiting,
            PatrollingLeft,
            attack,
            chasing
        }
        #endregion

        #region Metodos De Acceso
        public string EnemyType { get => enemyType; set => enemyType = value; }
        public int SpawnPosition { get => spawnPosition; set => spawnPosition = value; }
        public string BasePath { get => basePath; set => basePath = value; }
        public MainCharacter TargetPlayer { get => targetPlayer; set => targetPlayer = value; }
        public Form GameForm { get => gameForm; set => gameForm = value; }

        public Point GetPosition()
        {
            return Position;
        }
        #endregion

        #region constructor
        public EnemyPro(PictureBox sprite, Point position, string tag, int health, int speed, int currentAnimation,
        int groundLevel, bool isAttacking, bool isActive, string enemyType, int spawnPosition, string basePath,
        MainCharacter targetPlayer, Form gameF)
        :base(sprite, position, tag, health, speed, currentAnimation, groundLevel, isAttacking, isActive)
        {
            EnemyType = enemyType;
            SpawnPosition = spawnPosition;
            GroundLevel = groundLevel;
            BasePath = basePath;
            this.TargetPlayer = targetPlayer;
            GameForm = gameF;

            InitializeSpritePaths(basePath);
            ConfigurePatrolLimits();
            InitializePatrolLogic();
            InitializeAttackSystem();
            InitializeCombatS();
        }
        #endregion

        #region Enemy Component Initialization

        /// <summary>
        /// Inicializa el sistema de combate para el enemigo.
        /// </summary>
        /// <remarks>
        /// Este método configura una instancia del sistema de combate, asociándolo con 
        /// el formulario del juego, el jugador principal, la lista de enemigos y el mapa.
        /// Es esencial para habilitar las interacciones de combate entre el enemigo y el jugador.
        /// </remarks>
        private void InitializeCombatS() => combatS = new CombatSystem(gameForm, targetPlayer, enemies, Mapa);


        /// <summary>
        /// Inicializa las rutas de los sprites asociados al enemigo.
        /// </summary>
        /// <param name="basePath">
        /// La ruta base donde se almacenan los recursos gráficos del enemigo.
        /// </param>
        /// <remarks>
        /// Este método establece las rutas específicas para los sprites de las diferentes acciones del enemigo, 
        /// como caminar, esperar, atacar y disparar. Las rutas se construyen combinando la ruta base con los nombres 
        /// de archivo correspondientes.
        /// </remarks>
        private void InitializeSpritePaths(string basePath)
        {
            string enemyProPath = Path.Combine(basePath, "EnemyPro");
            walkingLeftSpritePath = Path.Combine(enemyProPath, "Walking.png");
            walkingRightSpritePath = Path.Combine(enemyProPath, "WalkingRight.png");
            waitingSpritePath = Path.Combine(enemyProPath, "Waiting.png");
            attackSpritePath = Path.Combine(enemyProPath, "EnemyAttack.png");
            shootSpritePath = Path.Combine(enemyProPath, "Shoot.png");
        }



        /// <summary>
        /// Configura la lógica de patrullaje del enemigo.
        /// </summary>
        /// <remarks>
        /// Este método inicializa todos los componentes necesarios para que el enemigo realice patrullajes.
        /// Incluye la carga de sprites, la configuración de timers de animación y espera, 
        /// y la definición de los límites del área de patrullaje.
        /// </remarks>
        private void InitializePatrolLogic()
        {
            LoadSprites();
            ConfigureAnimationTimer();
            ConfigureWaitTimer();
            ConfigurePatrolLimits();
        }


        /// <summary>
        /// Carga y aplica transparencia a los sprites necesarios para el enemigo.
        /// </summary>
        /// <remarks>
        /// Este método carga los sprites desde sus rutas respectivas y aplica transparencia a cada uno de ellos.
        /// Utiliza el método `LoadAndMakeTransparent` para procesar los archivos de imagen y hacerlos adecuados para su uso en el juego.
        /// Los sprites cargados incluyen los de caminar hacia la izquierda, caminar hacia la derecha, atacar y disparar.
        /// </remarks>
        private void LoadSprites()
        {
            spriteSheet = LoadAndMakeTransparent(walkingLeftSpritePath);
            walkingRight = LoadAndMakeTransparent(walkingRightSpritePath);
            enemyAttack = LoadAndMakeTransparent(attackSpritePath);
            shoot = LoadAndMakeTransparent(shootSpritePath);
        }

        /// <summary>
        /// Carga un archivo de imagen y aplica transparencia al color blanco.
        /// </summary>
        /// <param name="filePath">
        /// La ruta del archivo de imagen que se va a cargar.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="Bitmap"/> con la imagen cargada y el color blanco transparente.
        /// </returns>
        /// <remarks>
        /// Este método carga una imagen desde la ruta especificada y luego utiliza el método 
        /// `MakeTransparent` para hacer que el color blanco de la imagen sea transparente.
        /// Esto se utiliza comúnmente para sprites, de modo que las áreas de fondo blanco
        /// no se muestren en el juego.
        /// </remarks>
        private Bitmap LoadAndMakeTransparent(string filePath)
        {
            var sprite = new Bitmap(filePath);
            sprite.MakeTransparent(Color.White);
            return sprite;
        }

        /// <summary>
        /// Establece los límites de patrullaje en función de la posición inicial del enemigo.
        /// </summary>
        /// <remarks>
        /// Este método configura los límites de patrullaje del enemigo. El límite izquierdo se establece
        /// restando 100 unidades de la posición de aparición del enemigo, mientras que el límite derecho
        /// se establece sumando 200 unidades a la misma posición. Estos límites determinan el área en la
        /// que el enemigo puede moverse mientras patrulla.
        /// </remarks>
        private void ConfigurePatrolLimits()
        {
            leftLimit = SpawnPosition - 100;
            rightLimit = SpawnPosition + 200;
        }

        /// <summary>
        /// Configura el temporizador de animación del enemigo.
        /// </summary>
        /// <remarks>
        /// Este método inicializa un temporizador para controlar la frecuencia de la animación del enemigo.
        /// El temporizador tiene un intervalo de 50 milisegundos, y en cada "tick" (evento que se dispara 
        /// en ese intervalo), se ejecutan varias acciones: renderizar el fotograma actual, actualizar el 
        /// fotograma de la animación y actualizar el estado del enemigo.
        /// </remarks>
        private void ConfigureAnimationTimer()
        {
            animationTimer = new Timer { Interval = 50 };
            animationTimer.Tick += (sender, e) => {
                RenderCurrentFrame();
                UpdateAnimationFrame();
                UpdateEnemyState();
            };
            animationTimer.Start();
        }

        /// <summary>
        /// Configura el temporizador de espera del enemigo.
        /// </summary>
        /// <remarks>
        /// Este método configura un temporizador que maneja los períodos de espera del enemigo. 
        /// El temporizador tiene un intervalo igual al valor de `waitTime` (en milisegundos).
        /// Cuando el temporizador alcanza su intervalo, se dispara el evento "tick" que llama
        /// al método `HandleWaitTimeout` para gestionar la lógica asociada con el tiempo de espera del enemigo.
        /// </remarks>
        private void ConfigureWaitTimer()
        {
            waitTimer = new Timer { Interval = waitTime };
            waitTimer.Tick += (sender, e) => HandleWaitTimeout();
        }

        /// <summary>
        /// Lógica a ejecutar cuando el temporizador de espera expira.
        /// </summary>
        /// <remarks>
        /// Este método maneja lo que ocurre cuando el temporizador de espera alcanza su límite. 
        /// Primero, detiene el temporizador de espera para evitar que se siga ejecutando.
        /// Luego, si el estado actual del enemigo es "Waiting", cambia el estado a "PatrollingLeft",
        /// lo que indica que el enemigo comienza a patrullar hacia la izquierda.
        /// Finalmente, se carga la hoja de sprites correspondiente al movimiento hacia la izquierda
        /// y se restablece el fotograma de la animación al valor inicial.
        /// </remarks>
        private void HandleWaitTimeout()
        {
            waitTimer.Stop();
            if (currentState == EnemyState.Waiting)
            {
                currentState = EnemyState.PatrollingLeft;
                LoadSpriteSheet(walkingLeftSpritePath);
                currentFrame = 0;
            }
        }
        #endregion

        #region Attack System Initialization
        /// <summary>
        /// Inicializa todos los componentes necesarios para el sistema de ataque del jugador.
        /// <para>Este método configura todos los elementos necesarios para que el sistema de ataque del jugador funcione correctamente, 
        /// incluyendo la creación del sprite de la bala, la carga de la imagen para el proyectil y la configuración del temporizador de recarga del ataque.</para>
        /// </summary>
        private void InitializeAttackSystem()
        {
            ConfigureBulletSprite();
            LoadBulletImage();
            InitializeShootSound(BasePath);
            ConfigureAttackCooldownTimer();
        }


        private void InitializeShootSound(string BasePath) => soundManager = new SoundManager(basePath); //Pendiente de Codigo XML

        /// <summary>
        /// Configura las propiedades del <see cref="PictureBox"/> que representa la bala (proyectil) en el juego.
        /// <para>Se definen el tamaño, el modo de ajuste de imagen, el color de fondo y la visibilidad inicial del sprite de la bala. 
        /// Además, si el formulario del juego está disponible, el sprite se agrega a los controles del formulario y se posiciona al frente.</para>
        /// </summary>
        private void ConfigureBulletSprite()
        {
            bulletSprite = new PictureBox
            {
                Size = new Size((int)(10 * scaleFactor), (int)(5 * scaleFactor)),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Visible = false
            };

            if (gameForm != null)
            {
                gameForm.Controls.Add(bulletSprite);
                bulletSprite.BringToFront();
            }
        }

        /// <summary>
        /// Carga la imagen del sprite de la bala desde un archivo y aplica transparencia.
        /// <para>Si el archivo de la imagen existe en la ruta especificada, se carga, se establece un color como transparente
        /// y se asigna como imagen del <see cref="PictureBox"/> que representa la bala.</para>
        /// </summary>
        private void LoadBulletImage()
        {
            if (File.Exists(shootSpritePath))
            {
                shoot = new Bitmap(shootSpritePath);
                shoot.MakeTransparent(Color.White);
                bulletSprite.Image = new Bitmap(shoot);
            }
        }

        /// <summary>
        /// Configura el temporizador para gestionar el tiempo de reutilización (cooldown) entre ataques.
        /// <para>El temporizador se inicializa con un intervalo específico y permanece deshabilitado hasta que se necesite. 
        /// Cuando se activa, ejecuta el método <see cref="ResetAttackCooldown"/> en cada tick.</para>
        /// </summary>
        private void ConfigureAttackCooldownTimer()
        {
            AttackCooldownTimer = new Timer { Interval = attackCooldown, Enabled = false };
            AttackCooldownTimer.Tick += (sender, e) => ResetAttackCooldown();
        }

        /// <summary>
        /// Restablece las propiedades de ataque al finalizar el periodo de cooldown.
        /// <para>Detiene el temporizador de cooldown y permite que el jugador pueda atacar nuevamente.
        /// También reinicia los estados relacionados con el ataque.</para>
        /// </summary>
        private void ResetAttackCooldown()
        {
            AttackCooldownTimer.Stop();
            IsAttacking = false;
            canAttack = true;
            bulletFired = false;
        }
        #endregion

        #region Animacion y Control de Frames

        /// <summary>
        /// Renderiza el frame actual del sprite del enemigo en el mapa.
        /// <para>Crea un nuevo <see cref="Bitmap"/> para el frame actual basado en el estado y las dimensiones del sprite, 
        /// y lo asigna al sprite del enemigo.</para>
        /// </summary>
        private void RenderCurrentFrame() //Dibujar el enemigo en el mapa
        {
            // Crear el Bitmap para el frame actual
            Bitmap frameBuffer = new Bitmap((int)(frameWidth * scaleFactor), (int)(frameHeight * scaleFactor));

            using (Graphics g = Graphics.FromImage(frameBuffer))
            {
                // Configuración de calidad de renderizado
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                Rectangle frameRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Bitmap currentSprite = GetCurrentSprite();
                g.DrawImage(currentSprite, new Rectangle(0, 0, frameBuffer.Width, frameBuffer.Height), frameRect, GraphicsUnit.Pixel);
                if (currentState == EnemyState.attack) currentSprite.Dispose();
            }
            // Liberar y asignar la imagen del sprite
            Sprite.Image?.Dispose();
            Sprite.Image = frameBuffer;
        }


        /// <summary>
        /// Obtiene el <see cref="Bitmap"/> correspondiente al sprite actual del enemigo,
        /// dependiendo de su estado y dirección.
        /// </summary>
        /// <returns>
        /// Un <see cref="Bitmap"/> que representa el sprite actual del enemigo.
        /// Si el enemigo está atacando, devuelve el sprite de ataque, aplicando
        /// una inversión horizontal si es necesario. Si no, devuelve el sprite de movimiento.
        /// </returns>
        private Bitmap GetCurrentSprite()
        {
            if (currentState == EnemyState.attack)
            {
                // Crear una copia del sprite de ataque y voltearlo si es necesario
                Bitmap attackSprite = new Bitmap(enemyAttack);
                if (targetPlayer.Position.X > Position.X)
                {
                    attackSprite.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                return attackSprite;
            }
            // Devolver el sprite en movimiento según la dirección
            return movingRight ? walkingRight : spriteSheet;
        }

        /// <summary>
        /// Actualiza el frame actual de la animación del enemigo y gestiona las acciones relacionadas,
        /// como disparar balas durante la animación de ataque y manejar el final de la animación.
        /// </summary>
        private void UpdateAnimationFrame()
        {
            currentFrame++;
            int maxFrames = GetMaxFramesForCurrentState();
            if (ShouldFireBullet(maxFrames)) // Lógica para disparar la bala en el momento adecuado durante la animación de ataque
            {
                FireBullet();
            }

            if (currentFrame >= maxFrames) // Verifica si se alcanzó el último frame de la animación
            {
                HandleAnimationEnd();
            }
        }

        /// <summary>
        /// Obtiene el número máximo de frames para la animación según el estado actual del enemigo.
        /// </summary>
        /// <returns>
        /// Número de frames disponibles para la animación correspondiente al estado actual.
        /// </returns>
        /// <remarks>
        /// Este método calcula la cantidad de frames dividiendo el ancho total del sprite
        /// entre el ancho de cada frame, que varía según el estado del enemigo.
        /// </remarks>
        private int GetMaxFramesForCurrentState() // Devuelve el número máximo de frames para el estado actual del enemigo
        {
            int frameWidth = currentState == EnemyState.attack ? 65 : 35;
            Bitmap spriteSource = GetSpriteSourceForCurrentState();
            return spriteSource.Width / frameWidth;
        }

        /// <summary>
        /// Obtiene el recurso gráfico (sprite) adecuado para el estado actual del enemigo.
        /// </summary>
        /// <returns>
        /// Una instancia de <see cref="Bitmap"/> que representa el sprite correspondiente al estado actual y dirección del enemigo.
        /// </returns>
        /// <remarks>
        /// Este método selecciona el recurso gráfico basado en:
        /// - El estado actual del enemigo.
        /// - La dirección en la que se está moviendo.
        /// </remarks>
        private Bitmap GetSpriteSourceForCurrentState()
        {
            return currentState == EnemyState.attack ? enemyAttack : (movingRight ? walkingRight : spriteSheet);
        }

        /// <summary>
        /// Determina si el enemigo debe disparar una bala durante la animación de ataque.
        /// </summary>
        /// <param name="maxFrames">
        /// Número total de frames de la animación de ataque.
        /// </param>
        /// <returns>
        /// <c>true</c> si el enemigo debe disparar una bala en el frame actual de la animación; de lo contrario, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Este método utiliza las siguientes condiciones para decidir si debe dispararse una bala:
        /// - El estado actual del enemigo debe ser <see cref="EnemyState.attack"/>.
        /// - El frame actual debe corresponder al punto medio de la animación de ataque.
        /// - No se debe haber disparado una bala previamente en esta secuencia de animación.
        /// </remarks>
        private bool ShouldFireBullet(int maxFrames)
        {
            return currentState == EnemyState.attack && currentFrame == maxFrames / 2 && !bulletFired;
        }

        /// <summary>
        /// Maneja la lógica cuando la animación del enemigo alcanza su último frame.
        /// </summary>
        /// <remarks>
        /// Este método verifica el estado actual del enemigo y, dependiendo de si está atacando o no, 
        /// realiza una de las siguientes acciones al finalizar la animación:
        /// - Si el enemigo estaba atacando, termina la animación de ataque.
        /// - Si el enemigo estaba en otro estado, reinicia el contador de frames de la animación.
        /// </remarks>
        private void HandleAnimationEnd()
        {
            if (currentState == EnemyState.attack)
            {
                EndAttackAnimation();
            }
            else
            {
                ResetAnimationFrame(); // Reinicia el frame para otras animaciones
            }
        }

        /// <summary>
        /// Finaliza la animación de ataque y cambia el estado del enemigo a "chasing".
        /// </summary>
        /// <remarks>
        /// Este método se ejecuta cuando la animación de ataque llega a su fin. Realiza las siguientes acciones:
        /// - Restablece el ancho del frame a su valor predeterminado (para otros estados de animación).
        /// - Reinicia el contador de frames para permitir que se inicie una nueva animación si es necesario.
        /// - Marca que el ataque ha terminado.
        /// - Cambia el estado del enemigo a "chasing", iniciando la persecución del jugador.
        /// </remarks>
        private void EndAttackAnimation()
        {
            frameWidth = 35;       // Restablece el ancho del frame para otros estados
            currentFrame = 0;      // Reinicia la animación
            IsAttacking = false;   // Marca que el ataque ha terminado
            currentState = EnemyState.chasing; // Cambia al estado de persecución
        }

        /// <summary>
        /// Reinicia el frame de animación a 4 para animaciones distintas al ataque.
        /// </summary>
        /// <remarks>
        /// Este método se utiliza para reiniciar el contador de frames de animación a un valor inicial,
        /// generalmente utilizado cuando el enemigo no está en estado de ataque. El valor 4 se establece
        /// como el punto de inicio para las animaciones de movimiento u otros estados que no sean ataque.
        /// </remarks>
        private void ResetAnimationFrame() => currentFrame = 4;

        /// <summary>
        /// Dispara una bala desde la posición del enemigo hacia el jugador.
        /// </summary>
        /// <remarks>
        /// Este método se encarga de calcular la posición inicial de la bala basada en la dirección en la que el enemigo está mirando,
        /// y luego la coloca en la posición adecuada para simular el disparo. Además, la bala puede ser volteada si el enemigo está mirando hacia la izquierda.
        /// El sprite de la bala se vuelve visible y se actualizan las variables correspondientes para su movimiento.
        /// </remarks>
        private void FireBullet()
        {
            bulletFired = true;
            bulletSprite.Visible = true;

            // Calcula la posición inicial de la bala basada en la dirección del enemigo
            bool shootingRight = targetPlayer.Position.X > Position.X;
            int enemyCenterX = Sprite.Location.X + 40; 
            int enemyCenterY = Position.Y + (Sprite.Height / 3);

            if (shootingRight) bulletX = enemyCenterX;
            else
            {
                bulletX = enemyCenterX - bulletSprite.Width - 10;
                if (bulletSprite.Image != null) bulletSprite.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            bulletY = enemyCenterY - (bulletSprite.Height / 2);
            bulletSprite.Location = new Point(bulletX, bulletY);
            movingRight = shootingRight;
            soundManager.PlayShootEnemySound(); //Aplica el sonido de la Bala del enemigo

        }
        #endregion

        #region Logica del enemigo

        /// <summary>
        /// Actualiza el estado del enemigo según la dirección y posición.
        /// </summary>
        /// <remarks>
        /// Este método gestiona el cambio de estado del enemigo y ejecuta la acción correspondiente según el estado actual.
        /// Los posibles estados incluyen patrullaje hacia la derecha o izquierda, persecución del jugador, y ataque.
        /// En el estado de ataque, si no se ha disparado una bala, se llama a <see cref="FireBullet"/> para dispararla.
        /// </remarks>
        private void UpdateEnemyState()
        {
            switch (currentState)
            {
                case EnemyState.PatrollingRight:
                    MoveRight();
                    break;
                case EnemyState.PatrollingLeft:
                    MoveLeft();
                    break;
                case EnemyState.chasing:
                    ChasePlayer();
                    break;
                case EnemyState.attack:
                    EnemyStateAttack();
                    break;
            }
        }

        //Pendiente Codigo XML
        private void EnemyStateAttack()  
        {
            //soundManager.PlayShootEnemySound();
            if (!bulletFired) FireBullet();
        }

        /// <summary>
        /// Actualiza la lógica del comportamiento del enemigo.
        /// Se encarga de gestionar los diferentes estados del enemigo, como ataque, patrullaje y persecución,
        /// así como la actualización de la bala disparada durante el ataque.
        /// </summary>
        public void UpdateEnemyLogic()
        {
            // Si el enemigo no está activo, no hace nada
            if (!IsActive) return;

            int playerX = targetPlayer.Position.X;
            int enemyX = Position.X;
            int distanceToPlayer = Math.Abs(playerX - enemyX);

            if (currentState == EnemyState.attack)
            {
                if (bulletFired) UpdateBullet();
                return;
            }

            bool inAttackRange = distanceToPlayer <= attackRange;

            if (inAttackRange && canAttack && !IsAttacking) // Si está en rango y puede atacar
            {
                EnterAttackState();
                return;
            }

            if (!inAttackRange) // Si está fuera del rango de ataque
            {
                ExitAttackState();
                UpdatePatrolBehavior();
                return;
            }

            if (inAttackRange && !canAttack && !IsAttacking) // Si está en rango pero en cooldown, perseguir al jugador
            {
                ChasePlayer(playerX, enemyX);
            }

            if (bulletFired) UpdateBullet();
        }

        /// <summary>
        /// Cambia el estado del enemigo a "attack", iniciando el proceso de ataque.
        /// Se configura el ancho de los frames para la animación de ataque y se gestionan las propiedades
        /// relacionadas con el ataque, como la activación del temporizador de cooldown y el control de disparo.
        /// </summary>
        private void EnterAttackState()
        {
            // Cambiar al estado de ataque
            frameWidth = 65;
            currentState = EnemyState.attack;
            IsAttacking = true;
            canAttack = false;
            bulletFired = false; // Se establecerá a true cuando se dispare la bala
            AttackCooldownTimer.Start();
        }

        /// <summary>
        /// Sale del estado de ataque, restableciendo los parámetros relacionados con el ataque.
        /// Se restablecen las propiedades para volver a un estado de patrullaje o movimiento.
        /// </summary>
        private void ExitAttackState()
        {
            // Dejar el estado de ataque
            frameWidth = 35;
            IsAttacking = false;
        }

        /// <summary>
        /// Cambia el estado del enemigo a "persecución" y mueve al enemigo hacia el jugador.
        /// Determina la dirección de movimiento y actualiza la posición del enemigo.
        ///
        /// <para>El enemigo comienza a moverse hacia el jugador en función de su posición relativa.</para>
        /// </summary>
        /// <param name="playerX">La coordenada X del jugador para determinar la dirección de persecución.</param>
        /// <param name="enemyX">La coordenada X del enemigo para calcular la dirección hacia el jugador.</param>
        private void ChasePlayer(int playerX, int enemyX)
        {
            // Cambiar al estado de persecución
            frameWidth = 35;
            currentState = EnemyState.chasing;
            movingRight = playerX > enemyX; // Determinar la dirección de movimiento
            Position = new Point((int)(Position.X + (movingRight ? patrolSpeed : -patrolSpeed)), Position.Y);
        }


        /// <summary>
        /// Actualiza el comportamiento de patrullaje del enemigo. 
        /// El enemigo patrulla hacia la derecha hasta alcanzar el límite derecho y luego cambia de dirección hacia la izquierda, y viceversa.
        /// Si el enemigo está esperando, mantiene su posición actual.
        /// </summary>
        private void UpdatePatrolBehavior()
        {
            if (movingRight)
            {
                currentState = EnemyState.PatrollingRight;
                Position = new Point(Position.X + Speed, Position.Y);
                if (Position.X >= rightLimit) movingRight = false;

            }
            else if (currentState == EnemyState.Waiting)
            {
                Position = new Point(Position.X, Position.Y);
            }
            else
            {
                currentState = EnemyState.PatrollingLeft;
                Position = new Point(Position.X - Speed, Position.Y);
                if (Position.X <= leftLimit) movingRight = true;
            }
        }

        /// <summary>
        /// Actualiza la posición de la bala disparada y verifica las condiciones de colisión y los límites del formulario.
        /// Si la bala sale del área visible o colisiona con el jugador, se resetea.
        /// </summary>
        private void UpdateBullet()
        {
            if (!bulletFired) return;
            bool movingRight = targetPlayer.Position.X >= Position.X;
            bulletX += (movingRight ? 1 : -1) * (int)bulletSpeed; //acutalizar posicion de la bala segun la direccion
            bulletSprite.Location = new Point(bulletX, bulletY);
            if (bulletX < 0 || bulletX > gameForm.Width) ResetBullet(); // Comprueba si la bala esta fuera de los limtes del form 
            if (bulletSprite.Bounds.IntersectsWith(targetPlayer.Sprite.Bounds)) //Verifica Collision con el jugador
            {
                ResetBullet();
                combatS.ApplyDamageToPlayer(Damage); //Daño al jugador
            }
        }

        /// <summary>
        /// Restablece el estado de la bala después de que haya salido del formulario o haya colisionado con el jugador.
        /// Hace que la bala sea invisible y detiene su movimiento.
        /// </summary>
        private void ResetBullet()
        {
            bulletFired = false;
            bulletSprite.Visible = false;
            if (bulletSprite.Image != null && !movingRight) bulletSprite.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        /// <summary>
        /// Mueve al enemigo hacia la derecha, verificando si ha alcanzado el límite derecho.
        /// Si el enemigo alcanza el límite, cambia su estado a "esperando" y detiene su movimiento.
        /// </summary>
        private void MoveRight()
        {
            if (Position.X >= rightLimit)
            {
                SetWaitingState();
                movingRight = false;
                LoadSpriteSheet(waitingSpritePath);
            }
        }

        /// <summary>
        /// Mueve al enemigo hacia la izquierda, verificando si ha alcanzado el límite izquierdo.
        /// Si el enemigo alcanza el límite, cambia su estado a "patrullando a la derecha" y comienza el movimiento hacia la derecha.
        /// </summary>
        private void MoveLeft()
        {
            if (Position.X <= leftLimit)
            {
                currentState = EnemyState.PatrollingRight;
                movingRight = true;
                LoadSpriteSheet(walkingRightSpritePath);
                waitTimer.Start();
            }
        }


        /// <summary>
        /// aumenta la velocidad de persecucion, Moviendolo hacia el jugador.
        /// </summary>
        private void ChasePlayer()
        {
            movingRight = targetPlayer.Position.X > Position.X;
            Position = new Point((int)(Position.X + (movingRight ? patrolSpeed + 10 : -patrolSpeed - 10)), Position.Y);
        }

        /// <summary>
        /// Cambia el estado del enemigo a "esperando" y comienza el temporizador de espera.
        /// </summary>
        private void SetWaitingState()
        {
            currentState = EnemyState.Waiting;
            waitTimer.Start();
        }

        #endregion

        /// <summary>
        /// Libera el sprite sheet actual y carga uno nuevo desde el archivo especificado.
        /// </summary>
        /// <param name="filePath">La ruta del archivo del sprite sheet a cargar.</param>
        private void LoadSpriteSheet(string filePath)
        {
            // Libera y recarga el sprite sheet con una nueva animación
            spriteSheet.Dispose();
            spriteSheet = LoadAndMakeTransparent(filePath);
            currentFrame = 0;
        }

        /// <summary>
        /// Limpia los recursos utilizados por el enemigo cuando se destruye o desactiva.
        /// Detiene los temporizadores, elimina el sprite de la bala y libera otros recursos asociados.
        /// </summary>
        public void Cleanup()
        {
            animationTimer?.Stop();
            waitTimer?.Stop();
            AttackCooldownTimer?.Stop();

            if (gameForm != null && !gameForm.IsDisposed)
            {
                if (bulletSprite != null)
                {
                    gameForm.Controls.Remove(bulletSprite);
                    bulletSprite.Dispose();
                    bulletSprite.Image?.Dispose();
                }
            }

            shoot?.Dispose();
            Sprite.Image?.Dispose();
            spriteSheet?.Dispose();
            walkingRight?.Dispose();
            Sprite.Image?.Dispose();
        }  
    }
}