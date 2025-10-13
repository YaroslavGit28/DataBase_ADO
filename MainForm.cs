using System;                    // Базовые типы и классы .NET Framework
using System.Data;               // Классы для работы с данными (DataTable, DataGridView)
using System.Drawing;            // Классы для работы с графикой и цветами
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Data;    // Наши классы для работы с базой данных
using CinemaBookingApp.Forms;    // Наши пользовательские формы

namespace CinemaBookingApp
{
    /// <summary>
    /// Главная форма приложения системы бронирования билетов в кинотеатре
    /// Содержит все основные функции: управление фильмами, сеансами, бронированиями и пользователями
    /// </summary>
    public partial class MainForm : Form
    {
        // =============================================
        // ПОЛЯ ДЛЯ РАБОТЫ С БАЗОЙ ДАННЫХ
        // =============================================
        
        /// <summary>
        /// Строка подключения к базе данных SQL Server
        /// Содержит адрес сервера, имя базы данных, учетные данные и параметры безопасности
        /// </summary>
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        
        /// <summary>
        /// Менеджер базы данных для работы с данными через ADO.NET
        /// Используется для выполнения SQL запросов и управления данными
        /// </summary>
        private DataBaseManager? dbManager;
        
        /// <summary>
        /// Репозиторий для работы с данными через Dapper ORM
        /// Предоставляет более удобный и современный способ работы с базой данных
        /// </summary>
        private DapperRepository? dapperRepo;
        
        // =============================================
        // ИНФОРМАЦИЯ О ТЕКУЩЕМ ПОЛЬЗОВАТЕЛЕ
        // =============================================
        
        /// <summary>
        /// Роль текущего пользователя (Admin или User)
        /// Определяет доступные функции и интерфейс приложения
        /// </summary>
        private string? currentUserRole;
        
        /// <summary>
        /// Имя текущего пользователя
        /// Используется для отображения в интерфейсе и фильтрации данных
        /// </summary>
        private string? currentUserName;
        
        // =============================================
        // ЭЛЕМЕНТЫ ПОЛЬЗОВАТЕЛЬСКОГО ИНТЕРФЕЙСА
        // =============================================
        
        /// <summary>
        /// Основной контейнер вкладок для организации интерфейса
        /// Содержит все основные разделы приложения
        /// </summary>
        private TabControl tabControl = new TabControl();
        
        /// <summary>
        /// Вкладка "Фильмы" - управление каталогом фильмов
        /// Доступна всем пользователям для просмотра, админам - для редактирования
        /// </summary>
        private TabPage tabMovies = new TabPage();
        
        /// <summary>
        /// Вкладка "Сеансы" - управление расписанием показов
        /// Доступна только администраторам
        /// </summary>
        private TabPage tabScreenings = new TabPage();
        
        /// <summary>
        /// Вкладка "Бронирования" - управление заказами билетов
        /// Пользователи видят только свои бронирования, админы - все
        /// </summary>
        private TabPage tabBookings = new TabPage();
        
        /// <summary>
        /// Вкладка "Пользователи" - управление учетными записями
        /// Доступна только администраторам
        /// </summary>
        private TabPage tabUsers = new TabPage();
        
        // =============================================
        // ЭЛЕМЕНТЫ ВКЛАДКИ "ФИЛЬМЫ"
        // =============================================
        
        /// <summary>
        /// Таблица для отображения списка фильмов
        /// Показывает: название, жанр, продолжительность, описание
        /// </summary>
        private DataGridView dataGridMovies = new DataGridView();
        
        /// <summary>
        /// Кнопка "Добавить фильм" - открывает форму создания нового фильма
        /// Доступна только администраторам
        /// </summary>
        private Button btnAddMovie = new Button();
        
        /// <summary>
        /// Кнопка "Редактировать фильм" - открывает форму редактирования выбранного фильма
        /// Доступна только администраторам
        /// </summary>
        private Button btnEditMovie = new Button();
        
        /// <summary>
        /// Кнопка "Удалить фильм" - удаляет выбранный фильм из базы данных
        /// Доступна только администраторам
        /// </summary>
        private Button btnDeleteMovie = new Button();
        
        // =============================================
        // ЭЛЕМЕНТЫ ВКЛАДКИ "СЕАНСЫ"
        // =============================================
        
        /// <summary>
        /// Таблица для отображения расписания сеансов
        /// Показывает: фильм, зал, дату, время начала и окончания
        /// </summary>
        private DataGridView dataGridScreenings = new DataGridView();
        
        /// <summary>
        /// Кнопка "Добавить сеанс" - открывает форму создания нового сеанса
        /// Доступна только администраторам
        /// </summary>
        private Button btnAddScreening = new Button();
        
