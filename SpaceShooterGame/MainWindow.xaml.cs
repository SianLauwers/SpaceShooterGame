using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;
using System.IO;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using PeopleListAddSqlite;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Numerics;
using SpaceShooterGame.DAL;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection.Metadata;

namespace SpaceShooterGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();
        List<User> people = new List<User>();

        Random rand = new Random();

        CRUDRepository crudrepository = new CRUDRepository();
        SqliteRepository sqliteRepository= new SqliteRepository();

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 15;
        int limit = 50;
        int score = 30;
        int damage = 0;
        int enemySpeed = 10;
        //int highScore = 0;

        Rect playerHitBox;

        public List<User> DatabaseUsers { get; private set; }


        //public void LoadPeopleList()
        //{
        //people = SqliteDataAccess.LoadPeople();

        //highScoreText.Content = "High Score: " + people;
        //WireUpPeopleList();
        //}


        //private void WireUpPeopleList()
        //{
        //listPeopleListBox.DataContext = null;
        //listPeopleListBox.DataContext= people;
        //listPeopleListBox.DisplayMemberPath = "highestScore";
        //}

        //private void addPerson()
        //{
        //User p = new User();
        //var highScore = SqliteDataAccess.LoadPeople();
        //p.HighScore = highScore;

        //SqliteDataAccess.SavePerson(p);

        //}



        public MainWindow()
        {
            InitializeComponent();

            //LoadPeopleList();
            //WireUpPeopleList();
            //addPerson();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            SoundPlayer playsound = new SoundPlayer(Properties.Resources.spaceShooter);
            playsound.Play();

            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/bg.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.75, 0.75);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            Player.Fill = playerImage;

        }

        public void Create()
        {
            using (DataContext context= new DataContext())
            {
                var highscore = 15;

                context.Users.Add(new User() {HighScore = highscore });
                context.SaveChanges();
            }
        }

        public string Read()
        {


            return crudrepository.GetUsers().ToString();

            
            //using (DataContext context = new DataContext())
            //{
                //var highScore = context.Users.FromSqlInterpolated<User>($"SELECT HighScore FROM HighScore");

                //return highScore;
            //}



            //var highScore = crudrepository.GetUsers().

            //Console.WriteLine(highScore.ToString());
            //MessageBox.Show(highScore);
        }

        public void Update()
        {
            using (DataContext context= new DataContext())
            {
                User selectedUser = ItemList.SelectedItem as User;

                var highscore = 15;

                if (highscore > 15)
                {
                    User user = context.Users.Find(selectedUser.Id);

                    user.HighScore = highscore;

                    context.SaveChanges();
                }
            }
        }
        
        public void Delete()
        {
            using (DataContext context = new DataContext())
            {
                User selectedUser = ItemList.SelectedItem as User;
                
                if (selectedUser != null)
                {
                    User user = context.Users.Find(selectedUser.Id);

                    context.Remove(user);
                    context.SaveChanges();
                }
                
            }
        }

        

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

            enemyCounter -= 1;

            scoreText.Content = "Score: " + score;
            damageText.Content = "Damage: " + damage;
            ///highScoreText.Content = "High Score: " + highScore;

            //addPerson();
            //WireUpPeopleList();

            

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(Player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + playerSpeed);
            }


            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                               itemRemover.Add(x);
                               itemRemover.Add(y);
                               SoundPlayer explode = new SoundPlayer(Properties.Resources.explosion);
                               explode.Play();
                               score++;
                            }
                        }
                    }

                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        SoundPlayer explode = new SoundPlayer(Properties.Resources.explosion);
                        explode.Play();
                        itemRemover.Add(x);
                        damage += 5;
                    }
                }
            }

            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }

            if (score > 10)
            {
                limit = 40;
                enemySpeed = 11;
            }

            if (score > 20)
            {
                limit = 30;
                enemySpeed = 12;
            }

            if (score > 30)
            {
                limit = 20;
                enemySpeed = 13;
            }

            if (score > 40)
            {
                limit = 20;
                enemySpeed = 14;
            }

            if (score > 50)
            {
                limit = 15;
                enemySpeed = 15;
            }

            if (score >= 100)
            {
                limit = 20;
                enemySpeed = 20;
            }

            if (damage > 99)
            {
                gameTimer.Stop();
                damageText.Content = "Damage: 100";
                damageText.Foreground = Brushes.Red;

                //addPerson();
                //var highScore = SqliteDataAccess.LoadPeople();
                //if (score > DatabaseUsers[1])
                //{

                //}
                MessageBox.Show("Captain, you have killed " + score + " enemy ships." + Environment.NewLine + "Your Highest score is: " + Read() + Environment.NewLine + "Press OK to Try Again.", "Game Over") ;
                ///System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(Player) - newBullet.Height);

                MyCanvas.Children.Add(newBullet);

            }
        }

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 6);

            switch (enemySpriteCounter) 
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/3.png"));
                    break;
                case 4:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/4.png"));
                    break;
                case 5:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/5.png"));
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430));
            MyCanvas.Children.Add(newEnemy);

        }
    }
}
