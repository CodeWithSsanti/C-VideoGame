/*
 * Clase CombatSystem
 * -------------------
 * Esta clase gestiona la lógica del sistema de combate en "Rebel Assault", coordinando elementos como el jugador,
 * los enemigos, los proyectiles y las interacciones entre ellos, incluyendo la detección de colisiones y la aplicación de daño.
 *
 * Características principales:
 * - Gestión de proyectiles: Crea, mueve y elimina proyectiles disparados por el jugador.
 * - Lógica de combate: Aplica daño al jugador y a los enemigos, actualiza las barras de salud y verifica muertes.
 * - Colisiones: Detecta impactos de proyectiles en enemigos y aplica el daño correspondiente.
 * - Gestión de enemigos: Administra los enemigos activos y elimina a los muertos.
 * - Puntaje: Incrementa el puntaje al destruir enemigos.
 *
 * Uso:
 * Utiliza un temporizador para actualizar el estado de los proyectiles y verificar las colisiones en intervalos regulares.
 * Cuando un proyectil impacta, se aplica daño y el enemigo muere si su salud llega a cero.
 *
 * Nota:
 * Requiere la correcta inicialización de las entidades como el jugador y los enemigos, y usa un temporizador para gestionar 
 * los proyectiles sin sobrecargar el rendimiento del juego.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection.Emit;

namespace Rebel_AssaultTesting
{
    class CombatSystem
    {
        #region Propiedades
        private Form gameForm;
        private MainCharacter player;
        private Mapa mapa;
        private List<EnemyPro> enemies;
        private List<Projectile> projectiles;
        private HealthBar playerHealthBar;
        private Dictionary<EnemyPro, HealthBar> enemyHealthBar;
        private Timer projectileTimer;

        private const int bullet_Speed = 30;
        private const int playerMaxHealth = 100;
        private const int enemyMaxHealth = 50;
        private const int playerBulletDamage = 19;
        private const int enemyBulletDamage = 5;

        private int score = 0;
        #endregion

        #region Metodos De Acceso
        public Form GameForm { get => gameForm; set => gameForm = value; }
        public MainCharacter Player { get => player; set => player = value; }
        internal List<EnemyPro> Enemies { get => enemies; set => enemies = value; }
        internal List<Projectile> Projectiles { get => projectiles; set => projectiles = value; }
        internal HealthBar PlayerHealthBar { get => playerHealthBar; set => playerHealthBar = value; }
        internal Dictionary<EnemyPro, HealthBar> EnemyHealthBar { get => enemyHealthBar; set => enemyHealthBar = value; }
        public Timer ProjectileTimer { get => projectileTimer; set => projectileTimer = value; }
        public Mapa Mapa { get => mapa; set => mapa = value; }
        public int Score { get => score; set => score = value; }
        #endregion

        #region Constructor
        public CombatSystem(Form gameForm, MainCharacter player, List<EnemyPro> enemies, Mapa mapa)
        {
            GameForm = gameForm;
            Player = player;
            Enemies = enemies;
            Mapa = mapa;
            Projectiles = new List<Projectile>();
            EnemyHealthBar = new Dictionary<EnemyPro, HealthBar>();
            InitializeProjectileTimer();
        }
        #endregion

        #region Inicializacion y Logica del Projectil
        /// <summary>
        /// Inicializa el temporizador para actualizar los proyectiles.
        /// </summary>
        /// <remarks>
        /// Este método crea un temporizador que se ejecuta a intervalos regulares de 16 milisegundos (aproximadamente 60 veces por segundo).
        /// Cada vez que el temporizador se activa, se llama al método `UpdateProjectiles` para actualizar el estado de los proyectiles en el juego.
        /// </remarks>
        private void InitializeProjectileTimer()
        {
            projectileTimer = new Timer();
            projectileTimer.Interval = 16; 
            projectileTimer.Tick += UpdateProjectiles;
            projectileTimer.Start();
        }

        /// <summary>
        /// Actualiza el estado de los proyectiles en el juego.
        /// </summary>
        /// <remarks>
        /// Este método recorre la lista de proyectiles y actualiza su posición en función de la dirección de movimiento.
        /// Si un proyectil ha dejado de estar activo o ha salido de los límites de la pantalla, se elimina de la lista.
        /// Además, verifica si el proyectil ha colisionado con un enemigo (si es disparado por el jugador).
        /// </remarks>
        /// <param name="sender">El objeto que genera el evento (el temporizador en este caso).</param>
        /// <param name="e">Los argumentos del evento, que no se utilizan en este método.</param>
        private void UpdateProjectiles(object sender, EventArgs e)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                if(!projectile.IsActive) continue;

                //actualizar posicion global de la bala
                projectile.GlobalX1 += projectile.MovingRight ? bullet_Speed : -bullet_Speed;
                int screenX = (int)projectile.GlobalX1;

                //actualizar posicion visual del sprite
                projectile.Sprite.Location = new Point(screenX, (int)projectile.GlobalY1);
                if(projectile.Owner == "player") CheckEnemyCollision(projectile);
                if (IsOutOfBounds(screenX)) // Verifica si está fuera de los límites del mapa
                {
                    RemovePorjectile(projectile);
                    projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Crea un nuevo proyectil (bala) y lo agrega a la lista de proyectiles activos.
        /// </summary>
        /// <param name="position">La posición en la que se creará el proyectil.</param>
        /// <param name="movingRight">Indica si el proyectil se mueve hacia la derecha.</param>
        /// <param name="damage">El daño que causa el proyectil.</param>
        /// <param name="owner">El propietario del proyectil, generalmente "player" o "enemy".</param>
        private void CreateProjectile(Point position, bool movingRight, int damage, string owner)
        {
            var bulletSprite = new PictureBox
            {
                Size = new Size(10, 5),
                BackColor = Color.Yellow,
                Location = position
            };

            var projectile = new Projectile(bulletSprite, position, player.FacingRight, playerBulletDamage, "player");
            projectiles.Add(projectile);
            gameForm.Controls.Add(bulletSprite);
            bulletSprite.BringToFront();
        }

        /// <summary>
        /// Verifica si la posición del proyectil está fuera de los límites de la pantalla.
        /// </summary>
        /// <remarks>
        /// Este método comprueba si la posición horizontal (`screenX`) del proyectil está fuera de los límites visibles de la pantalla,
        /// considerando un margen adicional de 50 píxeles a la izquierda y derecha para determinar si el proyectil ha salido del área jugable.
        /// </remarks>
        /// <param name="screenX">La posición horizontal del proyectil en la pantalla.</param>
        /// <returns>Devuelve `true` si el proyectil está fuera de los límites de la pantalla, de lo contrario, devuelve `false`.</returns>
        private bool IsOutOfBounds(int screenX)
        {
            return screenX < -50 || screenX > gameForm.ClientSize.Width + 50;
        }

        /// <summary>
        /// Elimina un proyectil de la pantalla y desactiva su estado.
        /// </summary>
        /// <remarks>
        /// Este método establece el estado del proyectil como inactivo (`IsActive = false`), elimina su sprite de los controles del formulario,
        /// y libera los recursos asociados al sprite llamando a `Dispose` para asegurar que no queden recursos no utilizados.
        /// </remarks>
        /// <param name="projectile">El proyectil que se desea eliminar.</param>
        private void RemovePorjectile(Projectile projectile)
        {
            projectile.IsActive = false;
            gameForm.Controls.Remove(projectile.Sprite);
            projectile.Sprite.Dispose();
        }


        /// <summary>
        /// Verifica si un proyectil ha colisionado con algún enemigo y aplica daño al enemigo.
        /// </summary>
        /// <remarks>
        /// Este método recorre la lista de enemigos y verifica si el proyectil ha colisionado con el sprite de algún enemigo.
        /// Si se detecta una colisión, se aplica el daño al enemigo correspondiente y se elimina el proyectil.
        /// </remarks>
        /// <param name="projectile">El proyectil que se verifica para detectar colisiones.</param>
        private void CheckEnemyCollision(Projectile projectile)
        {
            foreach(var enemy in enemies.ToArray())
            {
                if (projectile.Sprite.Bounds.IntersectsWith(enemy.Sprite.Bounds))
                {
                    ApplyDamageToEnemy(enemy, projectile.Damage); RemovePorjectile(projectile);
                    score += 100;
                    break;
                }
            }

        }

        /// <summary>
        /// Dispara un proyectil desde la posición del jugador en la dirección indicada.
        /// </summary>
        /// <remarks>
        /// Este método calcula la posición de disparo del jugador en función de su posición actual y la dirección en la que está mirando.
        /// Luego, crea un nuevo proyectil con las propiedades correspondientes (dirección, daño, propietario).
        /// </remarks>
        /// <param name="facinRight">Indica si el jugador está mirando hacia la derecha (true) o hacia la izquierda (false).</param>
        public void PlayerShoot(bool facinRight)
        {
            int visualX = player.Position.X - mapa.OffsetX;
            int offsetX = facinRight ? 50 : -50;
            Point shootPosition = new Point(visualX + offsetX, player.Position.Y + 50);

            CreateProjectile(shootPosition, facinRight, playerBulletDamage, "player");
        }


        #endregion

        #region Logica de Daño
        /// <summary>
        /// Aplica daño a un enemigo y actualiza su salud.
        /// </summary>
        /// <remarks>
        /// Este método reduce la salud del enemigo en función del daño recibido y actualiza la barra de salud del enemigo si está registrada.
        /// Si la salud del enemigo llega a 0 o menos, se llama al método `KillEnemy` para eliminar al enemigo del juego.
        /// </remarks>
        /// <param name="enemy">El enemigo al que se le aplicará el daño.</param>
        /// <param name="damage">La cantidad de daño que se le aplicará al enemigo.</param
        private void ApplyDamageToEnemy(EnemyPro enemy, int damage)
        {
            enemy.Health -= damage;
            if (enemyHealthBar.ContainsKey(enemy)) enemyHealthBar[enemy].CurrentHealth = enemy.Health;
            if(enemy.Health <= 0) KillEnemy(enemy);
        }

        /// <summary>
        /// Aplica daño al jugador y verifica si su salud llega a cero.
        /// </summary>
        /// <remarks>
        /// Este método reduce la salud del jugador en función del daño recibido. Si la salud del jugador llega a 0 o menos,
        /// se llama al método `GameOver` para finalizar el juego.
        /// </remarks>
        /// <param name="damage">La cantidad de daño que se le aplicará al jugador.</param>
        public void ApplyDamageToPlayer(int damage)
        {
            Player.Health -= damage;
            if (Player.Health <= 0)
            {
                //gameOver.ShowDialog();
                Player.IsActive = false;
                FinishGame();
            }
        }

        /// <summary>
        /// Elimina un enemigo del juego, limpiando sus recursos y eliminando su barra de salud.
        /// </summary>
        /// <remarks>
        /// Este método elimina un enemigo del juego, primero limpiando los recursos asociados, como su barra de salud,
        /// y luego eliminándolo de la lista de enemigos activos. Se asegura de liberar adecuadamente los recursos del enemigo.
        /// </remarks>
        /// <param name="enemy">El enemigo que se eliminará del juego.</param>
        private void KillEnemy(EnemyPro enemy)
        {
            if (enemyHealthBar.ContainsKey(enemy))
            {
                enemyHealthBar[enemy].Dispose();
                enemyHealthBar.Remove(enemy);
            }

            enemy.Cleanup();
            enemies.Remove(enemy);
        }
        #endregion

        #region Logica de Muerte
        /// <summary>
        /// Finaliza el juego, deteniendo el temporizador de proyectiles y mostrando un mensaje de "Game OVER".
        /// </summary>
        /// <remarks>
        /// Este método detiene el temporizador de los proyectiles, oculta la ventana del juego y muestra un cuadro de mensaje con el texto "Game OVER!"
        /// para indicar que el juego ha terminado.
        /// </remarks>
        private void FinishGame()
        {
            projectileTimer.Stop();
            gameForm.Hide();
            gameForm.Close();
            gameForm.Dispose();
        }

        #endregion


    }
}
