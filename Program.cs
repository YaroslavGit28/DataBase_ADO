using System;
using System.Windows.Forms;
using CinemaBookingApp.Forms;

namespace CinemaBookingApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Показываем форму входа
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Если авторизация успешна, запускаем основное приложение
                    using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserName))
                    {
                        Application.Run(mainForm);
                    }
                }
                else
                {
                    // Если пользователь отменил вход, закрываем приложение
                    Application.Exit();
                }
            }
        }
    }
}