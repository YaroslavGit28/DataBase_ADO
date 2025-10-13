using System;                    // Базовые типы и классы .NET Framework
using System.Data;               // Классы для работы с данными (DataTable, DataSet)
using System.Drawing;            // Классы для работы с графикой и цветами
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Data;    // Наши классы для работы с базой данных

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Форма для добавления и редактирования фильмов в каталоге кинотеатра
    /// Позволяет ввести название, жанр, продолжительность и описание фильма
    /// </summary>
    public partial class MovieForm : Form
    {
        // =============================================
        // ПОЛЯ ДЛЯ РАБОТЫ С ДАННЫМИ
        // =============================================
        
        /// <summary>
        /// ID фильма для редактирования (null для создания нового)
        /// </summary>
        private int? movieId;
        
        /// <summary>
        /// Менеджер базы данных для выполнения операций с фильмами
        /// </summary>
        private DataBaseManager dbManager;
        
        /// <summary>
        /// Строка подключения к базе данных SQL Server
        /// </summary>
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";

        // =============================================
        // ЭЛЕМЕНТЫ ПОЛЬЗОВАТЕЛЬСКОГО ИНТЕРФЕЙСА
        // =============================================
        
        /// <summary>
        /// Поле ввода названия фильма
        /// </summary>
        private TextBox txtTitle = null!;
        
        /// <summary>
        /// Поле ввода жанра фильма
        /// </summary>
        private TextBox txtGenre = null!;
        
        /// <summary>
        /// Числовое поле для ввода продолжительности фильма в минутах
        /// </summary>
        private NumericUpDown numDuration = null!;
        
        /// <summary>
        /// Многострочное поле для ввода описания фильма
        /// </summary>
        private TextBox txtDescription = null!;
        
        /// <summary>
        /// Кнопка "Сохранить" для сохранения данных фильма
        /// </summary>
        private Button btnSave = null!;
        
        /// <summary>
        /// Кнопка "Отмена" для закрытия формы без сохранения
        /// </summary>
        private Button btnCancel = null!;

        /// <summary>
        /// Конструктор для создания нового фильма
        /// Инициализирует форму в режиме добавления
        /// </summary>
        public MovieForm()
        {
            InitializeComponent();                                    // Создаем элементы интерфейса
            dbManager = new DataBaseManager(connectionString);       // Инициализируем менеджер БД
        }

        /// <summary>
        /// Конструктор для редактирования существующего фильма
        /// Загружает данные фильма в форму
        /// </summary>
        /// <param name="movieId">ID фильма для редактирования</param>
        public MovieForm(int movieId) : this()  // Вызываем базовый конструктор
        {
            this.movieId = movieId;             // Сохраняем ID фильма
            LoadMovieData();                    // Загружаем данные фильма
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