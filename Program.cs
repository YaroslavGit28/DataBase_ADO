using System;                    // Базовые типы и классы .NET Framework
using System.Windows.Forms;      // Классы для создания Windows Forms приложений
using CinemaBookingApp.Forms;    // Наши пользовательские формы

namespace CinemaBookingApp
{
    /// <summary>
    /// Главный класс приложения системы бронирования билетов в кинотеатре
    /// Содержит точку входа в приложение и логику запуска
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Атрибут для указания, что метод Main должен выполняться в однопоточном режиме
        /// Необходим для корректной работы Windows Forms приложений
        /// </summary>
        [STAThread]
        
        /// <summary>
        /// Главная точка входа в приложение
        /// Инициализирует Windows Forms и запускает процесс авторизации
        /// </summary>
        static void Main()
        {
            // Настраиваем визуальные стили для Windows Forms
            Application.EnableVisualStyles();                    // Включаем современные визуальные стили
            Application.SetCompatibleTextRenderingDefault(false); // Отключаем устаревший рендеринг текста
            
            // =============================================
            // ПРОЦЕСС АВТОРИЗАЦИИ ПОЛЬЗОВАТЕЛЯ
            // =============================================
            
            // Показываем форму входа в систему
            using (var loginForm = new LoginForm())  // Создаем форму авторизации
            {
                // Показываем форму как диалог и ждем результата
                if (loginForm.ShowDialog() == DialogResult.OK)  // Если пользователь успешно авторизовался
                {
                    // Если авторизация успешна, запускаем основное приложение
                    using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserName))  // Создаем главную форму с данными пользователя
                    {
                        Application.Run(mainForm);  // Запускаем главное приложение
                    }
                }
                else  // Если пользователь отменил вход или произошла ошибка
                {
                    // Если пользователь отменил вход, закрываем приложение
                    Application.Exit();  // Завершаем работу приложения
                }
            }
        }
    }
}