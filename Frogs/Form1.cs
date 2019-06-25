﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Input;

namespace Frogs
{
    public partial class Form1 : Form
    {
        public bool players;

        public Timer gametimer;
        public Timer frametimer;
        public Timer flyspawntimer;
        public Timer secondtimer;

        int seconds;
        int time;

        Player1 player1;
        Player2 player2;

        Image logoimage;
        Image background;
        Bitmap bitmap;

        bool playing;
        bool pause;

        public string file;
        public GameFile info;

        public Form1()
        {
            logoimage = Properties.Resources.logo;
            background = Properties.Resources.background;
            bitmap = new Bitmap(background);

            InitializeComponent();
            DoubleBuffered = true;
            KeyPreview = true;
            info = new GameFile();
            logo.BackColor = Color.Transparent;
            logo.Image = logoimage;
            playing = false;
            pause = false;
        }


        void StartGame(int t) {

            playing = true;
            time = t;

            seconds = 0;

            player1 = new Player1();

            if(players)
            player2 = new Player2();

            secondtimer = new Timer();
            secondtimer.Interval = 1000;

            gametimer = new Timer();
            gametimer.Interval = time*1000;

            frametimer = new Timer();
            frametimer.Interval = 16;

            flyspawntimer = new Timer();
            if (players)
                flyspawntimer.Interval = 800;
            else flyspawntimer.Interval = 1500;

            gametimer.Tick += new EventHandler(gametimer_Tick);
            frametimer.Tick += new EventHandler(frametimer_Tick);
            flyspawntimer.Tick += new EventHandler(flyspawn_Tick);
            secondtimer.Tick += new EventHandler(secondtimer_Tick);

            gametimer.Enabled = true;
            frametimer.Enabled = true;
            flyspawntimer.Enabled = true;
            secondtimer.Enabled = true;

        }

        private void secondtimer_Tick(object sender, System.EventArgs e)
        {
            lblTime.Text = (time - seconds).ToString();
            seconds++;
        }

        private void gametimer_Tick(object sender, System.EventArgs e)
        {
            frametimer.Dispose();
            flyspawntimer.Dispose();
            secondtimer.Dispose();
            playing = false;
            gametimer.Dispose();
            Invalidate(true);

        }


        private void frametimer_Tick(object sender, System.EventArgs e)
        {
            CheckInputs();
            player1.CheckJump();

            if(players)
            player2.CheckJump();

            Invalidate(true);
        }

        private void flyspawn_Tick(object sender, System.EventArgs e)
        {

        }

        private void SaveGameFile()
        {
            if (file == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Frogs game file (*.fro)|*.fro";
                dialog.Title = "Save Frogs game file";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    file = dialog.FileName;
                }
            }

            else
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, info);
                }
            }
        }
        private void LoadGameFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Frogs game file (*.fro)|*.fro";
            dialog.Title = "Open Frogs game file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                file = dialog.FileName;
                try
                {
                    using (FileStream fileStream = new FileStream(file, FileMode.Open))
                    {
                        IFormatter formater = new BinaryFormatter();
                        info = (GameFile)formater.Deserialize(fileStream);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not read file: " + file);
                    file = null;
                    return;
                }
                Invalidate(true);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveGameFile();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadGameFile();
        }

        private void btnControls_Click(object sender, EventArgs e)
        {

            ControlsConfiguration form = new ControlsConfiguration(DeepClone(info.controls));
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
                info.controls = form.c;

            else
                return;  
        }


        public static T DeepClone<T>(T obj)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        private void btnHighScores_Click(object sender, EventArgs e)
        {
            HighScoresList form = new HighScoresList(info.highscores);
            DialogResult result = form.ShowDialog();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewGame form = new NewGame();
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                players = form.players;
                StartGame(form.time);
            }

            else return;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.Clear(Color.White);
            e.Graphics.DrawImage(bitmap, new Point(0, 0));


            if (playing)
            {
                player1.Draw(e.Graphics);
                if (players) player2.Draw(e.Graphics);
            }
            
        }

        private void CheckInputs()
        {
            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P1right)))
                player1.Move(true);
            else if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P1left)))
                player1.Move(false);
            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P1jump)))
                player1.Jump();
            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P1tongue)))
                player1.Tongue();

            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P2right)))
                player2.Move(true);
            else if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P2left)))
                player2.Move(false);
            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P2jump)))
                player2.Jump();
            if (Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)info.controls.P1tongue)))
                player2.Tongue();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!playing) return;

            if (!pause)
            {

                frametimer.Stop();
                gametimer.Stop();
                flyspawntimer.Stop();
                secondtimer.Stop();
                pause = true;
                btnPause.Text = "Unpause";

            }

            else
            {
                frametimer.Start();
                gametimer.Start();
                flyspawntimer.Start();
                secondtimer.Start();
                pause = false;
                btnPause.Text = "Pause";
            }

        }
    }
}