        /// <summary>
        /// Кнопка "Редактировать сеанс" - открывает форму редактирования выбранного сеанса
        /// Доступна только администраторам
        /// </summary>
        private Button btnEditScreening = new Button();
        
        /// <summary>
        /// Кнопка "Удалить сеанс" - удаляет выбранный сеанс из базы данных
        /// Доступна только администраторам
        /// </summary>
        private Button btnDeleteScreening = new Button();
        
        // =============================================
        // ЭЛЕМЕНТЫ ВКЛАДКИ "БРОНИРОВАНИЯ"
        // =============================================
        
        /// <summary>
        /// Таблица для отображения списка бронирований
        /// Показывает: пользователь, дата заказа, сумма, статус
        /// </summary>
        private DataGridView dataGridBookings = new DataGridView();
        
        /// <summary>
        /// Кнопка "Новое бронирование" - открывает форму создания нового заказа билетов
        /// Доступна всем пользователям
        /// </summary>
        private Button btnNewBooking = new Button();
        
        /// <summary>
        /// Кнопка "Отменить бронирование" - отменяет выбранное бронирование
        /// Админы могут отменять любые, пользователи - только свои
        /// </summary>
        private Button btnCancelBooking = new Button();
        
        /// <summary>
        /// Кнопка "Просмотр билетов" - открывает форму с деталями бронирования
        /// Показывает список билетов и места в зале
        /// </summary>
        private Button btnViewTickets = new Button();
        
        // =============================================
        // ЭЛЕМЕНТЫ ВКЛАДКИ "ПОЛЬЗОВАТЕЛИ"
        // =============================================
        
        /// <summary>
        /// Таблица для отображения списка пользователей системы
        /// Показывает: имя, email, телефон, роль
        /// </summary>
        private DataGridView dataGridUsers = new DataGridView();
        
        /// <summary>
        /// Кнопка "Добавить пользователя" - открывает форму создания нового пользователя
        /// Доступна только администраторам
        /// </summary>
        private Button btnAddUser = new Button();
        
        /// <summary>
        /// Кнопка "Редактировать пользователя" - открывает форму редактирования выбранного пользователя
        /// Доступна только администраторам
        /// </summary>
        private Button btnEditUser = new Button();
        
        /// <summary>
        /// Кнопка "Удалить пользователя" - удаляет выбранного пользователя из базы данных
        /// Доступна только администраторам
        /// </summary>
        private Button btnDeleteUser = new Button();
        
        // =============================================
        // КОМПОНЕНТЫ ПОИСКА И ФИЛЬТРАЦИИ
        // =============================================
        
        /// <summary>
        /// Компонент поиска и фильтрации для таблицы фильмов
        /// Позволяет искать по названию, жанру и фильтровать данные
        /// </summary>
        private SearchFilterControl searchFilterMovies = new SearchFilterControl();
        
        /// <summary>
        /// Компонент поиска и фильтрации для таблицы сеансов
        /// Позволяет искать по фильму, залу и фильтровать по дате
        /// </summary>
        private SearchFilterControl searchFilterScreenings = new SearchFilterControl();
        
        /// <summary>
        /// Компонент поиска и фильтрации для таблицы бронирований
        /// Позволяет искать по пользователю, дате и фильтровать по статусу
        /// </summary>
        private SearchFilterControl searchFilterBookings = new SearchFilterControl();
        
        /// <summary>
        /// Компонент поиска и фильтрации для таблицы пользователей
        /// Позволяет искать по имени, email и фильтровать по роли
        /// </summary>
        private SearchFilterControl searchFilterUsers = new SearchFilterControl();
        
        /// <summary>
        /// Конструктор главной формы приложения
        /// Инициализирует форму с учетом роли и имени пользователя
        /// </summary>
        /// <param name="userRole">Роль пользователя (Admin или User)</param>
        /// <param name="userName">Имя пользователя для отображения и фильтрации данных</param>
        public MainForm(string? userRole = null, string? userName = null)
        {
            // Сохраняем информацию о текущем пользователе
            currentUserRole = userRole;    // Устанавливаем роль пользователя
            currentUserName = userName;    // Устанавливаем имя пользователя
            
            // Инициализируем пользовательский интерфейс
            InitializeComponent();         // Создаем все элементы интерфейса
            
            // Инициализируем менеджеры базы данных
            dbManager = new DataBaseManager(connectionString);    // Создаем менеджер ADO.NET
            dapperRepo = new DapperRepository(connectionString);  // Создаем репозиторий Dapper
            
            // Проверяем подключение к базе данных
            if (!dbManager.TestConnection())  // Тестируем соединение с базой данных
            {
                MessageBox.Show("Ошибка подключения к базе данных!");  // Показываем сообщение об ошибке
                return;  // Прерываем инициализацию при ошибке подключения
            }
            
            // Загружаем данные в зависимости от роли пользователя
            LoadMovies();                  // Загружаем список фильмов (доступно всем)
            LoadBookings();                // Загружаем бронирования (фильтруются по пользователю)
            
            // Загружаем данные только для администраторов
            if (IsCurrentUserAdmin())      // Проверяем, является ли пользователь администратором
            {
                LoadScreenings();          // Загружаем расписание сеансов
                LoadUsers();               // Загружаем список пользователей
            }
        }

