/*
 * Clase MainCharacter
 * --------------------
 * Representa al personaje principal controlado por el jugador en el juego.
 *
 * Características principales:
 * - Almacena estados básicos del personaje, como correr, saltar o agacharse.
 * - Define velocidades de movimiento, gravedad y niveles del suelo.
 * - Maneja animaciones y sprites para representar visualmente las acciones del personaje.
 *
 * Uso de la clase:
 * Se utiliza para implementar la lógica del personaje principal, incluyendo movimiento, interacción y animaciones.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Rebel_AssaultTesting
{
    public class MainCharacter : Character
    {
        #region Atributos y Constructor
        public bool IsJumping { get; set; }
        public bool IsRunning { get; set; }
        public bool IsCrouching { get; set; }
        public bool MovingLeft { get; set; }
        public bool MovingRight { get; set; }
        public bool LastDirectionLeft { get; set; }
        public bool FacingRight { get; private set; } = true;

        // Campos privados
        //private int speed;
        private int runningSpeed;
        private int jumpSpeed;
        private int initialJumpSpeed;
        private int gravity;
        private int groundLevel;
        private int currentFrame;
        public new int Speed => IsRunning ? runningSpeed : base.Speed;
        private const int runFrameLimit = 5;
        private const int crouchingFrameLimit = 3;
        private bool startedRunning;
        private bool continueFromFrameSix;
        private bool startedCrouching;
        private bool continueToFrameFour;

        // Imágenes y animaciones
        private Dictionary<string, Image[]> animations;
        private Image stillRight, stillLeft;

        // Constructor
        public MainCharacter(PictureBox caracterSprite, Point initialPosition, int speed, int runningSpeed, int jumpSpeed,
        int gravity, int groundLevel, string basePath) : base(caracterSprite, initialPosition, "MainCharacter", 100, speed, 0,
        groundLevel, false, true)
        {
            Sprite = caracterSprite ?? new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = initialPosition,
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent,
            };

            this.groundLevel = groundLevel;
            this.runningSpeed = runningSpeed;
            this.initialJumpSpeed = jumpSpeed;
            this.jumpSpeed = jumpSpeed;
            this.gravity = gravity;

            IsJumping = false;
            IsRunning = false;
            IsCrouching = false;
            MovingLeft = false;
            MovingRight = false;
            LastDirectionLeft = false;

            LoadAnimations(basePath);
            Sprite.Image = stillRight;
        }
        #endregion

        #region Cargar Animaciones

        /// <summary>
        /// Carga todas las animaciones de un personaje desde un directorio base.
        /// Las animaciones incluyen caminar, correr, saltar, agacharse y estar quieto, tanto a la izquierda como a la derecha.
        /// También se asegura de que haya una imagen predeterminada para cuando el personaje está en reposo.
        /// </summary>
        /// <param name="basePath">La ruta base del directorio donde se encuentran las animaciones.</param>
        private void LoadAnimations(string basePath)
        {
            animations = new Dictionary<string, Image[]>
            {
            {"walkingLeft", LoadFrames(Path.Combine(basePath, "RugalMoving"), 12)},
            {"walkingRight", CreateMirroredFrames(LoadFrames(Path.Combine(basePath, "RugalMoving"), 12))},
            {"runningLeft", LoadFrames(Path.Combine(basePath, "RugalRunning"), 13)},
            {"runningRight", CreateMirroredFrames(LoadFrames(Path.Combine(basePath, "RugalRunning"), 13))},
            {"jumpLeft", LoadFrames(Path.Combine(basePath, "RugalJump"), 18)},
            {"jumpRight", CreateMirroredFrames(LoadFrames(Path.Combine(basePath, "RugalJump"), 18))},
            {"crouchingLeft", LoadFrames(Path.Combine(basePath, "RugalCrouching"), 4)},
            {"crouchingRight", CreateMirroredFrames(LoadFrames(Path.Combine(basePath, "RugalCrouching"), 4))},
            {"StillLeft", LoadFrames(Path.Combine(basePath, "RugalStill"), 30)},
            {"StillRight", CreateMirroredFrames(LoadFrames(Path.Combine(basePath, "RugalStill"), 30))}
            };

            //Asegura que haya una imagen predeterminada para cuando el personaje este en reposo
            stillRight = animations["StillRight"].FirstOrDefault(); //toma el primer frame del arreglo de imagenes
            stillLeft = animations["StillLeft"].FirstOrDefault();
        }

        /// <summary>
        /// Carga un conjunto de frames de imagen desde una carpeta específica.
        /// Asume que las imágenes están numeradas secuencialmente y las carga en un arreglo de imágenes.
        /// </summary>
        /// <param name="folder">La ruta de la carpeta que contiene los archivos de imagen.</param>
        /// <param name="count">El número total de frames a cargar.</param>
        /// <returns>Un arreglo de imágenes cargadas desde los archivos de la carpeta.</returns>
        private Image[] LoadFrames(string folder, int count)
        {
            var frames = new Image[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = Image.FromFile(Path.Combine(folder, $"{Path.GetFileName(folder)}{i + 1}.png"));
            }
            return frames;
        }

        /// <summary>
        /// Crea una copia espejada (flip horizontal) de cada frame de un arreglo de imágenes.
        /// Esto se utiliza para crear animaciones que se muestren en la dirección opuesta sin necesidad de duplicar las imágenes manualmente.
        /// </summary>
        /// <param name="originalFrames">Un arreglo de imágenes que contiene los frames originales.</param>
        /// <returns>Un arreglo de imágenes con los frames originales reflejados horizontalmente.</returns>
        private Image[] CreateMirroredFrames(Image[] originalFrames)
        {
            var mirroredFrames = new Image[originalFrames.Length];
            for (int i = 0; i < originalFrames.Length; i++)
            {
                mirroredFrames[i] = (Image)originalFrames[i].Clone();
                mirroredFrames[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return mirroredFrames;
        }
        #endregion

        #region Metodos para las Animaciones

        /// <summary>
        /// Mueve el personaje hacia la derecha por una cantidad especificada.
        /// Este método asegura que el personaje no se salga de los límites del área de juego y actualiza las direcciones de movimiento.
        /// </summary>
        /// <param name="amount">La cantidad de unidades que el personaje se moverá hacia la derecha.</param>
        public void MoveRight(int amount)
        {
            int newX = Position.X + amount;
            // Asegurar que el personaje no se salga de los límites
            Position = new Point(newX, Position.Y);
            MovingRight = true;
            MovingLeft = false;
            LastDirectionLeft = false;
        }


        /// <summary>
        /// Mueve el personaje hacia la izquierda por una cantidad especificada.
        /// Este método asegura que el personaje no se salga del límite izquierdo del área de juego y actualiza las direcciones de movimiento.
        /// </summary>
        /// <param name="amount">La cantidad de unidades que el personaje se moverá hacia la izquierda.</param>
        public void MoveLeft(int amount)
        {
            int newX = Math.Max(0, Position.X - amount);
            Position = new Point(newX, Position.Y);
            MovingLeft = true;
            MovingRight = false;
            LastDirectionLeft = true;
        }

        /// <summary>
        /// Mueve y anima al personaje en función de su estado y dirección.
        /// Este método actualiza tanto la posición del personaje como su animación, 
        /// considerando si está saltando, agachado, caminando, corriendo o en reposo.
        /// </summary>
        public void MoveAndAnimate()
        {
            MoveHorizontally();

            if (IsJumping)
            {
                ApplyJump();
                AnimateJump();
            }
            else if (IsCrouching)
            {
                AnimateCrouching();
            }
            else
            {
                if (MovingLeft || MovingRight)
                {
                    if (IsRunning)
                    {
                        AnimateRunning();
                    }
                    else
                    {
                        AnimateWalking();
                    }
                }
                else
                {
                    AnimateStill();
                }
            }

            ApplyGravity();
        }

        /// <summary>
        /// Inicia el salto del personaje, estableciendo las condiciones necesarias para que el personaje salte.
        /// </summary>
        /// <remarks>
        /// El salto solo se iniciará si el personaje no está actualmente en el aire y no está agachado.
        /// Se establece la velocidad inicial del salto y se reinicia el contador de fotogramas.
        /// </remarks>
        public void Jump()
        {
            if (!IsJumping && !IsCrouching)
            {
                IsJumping = true;
                jumpSpeed = initialJumpSpeed;
                currentFrame = 0;
            }
        }


        /// <summary>
        /// Aplica el movimiento de salto al personaje, actualizando su posición y velocidad.
        /// </summary>
        /// <remarks>
        /// Este método ajusta la posición vertical del personaje, disminuyendo la velocidad del salto por la gravedad. 
        /// Si el personaje llega al nivel del suelo, se detiene el salto y se restablecen los valores correspondientes.
        /// </remarks>
        private void ApplyJump()
        {
            Position = new Point(Position.X, Position.Y - jumpSpeed);
            jumpSpeed -= gravity;

            if (Position.Y >= groundLevel)
            {
                Position = new Point(Position.X, groundLevel);
                IsJumping = false;
                jumpSpeed = initialJumpSpeed;
                Sprite.Image = LastDirectionLeft ? stillLeft : stillRight;
            }
        }

        /// <summary>
        /// Mueve al personaje horizontalmente, ajustando su posición según la dirección y velocidad.
        /// </summary>
        /// <remarks>
        /// Este método mueve al personaje hacia la izquierda o derecha, dependiendo de las condiciones de las propiedades 
        /// `MovingLeft` y `MovingRight`. La velocidad del movimiento depende de si el personaje está corriendo o no.
        /// </remarks>
        private void MoveHorizontally()
        {
            int currentSpeed = IsRunning ? runningSpeed : Speed;

            if (MovingLeft)
            {
                Position = new Point(Position.X - currentSpeed, Position.Y);
                LastDirectionLeft = true;
            }
            else if (MovingRight)
            {
                Position = new Point(Position.X + currentSpeed, Position.Y);
                LastDirectionLeft = false;
            }
        }

        /// <summary>
        /// Aplica la gravedad al personaje, ajustando su posición vertical.
        /// </summary>
        /// <remarks>
        /// Este método aumenta la posición vertical del personaje en cada llamada, simulando el efecto de la gravedad,
        /// hasta que el personaje alcanza el nivel del suelo. La gravedad solo se aplica cuando el personaje no está saltando.
        /// </remarks>
        private void ApplyGravity()
        {
            if (!IsJumping && Position.Y < groundLevel)
            {
                Position = new Point(Position.X, Math.Min(Position.Y + gravity, groundLevel));
            }
        }

        /// <summary>
        /// Actualiza la animación de caminar del personaje según la dirección del movimiento.
        /// </summary>
        /// <remarks>
        /// Este método selecciona y actualiza el fotograma actual de la animación de caminar, dependiendo de si el personaje 
        /// se está moviendo hacia la derecha o hacia la izquierda. La animación se cicla entre los fotogramas correspondientes.
        /// </remarks>
        private void AnimateWalking()
        {
            string key = MovingRight ? "walkingRight" : "walkingLeft";
            currentFrame = (currentFrame + 1) % animations[key].Length;
            Sprite.Image = animations[key][currentFrame];
        }

        /// <summary>
        /// Actualiza la animación de correr del personaje según la dirección del movimiento y la lógica específica de las animaciones.
        /// </summary>
        /// <remarks>
        /// Este método controla el ciclo de animación para la acción de correr, gestionando la dirección del movimiento (derecha o izquierda).
        /// Además, maneja un comportamiento especial para las animaciones, comenzando en un fotograma específico y reanudando desde el fotograma seis cuando es necesario.
        /// </remarks>
        private void AnimateRunning()
        {
            string key = MovingRight ? "runningRight" : "runningLeft";

            if (!startedRunning)
            {
                currentFrame = 0;
                startedRunning = true;
                continueFromFrameSix = false;
            }

            if (continueFromFrameSix)
            {
                currentFrame = (currentFrame + 1) % animations[key].Length;
                if (currentFrame <= runFrameLimit) currentFrame = runFrameLimit + 1;
            }
            else
            {
                currentFrame = (currentFrame + 1) % animations[key].Length;
                if (currentFrame == runFrameLimit)
                {
                    continueFromFrameSix = true;
                }
            }

            Sprite.Image = animations[key][currentFrame];
        }

        /// <summary>
        /// Actualiza la animación de salto del personaje según la dirección del movimiento.
        /// </summary>
        /// <remarks>
        /// Este método actualiza el fotograma de la animación de salto, basándose en la última dirección en la que se movió el personaje.
        /// La animación se cicla entre los fotogramas correspondientes al salto hacia la izquierda o hacia la derecha.
        /// </remarks>
        private void AnimateJump()
        {
            string key = LastDirectionLeft ? "jumpLeft" : "jumpRight";
            currentFrame = (currentFrame + 1) % animations[key].Length;
            Sprite.Image = animations[key][currentFrame];
        }

        /// <summary>
        /// Actualiza la animación de estado estático del personaje según la última dirección en la que se movió.
        /// </summary>
        /// <remarks>
        /// Este método cicla entre los fotogramas de la animación de estado estático (sin movimiento) del personaje,
        /// eligiendo la animación correspondiente a la dirección en la que el personaje estaba mirando la última vez.
        /// </remarks>
        private void AnimateStill()
        {
            string key = LastDirectionLeft ? "StillLeft" : "StillRight";
            currentFrame = (currentFrame + 1) % animations[key].Length;
            Sprite.Image = animations[key][currentFrame];
        }

        /// <summary>
        /// Actualiza la animación de agacharse del personaje según la dirección en la que se mueve.
        /// </summary>
        /// <remarks>
        /// Este método controla la animación de agacharse, seleccionando la animación correspondiente según si el personaje está mirando hacia la izquierda o hacia la derecha.
        /// Además, maneja una lógica especial para ciclar entre los fotogramas de la animación, comenzando desde el primer fotograma y deteniéndose en un fotograma específico después de alcanzar un límite.
        /// </remarks>
        private void AnimateCrouching()
        {
            string key = LastDirectionLeft ? "crouchingLeft" : "crouchingRight";

            if (!startedCrouching)
            {
                currentFrame = 0;
                startedCrouching = true;
                continueToFrameFour = false;
            }

            if (!continueToFrameFour)
            {
                currentFrame = (currentFrame + 1) % animations[key].Length;
                if (currentFrame == crouchingFrameLimit)
                {
                    continueToFrameFour = true;
                }
            }
            else
            {
                currentFrame = crouchingFrameLimit;
            }

            Sprite.Image = animations[key][currentFrame];
        }


        /// <summary>
        /// Inicia el movimiento del personaje hacia la izquierda.
        /// </summary>
        /// <remarks>
        /// Este método cambia el estado del personaje para que empiece a moverse hacia la izquierda,
        /// actualizando las propiedades `MovingLeft` y `FacingRight` para reflejar la nueva dirección.
        /// </remarks>
        public void StartMovingLeft()
        {
            MovingLeft = true;
            FacingRight = false;
        }

        /// <summary>
        /// Inicia el movimiento del personaje hacia la derecha.
        /// </summary>
        /// <remarks>
        /// Este método cambia el estado del personaje para que empiece a moverse hacia la derecha,
        /// actualizando las propiedades `MovingRight` y `FacingRight` para reflejar la nueva dirección.
        /// </remarks>
        public void StartMovingRight()
        {
            MovingRight = true;
            FacingRight = true;
        }

        /// <summary>
        /// Detiene el movimiento del personaje hacia la izquierda.
        /// </summary>
        /// <remarks>
        /// Este método cambia el estado del personaje para que deje de moverse hacia la izquierda,
        /// estableciendo la propiedad `MovingLeft` a `false`.
        /// </remarks>
        public void StopMovingLeft() => MovingLeft = false;

        /// <summary>
        /// Detiene el movimiento del personaje hacia la derecha.
        /// </summary>
        /// <remarks>
        /// Este método cambia el estado del personaje para que deje de moverse hacia la derecha,
        /// estableciendo la propiedad `MovingRight` a `false`.
        /// </remarks>
        public void StopMovingRight() => MovingRight = false;

        /// <summary>
        /// Inicia el estado de correr del personaje.
        /// </summary>
        /// <remarks>
        /// Este método establece la propiedad `IsRunning` a `true`, indicando que el personaje ha comenzado a correr.
        /// </remarks>
        public void StartRunning() => IsRunning = true;

        /// <summary>
        /// Detiene el estado de correr del personaje.
        /// </summary>
        /// <remarks>
        /// Este método establece la propiedad `IsRunning` a `false`, lo que indica que el personaje ha dejado de correr.
        /// También restablece la variable `startedRunning` a `false` para reiniciar el estado de carrera.
        /// </remarks>
        public void StopRunning()
        {
            IsRunning = false;
            startedRunning = false;
        }

        /// <summary>
        /// Inicia el estado de agacharse del personaje.
        /// </summary>
        /// <remarks>
        /// Este método establece la propiedad `IsCrouching` a `true`, indicando que el personaje ha comenzado a agacharse.
        /// </remarks>
        public void StartCrouching() => IsCrouching = true;

        /// <summary>
        /// Detiene el estado de agacharse del personaje.
        /// </summary>
        /// <remarks>
        /// Este método establece la propiedad `IsCrouching` a `false`, indicando que el personaje ha dejado de agacharse.
        /// También restablece la variable `startedCrouching` a `false` para reiniciar el estado de agachado.
        /// </remarks>
        public void StopCrouching()
        {
            IsCrouching = false;
            startedCrouching = false;
        }
        #endregion

    }

}
