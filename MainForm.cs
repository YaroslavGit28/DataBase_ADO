using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CinemaBookingApp.Data;
using CinemaBookingApp.Forms;

namespace CinemaBookingApp
{
    public partial class MainForm : Form
    {
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        private DataBaseManager? dbManager;
        private DapperRepository? dapperRepo;
        
        // Информация о текущем пользователе
        private string? currentUserRole;
        private string? currentUserName;
        
        // Объявляем поля с инициализацией
        private TabControl tabControl = new TabControl();
        private TabPage tabMovies = new TabPage();
        private TabPage tabScreenings = new TabPage();
        private TabPage tabBookings = new TabPage();
        private TabPage tabUsers = new TabPage();
        
        private DataGridView dataGridMovies = new DataGridView();
        private Button btnAddMovie = new Button();
        private Button btnEditMovie = new Button();
        private Button btnDeleteMovie = new Button();
        
        private DataGridView dataGridScreenings = new DataGridView();
        private Button btnAddScreening = new Button();
        private Button btnEditScreening = new Button();
        private Button btnDeleteScreening = new Button();
        
        private DataGridView dataGridBookings = new DataGridView();
        private Button btnNewBooking = new Button();
        private Button btnCancelBooking = new Button();
        private Button btnViewTickets = new Button();
        
        private DataGridView dataGridUsers = new DataGridView();
        private Button btnAddUser = new Button();
        private Button btnEditUser = new Button();
        private Button btnDeleteUser = new Button();
        
        // Компоненты поиска и фильтрации
        private SearchFilterControl searchFilterMovies = new SearchFilterControl();
        private SearchFilterControl searchFilterScreenings = new SearchFilterControl();
        private SearchFilterControl searchFilterBookings = new SearchFilterControl();
        private SearchFilterControl searchFilterUsers = new SearchFilterControl();
        
        public MainForm(string? userRole = null, string? userName = null)
        {
            currentUserRole = userRole;
            currentUserName = userName;
            
            InitializeComponent();
            dbManager = new DataBaseManager(connectionString);
            dapperRepo = new DapperRepository(connectionString);
            
            if (!dbManager.TestConnection())
            {
                MessageBox.Show("Ошибка подключения к базе данных!");
                return;
            }
            
            LoadMovies();
            LoadBookings();
            
            // Загружаем данные только для администраторов
            if (IsCurrentUserAdmin())
            {
                LoadScreenings();
                LoadUsers();
            }
        }

        private void InitializeComponent()
        {
            // Обновляем заголовок с информацией о пользователе
            string title = "Система бронирования кинотеатра";
            if (!string.IsNullOrEmpty(currentUserName))
            {
                title += $" - {currentUserName}";
                if (!string.IsNullOrEmpty(currentUserRole))
                {
                    title += $" ({currentUserRole})";
                }
            }
            this.Text = title;
            
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            tabControl.Dock = DockStyle.Fill;
            
            // Добавляем вкладки в зависимости от роли пользователя
            if (IsCurrentUserAdmin())
            {
                // Администратор видит все вкладки
                tabControl.Controls.AddRange(new TabPage[] {
                    tabMovies, tabScreenings, tabBookings, tabUsers
                });
            }
            else
            {
                // Обычный пользователь видит только фильмы и бронирования
                tabControl.Controls.AddRange(new TabPage[] {
                    tabMovies, tabBookings
                });
            }
            
            tabMovies.Text = "Фильмы";
            InitializeMoviesTab();
            
            // Инициализируем вкладки только для администраторов
            if (IsCurrentUserAdmin())
            {
                tabScreenings.Text = "Сеансы";
                InitializeScreeningsTab();
                
                tabUsers.Text = "Пользователи";
                InitializeUsersTab();
            }
            
            tabBookings.Text = "Бронирования";
            InitializeBookingsTab();
            
            this.Controls.Add(tabControl);
        }

        /// <summary>
        /// Проверяет, является ли текущий пользователь администратором
        /// </summary>
        private bool IsCurrentUserAdmin()
        {
            return currentUserRole == "Admin";
        }