        /// <summary>
        /// Инициализация пользовательского интерфейса главной формы
        /// Создает все элементы интерфейса и настраивает их свойства
        /// </summary>
        private void InitializeComponent()
        {
            // =============================================
            // НАСТРОЙКА ОСНОВНЫХ СВОЙСТВ ФОРМЫ
            // =============================================
            
            // Обновляем заголовок с информацией о пользователе
            string title = "Система бронирования кинотеатра";  // Базовый заголовок формы
            if (!string.IsNullOrEmpty(currentUserName))  // Если имя пользователя указано
            {
                title += $" - {currentUserName}";          // Добавляем имя в заголовок
                if (!string.IsNullOrEmpty(currentUserRole))  // Если роль указана
                {
                    title += $" ({currentUserRole})";      // Добавляем роль в скобках
                }
            }
            this.Text = title;                             // Устанавливаем заголовок формы
            
            // Настраиваем размер и позицию формы
            this.Size = new Size(900, 650);                 // Устанавливаем размер окна
            this.StartPosition = FormStartPosition.CenterScreen;  // Центрируем окно на экране
            
            // Настраиваем основной контейнер вкладок
            tabControl.Dock = DockStyle.Fill;              // Растягиваем на всю форму
            
            // =============================================
            // НАСТРОЙКА ВКЛАДОК В ЗАВИСИМОСТИ ОТ РОЛИ ПОЛЬЗОВАТЕЛЯ
            // =============================================
            
            if (IsCurrentUserAdmin())                      // Если пользователь - администратор
            {
                // Администратор видит все вкладки
                tabControl.Controls.AddRange(new TabPage[] {  // Добавляем все доступные вкладки
                    tabMovies,      // Фильмы
                    tabScreenings,  // Сеансы
                    tabBookings,    // Бронирования
                    tabUsers        // Пользователи
                });
            }
            else                                           // Если пользователь - обычный
            {
                // Обычный пользователь видит только фильмы и бронирования
                tabControl.Controls.AddRange(new TabPage[] {  // Добавляем ограниченный набор вкладок
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
        /// Используется для определения доступных функций и элементов интерфейса
        /// </summary>
        /// <returns>true если пользователь - администратор, false - обычный пользователь</returns>
        private bool IsCurrentUserAdmin()
        {
            return currentUserRole == "Admin";  // Сравниваем роль пользователя со строкой "Admin"
        }

        /// <summary>
        /// Проверяет права доступа для административных функций
        /// Показывает сообщение об ошибке, если пользователь не является администратором
        /// </summary>
        /// <returns>true если доступ разрешен, false если запрещен</returns>
        private bool CheckAdminAccess()
        {
            if (!IsCurrentUserAdmin())  // Если пользователь не администратор
            {
                // Показываем сообщение об ошибке доступа
                MessageBox.Show("Доступ запрещен. Требуются права администратора.", "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;  // Возвращаем false - доступ запрещен
            }
            return true;  // Возвращаем true - доступ разрешен
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

        /// <summary>
        /// Загружает список фильмов из базы данных и отображает их в таблице
        /// Настраивает заголовки колонок и компонент поиска
        /// </summary>
        private void LoadMovies()
        {
            try  // Обрабатываем возможные ошибки при загрузке данных
            {
                // Проверяем, что элементы интерфейса и менеджер БД инициализированы
                if (dataGridMovies == null || dbManager == null) return;
                
                // Получаем данные фильмов из базы данных и привязываем к таблице
                dataGridMovies.DataSource = dbManager.GetMovies();
                
                // Устанавливаем целевую таблицу для поиска и фильтрации
                searchFilterMovies.SetTargetDataGrid(dataGridMovies);
                
                // Настраиваем заголовки колонок для удобочитаемости
                if (dataGridMovies.Columns.Count > 0)  // Если колонки были созданы
                {
                    dataGridMovies.Columns["movie_id"]!.HeaderText = "ID";                    // ID фильма
                    dataGridMovies.Columns["title"]!.HeaderText = "Название";                 // Название фильма
                    dataGridMovies.Columns["genre"]!.HeaderText = "Жанр";                     // Жанр фильма
                    dataGridMovies.Columns["duration_minutes"]!.HeaderText = "Длительность (мин)";  // Продолжительность
                    if (dataGridMovies.Columns.Contains("description"))                      // Если есть колонка описания
                        dataGridMovies.Columns["description"]!.HeaderText = "Описание";     // Описание фильма
                }
            }
            catch (Exception ex)  // Обрабатываем любые ошибки при загрузке
            {
                // Показываем пользователю сообщение об ошибке
                MessageBox.Show($"Ошибка загрузки фильмов: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить фильм"
        /// Открывает форму создания нового фильма (только для администраторов)
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка)</param>
        /// <param name="e">Аргументы события</param>
        private void BtnAddMovie_Click(object? sender, EventArgs e)
        {
            if (!CheckAdminAccess()) return;  // Проверяем права администратора
            
            // Создаем и открываем форму добавления фильма
            var movieForm = new MovieForm();                    // Создаем экземпляр формы MovieForm
            if (movieForm.ShowDialog() == DialogResult.OK)     // Показываем форму как диалог
            {
                LoadMovies();                                   // Если фильм был добавлен, обновляем список
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
                // Обычный пользователь видит кнопки для работы со своими бронированиями
                btnCancelBooking.Text = "Отменить мою бронь";
                btnCancelBooking.Location = new Point(170, 420);
                btnCancelBooking.Size = new Size(150, 30);
                btnCancelBooking.Click += BtnCancelMyBooking_Click;
                
                btnViewTickets.Text = "Мои билеты";
                btnViewTickets.Location = new Point(330, 420);
                btnViewTickets.Size = new Size(150, 30);
                btnViewTickets.Click += BtnViewTickets_Click;
                
                tabBookings.Controls.AddRange(new Control[] {
                    searchFilterBookings, dataGridBookings, btnNewBooking, btnCancelBooking, btnViewTickets
                });
            }
        }

        private void LoadBookings()
        {
            try
            {
                if (dataGridBookings == null || dbManager == null) return;
                
                // Если пользователь не администратор, показываем только его бронирования
                if (!IsCurrentUserAdmin())
                {
                    var userId = dbManager.GetUserIdByName(currentUserName ?? "");
                    if (userId.HasValue)
                    {
                        dataGridBookings.DataSource = dbManager.GetBookingsByUserId(userId.Value);
                    }
                    else
                    {
                        // Если пользователь не найден в БД, показываем пустую таблицу
                        dataGridBookings.DataSource = new DataTable();
                    }
                }
                else
                {
                    // Администратор видит все бронирования
                dataGridBookings.DataSource = dbManager.GetBookings();
                }
                
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
            var bookingForm = new BookingForm(currentUserName, IsCurrentUserAdmin());
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

        /// <summary>
        /// Отмена собственного бронирования пользователем
        /// </summary>
        private void BtnCancelMyBooking_Click(object? sender, EventArgs e)
        {
            if (dataGridBookings.CurrentRow != null)
            {
                var bookingIdCell = dataGridBookings.CurrentRow.Cells["booking_id"];
                var statusCell = dataGridBookings.CurrentRow.Cells["status"];
                
                if (bookingIdCell?.Value != null && statusCell?.Value != null)
                {
                    int bookingId = Convert.ToInt32(bookingIdCell.Value);
                    string status = statusCell.Value.ToString() ?? "";
                    
                    // Проверяем, можно ли отменить бронирование
                    if (status.ToLower() == "cancelled")
                    {
                        MessageBox.Show("Это бронирование уже отменено", "Информация", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    // Подтверждение отмены
                    string message = "Вы уверены, что хотите отменить свое бронирование?\n" +
                                    "Это действие нельзя отменить!";
                    
                    if (MessageBox.Show(message, "Подтверждение отмены", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            dbManager?.UpdateBookingStatus(bookingId, "cancelled");
                            LoadBookings();
                            
                            MessageBox.Show("Ваше бронирование успешно отменено", "Отмена выполнена", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка отмены брони: {ex.Message}", "Ошибка", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите бронирование для отмены", "Внимание", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    var ticketsForm = new TicketsForm(bookingId, currentUserName, IsCurrentUserAdmin());
                    
                    // Если в форме билетов были изменения, обновляем список бронирований
                    if (ticketsForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadBookings();
                    }
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