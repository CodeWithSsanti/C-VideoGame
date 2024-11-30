using System;
using System.Drawing;
using System.Windows.Forms;

// Clase GameManager
public class GameManager
{
    private MainCharacter player;
    private Timer gameTimer;
    private Form gameForm;

    public GameManager(Form form, string basePath)
    {
        gameForm = form;
        InitializePlayer(basePath);
        SetupTimer();
    }

    private void InitializePlayer(string basePath)
    {
        player = new MainCharacter(
            initialPosition: new Point(20, 270),
            speed: 11,
            runningSpeed: 30,
            jumpSpeed: 20,
            gravity: 3,
            groundLevel: 270,
            basePath: basePath
        );
        gameForm.Controls.Add(player.Sprite);
    }

    private void SetupTimer()
    {
        gameTimer = new Timer { Interval = 50 };  // 20 fps
        gameTimer.Tick += GameLoop;
        gameTimer.Start();
    }

    private void GameLoop(object sender, EventArgs e)
    {
        player.MoveAndAnimate();
        // Aquí puedes añadir más lógica del juego en el futuro
    }

    public void HandleKeyDown(Keys keyCode)
    {
        switch (keyCode)
        {
            case Keys.Left:
                player.StartMovingLeft();
                break;
            case Keys.Right:
                player.StartMovingRight();
                break;
            case Keys.Space:
                player.Jump();
                break;
            case Keys.Down:
                player.StartCrouching();
                break;
            case Keys.ShiftKey:
                player.StartRunning();
                break;
        }
    }

    public void HandleKeyUp(Keys keyCode)
    {
        switch (keyCode)
        {
            case Keys.Left:
                player.StopMovingLeft();
                break;
            case Keys.Right:
                player.StopMovingRight();
                break;
            case Keys.Down:
                player.StopCrouching();
                break;
            case Keys.ShiftKey:
                player.StopRunning();
                break;
        }
    }
}

// Clase GameForm (anteriormente Form1)
public partial class GameForm : Form
{
    private GameManager gameManager;

    public GameForm()
    {
        InitializeComponent();
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        gameManager = new GameManager(this, basePath);

        this.KeyPreview = true;
        this.KeyDown += GameForm_KeyDown;
        this.KeyUp += GameForm_KeyUp;
    }

    private void GameForm_KeyDown(object sender, KeyEventArgs e)
    {
        gameManager.HandleKeyDown(e.KeyCode);
    }

    private void GameForm_KeyUp(object sender, KeyEventArgs e)
    {
        gameManager.HandleKeyUp(e.KeyCode);
    }
}
