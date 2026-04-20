using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EN_Lec_07_KeyEvents_PingPong
{
    public partial class Form1 : Form
    {
        private RectangleF player;
        private Keys playerDirection;

        private RectangleF ball;
        private PointF ballDirection;

        private List<RectangleF> bricks;
        private List<RectangleF> bullets;

        private Timer gameTimer;
        private int score;

        // === CONSTS ===
        private const int PLAYER_WIDTH = 100;
        private const int PLAYER_HEIGHT = 20;

        private const float PLAYER_SPEED = 20f;

        private const float BALL_SIZE = 20f;
        private const float BALL_SPEED_X = 10f;
        private const float BALL_SPEED_Y = 10f;

        private const float BULLET_WIDTH = 10f;
        private const float BULLET_HEIGHT = 10f;
        private const float BULLET_SPEED = 10f;

        private const int BRICK_ROWS = 5;
        private const int BRICK_COLUMNS = 10;
        private const float BRICK_HEIGHT = 30f;

        private const int TIMER_INTERVAL = 20;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            gameTimer = new Timer();
            gameTimer.Interval = TIMER_INTERVAL;
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (playerDirection == Keys.Left)
            {
                player.X -= PLAYER_SPEED;
            }
            else if (playerDirection == Keys.Right)
            {
                player.X += PLAYER_SPEED;
            }

            ball.X += ballDirection.X;
            ball.Y += ballDirection.Y;

            // player
            if (player.IntersectsWith(ball) && ballDirection.Y > 0)
            {
                ballDirection.Y = -ballDirection.Y;
            }

            // top
            if (ball.Y <= 0)
            {
                ballDirection.Y = -ballDirection.Y;
            }

            // left and right
            if (ball.X <= 0 || ball.X + ball.Width >= this.Width)
            {
                ballDirection.X = -ballDirection.X;
            }

            // bricks
            for (int i = 0; i < bricks.Count; i++)
            {
                if (bricks[i].IntersectsWith(ball))
                {
                    bricks.RemoveAt(i);
                    ballDirection.Y = -ballDirection.Y;
                    score++;
                    break;
                }
            }

            // bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i] = new RectangleF(
                    bullets[i].X,
                    bullets[i].Y - BULLET_SPEED,
                    bullets[i].Width,
                    bullets[i].Height);

                for (int j = 0; j < bricks.Count; j++)
                {
                    if (bricks[j].IntersectsWith(bullets[i]))
                    {
                        bricks.RemoveAt(j);
                        bullets.RemoveAt(i);
                        score++;
                        break;
                    }
                }
            }

            if (bricks.Count == 0)
            {
                gameTimer.Stop();
                MessageBox.Show("You Win!");
                PrepareNewGame();
            }

            if (ball.Y > this.Height)
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over!");
                PrepareNewGame();
            }

            Invalidate();
        }

        private void PrepareNewGame()
        {
            player = new RectangleF(
                (this.Width / 2) - PLAYER_WIDTH / 2,
                this.Height - 50,
                PLAYER_WIDTH,
                PLAYER_HEIGHT);

            playerDirection = Keys.None;

            ball = new RectangleF(
                this.Width / 2,
                this.Height / 2,
                BALL_SIZE,
                BALL_SIZE);

            ballDirection = new PointF(BALL_SPEED_X, BALL_SPEED_Y);

            bricks = new List<RectangleF>();

            for (int row = 0; row < BRICK_ROWS; row++)
            {
                for (int col = 0; col < BRICK_COLUMNS; col++)
                {
                    bricks.Add(new RectangleF(
                        col * this.Width / BRICK_COLUMNS,
                        row * BRICK_HEIGHT,
                        this.Width / BRICK_COLUMNS,
                        BRICK_HEIGHT));
                }
            }

            bullets = new List<RectangleF>();

            score = 0;

            Invalidate();
            gameTimer.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                playerDirection = e.KeyCode;
            }

            if (e.KeyCode == Keys.Space)
            {
                bullets.Add(new RectangleF(
                    player.X + player.Width / 2 - BULLET_WIDTH / 2,
                    player.Y - BULLET_HEIGHT,
                    BULLET_WIDTH,
                    BULLET_HEIGHT));
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.P)
            {
                gameTimer.Enabled = !gameTimer.Enabled;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                playerDirection = Keys.None;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            e.Graphics.FillRectangle(Brushes.White, player);
            e.Graphics.FillEllipse(Brushes.Yellow, ball);

            foreach (RectangleF brick in bricks)
            {
                e.Graphics.FillRectangle(Brushes.Red, brick);
                e.Graphics.DrawRectangle(Pens.White, brick.X, brick.Y, brick.Width, brick.Height);
            }

            foreach (RectangleF bullet in bullets)
            {
                e.Graphics.FillEllipse(Brushes.Yellow, bullet);
            }

            e.Graphics.DrawString("Score: " + score,
                new Font("Arial", 16),
                Brushes.White,
                10,
                10);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrepareNewGame();
        }
    }
}