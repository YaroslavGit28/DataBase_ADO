using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CinemaBookingApp.Data;

namespace CinemaBookingApp.Forms
{
    public partial class TicketsForm : Form
    {
        private int bookingId;
        private DataBaseManager dbManager;
        private string connectionString = "Server=192.168.9.203\\SQLEXPRESS;Database=Проект Вакула, Белов, Сухинин;User Id=student1;Password=123456;TrustServerCertificate=true;";
        private string? currentUserName;
        private bool isCurrentUserAdmin;

        private DataGridView dataGridTickets = null!;
        private Label lblBookingInfo = null!;
        private Button btnClose = null!;
        private Button btnCancelSelected = null!;
        private CheckBox chkSelectAll = null!;

        public TicketsForm(int bookingId, string? currentUserName = null, bool isCurrentUserAdmin = false)
        {
            this.bookingId = bookingId;
            this.currentUserName = currentUserName;
            this.isCurrentUserAdmin = isCurrentUserAdmin;
            InitializeComponent();
            dbManager = new DataBaseManager(connectionString);
            
            // Проверяем доступ к бронированию
            if (!CheckBookingAccess())
            {
                MessageBox.Show("У вас нет доступа к этому бронированию", "Ошибка доступа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            
            LoadTicketsData();
            LoadBookingInfo();
        }

        /// <summary>
        /// Проверяет, есть ли у текущего пользователя доступ к бронированию
        /// </summary>
        private bool CheckBookingAccess()
        {
            try
            {
                // Администратор имеет доступ ко всем бронированиям
                if (isCurrentUserAdmin)
                    return true;
                
                // Обычный пользователь может видеть только свои бронирования
                if (string.IsNullOrEmpty(currentUserName))
                    return false;
                
                var booking = dbManager.GetBookingById(bookingId);
                if (booking == null)
                    return false;
                
                var customerName = booking["customer_name"]?.ToString();
                return string.Equals(customerName, currentUserName, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"Билеты бронирования #{bookingId}";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Информация о бронировании
            lblBookingInfo = new Label();
            lblBookingInfo.Location = new Point(20, 20);
            lblBookingInfo.Size = new Size(650, 40);
            lblBookingInfo.Font = new Font(lblBookingInfo.Font, FontStyle.Bold);

            // Чекбокс "Выбрать все"
            chkSelectAll = new CheckBox();
            chkSelectAll.Text = "Выбрать все билеты";
            chkSelectAll.Location = new Point(20, 60);
            chkSelectAll.Size = new Size(150, 20);
            chkSelectAll.CheckedChanged += ChkSelectAll_CheckedChanged;

            // Таблица билетов
            dataGridTickets = new DataGridView();
            dataGridTickets.Location = new Point(20, 90);
            dataGridTickets.Size = new Size(650, 300);
            dataGridTickets.ReadOnly = false;
            dataGridTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridTickets.AllowUserToAddRows = false;
            dataGridTickets.AllowUserToDeleteRows = false;

            // Кнопка отмены выбранных билетов
            btnCancelSelected = new Button();
            btnCancelSelected.Text = "Отменить выбранные билеты";
            btnCancelSelected.Location = new Point(20, 400);
            btnCancelSelected.Size = new Size(200, 35);
            btnCancelSelected.BackColor = Color.Orange;
            btnCancelSelected.Click += BtnCancelSelected_Click;

            // Кнопка закрытия
            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Location = new Point(570, 400);
            btnClose.Size = new Size(100, 35);
            btnClose.Click += BtnClose_Click;

            this.Controls.AddRange(new Control[] {
                lblBookingInfo,
                chkSelectAll,
                dataGridTickets,
                btnCancelSelected,
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
                var tickets = dbManager.GetTicketsByBookingId(bookingId);
                dataGridTickets.DataSource = tickets;

                // Добавляем колонку с чекбоксами для выбора билетов
                if (dataGridTickets.Columns.Count > 0)
                {
                    // Добавляем колонку выбора в начало
                    var selectColumn = new DataGridViewCheckBoxColumn();
                    selectColumn.HeaderText = "Выбрать";
                    selectColumn.Name = "SelectTicket";
                    selectColumn.Width = 60;
                    dataGridTickets.Columns.Insert(0, selectColumn);

                    // Переименование заголовков
                    if (dataGridTickets.Columns.Contains("ticket_id"))
                        dataGridTickets.Columns["ticket_id"]!.HeaderText = "ID билета";
                    if (dataGridTickets.Columns.Contains("seat_number"))
                        dataGridTickets.Columns["seat_number"]!.HeaderText = "Место";
                    if (dataGridTickets.Columns.Contains("price"))
                        dataGridTickets.Columns["price"]!.HeaderText = "Цена";
                    if (dataGridTickets.Columns.Contains("movie_title"))
                        dataGridTickets.Columns["movie_title"]!.HeaderText = "Фильм";
                    if (dataGridTickets.Columns.Contains("start_time"))
                        dataGridTickets.Columns["start_time"]!.HeaderText = "Время сеанса";
                    if (dataGridTickets.Columns.Contains("hall_name"))
                        dataGridTickets.Columns["hall_name"]!.HeaderText = "Зал";

                    // Делаем колонку выбора единственной редактируемой
                    foreach (DataGridViewColumn column in dataGridTickets.Columns)
                    {
                        column.ReadOnly = true;
                    }
                    dataGridTickets.Columns["SelectTicket"].ReadOnly = false;
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

        private void ChkSelectAll_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                // Выбираем все билеты
                foreach (DataGridViewRow row in dataGridTickets.Rows)
                {
                    if (row.Cells["SelectTicket"] != null)
                    {
                        row.Cells["SelectTicket"].Value = true;
                    }
                }
            }
            else
            {
                // Снимаем выбор со всех билетов
                foreach (DataGridViewRow row in dataGridTickets.Rows)
                {
                    if (row.Cells["SelectTicket"] != null)
                    {
                        row.Cells["SelectTicket"].Value = false;
                    }
                }
            }
        }

        private void BtnCancelSelected_Click(object? sender, EventArgs e)
        {
            try
            {
                // Проверяем статус бронирования для обычных пользователей
                if (!isCurrentUserAdmin)
                {
                    var booking = dbManager.GetBookingById(bookingId);
                    if (booking != null)
                    {
                        var status = booking["status"]?.ToString()?.ToLower();
                        if (status == "cancelled")
                        {
                            MessageBox.Show("Это бронирование уже отменено", "Информация", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                // Получаем список выбранных билетов
                var selectedTicketIds = new List<int>();
                
                foreach (DataGridViewRow row in dataGridTickets.Rows)
                {
                    if (row.Cells["SelectTicket"]?.Value is bool isSelected && isSelected)
                    {
                        if (row.Cells["ticket_id"]?.Value is int ticketId)
                        {
                            selectedTicketIds.Add(ticketId);
                        }
                    }
                }

                if (selectedTicketIds.Count == 0)
                {
                    MessageBox.Show("Выберите билеты для отмены", "Внимание", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Подтверждение отмены
                string message = $"Вы уверены, что хотите отменить {selectedTicketIds.Count} билет(ов)?\n" +
                                "Это действие нельзя отменить!";
                
                if (MessageBox.Show(message, "Подтверждение отмены", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Отменяем выбранные билеты
                    dbManager.CancelTickets(bookingId, selectedTicketIds);
                    
                    MessageBox.Show($"Успешно отменено {selectedTicketIds.Count} билет(ов)", 
                        "Отмена выполнена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Обновляем данные
                    LoadTicketsData();
                    LoadBookingInfo();
                    
                    // Уведомляем родительскую форму об изменениях
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене билетов: {ex.Message}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}