/*
 * Clase Projectile
 * -----------------
 * Representa un proyectil con propiedades básicas como dirección, daño y propietario.
 *
 * Características principales:
 * - Almacena información sobre dirección, daño y propietario.
 * - Permite rastrear la posición del proyectil.
 * - Integra un sprite para su representación visual.
 *
 * Uso de la clase:
 * Se utiliza para manejar proyectiles en el juego.
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
    class Projectile : GameObject
    {
        #region propiedades
        private bool movingRight;
        private int damage;
        private string owner;
        private float GlobalX;
        private float GlobalY;
        #endregion

        #region Metodos de Acceso
        public bool MovingRight { get => movingRight; set => movingRight = value; }
        public int Damage { get => damage; set => damage = value; }
        public string Owner { get => owner; set => owner = value; }
        public float GlobalX1 { get => GlobalX; set => GlobalX = value; }
        public float GlobalY1 { get => GlobalY; set => GlobalY = value; }

        #endregion

        #region Constructor
        public Projectile(PictureBox sprite, Point position, bool movingRight, int damage, string tag)
        :base(sprite, position, tag)
        { 
            Sprite = sprite;
            Position = position;
            MovingRight = movingRight;
            Damage = damage;
            Owner = tag;
            IsActive = true;

            GlobalX = position.X;
            GlobalY = position.Y;

        }
        #endregion
    }
}
