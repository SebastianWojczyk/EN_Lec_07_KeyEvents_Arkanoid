using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EN_Lec_07_KeyEvents_PingPong
{
    public partial class Form1 : Form
    {
        private RectangleF player;
        private int playerWidth = 100;
        private int playerHeight = 20;
        private Keys playerDirection;

        private RectangleF ball;
        private PointF ballDirection;

        List<RectangleF> bricks;

        List<RectangleF> bullets;

        private Timer gameTimer;

        private int score;
        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (playerDirection == Keys.Left)
            {
                player.X -= 20;
            }
            else if (playerDirection == Keys.Right)
            {
                player.X += 20;
            }
            ball.X += ballDirection.X;
            ball.Y += ballDirection.Y;

            //player
            if (player.IntersectsWith(ball) && ballDirection.Y > 0)
            {
                ballDirection.Y = -ballDirection.Y;
            }
            //top
            if (ball.Y <= 0)
            {
                ballDirection.Y = -ballDirection.Y;
            }
            //left and right
            if (ball.X <= 0 || ball.X + ball.Width >= this.Width)
            {
                ballDirection.X = -ballDirection.X;
            }
            //bricks
            foreach (RectangleF brick in bricks)
            {
                if (brick.IntersectsWith(ball))
                {
                    bricks.Remove(brick);
                    ballDirection.Y = -ballDirection.Y;
                    score++;
                    break;
                }
            }

            //foreach(RectangleF bullet in bullets)
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i] = new RectangleF(bullets[i].X,
                                            bullets[i].Y - 10,
                                            bullets[i].Width,
                                            bullets[i].Height);
                foreach (RectangleF brick in bricks)
                {
                    if (brick.IntersectsWith(bullets[i]))
                    {
                        bricks.Remove(brick);
                        bullets.Remove(bullets[i]);
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
            player = new RectangleF((this.Width / 2) - playerWidth / 2,
                                   this.Height - 50,
                                   playerWidth,
                                   playerHeight);
            playerDirection = Keys.None;

            ball = new RectangleF(this.Width / 2, this.Height / 2, 20, 20);
            ballDirection = new PointF(10, 10);

            bricks = new List<RectangleF>();

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    bricks.Add(new RectangleF(col * this.Width / 10,
                                               row * 30,
                                               this.Width / 10,
                                               30));
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
                bullets.Add(new RectangleF(player.X + player.Width / 2 - 5,
                                           player.Y - 10,
                                           10,
                                           10));
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

            e.Graphics.DrawString("Score: " + score, new Font("Arial", 16), Brushes.White, 10, 10);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrepareNewGame();
        }


    }
}
