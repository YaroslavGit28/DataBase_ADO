using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace CinemaBookingApp.Forms
{
    /// <summary>
    /// Универсальный компонент для поиска и фильтрации данных
    /// </summary>
    public partial class SearchFilterControl : UserControl
    {
        private TextBox txtSearch = null!;
        private ComboBox cmbFilterColumn = null!;
        private ComboBox cmbFilterOperator = null!;
        private TextBox txtFilterValue = null!;
        private Button btnSearch = null!;
        private Button btnClear = null!;
        private Label lblSearch = null!;
        private Label lblFilter = null!;
        
        private DataGridView? targetDataGrid;
        private DataTable? originalData;
        private DataTable? filteredData;
        
        public event EventHandler? SearchPerformed;
        public event EventHandler? FilterCleared;

        public SearchFilterControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 80);
            this.BackColor = Color.LightGray;
            this.BorderStyle = BorderStyle.FixedSingle;

            // Поиск
            lblSearch = new Label();
            lblSearch.Text = "Поиск:";
            lblSearch.Location = new Point(10, 15);
            lblSearch.Size = new Size(50, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(70, 12);
            txtSearch.Size = new Size(200, 20);
            txtSearch.PlaceholderText = "Введите текст для поиска...";
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnSearch = new Button();
            btnSearch.Text = "Найти";
            btnSearch.Location = new Point(280, 10);
            btnSearch.Size = new Size(80, 25);
            btnSearch.Click += BtnSearch_Click;

            btnClear = new Button();
            btnClear.Text = "Очистить";
            btnClear.Location = new Point(370, 10);
            btnClear.Size = new Size(80, 25);
            btnClear.Click += BtnClear_Click;

            // Фильтрация
            lblFilter = new Label();
            lblFilter.Text = "Фильтр:";
            lblFilter.Location = new Point(10, 45);
            lblFilter.Size = new Size(50, 20);

            cmbFilterColumn = new ComboBox();
            cmbFilterColumn.Location = new Point(70, 42);
            cmbFilterColumn.Size = new Size(120, 20);
            cmbFilterColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterColumn.SelectedIndexChanged += CmbFilterColumn_SelectedIndexChanged;

            cmbFilterOperator = new ComboBox();
            cmbFilterOperator.Location = new Point(200, 42);
            cmbFilterOperator.Size = new Size(80, 20);
            cmbFilterOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterOperator.Items.AddRange(new string[] { "=", "!=", ">", "<", ">=", "<=", "Содержит", "Начинается с", "Заканчивается на" });
            cmbFilterOperator.SelectedIndex = 0;

            txtFilterValue = new TextBox();
            txtFilterValue.Location = new Point(290, 42);
            txtFilterValue.Size = new Size(100, 20);
            txtFilterValue.PlaceholderText = "Значение";

            Button btnApplyFilter = new Button();
            btnApplyFilter.Text = "Применить";
            btnApplyFilter.Location = new Point(400, 40);
            btnApplyFilter.Size = new Size(80, 25);
            btnApplyFilter.Click += BtnApplyFilter_Click;

            Button btnClearFilter = new Button();
            btnClearFilter.Text = "Сбросить";
            btnClearFilter.Location = new Point(490, 40);
            btnClearFilter.Size = new Size(80, 25);
            btnClearFilter.Click += BtnClearFilter_Click;

            this.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnClear,
                lblFilter, cmbFilterColumn, cmbFilterOperator, txtFilterValue, btnApplyFilter, btnClearFilter
            });
        }

        /// <summary>
        /// Устанавливает целевую таблицу для поиска и фильтрации
        /// </summary>
        public void SetTargetDataGrid(DataGridView dataGrid)
        {
            targetDataGrid = dataGrid;
            originalData = dataGrid.DataSource as DataTable;
            filteredData = originalData?.Copy();
            
            // Заполняем список колонок для фильтрации
            cmbFilterColumn.Items.Clear();
            if (originalData != null)
            {
                foreach (DataColumn column in originalData.Columns)
                {
                    cmbFilterColumn.Items.Add(column.ColumnName);
                }
                if (cmbFilterColumn.Items.Count > 0)
                {
                    cmbFilterColumn.SelectedIndex = 0;
                }
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            // Автоматический поиск при вводе текста
            PerformSearch();
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            PerformSearch();
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            if (originalData != null && targetDataGrid != null)
            {
                targetDataGrid.DataSource = originalData;
                filteredData = originalData.Copy();
            }
            FilterCleared?.Invoke(this, EventArgs.Empty);
        }

        private void CmbFilterColumn_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Можно добавить логику для изменения операторов в зависимости от типа колонки
        }

        private void BtnApplyFilter_Click(object? sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void BtnClearFilter_Click(object? sender, EventArgs e)
        {
            ClearFilter();
        }

        /// <summary>
        /// Выполняет поиск по всем колонкам
        /// </summary>
        private void PerformSearch()
        {
            if (originalData == null || targetDataGrid == null) return;

            string searchText = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchText))
            {
                targetDataGrid.DataSource = originalData;
                filteredData = originalData.Copy();
                return;
            }

            DataTable searchResult = originalData.Clone();
            
            foreach (DataRow row in originalData.Rows)
            {
                bool found = false;
                foreach (DataColumn column in originalData.Columns)
                {
                    if (row[column].ToString().ToLower().Contains(searchText))
                    {
                        found = true;
                        break;
                    }
                }
                
                if (found)
                {
                    searchResult.ImportRow(row);
                }
            }

            targetDataGrid.DataSource = searchResult;
            filteredData = searchResult.Copy();
            SearchPerformed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Применяет фильтр по выбранной колонке
        /// </summary>
        private void ApplyFilter()
        {
            if (filteredData == null || targetDataGrid == null) return;
            if (cmbFilterColumn.SelectedItem == null || string.IsNullOrEmpty(txtFilterValue.Text.Trim())) return;

            string columnName = cmbFilterColumn.SelectedItem.ToString() ?? "";
            string operatorValue = cmbFilterOperator.SelectedItem?.ToString() ?? "=";
            string filterValue = txtFilterValue.Text.Trim();

            DataTable filterResult = filteredData.Clone();

            foreach (DataRow row in filteredData.Rows)
            {
                bool matches = false;
                object cellValue = row[columnName];
                string cellValueStr = cellValue?.ToString() ?? "";

                switch (operatorValue)
                {
                    case "=":
                        matches = cellValueStr.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "!=":
                        matches = !cellValueStr.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                        break;
                    case "Содержит":
                        matches = cellValueStr.ToLower().Contains(filterValue.ToLower());
                        break;
                    case "Начинается с":
                        matches = cellValueStr.ToLower().StartsWith(filterValue.ToLower());
                        break;
                    case "Заканчивается на":
                        matches = cellValueStr.ToLower().EndsWith(filterValue.ToLower());
                        break;
                    case ">":
                        if (IsNumeric(cellValue) && IsNumeric(filterValue))
                            matches = Convert.ToDecimal(cellValue) > Convert.ToDecimal(filterValue);
                        break;
                    case "<":
                        if (IsNumeric(cellValue) && IsNumeric(filterValue))
                            matches = Convert.ToDecimal(cellValue) < Convert.ToDecimal(filterValue);
                        break;
                    case ">=":
                        if (IsNumeric(cellValue) && IsNumeric(filterValue))
                            matches = Convert.ToDecimal(cellValue) >= Convert.ToDecimal(filterValue);
                        break;
                    case "<=":
                        if (IsNumeric(cellValue) && IsNumeric(filterValue))
                            matches = Convert.ToDecimal(cellValue) <= Convert.ToDecimal(filterValue);
                        break;
                }

                if (matches)
                {
                    filterResult.ImportRow(row);
                }
            }

            targetDataGrid.DataSource = filterResult;
        }

        /// <summary>
        /// Очищает фильтр
        /// </summary>
        private void ClearFilter()
        {
            txtFilterValue.Clear();
            if (filteredData != null && targetDataGrid != null)
            {
                targetDataGrid.DataSource = filteredData;
            }
        }

        /// <summary>
        /// Проверяет, является ли значение числовым
        /// </summary>
        private bool IsNumeric(object value)
        {
            return value != null && (value is int || value is decimal || value is double || value is float || 
                   decimal.TryParse(value.ToString(), out _));
        }

        /// <summary>
        /// Получает текущее количество отфильтрованных записей
        /// </summary>
        public int GetFilteredRowCount()
        {
            return targetDataGrid?.RowCount ?? 0;
        }

        /// <summary>
        /// Получает общее количество записей
        /// </summary>
        public int GetTotalRowCount()
        {
            return originalData?.Rows.Count ?? 0;
        }
    }
}
