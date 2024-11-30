/*
 * Clase SoundManager
 * -------------------
 * Esta clase proporciona funcionalidades para manejar el audio en el juego "Rebel Assault".
 * Utiliza la biblioteca NAudio, una solución open-source avanzada para manejar audio en entornos .NET.
 *
 * Características principales:
 * - Reproducción de efectos de sonido, como disparos del jugador y del enemigo.
 * - Gestión de música de fondo con soporte para bucles continuos.
 * - Manejo eficiente de recursos de audio para evitar conflictos entre sonidos.
 *
 * Uso de NAudio:
 * La biblioteca NAudio permite reproducir múltiples sonidos simultáneamente y manejar formatos avanzados de audio,
 * como MP3 y WAV, superando las limitaciones de clases básicas como SoundPlayer.
 * Es ideal para proyectos de videojuegos que requieren un control detallado del audio.
 *
 * Nota:
 * Para usar esta clase, se debe proporcionar una ruta base (`BasePath`) donde se encuentren los archivos de audio organizados en una carpeta `Sounds`.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace Rebel_AssaultTesting
{
    public class SoundManager
    {
        #region Properties
        private string BasePath;
        private string SoundShootPlayer;
        private string SoundShootEnemy;
        private string Background;
        private string Mission1;

        private IWavePlayer wavePlayer; // Controlador para el sonido de Fondo
        private AudioFileReader AudioFileReader;

        #endregion

        #region Construtor
        public SoundManager(string BasePath)
        {
            this.BasePath = BasePath;
            InitializeSounds();
        }
        #endregion

        #region Inicializacion de Sonidos
        /// <summary>
        /// Carga y asigna los sonidos utilizados en el juego.
        /// </summary>
        /// <remarks>
        /// Este método carga los archivos de sonido para el disparo del jugador, el disparo del enemigo, 
        /// el fondo y la misión 1, asignándolos a las variables correspondientes.
        /// </remarks>
        private void InitializeSounds()
        {
            SoundShootPlayer = LoadSound("ShootPlayer1.wav");
            SoundShootEnemy = LoadSound("ShootEnemy1.wav");
            Background = LoadSound("Background.wav");
            Mission1 = LoadSound("Mission1.wav");
        }

        /// <summary>
        /// Carga la ruta completa de un archivo de sonido.
        /// </summary>
        /// <param name="Sound">El nombre del archivo de sonido a cargar.</param>
        /// <returns>La ruta completa al archivo de sonido.</returns>
        /// <remarks>
        /// Este método combina la ruta base con la carpeta "Sounds" y el nombre del archivo de sonido 
        /// para devolver la ruta completa donde se encuentra el archivo de sonido.
        /// </remarks>
        private string LoadSound(string Sound)
        {
            string FileSound = Path.Combine(BasePath, "Sounds");
            return Path.Combine(FileSound, Sound);
        }


        /// <summary>
        /// Reproduce el sonido de disparo del jugador.
        /// </summary>
        /// <remarks>
        /// Este método reproduce el sonido de disparo del jugador utilizando la ruta del archivo cargada 
        /// en la variable <see cref="SoundShootPlayer"/>.
        /// </remarks>
        internal void PlayShootPlayerSound()
        {
            PlaySound(SoundShootPlayer);
        }


        /// <summary>
        /// Reproduce el sonido de disparo del enemigo.
        /// </summary>
        /// <remarks>
        /// Este método reproduce el sonido de disparo del enemigo utilizando la ruta del archivo cargada 
        /// en la variable <see cref="SoundShootEnemy"/>.
        /// </remarks>
        internal void PlayShootEnemySound()
        {
            PlaySound(SoundShootEnemy);
        }


        /// <summary>
        /// Reproduce un sonido especificado por su ruta de archivo.
        /// </summary>
        /// <param name="StringSound">La ruta completa del archivo de sonido a reproducir.</param>
        /// <remarks>
        /// Este método crea una instancia de <see cref="SoundPlayer"/> utilizando la ruta proporcionada en 
        /// el parámetro <paramref name="StringSound"/> y reproduce el sonido. Si ocurre algún error durante 
        /// la carga o reproducción del sonido, se captura la excepción y se muestra un mensaje de error.
        /// </remarks>
        private void PlaySound(string StringSound)
        {
            try
            {
                using (SoundPlayer Sound = new SoundPlayer(StringSound))
                {
                    Sound.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Sonido Inicial y de Fondo
        /// <summary>
        /// Reproduce el sonido inicial del juego.
        /// </summary>
        /// <remarks>
        /// Este método intenta reproducir el sonido inicial llamando al método <see cref="PlayInitialSound"/>.
        /// Si ocurre algún error durante la reproducción, se captura la excepción y se muestra un mensaje de error.
        /// </remarks>
        internal void PlayInitialSoundAndBackground()
        {
            try
            {
                PlayInitialSound(); //Sonido Inicial
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Reproduce el sonido inicial del juego (por ejemplo, una introducción o música de inicio).
        /// </summary>
        /// <remarks>
        /// Este método carga el archivo de sonido especificado en la variable <see cref="Mission1"/> utilizando 
        /// un objeto <see cref="AudioFileReader"/> y un <see cref="WaveOutEvent"/> para la reproducción.
        /// Además, se suscribe al evento <see cref="PlaybackStopped"/> para iniciar la reproducción del sonido de fondo 
        /// cuando termine el sonido inicial.
        /// </remarks>
        private void PlayInitialSound()
        {
            AudioFileReader = new AudioFileReader(Mission1);
            wavePlayer = new WaveOutEvent();
            wavePlayer.Init(AudioFileReader);
            wavePlayer.PlaybackStopped += (s, e) => PlayBackgroundSound();
            wavePlayer.Play();
        }


        /// <summary>
        /// Reproduce el sonido de fondo del juego.
        /// </summary>
        /// <remarks>
        /// Este método detiene cualquier sonido de fondo previamente en reproducción (si lo hay) y luego 
        /// inicializa y reproduce el sonido de fondo utilizando el método <see cref="InitializeBackgroundSound"/>.
        /// Si ocurre un error durante la reproducción, se captura la excepción y se muestra un mensaje de error.
        /// </remarks>
        internal void PlayBackgroundSound()
        {
            try
            {
                if (wavePlayer != null) StopBackgroundSound();
                InitializeBackgroundSound();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Inicializa y reproduce el sonido de fondo del juego.
        /// </summary>
        /// <remarks>
        /// Este método carga el archivo de sonido de fondo especificado en la variable <see cref="Background"/> 
        /// utilizando un objeto <see cref="AudioFileReader"/> y un <see cref="WaveOutEvent"/> para la reproducción.
        /// Además, se suscribe al evento <see cref="PlaybackStopped"/> para reiniciar el sonido de fondo 
        /// cuando termine la reproducción.
        /// </remarks>
        private void InitializeBackgroundSound()
        {
            AudioFileReader = new AudioFileReader(Background);
            wavePlayer = new WaveOutEvent();
            wavePlayer.Init(AudioFileReader);
            wavePlayer.PlaybackStopped += (s, e) => RestartBackground();
            wavePlayer.Play();
        }


        /// <summary>
        /// Reinicia la reproducción del sonido de fondo del juego.
        /// </summary>
        /// <remarks>
        /// Este método llama al método <see cref="PlayBackgroundSound"/> para reiniciar la reproducción del sonido de fondo
        /// cuando se detiene o finaliza la reproducción actual.
        /// </remarks>
        private void RestartBackground() => PlayBackgroundSound();
        #endregion

        #region Detencion y Liberacion de Sonidos
        /// <summary>
        /// Detiene la reproducción del sonido de fondo y libera los recursos asociados.
        /// </summary>
        /// <remarks>
        /// Este método detiene la reproducción del sonido de fondo, elimina el objeto de reproducción 
        /// y libera los recursos utilizados, como el <see cref="AudioFileReader"/> y el <see cref="wavePlayer"/>.
        /// Si ocurre algún error durante el proceso, se captura la excepción y se muestra un mensaje de error.
        /// </remarks>
        private void StopBackgroundSound()
        {
            try
            {
                wavePlayer?.Stop();
                AudioFileReader?.Dispose();
                wavePlayer.Dispose();
                AudioFileReader = null;
                wavePlayer = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deteniendo el sonido de fondo: {ex.Message}");
                throw;
            }

        }
        #endregion

    }
}
