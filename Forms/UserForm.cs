using System;                    // Базовые типы и классы .NET Framework
using System.Drawing;            // Классы для работы с графикой и цветами
using System.Text.RegularExpressions;  // Классы для работы с регулярными выражениями
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Data;    // Наши классы для работы с базой данных

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Форма для добавления и редактирования пользователей системы
    /// Включает валидацию данных и автоматическое форматирование номера телефона
    /// </summary>
    public partial class UserForm : Form
    {
        // =============================================
        // ПОЛЯ ДЛЯ РАБОТЫ С ДАННЫМИ
        // =============================================
        
        /// <summary>
        /// ID пользователя для редактирования (null для создания нового)
        /// </summary>
        private int? userId;
        
        /// <summary>
        /// Менеджер базы данных для выполнения операций с пользователями
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
        /// Поле ввода имени пользователя
        /// </summary>
        private TextBox txtName = null!;
        
        /// <summary>
        /// Поле ввода email адреса пользователя
        /// </summary>
        private TextBox txtEmail = null!;
        
        /// <summary>
        /// Поле ввода номера телефона с автоматическим форматированием
        /// </summary>
        private TextBox txtPhone = null!;
        
        /// <summary>
        /// Кнопка "Сохранить" для сохранения данных пользователя
        /// </summary>
        private Button btnSave = null!;
        
        /// <summary>
        /// Кнопка "Отмена" для закрытия формы без сохранения
        /// </summary>
        private Button btnCancel = null!;

        /// <summary>
        /// Конструктор для создания нового пользователя
        /// Инициализирует форму в режиме добавления
        /// </summary>
        public UserForm()
        {
            InitializeComponent();                                    // Создаем элементы интерфейса
            dbManager = new DataBaseManager(connectionString);       // Инициализируем менеджер БД
        }

        /// <summary>
        /// Конструктор для редактирования существующего пользователя
        /// Загружает данные пользователя в форму
        /// </summary>
        /// <param name="userId">ID пользователя для редактирования</param>
        public UserForm(int userId) : this()  // Вызываем базовый конструктор
        {
            this.userId = userId;             // Сохраняем ID пользователя
            LoadUserData();                   // Загружаем данные пользователя
        }

        private void InitializeComponent()
        {
            this.Text = userId.HasValue ? "Редактировать пользователя" : "Добавить пользователя";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Имя
            var lblName = new Label();
            lblName.Text = "Имя:";
            lblName.Location = new Point(20, 20);
            lblName.Size = new Size(100, 20);

            txtName = new TextBox();
            txtName.Location = new Point(120, 20);
            txtName.Size = new Size(250, 20);

            // Email
            var lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(20, 60);
            lblEmail.Size = new Size(100, 20);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(120, 60);
            txtEmail.Size = new Size(250, 20);

            // Телефон
            var lblPhone = new Label();
            lblPhone.Text = "Телефон:";
            lblPhone.Location = new Point(20, 100);
            lblPhone.Size = new Size(100, 20);

            txtPhone = new TextBox();
            txtPhone.Location = new Point(120, 100);
            txtPhone.Size = new Size(250, 20);
            txtPhone.PlaceholderText = "+7 (999) 999-99-99";
            txtPhone.MaxLength = 18; // Максимальная длина с учетом маски
            txtPhone.TextChanged += TxtPhone_TextChanged;

            // Кнопки
            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(120, 150);
            btnSave.Size = new Size(100, 30);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(230, 150);
            btnCancel.Size = new Size(100, 30);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                btnSave, btnCancel
            });
        }

        private void LoadUserData()
        {
            if (!userId.HasValue) return;

            try
            {
                var user = dbManager.GetUserById(userId.Value);
                if (user != null)
                {
                    txtName.Text = user["name"].ToString();
                    txtEmail.Text = user["email"].ToString();
                    txtPhone.Text = user["phone"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик изменения текста в поле телефона
        /// </summary>
        private void TxtPhone_TextChanged(object? sender, EventArgs e)
        {
            if (txtPhone.Text.Length > 0)
            {
                // Получаем только цифры из введенного текста
                string digitsOnly = Regex.Replace(txtPhone.Text, @"[^\d]", "");
                
                // Если введено достаточно цифр, форматируем номер
                if (digitsOnly.Length >= 10)
                {
                    string formatted = FormatPhoneNumber(digitsOnly);
                    if (formatted != txtPhone.Text)
                    {
                        // Временно отключаем событие чтобы избежать рекурсии
                        txtPhone.TextChanged -= TxtPhone_TextChanged;
                        txtPhone.Text = formatted;
                        txtPhone.SelectionStart = txtPhone.Text.Length; // Курсор в конец
                        txtPhone.TextChanged += TxtPhone_TextChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Валидация номера телефона
        /// </summary>
        /// <param name="phoneNumber">Номер телефона для проверки</param>
        /// <returns>true если номер валидный, false если нет</returns>
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return true; // Телефон не обязателен

            // Убираем все символы кроме цифр и +
            string cleanPhone = Regex.Replace(phoneNumber, @"[^\d+]", "");
            
            // Проверяем различные форматы российских номеров
            string[] validPatterns = {
                @"^\+7\d{10}$",           // +7XXXXXXXXXX
                @"^8\d{10}$",             // 8XXXXXXXXXX
                @"^7\d{10}$",             // 7XXXXXXXXXX
                @"^\d{10}$"               // XXXXXXXXXX
            };

            foreach (string pattern in validPatterns)
            {
                if (Regex.IsMatch(cleanPhone, pattern))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Форматирует номер телефона в стандартный вид
        /// </summary>
        /// <param name="phoneNumber">Исходный номер телефона</param>
        /// <returns>Отформатированный номер</returns>
        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            // Убираем все символы кроме цифр и +
            string cleanPhone = Regex.Replace(phoneNumber, @"[^\d+]", "");
            
            // Приводим к формату +7XXXXXXXXXX
            if (cleanPhone.StartsWith("8") && cleanPhone.Length == 11)
            {
                cleanPhone = "+7" + cleanPhone.Substring(1);
            }
            else if (cleanPhone.StartsWith("7") && cleanPhone.Length == 11)
            {
                cleanPhone = "+" + cleanPhone;
            }
            else if (cleanPhone.Length == 10)
            {
                cleanPhone = "+7" + cleanPhone;
            }

            // Форматируем в читаемый вид: +7 (999) 999-99-99
            if (cleanPhone.Length == 12 && cleanPhone.StartsWith("+7"))
            {
                return $"+7 ({cleanPhone.Substring(2, 3)}) {cleanPhone.Substring(5, 3)}-{cleanPhone.Substring(8, 2)}-{cleanPhone.Substring(10, 2)}";
            }

            return phoneNumber; // Возвращаем исходный номер если не удалось отформатировать
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите имя пользователя");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите email пользователя");
                return;
            }

            // Простая валидация email
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Введите корректный email");
                return;
            }

            // Валидация номера телефона
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !IsValidPhoneNumber(txtPhone.Text))
            {
                MessageBox.Show("Введите корректный номер телефона.\nПоддерживаемые форматы:\n• +7 (999) 999-99-99\n• 8 (999) 999-99-99\n• 7 (999) 999-99-99\n• (999) 999-99-99");
                txtPhone.Focus();
                return;
            }

            try
            {
                // Форматируем номер телефона перед сохранением
                string formattedPhone = string.IsNullOrWhiteSpace(txtPhone.Text) ? 
                    string.Empty : FormatPhoneNumber(txtPhone.Text);

                if (userId.HasValue)
                {
                    // Редактирование
                    dbManager.UpdateUser(
                        userId.Value,
                        txtName.Text.Trim(),
                        txtEmail.Text.Trim(),
                        string.IsNullOrWhiteSpace(formattedPhone) ? null : formattedPhone
                    );
                    MessageBox.Show("Пользователь успешно обновлен");
                }
                else
                {
                    // Проверка уникальности email
                    var existingUser = dbManager.GetUserByEmail(txtEmail.Text.Trim());
                    if (existingUser != null)
                    {
                        MessageBox.Show("Пользователь с таким email уже существует");
                        return;
                    }

                    // Добавление
                    dbManager.AddUser(
                        txtName.Text.Trim(),
                        txtEmail.Text.Trim(),
                        string.IsNullOrWhiteSpace(formattedPhone) ? null : formattedPhone
                    );
                    MessageBox.Show("Пользователь успешно добавлен");
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