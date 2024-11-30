/*Esta clase sirvio de testing durante el desarollo
 * del proyecto pero sera implementada en un futuro*/




/*                                          ░░░░░░░░░░░░░░░░░░░░░░█████████
                                            ░░███████░░░░░░░░░░███▒▒▒▒▒▒▒▒███
                                            ░░█▒▒▒▒▒▒█░░░░░░░███▒▒▒▒▒▒▒▒▒▒▒▒▒███
                                            ░░░█▒▒▒▒▒▒█░░░░██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██
                                            ░░░░█▒▒▒▒▒█░░░██▒▒▒▒▒██▒▒▒▒▒▒██▒▒▒▒▒███
                                            ░░░░░█▒▒▒█░░░█▒▒▒▒▒▒████▒▒▒▒████▒▒▒▒▒▒██
                                            ░░█████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██
                                            ░░░█▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒██
                                            ░██▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒██▒▒▒▒▒▒▒▒▒▒██▒▒▒▒██
                                            ██▒▒▒███████████▒▒▒▒▒██▒▒▒▒▒▒▒▒██▒▒▒▒▒██
                                            █▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒████████▒▒▒▒▒▒▒██
                                            ██▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██
                                            ░█▒▒▒███████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██
                                            ░██▒▒▒▒▒▒▒▒▒▒████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█
░                                            ░████████████░░░█████████████████
*/

//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Windows.Forms;

//namespace Rebel_AssaultTesting
//{
//    public class Enemy
//    {
//        #region Properties
//        public PictureBox Sprite { get; private set; }
//        public string Tag { get; private set; }
//        public bool IsActive { get; private set; }
//        public int Health { get; private set; }
//        public Rectangle HitBox => new Rectangle(Position.X - MapOffset, Position.Y, Sprite.Width, Sprite.Height);

//        private Point Position;
//        private int MapOffset;
//        private int Speed;
//        private bool FacingLeft;
//        private int CurrentFrame;
//        private int SpawnPoint;
//        private bool IsAttacking;
//        private Dictionary<string, Image[]> Animations;
//        private string CurrentAnimation;
//        private readonly int GroundLevel;

//        // Animation constants
//        private const int IDLE_FRAMES = 4;
//        private const int WALK_FRAMES = 6;
//        private const int ATTACK_FRAMES = 8;
//        private const int DEATH_FRAMES = 6;
//        #endregion

//        #region Constructor
//        public Enemy(string enemyType, Point spawnPosition, int groundLevel, string basePath)
//        {
//            Tag = enemyType;
//            Position = spawnPosition;
//            SpawnPoint = spawnPosition.X;
//            GroundLevel = groundLevel;
//            IsActive = false;
//            FacingLeft = true;

//            InitializeProperties(enemyType);
//            InitializeSprite();
//            LoadAnimations(basePath, enemyType);
//        }

//        private void InitializeProperties(string enemyType)
//        {
//            switch (enemyType)
//            {
//                case "Soldier":
//                    Health = 100;
//                    Speed = 5;
//                    break;
//                case "Heavy":
//                    Health = 200;
//                    Speed = 3;
//                    break;
//                case "Scout":
//                    Health = 50;
//                    Speed = 8;
//                    break;
//                case "Boos":
//                    Health = 1500;
//                    Speed = 4;
//                    break;
//                default:
//                    throw new ArgumentException("Invalid enemy type");
//            }
//        }

//        private void InitializeSprite()
//        {
//            Sprite = new PictureBox
//            {
//                SizeMode = PictureBoxSizeMode.AutoSize,
//                Location = Position,
//                BorderStyle = BorderStyle.None,
//                BackColor = Color.Transparent,
//                Tag = Tag
//            };
//        }
//        #endregion

//        #region Animation Methods
//        private void LoadAnimations(string basePath, string enemyType)
//        {
//            string enemyPath = Path.Combine(basePath, "Enemies", enemyType);
//            Animations = new Dictionary<string, Image[]>
//            {
//                {"idle", LoadFrames(Path.Combine(enemyPath, "Idle"), IDLE_FRAMES)},
//                {"walk", LoadFrames(Path.Combine(enemyPath, "Walk"), WALK_FRAMES)},
//                {"attack", LoadFrames(Path.Combine(enemyPath, "Attack"), ATTACK_FRAMES)},
//                {"death", LoadFrames(Path.Combine(enemyPath, "Death"), DEATH_FRAMES)}
//            };

//            CurrentAnimation = "idle";
//            Sprite.Image = Animations[CurrentAnimation][0];
//        }

