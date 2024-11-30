/*
 * Clase GameObject
 * -----------------
 * Representa un objeto genérico en el juego con propiedades básicas para su manejo.
 *
 * Características principales:
 * - Almacena un sprite (`PictureBox`) para la representación visual del objeto.
 * - Define la posición, estado de actividad y una etiqueta (`tag`) para identificar el objeto.
 * - Proporciona una base común para otros objetos del juego que comparten estas propiedades.
 *
 * Uso de la clase:
 * Se utiliza como clase base para objetos interactivos o visuales del juego.
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
    public class GameObject
    {
        private PictureBox sprite;
        private Point position;
        private bool isActive;
        private string tag;

        public PictureBox Sprite { get => sprite; set => sprite = value; }
        public Point Position { get => position; set => position = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        protected string Tag { get => tag; set => tag = value; }

        public GameObject(PictureBox sprite, Point position, string tag)
        {
            this.Sprite = sprite;
            this.Position = position;
            this.IsActive = true; // Por defecto, el objeto está activo
            this.Tag = tag;

            // Configuración del PictureBox
            this.sprite = new PictureBox
            {
                Location = position,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Visible = true,
                BackColor = Color.Transparent,
                
            };

            this.sprite.Location = position;
        }

    } // End Class
}
