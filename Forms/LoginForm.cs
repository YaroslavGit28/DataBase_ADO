using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CinemaBookingApp.Data;

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Форма авторизации пользователя
    /// </summary>
    public partial class LoginForm : Form
    {
        private TextBox txtLogin = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Button btnCancel = null!;
        private Label lblStatus = null!;
        
        private DapperRepository? dapperRepo;
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsAuthenticated { get; private set; } = false;
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string? UserRole { get; private set; }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string? UserName { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            dapperRepo = new DapperRepository(connectionString);
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

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Введите логин и пароль";
                return;
            }

            btnLogin.Enabled = false;
            lblStatus.Text = "Проверка учетных данных...";

            try
            {
                // Проверка администратора
                if (txtLogin.Text.ToLower() == "admin" && txtPassword.Text == "123")
                {
                    IsAuthenticated = true;
                    UserRole = "Admin";
                    UserName = "Администратор";
                    lblStatus.Text = "Успешный вход как администратор";
                    lblStatus.ForeColor = Color.Green;
                    
                    await Task.Delay(1000); // Небольшая задержка для показа сообщения
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // Проверка обычного пользователя по имени или email
                var user = await dapperRepo!.GetUserByEmailAsync(txtLogin.Text.Trim());
                if (user == null)
                {
                    // Если не найден по email, ищем по имени
                    var users = await dapperRepo.GetUsersAsync();
                    user = users.FirstOrDefault(u => u.Name.ToLower().Contains(txtLogin.Text.Trim().ToLower()));
                }
                
                if (user != null)
                {
                    // Для обычных пользователей пароль не проверяем
                    IsAuthenticated = true;
                    UserRole = user.Role?.RoleName ?? "User";
                    UserName = user.Name;
                    lblStatus.Text = $"Добро пожаловать, {user.Name}!";
                    lblStatus.ForeColor = Color.Green;
                    
                    await Task.Delay(1000);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