//        private Image[] LoadFrames(string folder, int frameCount)
//        {
//            var frames = new Image[frameCount];
//            for (int i = 0; i < frameCount; i++)
//            {
//                frames[i] = Image.FromFile(Path.Combine(folder, $"frame{i}.png"));
//            }
//            return frames;
//        }

//        public void Animate()
//        {
//            if (!IsActive) return;

//            CurrentFrame = (CurrentFrame + 1) % Animations[CurrentAnimation].Length;
//            var currentImage = Animations[CurrentAnimation][CurrentFrame];

//            if (FacingLeft)
//            {
//                currentImage = (Image)currentImage.Clone();
//                currentImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
//            }

//            Sprite.Image = currentImage;

//            // Si la animación de ataque termina, volver a idle
//            if (IsAttacking && CurrentFrame == Animations[CurrentAnimation].Length - 1)
//            {
//                IsAttacking = false;
//                CurrentAnimation = "idle";
//            }
//        }
//        #endregion

//        #region Game Logic Methods
//        public void Update(int playerX, int mapOffset)
//        {
//            if (!IsActive) return;

//            MapOffset = mapOffset;
//            UpdatePosition(playerX);
//            UpdateAnimation(playerX);
//            UpdateVisualPosition();
//        }

//        private void UpdatePosition(int playerX)
//        {
//            int relativeX = Position.X - MapOffset;
//            int distance = Math.Abs(playerX - relativeX);

//            // Determinar dirección basada en la posición del jugador
//            FacingLeft = playerX < relativeX;

//            // Mover hacia el jugador si está en rango
//            if (distance > 100 && distance < 300 && !IsAttacking)
//            {
//                Position = new Point(
//                    Position.X + (FacingLeft ? -Speed : Speed),
//                    Position.Y
//                );
//            }
//        }

//        private void UpdateAnimation(int playerX)
//        {
//            int distance = Math.Abs(playerX - (Position.X - MapOffset));

//            if (IsAttacking)
//            {
//                CurrentAnimation = "attack";
//            }
//            else if (distance <= 100)
//            {
//                StartAttack();
//            }
//            else if (distance < 300)
//            {
//                CurrentAnimation = "walk";
//            }
//            else
//            {
//                CurrentAnimation = "idle";
//            }
//        }

//        private void UpdateVisualPosition()
//        {
//            Sprite.Location = new Point(Position.X - MapOffset, Position.Y);
//        }

//        public void Activate()
//        {
//            IsActive = true;
//            CurrentAnimation = "idle";
//            CurrentFrame = 0;
//        }

//        public void TakeDamage(int damage)
//        {
//            Health -= damage;
//            if (Health <= 0)
//            {
//                Die();
//            }
//        }

//        private void StartAttack()
//        {
//            if (!IsAttacking)
//            {
//                IsAttacking = true;
//                CurrentAnimation = "attack";
//                CurrentFrame = 0;
//            }
//        }

//        private void Die()
//        {
//            CurrentAnimation = "death";
//            CurrentFrame = 0;
//            IsActive = false;
//        }
//        #endregion
//    }

//    public class EnemyManager
//    {
//        private List<Enemy> Enemies;
//        private readonly string BasePath;
//        private readonly int GroundLevel;
//        private readonly Random Random;

//        public EnemyManager(string basePath, int groundLevel)
//        {
//            BasePath = basePath;
//            GroundLevel = groundLevel;
//            Enemies = new List<Enemy>();
//            Random = new Random();
//            InitializeEnemies();
//        }

//        private void InitializeEnemies()
//        {
//            // Crear enemigos en diferentes puntos del mapa
//            for (int i = 0; i < 10; i++)
//            {
//                string enemyType = ChooseRandomEnemyType();
//                int spawnX = 800 + (i * 500) + Random.Next(-200, 200);

//                var enemy = new Enemy(
//                    enemyType,
//                    new Point(spawnX, GroundLevel - 100),
//                    GroundLevel,
//                    BasePath
//                );

//                Enemies.Add(enemy);
//            }
//        }

//        private string ChooseRandomEnemyType()
//        {
//            string[] types = { "Soldier", "Heavy", "Scout" };
//            return types[Random.Next(types.Length)];
//        }

//        public void UpdateEnemies(int playerX, int mapOffset)
//        {
//            foreach (var enemy in Enemies)
//            {
//                // Activar enemigos cuando el jugador se acerca
//                if (!enemy.IsActive && Math.Abs((enemy.Sprite.Location.X + mapOffset) - playerX) < 800)
//                {
//                    enemy.Activate();
//                }

//                enemy.Update(playerX, mapOffset);
//                enemy.Animate();
//            }
//        }



//        public List<Enemy> GetActiveEnemies()
//        {
//            return Enemies.FindAll(e => e.IsActive);
//        }
//    }
//}