/*
 * Clase Mapa
 * -----------
 * Esta clase gestiona la representación y el movimiento del mapa en el juego "Rebel Assault",
 * permitiendo la carga y el desplazamiento del fondo de la pantalla.
 *
 * Características principales:
 * - Carga la imagen del mapa desde el directorio del juego.
 * - Permite desplazar el mapa horizontalmente a lo largo del eje X.
 * - Dibuja el mapa escalado al tamaño de la ventana del juego.
 *
 * Uso:
 * La clase maneja el fondo del juego y su movimiento para simular el avance del jugador. Asegura que el mapa
 * se mantenga dentro de los límites de la pantalla y se dibuje adecuadamente.
 * 
 * Nota:
 * La imagen del mapa debe estar ubicada en el directorio "map" del juego.
 */


using System;
using System.Drawing;
using System.IO;

namespace Rebel_AssaultTesting
{
    public class Mapa
    {
        #region Propiedades
        private Bitmap mapa; 
        private double mapaX; 
        private double mapaY; 
        private int mapaOffsetX; 

        public int Width { get; private set; } 
        public int OffsetX => mapaOffsetX; 

        private int ventanaAncho; 
        private int ventanaAlto; 

        // Flags para controlar eventos del juego
        private bool beginningFlag;
        private bool missionStartFlag;
        private bool alarmFlag;
        private bool finalWaveFlag;
        #endregion

        #region Constructor
        public Mapa(string type, double x, double y, int anchoVentana, int altoVentana)
        {
            ventanaAncho = anchoVentana;
            ventanaAlto = altoVentana;

            if (type == "map")
            {
                InicializarMapa(x, y);
            }
        }
        #endregion

        #region Inicializacion del Mapa
        /// <summary>
        /// Inicializa el mapa del juego, configurando su posición y cargando su imagen.
        /// </summary>
        /// <param name="x">La coordenada X de la posición inicial del mapa.</param>
        /// <param name="y">La coordenada Y de la posición inicial del mapa.</param>
        private void InicializarMapa(double x, double y)
        {
            mapaX = x;
            mapaY = y;
            mapa = new Bitmap(CargarMapa(AppDomain.CurrentDomain.BaseDirectory));
            Width = mapa.Width;

            // Inicialización de flags
            finalWaveFlag = true;
            beginningFlag = true;
            alarmFlag = true;
            missionStartFlag = true;
        }

        /// <summary>
        /// Carga la ruta del mapa utilizando la ruta base proporcionada.
        /// </summary>
        /// <param name="basePath">La ruta base del directorio donde se encuentra el mapa.</param>
        /// <returns>La ruta completa del archivo de mapa (en este caso, "Map1.png").</returns>
        private string CargarMapa(string basePath)
        {
            return Path.Combine(basePath, "map", "Map1.png");
        }
        #endregion

        #region Logica Movimiento del Mapa

        /// <summary>
        /// Desplaza el mapa en la dirección horizontal, actualizando el offset X.
        /// </summary>
        /// <param name="deltaX">El valor de desplazamiento en el eje X. Un valor positivo mueve el mapa a la derecha, y un valor negativo lo mueve a la izquierda.</param>
        public void DesplazarX(int deltaX)
        {
            // Actualiza el offset X del mapa, asegurándose de que no exceda los límites a la izquierda ni a la derecha
            mapaOffsetX += deltaX;
            mapaOffsetX = Math.Max(0, Math.Min(mapaOffsetX, mapa.Width - ventanaAncho));
        }

        /// <summary>
        /// Dibuja el mapa escalado al tamaño de la ventana.
        /// </summary>
        /// <param name="g">El objeto <see cref="Graphics"/> utilizado para dibujar el mapa en la ventana.</param>
        public void Dibujar(Graphics g)
        {
            // Dibuja el mapa escalado al tamaño de la ventana
            g.DrawImage(mapa,
                new Rectangle(0, 0, ventanaAncho, ventanaAlto), // Dibuja el mapa en el tamaño de la ventana
                new Rectangle(mapaOffsetX, 0, ventanaAncho, ventanaAlto), // Parte del mapa a dibujar
                GraphicsUnit.Pixel);
        }

        #endregion
    }
}
