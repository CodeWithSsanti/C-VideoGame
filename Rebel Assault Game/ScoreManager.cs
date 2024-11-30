/* 
 * Clase ScoreManager
 * ------------------
 * responsable de gestionar las puntuaciones de los jugadores.
 *
 * La clase ScoreManager permite guardar, cargar y procesar puntuaciones de jugadores.
 * Las puntuaciones se almacenan en un archivo de texto en el formato "NombreJugador,Puntuación".
 *
 * Funcionalidades principales:
 * - Guardar una nueva puntuación, manteniendo un máximo de 10 mejores puntajes.
 * - Cargar una lista de puntuaciones desde el archivo.
 * - Procesar las líneas del archivo para convertirlas en objetos ScoreEntry.
 *
 * Uso típico:
 * 1. Llamar a SaveScore(string playerName, int score) para agregar una puntuación.
 * 2. Llamar a LoadScores() para obtener las puntuaciones almacenadas.
 *
 * Requisitos:
 * - El archivo especificado por la variable filePath debe ser accesible y estar en el formato esperado.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace Rebel_AssaultTesting
{
    public class ScoreManager
    {
        private string filePath = "highscores.txt"; //Archivo donde se guardan los puntajes


        /// <summary>
        /// Guarda el puntaje de un jugador en la lista de puntuaciones.
        /// </summary>
        /// <param name="playerName">El nombre del jugador cuyo puntaje será guardado.</param>
        /// <param name="score">El puntaje obtenido por el jugador.</param>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Crea una nueva entrada de puntaje con el nombre del jugador y su puntaje.
        /// 2. Carga la lista actual de puntuaciones.
        /// 3. Agrega la nueva entrada a la lista y la ordena de mayor a menor.
        /// 4. Si la lista excede 10 entradas, se trunca para mantener solo las 10 mejores.
        /// 5. Guarda la lista actualizada de puntuaciones.
        /// </remarks>
        public void SaveScore(string playerName, int score)
        {
            var scoreEntry = new ScoreEntry(playerName, score);

            List<ScoreEntry> scores = LoadScores();
            scores.Add(scoreEntry);
            scores.Sort((x,y) => y.Score.CompareTo(x.Score)); //ordena de mayor a menor
            
            if(scores.Count > 10) scores = scores.GetRange(0, 10);
            SaveScores(scores);
        }


        /// <summary>
        /// Carga la lista de puntuaciones desde un archivo, si existe.
        /// </summary>
        /// <returns>
        /// Una lista de <see cref="ScoreEntry"/> que contiene las puntuaciones guardadas.
        /// Si el archivo no existe, se devuelve una lista vacía.
        /// </returns>
        /// <remarks>
        /// Este método verifica la existencia del archivo de puntuaciones en la ruta especificada.
        /// Si el archivo existe, se procesan los datos para llenar la lista de puntuaciones.
        /// De lo contrario, se devuelve una lista vacía.
        /// </remarks>
        public List<ScoreEntry> LoadScores()
        {
            List<ScoreEntry> scores = new List<ScoreEntry>();
            if (File.Exists(filePath)) NameAndScore(scores);
            return scores;
        }


        /// <summary>
        /// Procesa las líneas de un archivo de puntuaciones y las convierte en una lista de entradas de puntuación.
        /// </summary>
        /// <param name="scores">
        /// Una lista de <see cref="ScoreEntry"/> que será llenada con los datos del archivo.
        /// </param>
        /// <remarks>
        /// Este método lee todas las líneas del archivo de puntuaciones, donde cada línea debe estar
        /// en el formato "NombreJugador,Puntuación". Divide cada línea en dos partes (nombre y puntuación),
        /// valida los datos y los agrega a la lista proporcionada.
        /// 
        /// Si el formato de una línea no es válido o no se puede convertir la puntuación a un entero,
        /// se omite esa línea.
        /// </remarks>
        private void NameAndScore(List<ScoreEntry> scores)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    string playerName = parts[0];
                    int score;

                    if (int.TryParse(parts[1], out score))
                    {
                        scores.Add(new ScoreEntry(playerName, score));
                    }
                }

            }
        }

        /// <summary>
        /// Guarda una lista de puntuaciones en un archivo, sobrescribiendo su contenido.
        /// </summary>
        /// <param name="scores">
        /// Una lista de <see cref="ScoreEntry"/> que contiene los nombres de los jugadores y sus puntuaciones.
        /// </param>
        /// <remarks>
        /// Este método convierte cada entrada de puntuación en una cadena con el formato "NombreJugador,Puntuación"
        /// y escribe todas las líneas en un archivo especificado, sobrescribiendo cualquier contenido previo.
        /// 
        /// Es importante asegurarse de que el archivo de destino tenga permisos de escritura y que el objeto 
        /// <see cref="ScoreEntry"/> contenga datos válidos para evitar inconsistencias.
        /// </remarks>
        private void SaveScores(List<ScoreEntry> scores)
        {
            List<string> lines = new List<string>();
            foreach (var scoreEntry in scores)
            {
                lines.Add($"{scoreEntry.PlayerName},{scoreEntry.Score}");
            }

            File.WriteAllLines(filePath, lines); //Escribir lineas en el archivo
        }
    }
}
