/*
 * Clase Character
 * ---------------
 * Esta clase gestiona los atributos y comportamientos básicos de un personaje en el juego "Rebel Assault".
 * Representa tanto a personajes jugables como enemigos, permitiendo controlar su estado y acciones durante el juego.
 *
 * Características principales:
 * - Salud, velocidad y animación del personaje.
 * - Gestión de la posición en el suelo y del estado de ataque.
 * - Control de la actividad del personaje (activo o inactivo).
 *
 * Uso:
 * Esta clase es fundamental para definir las propiedades de los personajes en el juego, como su movimiento, acciones y estado.
 * Es fácilmente extendible para añadir más funcionalidades como habilidades o IA para enemigos.
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rebel_AssaultTesting
{
    public class Character : GameObject
    {

        #region Atributos
        private int health;
        private int speed;
        private int currentAnimation;
        private int groundLevel;

        //boolean
        private bool isAttacking;
        private bool isActive;
        #endregion


        #region Constructor
        public Character(PictureBox sprite, Point position, string tag, int healt, int speed, int currentAnimation, int groundLevel,
        bool isAttacking, bool isActive) : base(sprite, position, tag)
        {

            Health = healt;
            Speed = speed;
            CurrentAnimation = currentAnimation;
            GroundLevel = groundLevel;
            IsAttacking = isAttacking;
            IsActive1 = isActive;
        }
        #endregion


        #region Metodos de acceso
        public int Health { get => health; set => health = value; }
        public int Speed { get => speed; set => speed = value; }
        public int CurrentAnimation { get => currentAnimation; set => currentAnimation = value; }
        public int GroundLevel { get => groundLevel; set => groundLevel = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public bool IsActive1 { get => isActive; set => isActive = value; }
        #endregion
    }
}
