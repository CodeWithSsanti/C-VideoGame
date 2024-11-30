/*Esta clase Sera la encargada de Mostrar el Mensaje cuando
 * El personaje Principal Muera, por Cuestiones de Tiempo
 * sera implementada en un Futuro*/


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
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Rebel_AssaultTesting
//{
//    public class GameOver : Form
//    {
//        private Panel ScorePanel;

//        public GameOver()
//        {
//            // Configuraciones básicas del form
//            this.Text = "Game Over";
//            this.Width = 400;
//            this.Height = 300;
//            this.StartPosition = FormStartPosition.CenterScreen;
//            this.FormBorderStyle = FormBorderStyle.FixedDialog;
//            this.MaximizeBox = false;
//            this.MinimizeBox = false;

//            // Etiqueta de Game Over
//            Label lblGameOver = new Label
//            {
//                Text = "GAME OVER",
//                Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold),
//                AutoSize = true,
//                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
//                Dock = DockStyle.Top
//            };
//            this.Controls.Add(lblGameOver);

//            // Panel para centrar los botones
//            TableLayoutPanel buttonPanel = new TableLayoutPanel
//            {
//                Dock = DockStyle.Bottom,
//                Height = 50,
//                ColumnCount = 2,
//                RowCount = 1
//            };
//            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
//            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

//            // Botón para reintentar
//            Button btnRetry = new Button
//            {
//                Text = "Reintentar",
//                Width = 100,
//                Anchor = AnchorStyles.None
//            };
//            btnRetry.Click += (s, e) =>
//            {
//                System.Diagnostics.Process.Start(Application.ExecutablePath);
//                Application.Exit();
//            };

//            // Botón para salir
//            Button btnExit = new Button
//            {
//                Text = "Salir",
//                Width = 100,
//                Anchor = AnchorStyles.None
//            };
//            btnExit.Click += (s, e) =>
//            {
//                Application.Exit();
//            };

//            // Añadir botones al panel
//            buttonPanel.Controls.Add(btnRetry, 0, 0);
//            buttonPanel.Controls.Add(btnExit, 1, 0);
//            this.Controls.Add(buttonPanel);

//            // Manejar el cierre del formulario
//            this.FormClosing += (s, e) =>
//            {
//                Application.Exit();
//            };
//        }

//        private void InitializeComponent()
//        {
//            this.ScorePanel = new System.Windows.Forms.Panel();
//            this.SuspendLayout();
//            // 
//            // GameOver
//            // 
//            this.ClientSize = new System.Drawing.Size(1335, 509);
//            this.Controls.Add(this.ScorePanel);
//            this.Name = "GameOver";
//            this.ResumeLayout(false);

//        }

//    }
//}
