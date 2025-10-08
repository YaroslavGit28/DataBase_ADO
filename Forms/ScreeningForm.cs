using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CinemaBookingApp.Data;

namespace CinemaBookingApp.Forms
{
    public partial class ScreeningForm : Form
    {
        private int? screeningId;
        private DataBaseManager dbManager;
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";

        private ComboBox comboMovies = null!;
        private ComboBox comboHalls = null!;
        private DateTimePicker dtpDate = null!;
        private DateTimePicker dtpStartTime = null!;
        private DateTimePicker dtpEndTime = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public ScreeningForm()
        {
            InitializeComponent();
            dbManager = new DataBaseManager(connectionString);
            LoadComboBoxData();
        }

        public ScreeningForm(int screeningId) : this()
        {
            this.screeningId = screeningId;
            LoadScreeningData();
        }

        private void InitializeComponent()
        {
            this.Text = screeningId.HasValue ? "Редактировать сеанс" : "Добавить сеанс";
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Фильм
            var lblMovie = new Label();
            lblMovie.Text = "Фильм:";
            lblMovie.Location = new Point(20, 20);
            lblMovie.Size = new Size(100, 20);

            comboMovies = new ComboBox();
            comboMovies.Location = new Point(120, 20);
            comboMovies.Size = new Size(300, 20);
            comboMovies.DropDownStyle = ComboBoxStyle.DropDownList;

            // Зал
            var lblHall = new Label();
            lblHall.Text = "Зал:";
            lblHall.Location = new Point(20, 60);
            lblHall.Size = new Size(100, 20);

            comboHalls = new ComboBox();
            comboHalls.Location = new Point(120, 60);
            comboHalls.Size = new Size(300, 20);
            comboHalls.DropDownStyle = ComboBoxStyle.DropDownList;

            // Дата
            var lblDate = new Label();
            lblDate.Text = "Дата:";
            lblDate.Location = new Point(20, 100);
            lblDate.Size = new Size(100, 20);

            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(120, 100);
            dtpDate.Size = new Size(150, 20);
            dtpDate.Format = DateTimePickerFormat.Short;

            // Время начала
            var lblStartTime = new Label();
            lblStartTime.Text = "Время начала:";
            lblStartTime.Location = new Point(20, 140);
            lblStartTime.Size = new Size(100, 20);

            dtpStartTime = new DateTimePicker();
            dtpStartTime.Location = new Point(120, 140);
            dtpStartTime.Size = new Size(150, 20);
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.ShowUpDown = true;

            // Время окончания
            var lblEndTime = new Label();
            lblEndTime.Text = "Время окончания:";
            lblEndTime.Location = new Point(20, 180);
            lblEndTime.Size = new Size(100, 20);

            dtpEndTime = new DateTimePicker();
            dtpEndTime.Location = new Point(120, 180);
            dtpEndTime.Size = new Size(150, 20);
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.ShowUpDown = true;

            // Кнопки
            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(120, 220);
            btnSave.Size = new Size(100, 30);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(230, 220);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblMovie, comboMovies,
                lblHall, comboHalls,
                lblDate, dtpDate,
                lblStartTime, dtpStartTime,
                lblEndTime, dtpEndTime,
                btnSave, btnCancel
            });
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка фильмов
                var movies = dbManager.GetMovies();
                comboMovies.DisplayMember = "title";
                comboMovies.ValueMember = "movie_id";
                comboMovies.DataSource = movies;

                // Загрузка залов
                var halls = dbManager.GetCinemaHalls();
                comboHalls.DisplayMember = "hall_name";
                comboHalls.ValueMember = "hall_id";
                comboHalls.DataSource = halls;

                // Установка времени по умолчанию
                dtpDate.Value = DateTime.Today;
                dtpStartTime.Value = DateTime.Today.AddHours(18); // 18:00
                dtpEndTime.Value = DateTime.Today.AddHours(20);   // 20:00
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadScreeningData()
        {
            if (!screeningId.HasValue) return;

            try
            {
                var screening = dbManager.GetScreeningById(screeningId.Value);
                if (screening != null)
                {
                    comboMovies.SelectedValue = Convert.ToInt32(screening["movie_id"]);
                    comboHalls.SelectedValue = Convert.ToInt32(screening["hall_id"]);
                    
                    var startTime = Convert.ToDateTime(screening["start_time"]);
                    dtpDate.Value = startTime.Date;
                    dtpStartTime.Value = startTime;
                    dtpEndTime.Value = Convert.ToDateTime(screening["end_time"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных сеанса: {ex.Message}");
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (comboMovies.SelectedValue == null || comboHalls.SelectedValue == null)
            {
                MessageBox.Show("Выберите фильм и зал");
                return;
            }

            var startDateTime = dtpDate.Value.Date + dtpStartTime.Value.TimeOfDay;
            var endDateTime = dtpDate.Value.Date + dtpEndTime.Value.TimeOfDay;

            if (startDateTime >= endDateTime)
            {
                MessageBox.Show("Время окончания должно быть позже времени начала");
                return;
            }

            try
            {
                var movieId = Convert.ToInt32(comboMovies.SelectedValue);
                var hallId = Convert.ToInt32(comboHalls.SelectedValue);

                // Проверка доступности зала
                var availableHalls = dbManager.GetAvailableHalls(startDateTime, endDateTime);
                bool isHallAvailable = false;
                foreach (DataRow row in availableHalls.Rows)
                {
                    if (Convert.ToInt32(row["hall_id"]) == hallId)
                    {
                        isHallAvailable = true;
                        break;
                    }
                }

                if (!isHallAvailable)
                {
                    MessageBox.Show("Зал занят в выбранное время");
                    return;
                }

                if (screeningId.HasValue)
                {
                    // Редактирование
                    dbManager.UpdateScreening(
                        screeningId.Value,
                        startDateTime,
                        endDateTime,
                        movieId,
                        hallId
                    );
                    MessageBox.Show("Сеанс успешно обновлен");
                }
                else
                {
                    // Добавление
                    dbManager.AddScreening(
                        startDateTime,
                        endDateTime,
                        movieId,
                        hallId
                    );
                    MessageBox.Show("Сеанс успешно добавлен");
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