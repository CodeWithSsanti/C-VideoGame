/*
 * Clase ScoreEntry
 * -----------------
 * Representa una entrada de puntuación asociada a un jugador. 
 * Contiene información básica como el nombre del jugador y su puntuación.
 *
 * Características principales:
 * - Almacenamiento de nombre y puntuación.
 * - Propiedades para acceder y modificar los datos.
 *
 * Uso de la clase:
 * Se utiliza para registrar puntuaciones en una lista o tabla.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rebel_AssaultTesting
{
    public class ScoreEntry
    {
        private string playerName;
        private int score;

        public string PlayerName { get => playerName; set => playerName = value; }
        public int Score { get => score; set => score = value; }

        public ScoreEntry(string playerName, int score)
        {
            PlayerName = playerName;
            Score = score;
        }
    }
}