        /// <summary>
        /// Проверяет права доступа для административных функций
        /// </summary>
        private bool CheckAdminAccess()
        {
            if (!IsCurrentUserAdmin())
            {
                MessageBox.Show("Доступ запрещен. Требуются права администратора.", "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        #region Вкладка фильмов
        private void InitializeMoviesTab()
        {
            // Настройка компонента поиска и фильтрации
            searchFilterMovies.Location = new Point(10, 10);
            searchFilterMovies.Size = new Size(850, 80);
            searchFilterMovies.SearchPerformed += SearchFilterMovies_SearchPerformed;
            searchFilterMovies.FilterCleared += SearchFilterMovies_FilterCleared;

            dataGridMovies.Location = new Point(10, 100);
            dataGridMovies.Size = new Size(850, 300);
            dataGridMovies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridMovies.ReadOnly = true;
            dataGridMovies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Добавляем кнопки только для администраторов
            if (IsCurrentUserAdmin())
            {
                btnAddMovie.Text = "Добавить фильм";
                btnAddMovie.Location = new Point(10, 420);
                btnAddMovie.Size = new Size(120, 30);
                btnAddMovie.Click += BtnAddMovie_Click;
                
                btnEditMovie.Text = "Редактировать";
                btnEditMovie.Location = new Point(140, 420);
                btnEditMovie.Size = new Size(120, 30);
                btnEditMovie.Click += BtnEditMovie_Click;
                
                btnDeleteMovie.Text = "Удалить";
                btnDeleteMovie.Location = new Point(270, 420);
                btnDeleteMovie.Size = new Size(120, 30);
                btnDeleteMovie.Click += BtnDeleteMovie_Click;
                
                tabMovies.Controls.AddRange(new Control[] {
                    searchFilterMovies, dataGridMovies, btnAddMovie, btnEditMovie, btnDeleteMovie
                });
            }
            else
            {
                // Для обычных пользователей только таблица фильмов и поиск
                tabMovies.Controls.AddRange(new Control[] {
                    searchFilterMovies, dataGridMovies
                });
            }
        }

        private void LoadMovies()
        {
            try
            {
                if (dataGridMovies == null || dbManager == null) return;
                
                dataGridMovies.DataSource = dbManager.GetMovies();
                
                // Устанавливаем целевую таблицу для поиска и фильтрации
                searchFilterMovies.SetTargetDataGrid(dataGridMovies);
                
                if (dataGridMovies.Columns.Count > 0)
                {
                    dataGridMovies.Columns["movie_id"]!.HeaderText = "ID";
                    dataGridMovies.Columns["title"]!.HeaderText = "Название";
                    dataGridMovies.Columns["genre"]!.HeaderText = "Жанр";
                    dataGridMovies.Columns["duration_minutes"]!.HeaderText = "Длительность (мин)";
                    if (dataGridMovies.Columns.Contains("description"))
                        dataGridMovies.Columns["description"]!.HeaderText = "Описание";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильмов: {ex.Message}");
            }
        }

        private void BtnAddMovie_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            var movieForm = new MovieForm();
            if (movieForm.ShowDialog() == DialogResult.OK)
            {
                LoadMovies();
            }
        }

        private void BtnEditMovie_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridMovies.CurrentRow != null)
            {
                var movieIdCell = dataGridMovies.CurrentRow.Cells["movie_id"];
                if (movieIdCell?.Value != null)
                {
                    int movieId = Convert.ToInt32(movieIdCell.Value);
                    var movieForm = new MovieForm(movieId);
                    if (movieForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadMovies();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите фильм для редактирования");
            }
        }

        private void BtnDeleteMovie_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridMovies.CurrentRow != null)
            {
                var movieIdCell = dataGridMovies.CurrentRow.Cells["movie_id"];
                var titleCell = dataGridMovies.CurrentRow.Cells["title"];
                
                if (movieIdCell?.Value != null && titleCell?.Value != null)
                {
                    int movieId = Convert.ToInt32(movieIdCell.Value);
                    string title = titleCell.Value.ToString() ?? "";
                    
                    if (MessageBox.Show($"Удалить фильм '{title}'?", "Подтверждение", 
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            dbManager?.DeleteMovie(movieId);
                            LoadMovies();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка удаления: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void SearchFilterMovies_SearchPerformed(object? sender, EventArgs e)
        {
            // Обновляем статус поиска
            UpdateSearchStatus(searchFilterMovies);
        }

        private void SearchFilterMovies_FilterCleared(object? sender, EventArgs e)
        {
            // Сбрасываем статус поиска
            UpdateSearchStatus(searchFilterMovies);
        }

        private void UpdateSearchStatus(SearchFilterControl searchControl)
        {
            int filteredCount = searchControl.GetFilteredRowCount();
            int totalCount = searchControl.GetTotalRowCount();
            
            // Можно добавить статусную строку или обновить заголовок
            if (filteredCount < totalCount)
            {
                this.Text = $"Система бронирования кинотеатра - {currentUserName} ({currentUserRole}) - Показано: {filteredCount} из {totalCount}";
            }
            else
            {
                this.Text = $"Система бронирования кинотеатра - {currentUserName} ({currentUserRole})";
            }
        }
        #endregion

        // Остальные методы остаются аналогичными с проверками на null
        // ... (остальной код из предыдущей версии с аналогичными исправлениями)

        #region Вкладка сеансов
        private void InitializeScreeningsTab()
        {
            // Настройка компонента поиска и фильтрации
            searchFilterScreenings.Location = new Point(10, 10);
            searchFilterScreenings.Size = new Size(850, 80);
            searchFilterScreenings.SearchPerformed += SearchFilterScreenings_SearchPerformed;
            searchFilterScreenings.FilterCleared += SearchFilterScreenings_FilterCleared;

            dataGridScreenings.Location = new Point(10, 100);
            dataGridScreenings.Size = new Size(850, 300);
            dataGridScreenings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridScreenings.ReadOnly = true;
            dataGridScreenings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            btnAddScreening.Text = "Добавить сеанс";
            btnAddScreening.Location = new Point(10, 420);
            btnAddScreening.Size = new Size(120, 30);
            btnAddScreening.Click += BtnAddScreening_Click;
            
            btnEditScreening.Text = "Редактировать";
            btnEditScreening.Location = new Point(140, 420);
            btnEditScreening.Size = new Size(120, 30);
            btnEditScreening.Click += BtnEditScreening_Click;
            
            btnDeleteScreening.Text = "Удалить";
            btnDeleteScreening.Location = new Point(270, 420);
            btnDeleteScreening.Size = new Size(120, 30);
            btnDeleteScreening.Click += BtnDeleteScreening_Click;
            
            tabScreenings.Controls.AddRange(new Control[] {
                searchFilterScreenings, dataGridScreenings, btnAddScreening, btnEditScreening, btnDeleteScreening
            });
        }

        private void LoadScreenings()
        {
            try
            {
                if (dataGridScreenings == null || dbManager == null) return;
                
                dataGridScreenings.DataSource = dbManager.GetScreenings();
                
                // Устанавливаем целевую таблицу для поиска и фильтрации
                searchFilterScreenings.SetTargetDataGrid(dataGridScreenings);
                
                if (dataGridScreenings.Columns.Count > 0)
                {
                    dataGridScreenings.Columns["screening_id"]!.HeaderText = "ID";
                    dataGridScreenings.Columns["title"]!.HeaderText = "Фильм";
                    dataGridScreenings.Columns["start_time"]!.HeaderText = "Начало";
                    dataGridScreenings.Columns["end_time"]!.HeaderText = "Конец";
                    dataGridScreenings.Columns["hall_name"]!.HeaderText = "Зал";
                    dataGridScreenings.Columns["type_name"]!.HeaderText = "Тип зала";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сеансов: {ex.Message}");
            }
        }

        private void BtnAddScreening_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            var screeningForm = new ScreeningForm();
            if (screeningForm.ShowDialog() == DialogResult.OK)
            {
                LoadScreenings();
            }
        }

        private void BtnEditScreening_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridScreenings.CurrentRow != null)
            {
                var screeningIdCell = dataGridScreenings.CurrentRow.Cells["screening_id"];
                if (screeningIdCell?.Value != null)
                {
                    int screeningId = Convert.ToInt32(screeningIdCell.Value);
                    var screeningForm = new ScreeningForm(screeningId);
                    if (screeningForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadScreenings();
                    }
                }
            }
        }

        private void BtnDeleteScreening_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridScreenings.CurrentRow != null)
            {
                var screeningIdCell = dataGridScreenings.CurrentRow.Cells["screening_id"];
                if (screeningIdCell?.Value != null)
                {
                    int screeningId = Convert.ToInt32(screeningIdCell.Value);
                    
                    if (MessageBox.Show("Удалить сеанс?", "Подтверждение", 
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            dbManager?.DeleteScreening(screeningId);
                            LoadScreenings();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка удаления: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void SearchFilterScreenings_SearchPerformed(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterScreenings);
        }

        private void SearchFilterScreenings_FilterCleared(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterScreenings);
        }
        #endregion

        #region Вкладка бронирований
        private void InitializeBookingsTab()
        {
            // Настройка компонента поиска и фильтрации
            searchFilterBookings.Location = new Point(10, 10);
            searchFilterBookings.Size = new Size(850, 80);
            searchFilterBookings.SearchPerformed += SearchFilterBookings_SearchPerformed;
            searchFilterBookings.FilterCleared += SearchFilterBookings_FilterCleared;

            dataGridBookings.Location = new Point(10, 100);
            dataGridBookings.Size = new Size(850, 300);
            dataGridBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridBookings.ReadOnly = true;
            dataGridBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            btnNewBooking.Text = "Новое бронирование";
            btnNewBooking.Location = new Point(10, 420);
            btnNewBooking.Size = new Size(150, 30);
            btnNewBooking.Click += BtnNewBooking_Click;
            
            if (IsCurrentUserAdmin())
            {
                // Администратор видит все кнопки
                btnCancelBooking.Text = "Отменить бронь";
                btnCancelBooking.Location = new Point(170, 420);
                btnCancelBooking.Size = new Size(150, 30);
                btnCancelBooking.Click += BtnCancelBooking_Click;
                
                btnViewTickets.Text = "Просмотреть билеты";
                btnViewTickets.Location = new Point(330, 420);
                btnViewTickets.Size = new Size(150, 30);
                btnViewTickets.Click += BtnViewTickets_Click;
                
                tabBookings.Controls.AddRange(new Control[] {
                    searchFilterBookings, dataGridBookings, btnNewBooking, btnCancelBooking, btnViewTickets
                });
            }
            else
            {
                // Обычный пользователь видит только кнопку создания бронирования
                tabBookings.Controls.AddRange(new Control[] {
                    searchFilterBookings, dataGridBookings, btnNewBooking
                });
            }
        }

        private void LoadBookings()
        {
            try
            {
                if (dataGridBookings == null || dbManager == null) return;
                
                dataGridBookings.DataSource = dbManager.GetBookings();
                
                // Устанавливаем целевую таблицу для поиска и фильтрации
                searchFilterBookings.SetTargetDataGrid(dataGridBookings);
                
                if (dataGridBookings.Columns.Count > 0)
                {
                    dataGridBookings.Columns["booking_id"]!.HeaderText = "ID брони";
                    dataGridBookings.Columns["customer_name"]!.HeaderText = "Клиент";
                    dataGridBookings.Columns["email"]!.HeaderText = "Email";
                    dataGridBookings.Columns["phone"]!.HeaderText = "Телефон";
                    dataGridBookings.Columns["order_date"]!.HeaderText = "Дата заказа";
                    dataGridBookings.Columns["total_amount"]!.HeaderText = "Сумма";
                    dataGridBookings.Columns["status"]!.HeaderText = "Статус";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}");
            }
        }

        private void BtnNewBooking_Click(object? sender, EventArgs e)
        {
            var bookingForm = new BookingForm();
            if (bookingForm.ShowDialog() == DialogResult.OK)
            {
                LoadBookings();
            }
        }

        private void BtnCancelBooking_Click(object? sender, EventArgs e)
        {
            if (dataGridBookings.CurrentRow != null)
            {
                var bookingIdCell = dataGridBookings.CurrentRow.Cells["booking_id"];
                if (bookingIdCell?.Value != null)
                {
                    int bookingId = Convert.ToInt32(bookingIdCell.Value);
                    
                    if (MessageBox.Show("Отменить бронирование?", "Подтверждение", 
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            dbManager?.UpdateBookingStatus(bookingId, "cancelled");
                            LoadBookings();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка отмены брони: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void BtnViewTickets_Click(object? sender, EventArgs e)
        {
            if (dataGridBookings.CurrentRow != null)
            {
                var bookingIdCell = dataGridBookings.CurrentRow.Cells["booking_id"];
                if (bookingIdCell?.Value != null)
                {
                    int bookingId = Convert.ToInt32(bookingIdCell.Value);
                    var ticketsForm = new TicketsForm(bookingId);
                    ticketsForm.ShowDialog();
                }
            }
        }

        private void SearchFilterBookings_SearchPerformed(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterBookings);
        }

        private void SearchFilterBookings_FilterCleared(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterBookings);
        }
        #endregion

        #region Вкладка пользователей
        private void InitializeUsersTab()
        {
            // Настройка компонента поиска и фильтрации
            searchFilterUsers.Location = new Point(10, 10);
            searchFilterUsers.Size = new Size(850, 80);
            searchFilterUsers.SearchPerformed += SearchFilterUsers_SearchPerformed;
            searchFilterUsers.FilterCleared += SearchFilterUsers_FilterCleared;

            dataGridUsers.Location = new Point(10, 100);
            dataGridUsers.Size = new Size(850, 300);
            dataGridUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridUsers.ReadOnly = true;
            dataGridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            btnAddUser.Text = "Добавить пользователя";
            btnAddUser.Location = new Point(10, 420);
            btnAddUser.Size = new Size(150, 30);
            btnAddUser.Click += BtnAddUser_Click;
            
            btnEditUser.Text = "Редактировать";
            btnEditUser.Location = new Point(170, 420);
            btnEditUser.Size = new Size(120, 30);
            btnEditUser.Click += BtnEditUser_Click;
            
            btnDeleteUser.Text = "Удалить";
            btnDeleteUser.Location = new Point(300, 420);
            btnDeleteUser.Size = new Size(120, 30);
            btnDeleteUser.Click += BtnDeleteUser_Click;
            
            tabUsers.Controls.AddRange(new Control[] {
                searchFilterUsers, dataGridUsers, btnAddUser, btnEditUser, btnDeleteUser
            });
        }

        private void LoadUsers()
        {
            try
            {
                if (dataGridUsers == null || dbManager == null) return;
                
                dataGridUsers.DataSource = dbManager.GetUsers();
                
                // Устанавливаем целевую таблицу для поиска и фильтрации
                searchFilterUsers.SetTargetDataGrid(dataGridUsers);
                
                if (dataGridUsers.Columns.Count > 0)
                {
                    dataGridUsers.Columns["user_id"]!.HeaderText = "ID";
                    dataGridUsers.Columns["name"]!.HeaderText = "Имя";
                    dataGridUsers.Columns["email"]!.HeaderText = "Email";
                    dataGridUsers.Columns["phone"]!.HeaderText = "Телефон";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        /// <summary>
        /// Пример загрузки пользователей через Dapper (альтернативный метод)
        /// </summary>
        private async void LoadUsersWithDapper()
        {
            try
            {
                if (dataGridUsers == null || dapperRepo == null) return;
                
                var users = await dapperRepo.GetUsersAsync();
                
                // Преобразуем в DataTable для совместимости с DataGridView
                var dataTable = new DataTable();
                dataTable.Columns.Add("user_id", typeof(int));
                dataTable.Columns.Add("name", typeof(string));
                dataTable.Columns.Add("email", typeof(string));
                dataTable.Columns.Add("phone", typeof(string));
                
                foreach (var user in users)
                {
                    dataTable.Rows.Add(user.UserId, user.Name, user.Email, user.Phone);
                }
                
                dataGridUsers.DataSource = dataTable;
                
                if (dataGridUsers.Columns.Count > 0)
                {
                    dataGridUsers.Columns["user_id"]!.HeaderText = "ID";
                    dataGridUsers.Columns["name"]!.HeaderText = "Имя";
                    dataGridUsers.Columns["email"]!.HeaderText = "Email";
                    dataGridUsers.Columns["phone"]!.HeaderText = "Телефон";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей через Dapper: {ex.Message}");
            }
        }

        private void BtnAddUser_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            var userForm = new UserForm();
            if (userForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void BtnEditUser_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridUsers.CurrentRow != null)
            {
                var userIdCell = dataGridUsers.CurrentRow.Cells["user_id"];
                if (userIdCell?.Value != null)
                {
                    int userId = Convert.ToInt32(userIdCell.Value);
                    var userForm = new UserForm(userId);
                    if (userForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsers();
                    }
                }
            }
        }

        private void BtnDeleteUser_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;
            
            if (dataGridUsers.CurrentRow != null)
            {
                var userIdCell = dataGridUsers.CurrentRow.Cells["user_id"];
                var nameCell = dataGridUsers.CurrentRow.Cells["name"];
                
                if (userIdCell?.Value != null && nameCell?.Value != null)
                {
                    int userId = Convert.ToInt32(userIdCell.Value);
                    string name = nameCell.Value.ToString() ?? "";
                    
                    if (MessageBox.Show($"Удалить пользователя '{name}'?", "Подтверждение", 
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            dbManager?.DeleteUser(userId);
                            LoadUsers();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка удаления: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void SearchFilterUsers_SearchPerformed(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterUsers);
        }

        private void SearchFilterUsers_FilterCleared(object? sender, EventArgs e)
        {
            UpdateSearchStatus(searchFilterUsers);
        }
        #endregion
    }
}