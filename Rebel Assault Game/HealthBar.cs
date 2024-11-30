/*
 * Clase HealthBar
 * ---------------
 * Esta clase gestiona la visualización de la barra de vida de un personaje en el juego "Rebel Assault",
 * actualizando la interfaz de usuario en función de la salud actual del personaje.
 *
 * Características principales:
 * - Inicialización de la barra: Crea el contenedor y el relleno de la barra de vida.
 * - Actualización de la barra: Ajusta el tamaño y color del relleno basado en el valor de salud.
 * - Posicionamiento: Permite cambiar la posición de la barra de vida en la pantalla.
 * - Liberación de recursos: Libera los recursos de los controles utilizados para la barra de vida.
 *
 * Lógica de funcionamiento:
 * La barra de vida se ajusta automáticamente cuando la salud del personaje cambia, modificando el tamaño
 * del relleno y cambiando el color para reflejar el estado de salud (verde para alta salud, amarillo para media, rojo para baja).
 * Además, la posición de la barra se puede actualizar en cualquier momento.
 *
 * Notas importantes:
 * - Se deben crear controles de tipo `PictureBox` para representar tanto el contenedor como el relleno de la barra.
 * - La barra se actualiza dinámicamente en función de los cambios en la salud del personaje.
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
    class HealthBar : GameObject
    {
        #region Propiedades
        private int maxHealth;
        private int currentHealth;
        private PictureBox container;
        private PictureBox fill;
        #endregion

        #region Metodos de Acceso
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int CurrentHealth { get => currentHealth; set => UpdateHealth(value); }
        public PictureBox Container { get => container; set => container = value; }
        public PictureBox Fill { get => fill; set => fill = value; }
        #endregion

        #region Constructor
        public HealthBar(Point position, int width, int height, int maxHealth)
       : base(null, position, "HealthBar")
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;

            InitializeContainer(position, width, height);
            InitializeFill(width, height);

        }
        #endregion

        #region Inicializacion de Contenedores

        /// <summary>
        /// Inicializa el contenedor de la barra de vida.
        /// </summary>
        private void InitializeContainer(Point position, int width, int height)
        {
            Container = new PictureBox
            {
                Location = position,
                Size = new Size(width, height),
                BackColor = Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };
            Container.Location = position;
        }



        /// <summary>
        /// Inicializa el relleno de la barra de vida.
        /// </summary>
        private void InitializeFill(int width, int height)
        {
            fill = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(width, height),
                BackColor = Color.Green
            };
            container.Controls.Add(fill);
        }

        #endregion

        #region Logica de Barras de Vida

        /// <summary>
        /// Actualiza la barra de vida con el nuevo valor de salud del personaje.
        /// </summary>
        /// <remarks>
        /// Este método ajusta la barra de vida en la interfaz de usuario según el valor de salud proporcionado.
        /// El ancho de la barra de vida se actualiza proporcionalmente, y el color del relleno cambia según el nivel de salud:
        /// verde para salud alta, amarillo para salud media, y rojo para salud baja.
        /// </remarks>
        /// <param name="newHealth">El nuevo valor de salud que se desea establecer en la barra de vida.</param>
        private void UpdateHealth(int newHealth)
        {
            if(container == null || fill == null) return;

            currentHealth = Math.Max(0, Math.Min(newHealth, MaxHealth));

            // Calcular el nuevo ancho del relleno basado en la salud 
            int newWidth =(int)((float)currentHealth / MaxHealth * (container.Width - 4));
            fill.Width = Math.Max(0, newWidth);

            // Cambiar el color según el nivel de vida
            if (currentHealth > maxHealth * 0.6f)
                fill.BackColor = Color.Green;
            else if (currentHealth > maxHealth * 0.3f)
                fill.BackColor = Color.Yellow;
            else
                fill.BackColor = Color.Red;

        }

        /// <summary>
        /// Cambia la posición de la barra de vida.
        /// </summary>
        public void UpdatePosition(Point newPosition)
        {
            container.Location = newPosition;
        }

        #endregion

        #region Liberador de Recursos
        /// <summary>
        /// Libera los recursos de los controles de la barra de vida.
        /// </summary>
        public void Dispose()
        {
            fill.Dispose();
            container.Dispose();
        }
        #endregion

    }
}
