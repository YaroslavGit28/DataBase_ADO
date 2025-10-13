using System;                    // Базовые типы и классы .NET Framework
using System.Drawing;            // Классы для работы с графикой и цветами
using System.Linq;               // LINQ для работы с коллекциями
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Data;    // Наши классы для работы с базой данных

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Форма авторизации пользователя в системе бронирования кинотеатра
    /// Обрабатывает вход администраторов и обычных пользователей
    /// </summary>
    public partial class LoginForm : Form
    {
        // =============================================
        // ЭЛЕМЕНТЫ ПОЛЬЗОВАТЕЛЬСКОГО ИНТЕРФЕЙСА
        // =============================================
        
        /// <summary>
        /// Поле ввода логина (email или имя пользователя)
        /// </summary>
        private TextBox txtLogin = null!;
        
        /// <summary>
        /// Поле ввода пароля (только для администраторов)
        /// </summary>
        private TextBox txtPassword = null!;
        
        /// <summary>
        /// Кнопка "Войти" для выполнения авторизации
        /// </summary>
        private Button btnLogin = null!;
        
        /// <summary>
        /// Кнопка "Отмена" для закрытия формы без входа
        /// </summary>
        private Button btnCancel = null!;
        
        /// <summary>
        /// Метка для отображения статуса авторизации и подсказок
        /// </summary>
        private Label lblStatus = null!;
        
        // =============================================
        // ПОЛЯ ДЛЯ РАБОТЫ С БАЗОЙ ДАННЫХ
        // =============================================
        
        /// <summary>
        /// Репозиторий для работы с данными через Dapper ORM
        /// Используется для проверки пользователей в базе данных
        /// </summary>
        private DapperRepository? dapperRepo;
        
        /// <summary>
        /// Строка подключения к базе данных SQL Server
        /// Содержит адрес сервера, имя базы данных, учетные данные и параметры безопасности
        /// </summary>
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        
        // =============================================
        // СВОЙСТВА ДЛЯ ПЕРЕДАЧИ РЕЗУЛЬТАТОВ АВТОРИЗАЦИИ
        // =============================================
        
        /// <summary>
        /// Флаг успешной авторизации пользователя
        /// true - пользователь успешно вошел в систему, false - авторизация не выполнена
        /// </summary>
        [System.ComponentModel.Browsable(false)]  // Скрываем от дизайнера форм
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]  // Не сериализуем
        public bool IsAuthenticated { get; private set; } = false;
        
        /// <summary>
        /// Роль авторизованного пользователя (Admin или User)
        /// Определяет доступные функции в главном приложении
        /// </summary>
        [System.ComponentModel.Browsable(false)]  // Скрываем от дизайнера форм
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]  // Не сериализуем
        public string? UserRole { get; private set; }
        
        /// <summary>
        /// Имя авторизованного пользователя
        /// Используется для отображения в главном приложении
        /// </summary>
        [System.ComponentModel.Browsable(false)]  // Скрываем от дизайнера форм
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]  // Не сериализуем
        public string? UserName { get; private set; }

        /// <summary>
        /// Конструктор формы авторизации
        /// Инициализирует интерфейс и подключается к базе данных
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();                                    // Создаем элементы интерфейса
            dapperRepo = new DapperRepository(connectionString);     // Инициализируем репозиторий для работы с БД
        }

        private void InitializeComponent()
        {
            this.Text = "Вход в систему";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Заголовок
            var lblTitle = new Label();
            lblTitle.Text = "Система бронирования кинотеатра";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(50, 20);
            lblTitle.Size = new Size(300, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Логин
            var lblLogin = new Label();
            lblLogin.Text = "Логин:";
            lblLogin.Location = new Point(50, 70);
            lblLogin.Size = new Size(80, 20);

            txtLogin = new TextBox();
            txtLogin.Location = new Point(140, 70);
            txtLogin.Size = new Size(200, 20);
            txtLogin.Text = "admin"; // По умолчанию

            // Пароль
            var lblPassword = new Label();
            lblPassword.Text = "Пароль:";
            lblPassword.Location = new Point(50, 110);
            lblPassword.Size = new Size(80, 20);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(140, 110);
            txtPassword.Size = new Size(200, 20);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Text = "123"; // По умолчанию

            // Статус
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Location = new Point(50, 140);
            lblStatus.Size = new Size(290, 20);
            lblStatus.ForeColor = Color.Red;

            // Кнопки
            btnLogin = new Button();
            btnLogin.Text = "Войти";
            btnLogin.Location = new Point(140, 180);
            btnLogin.Size = new Size(100, 30);
            btnLogin.Click += BtnLogin_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(250, 180);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += BtnCancel_Click;

            // Подсказка
            var lblHint = new Label();
            lblHint.Text = "Админ: admin / 123\nПользователь: введите свое имя";
            lblHint.Location = new Point(50, 220);
            lblHint.Size = new Size(300, 40);
            lblHint.Font = new Font("Arial", 8);
            lblHint.ForeColor = Color.Gray;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblLogin, txtLogin,
                lblPassword, txtPassword, lblStatus,
                btnLogin, btnCancel, lblHint
            });

            // Обработчики событий
            txtPassword.KeyPress += TxtPassword_KeyPress;
            txtLogin.KeyPress += TxtLogin_KeyPress;
        }

        private void TxtPassword_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void TxtLogin_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Войти"
        /// Выполняет авторизацию пользователя (администратора или обычного пользователя)
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка)</param>
        /// <param name="e">Аргументы события</param>
        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            // Проверяем, что поля логина и пароля заполнены
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Введите логин и пароль";  // Показываем сообщение об ошибке
                return;  // Прерываем выполнение
            }

            // Блокируем кнопку и показываем статус проверки
            btnLogin.Enabled = false;                        // Отключаем кнопку для предотвращения повторных нажатий
            lblStatus.Text = "Проверка учетных данных...";   // Показываем статус проверки

            try  // Обрабатываем возможные ошибки при авторизации
            {
                // =============================================
                // ПРОВЕРКА АДМИНИСТРАТОРА
                // =============================================
                
                // Проверяем учетные данные администратора
                if (txtLogin.Text.ToLower() == "admin" && txtPassword.Text == "123")
                {
                    // Устанавливаем флаги успешной авторизации администратора
                    IsAuthenticated = true;                    // Помечаем как авторизованного
                    UserRole = "Admin";                         // Устанавливаем роль администратора
                    UserName = "Администратор";                 // Устанавливаем имя администратора
                    lblStatus.Text = "Успешный вход как администратор";  // Показываем сообщение об успехе
                    lblStatus.ForeColor = Color.Green;         // Устанавливаем зеленый цвет текста
                    
                    await Task.Delay(1000);                     // Небольшая задержка для показа сообщения
                    this.DialogResult = DialogResult.OK;        // Устанавливаем результат диалога как успешный
                    this.Close();                               // Закрываем форму авторизации
                    return;                                     // Выходим из метода
                }

                // =============================================
                // ПРОВЕРКА ОБЫЧНОГО ПОЛЬЗОВАТЕЛЯ
                // =============================================
                
                // Ищем пользователя по email в базе данных
                var user = await dapperRepo!.GetUserByEmailAsync(txtLogin.Text.Trim());
                if (user == null)  // Если не найден по email
                {
                    // Ищем пользователя по имени (частичное совпадение)
                    var users = await dapperRepo.GetUsersAsync();  // Получаем всех пользователей
                    user = users.FirstOrDefault(u => u.Name.ToLower().Contains(txtLogin.Text.Trim().ToLower()));  // Ищем по имени
                }
                
                if (user != null)  // Если пользователь найден
                {
                    // Для обычных пользователей пароль не проверяем (только имя)
                    IsAuthenticated = true;                           // Помечаем как авторизованного
                    UserRole = user.Role?.RoleName ?? "User";        // Устанавливаем роль пользователя
                    UserName = user.Name;                             // Устанавливаем имя пользователя
                    lblStatus.Text = $"Добро пожаловать, {user.Name}!";  // Показываем приветствие
                    lblStatus.ForeColor = Color.Green;                // Устанавливаем зеленый цвет текста
                    
                    await Task.Delay(1000);                           // Небольшая задержка для показа сообщения
                    this.DialogResult = DialogResult.OK;              // Устанавливаем результат диалога как успешный
                    this.Close();                                     // Закрываем форму авторизации
                    return;
                }
                
                // Если пользователь не найден, создаем временного пользователя
                if (!string.IsNullOrWhiteSpace(txtLogin.Text.Trim()))
                {
                    IsAuthenticated = true;
                    UserRole = "User";
                    UserName = txtLogin.Text.Trim();
                    lblStatus.Text = $"Добро пожаловать, {UserName}!";
                    lblStatus.ForeColor = Color.Green;
                    
                    await Task.Delay(1000);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // Неверные учетные данные
                lblStatus.Text = "Неверный логин или пароль";
                lblStatus.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка авторизации: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
