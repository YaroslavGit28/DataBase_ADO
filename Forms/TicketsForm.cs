using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CinemaBookingApp.Data;

namespace CinemaBookingApp.Forms
{
    public partial class TicketsForm : Form
    {
        private int bookingId;
        private DataBaseManager dbManager;
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";

        private DataGridView dataGridTickets = null!;
        private Label lblBookingInfo = null!;
        private Button btnClose = null!;

        public TicketsForm(int bookingId)
        {
            this.bookingId = bookingId;
            InitializeComponent();
            dbManager = new DataBaseManager(connectionString);
            LoadTicketsData();
            LoadBookingInfo();
        }

        private void InitializeComponent()
        {
            this.Text = $"Билеты бронирования #{bookingId}";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Информация о бронировании
            lblBookingInfo = new Label();
            lblBookingInfo.Location = new Point(20, 20);
            lblBookingInfo.Size = new Size(550, 40);
            lblBookingInfo.Font = new Font(lblBookingInfo.Font, FontStyle.Bold);

            // Таблица билетов
            dataGridTickets = new DataGridView();
            dataGridTickets.Location = new Point(20, 70);
            dataGridTickets.Size = new Size(550, 250);
            dataGridTickets.ReadOnly = true;
            dataGridTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Кнопка закрытия
            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Location = new Point(470, 330);
            btnClose.Size = new Size(100, 30);
            btnClose.Click += BtnClose_Click;

            this.Controls.AddRange(new Control[] {
                lblBookingInfo,
                dataGridTickets,
                btnClose
            });
        }

        private void LoadBookingInfo()
        {
            try
            {
                var booking = dbManager.GetBookingById(bookingId);
                if (booking != null)
                {
                    var customerName = booking["customer_name"].ToString();
                    var totalAmount = Convert.ToDecimal(booking["total_amount"]);
                    var status = booking["status"].ToString();
                    var orderDate = Convert.ToDateTime(booking["order_date"]).ToString("dd.MM.yyyy HH:mm");

                    lblBookingInfo.Text = $"Бронирование #{bookingId} | Клиент: {customerName} | " +
                                         $"Сумма: {totalAmount} руб | Статус: {status} | " +
                                         $"Дата: {orderDate}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки информации о бронировании: {ex.Message}");
            }
        }

        private void LoadTicketsData()
        {
            try
            {
                var tickets = dbManager.GetTicketsByBooking(bookingId);
                dataGridTickets.DataSource = tickets;

                // Переименование заголовков
                if (dataGridTickets.Columns.Count > 0)
                {
                    dataGridTickets.Columns["ticket_id"]!.HeaderText = "ID билета";
                    dataGridTickets.Columns["seat_number"]!.HeaderText = "Место";
                    dataGridTickets.Columns["base_price"]!.HeaderText = "Базовая цена";
                    dataGridTickets.Columns["final_price"]!.HeaderText = "Финальная цена";
                    dataGridTickets.Columns["title"]!.HeaderText = "Фильм";
                    dataGridTickets.Columns["start_time"]!.HeaderText = "Время сеанса";
                    dataGridTickets.Columns["hall_name"]!.HeaderText = "Зал";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки билетов: {ex.Message}");
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}