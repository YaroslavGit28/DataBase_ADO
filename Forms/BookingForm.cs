using System;                    // Базовые типы и классы .NET Framework
using System.Data;               // Классы для работы с данными (DataTable, DataSet)
using System.Drawing;            // Классы для работы с графикой и цветами
using System.Linq;               // LINQ для работы с коллекциями
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Data;    // Наши классы для работы с базой данных

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Форма для создания нового бронирования билетов в кинотеатре
    /// Позволяет выбрать пользователя, сеанс, места и количество билетов
    /// Поддерживает разные режимы для администраторов и обычных пользователей
    /// </summary>
    public partial class BookingForm : Form
    {
        // =============================================
        // ПОЛЯ ДЛЯ РАБОТЫ С БАЗОЙ ДАННЫХ
        // =============================================
        
        /// <summary>
        /// Менеджер базы данных для выполнения операций с бронированиями
        /// </summary>
        private DataBaseManager dbManager;
        
        /// <summary>
        /// Строка подключения к базе данных SQL Server
        /// </summary>
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        
        // =============================================
        // ИНФОРМАЦИЯ О ТЕКУЩЕМ ПОЛЬЗОВАТЕЛЕ
        // =============================================
        
        /// <summary>
        /// Имя текущего пользователя для ограничения доступа
        /// </summary>
        private string? currentUserName;
        
        /// <summary>
        /// Флаг, указывающий является ли текущий пользователь администратором
        /// </summary>
        private bool isCurrentUserAdmin;

        // =============================================
        // ЭЛЕМЕНТЫ ПОЛЬЗОВАТЕЛЬСКОГО ИНТЕРФЕЙСА
        // =============================================
        
        /// <summary>
        /// Выпадающий список для выбора пользователя (только для администраторов)
        /// </summary>
        private ComboBox comboUsers = null!;
        
        /// <summary>
        /// Выпадающий список для выбора сеанса
        /// </summary>
        private ComboBox comboScreenings = null!;
        
        /// <summary>
        /// Список доступных мест в зале
        /// </summary>
        private ListBox lstSeats = null!;
        
        /// <summary>
        /// Числовое поле для выбора количества билетов
        /// </summary>
        private NumericUpDown numTickets = null!;
        
        /// <summary>
        /// Метка для отображения общей стоимости бронирования
        /// </summary>
        private Label lblTotalPrice = null!;
        
        /// <summary>
        /// Кнопка "Создать бронирование" для сохранения заказа
        /// </summary>
        private Button btnCreateBooking = null!;
        
        /// <summary>
        /// Кнопка "Отмена" для закрытия формы без сохранения
        /// </summary>
        private Button btnCancel = null!;

        // =============================================
        // ПОЛЯ ДЛЯ РАСЧЕТА СТОИМОСТИ
        // =============================================
        
        /// <summary>
        /// Базовая цена одного билета в рублях
        /// </summary>
        private decimal basePrice = 300m;
        
        /// <summary>
        /// ID текущего выбранного сеанса
        /// </summary>
        private int currentScreeningId = -1;

        /// <summary>
        /// Конструктор формы бронирования
        /// Инициализирует форму с учетом роли и имени пользователя
        /// </summary>
        /// <param name="currentUserName">Имя текущего пользователя</param>
        /// <param name="isCurrentUserAdmin">Флаг администратора</param>
        public BookingForm(string? currentUserName = null, bool isCurrentUserAdmin = false)
        {
            // Сохраняем информацию о текущем пользователе
            this.currentUserName = currentUserName;         // Устанавливаем имя пользователя
            this.isCurrentUserAdmin = isCurrentUserAdmin;  // Устанавливаем флаг администратора
            
            // Инициализируем форму
            InitializeComponent();                                    // Создаем элементы интерфейса
            dbManager = new DataBaseManager(connectionString);       // Инициализируем менеджер БД
            LoadComboBoxData();                                       // Загружаем данные для выпадающих списков
        }

        private void InitializeComponent()
        {
            // Устанавливаем заголовок в зависимости от роли пользователя
            if (isCurrentUserAdmin)
            {
                this.Text = "Новое бронирование";
            }
            else
            {
                this.Text = $"Новое бронирование - {currentUserName}";
            }
            
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Пользователь
            var lblUser = new Label();
            if (isCurrentUserAdmin)
            {
                lblUser.Text = "Пользователь:";
            }
            else
            {
                lblUser.Text = "Пользователь (ваше имя):";
            }
            lblUser.Location = new Point(20, 20);
            lblUser.Size = new Size(100, 20);

            comboUsers = new ComboBox();
            comboUsers.Location = new Point(120, 20);
            comboUsers.Size = new Size(350, 20);
            comboUsers.DropDownStyle = ComboBoxStyle.DropDownList;

            // Сеанс
            var lblScreening = new Label();
            lblScreening.Text = "Сеанс:";
            lblScreening.Location = new Point(20, 60);
            lblScreening.Size = new Size(100, 20);

            comboScreenings = new ComboBox();
            comboScreenings.Location = new Point(120, 60);
            comboScreenings.Size = new Size(350, 20);
            comboScreenings.DropDownStyle = ComboBoxStyle.DropDownList;
            comboScreenings.SelectedIndexChanged += ComboScreenings_SelectedIndexChanged;

            // Количество билетов
            var lblTickets = new Label();
            lblTickets.Text = "Количество билетов:";
            lblTickets.Location = new Point(20, 100);
            lblTickets.Size = new Size(100, 20);

            numTickets = new NumericUpDown();
            numTickets.Location = new Point(120, 100);
            numTickets.Size = new Size(100, 20);
            numTickets.Minimum = 1;
            numTickets.Maximum = 10;
            numTickets.Value = 1;
            numTickets.ValueChanged += NumTickets_ValueChanged;

            // Доступные места
            var lblSeats = new Label();
            lblSeats.Text = "Доступные места:";
            lblSeats.Location = new Point(20, 140);
            lblSeats.Size = new Size(100, 20);

            lstSeats = new ListBox();
            lstSeats.Location = new Point(120, 140);
            lstSeats.Size = new Size(350, 150);
            lstSeats.SelectionMode = SelectionMode.MultiSimple;

            // Общая стоимость
            var lblTotal = new Label();
            lblTotal.Text = "Общая стоимость:";
            lblTotal.Location = new Point(20, 310);
            lblTotal.Size = new Size(100, 20);

            lblTotalPrice = new Label();
            lblTotalPrice.Text = "0 руб";
            lblTotalPrice.Location = new Point(120, 310);
            lblTotalPrice.Size = new Size(200, 20);
            lblTotalPrice.Font = new Font(lblTotalPrice.Font, FontStyle.Bold);

            // Кнопки
            btnCreateBooking = new Button();
            btnCreateBooking.Text = "Создать бронь";
            btnCreateBooking.Location = new Point(120, 350);
            btnCreateBooking.Size = new Size(120, 30);
            btnCreateBooking.Click += BtnCreateBooking_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(250, 350);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblUser, comboUsers,
                lblScreening, comboScreenings,
                lblTickets, numTickets,
                lblSeats, lstSeats,
                lblTotal, lblTotalPrice,
                btnCreateBooking, btnCancel
            });

            UpdateTotalPrice();
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка пользователей
                if (isCurrentUserAdmin)
                {
                    // Администратор видит всех пользователей
                    var users = dbManager.GetUsers();
                    if (users.Rows.Count == 0)
                    {
                        MessageBox.Show("В базе данных нет пользователей. Сначала добавьте пользователей.");
                        return;
                    }
                    
                    comboUsers.DisplayMember = "name";
                    comboUsers.ValueMember = "user_id";
                    comboUsers.DataSource = users;
                }
                else
                {
                    // Обычный пользователь видит только себя
                    if (!string.IsNullOrEmpty(currentUserName))
                    {
                        var userId = dbManager.GetUserIdByName(currentUserName);
                        if (userId.HasValue)
                        {
                            // Создаем DataTable с одним пользователем
                            var userTable = new DataTable();
                            userTable.Columns.Add("user_id", typeof(int));
                            userTable.Columns.Add("name", typeof(string));
                            
                            var row = userTable.NewRow();
                            row["user_id"] = userId.Value;
                            row["name"] = currentUserName;
                            userTable.Rows.Add(row);
                            
                            comboUsers.DisplayMember = "name";
                            comboUsers.ValueMember = "user_id";
                            comboUsers.DataSource = userTable;
                            
                            // Делаем ComboBox недоступным для изменения
                            comboUsers.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден в базе данных.", "Ошибка", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось определить текущего пользователя.", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }

                // Загрузка сеансов - показываем все сеансы, не только будущие
                var screenings = dbManager.GetScreenings();
                if (screenings.Rows.Count == 0)
                {
                    MessageBox.Show("В базе данных нет сеансов. Сначала добавьте сеансы.");
                    return;
                }
                
                // Показываем все сеансы без фильтрации по дате
                comboScreenings.DisplayMember = "title";
                comboScreenings.ValueMember = "screening_id";
                comboScreenings.DataSource = screenings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}\n\nПроверьте подключение к базе данных и наличие данных в таблицах.");
            }
        }

        private void ComboScreenings_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboScreenings.SelectedValue != null)
            {
                currentScreeningId = Convert.ToInt32(comboScreenings.SelectedValue);
                LoadAvailableSeats();
                UpdateTotalPrice();
            }
        }

        private void NumTickets_ValueChanged(object? sender, EventArgs e)
        {
            UpdateTotalPrice();
        }

        private void LoadAvailableSeats()
        {
            if (currentScreeningId == -1) return;

            try
            {
                var screening = dbManager.GetScreeningById(currentScreeningId);
                if (screening == null) 
                {
                    MessageBox.Show("Не удалось загрузить информацию о сеансе.");
                    return;
                }

                var hallCapacity = Convert.ToInt32(screening["capacity"]);
                var occupiedSeats = dbManager.GetOccupiedSeats(currentScreeningId);

                lstSeats.Items.Clear();

                // Генерируем доступные места (ряды A-Z, места 1-20)
                for (char row = 'A'; row <= 'Z'; row++)
                {
                    for (int seatNum = 1; seatNum <= 20; seatNum++)
                    {
                        var seat = $"{row}{seatNum}";
                        if (!occupiedSeats.Contains(seat))
                        {
                            lstSeats.Items.Add(seat);
                        }
                    }
                }

                if (lstSeats.Items.Count == 0)
                {
                    MessageBox.Show("Нет доступных мест для данного сеанса.");
                    return;
                }

                // Автоматически выбираем первые N мест
                var ticketsCount = (int)numTickets.Value;
                for (int i = 0; i < Math.Min(ticketsCount, lstSeats.Items.Count); i++)
                {
                    lstSeats.SetSelected(i, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки мест: {ex.Message}");
            }
        }

        private void UpdateTotalPrice()
        {
            if (currentScreeningId == -1) 
            {
                lblTotalPrice.Text = "0 руб";
                return;
            }

            try
            {
                var screening = dbManager.GetScreeningById(currentScreeningId);
                if (screening != null)
                {
                    var hallTypeId = Convert.ToInt32(screening["type_id"]);
                    var ticketPrice = dbManager.CalculateTicketPrice(basePrice, hallTypeId);
                    var total = ticketPrice * (int)numTickets.Value;
                    lblTotalPrice.Text = $"{total:F2} руб";
                }
                else
                {
                    lblTotalPrice.Text = "0 руб";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета стоимости: {ex.Message}");
                lblTotalPrice.Text = "Ошибка";
            }
        }

        private void BtnCreateBooking_Click(object? sender, EventArgs e)
        {
            if (comboUsers.SelectedValue == null || comboScreenings.SelectedValue == null)
            {
                MessageBox.Show("Выберите пользователя и сеанс");
                return;
            }

            var selectedSeats = lstSeats.SelectedItems;
            if (selectedSeats.Count != numTickets.Value)
            {
                MessageBox.Show($"Выберите ровно {numTickets.Value} мест(а)");
                return;
            }

            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Выберите места для бронирования");
                return;
            }

            try
            {
                var userId = Convert.ToInt32(comboUsers.SelectedValue);
                var screeningId = Convert.ToInt32(comboScreenings.SelectedValue);
                
                var screening = dbManager.GetScreeningById(screeningId);
                if (screening == null) 
                {
                    MessageBox.Show("Не удалось загрузить информацию о сеансе");
                    return;
                }

                var hallTypeId = Convert.ToInt32(screening["type_id"]);
                var ticketPrice = dbManager.CalculateTicketPrice(basePrice, hallTypeId);
                var totalAmount = ticketPrice * (int)numTickets.Value;

                // Создаем бронирование
                var bookingId = dbManager.CreateBooking(userId, totalAmount, "confirmed");

                // Создаем билеты для каждого выбранного места
                foreach (var seat in selectedSeats)
                {
                    if (seat != null)
                    {
                        dbManager.AddTicket(
                            seat.ToString()!,
                            basePrice,
                            ticketPrice,
                            bookingId,
                            screeningId
                        );
                    }
                }

                MessageBox.Show($"Бронирование успешно создано!\nID бронирования: {bookingId}\nОбщая сумма: {totalAmount:F2} руб");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания бронирования: {ex.Message}\n\nПроверьте подключение к базе данных и корректность данных.");
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}