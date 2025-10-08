using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CinemaBookingApp.Data;

namespace CinemaBookingApp.Forms
{
    public partial class MovieForm : Form
    {
        private int? movieId;
        private DataBaseManager dbManager;
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";

        private TextBox txtTitle = null!;
        private TextBox txtGenre = null!;
        private NumericUpDown numDuration = null!;
        private TextBox txtDescription = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public MovieForm()
        {
            InitializeComponent();
            dbManager = new DataBaseManager(connectionString);
        }

        public MovieForm(int movieId) : this()
        {
            this.movieId = movieId;
            LoadMovieData();
        }

        private void InitializeComponent()
        {
            this.Text = movieId.HasValue ? "Редактировать фильм" : "Добавить фильм";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Название
            var lblTitle = new Label();
            lblTitle.Text = "Название:";
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(100, 20);

            txtTitle = new TextBox();
            txtTitle.Location = new Point(120, 20);
            txtTitle.Size = new Size(300, 20);

            // Жанр
            var lblGenre = new Label();
            lblGenre.Text = "Жанр:";
            lblGenre.Location = new Point(20, 60);
            lblGenre.Size = new Size(100, 20);

            txtGenre = new TextBox();
            txtGenre.Location = new Point(120, 60);
            txtGenre.Size = new Size(300, 20);

            // Длительность
            var lblDuration = new Label();
            lblDuration.Text = "Длительность (мин):";
            lblDuration.Location = new Point(20, 100);
            lblDuration.Size = new Size(100, 20);

            numDuration = new NumericUpDown();
            numDuration.Location = new Point(120, 100);
            numDuration.Size = new Size(100, 20);
            numDuration.Minimum = 1;
            numDuration.Maximum = 300;
            numDuration.Value = 120;

            // Описание
            var lblDescription = new Label();
            lblDescription.Text = "Описание:";
            lblDescription.Location = new Point(20, 140);
            lblDescription.Size = new Size(100, 20);

            txtDescription = new TextBox();
            txtDescription.Location = new Point(120, 140);
            txtDescription.Size = new Size(300, 100);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;

            // Кнопки
            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(120, 260);
            btnSave.Size = new Size(100, 30);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(230, 260);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblGenre, txtGenre,
                lblDuration, numDuration,
                lblDescription, txtDescription,
                btnSave, btnCancel
            });
        }

        private void LoadMovieData()
        {
            if (!movieId.HasValue) return;

            try
            {
                var movie = dbManager.GetMovieById(movieId.Value);
                if (movie != null)
                {
                    txtTitle.Text = movie["title"].ToString();
                    txtGenre.Text = movie["genre"].ToString();
                    numDuration.Value = Convert.ToDecimal(movie["duration_minutes"]);
                    txtDescription.Text = movie["description"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных фильма: {ex.Message}");
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название фильма");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                MessageBox.Show("Введите жанр фильма");
                return;
            }

            try
            {
                if (movieId.HasValue)
                {
                    // Редактирование
                    dbManager.UpdateMovie(
                        movieId.Value,
                        txtTitle.Text.Trim(),
                        txtGenre.Text.Trim(),
                        (int)numDuration.Value,
                        txtDescription.Text.Trim()
                    );
                    MessageBox.Show("Фильм успешно обновлен");
                }
                else
                {
                    // Добавление
                    dbManager.AddMovie(
                        txtTitle.Text.Trim(),
                        txtGenre.Text.Trim(),
                        (int)numDuration.Value,
                        txtDescription.Text.Trim()
                    );
                    MessageBox.Show("Фильм успешно добавлен");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}