﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class Customer : Form
    {
        DBConnection cnn = new DBConnection();

        public Customer()
        {
            InitializeComponent();
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            cnn.Connect();
            LoadData();
        }

        private void LoadData()
        {
            dgCustomer.DataSource = cnn.ListCustomer();
            dgCustomer.Refresh();
            AddEditAndDeleteButtons();
        }

        private void AddEditAndDeleteButtons()
        {
            // Thêm cột nút sửa
            DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
            editButton.HeaderText = "";
            editButton.Name = "Edit";
            editButton.Text = "Sửa";
            editButton.UseColumnTextForButtonValue = true;
            dgCustomer.Columns.Add(editButton);

            // Thêm cột nút xóa
            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
            deleteButton.HeaderText = "";
            deleteButton.Name = "Delete";
            deleteButton.Text = "Xóa";
            deleteButton.UseColumnTextForButtonValue = true;
            dgCustomer.Columns.Add(deleteButton);

            dgCustomer.CellClick += new DataGridViewCellEventHandler(dgCustomer_CellClick);
        }

        private void dgCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgCustomer.Rows.Count)
            {
                DataGridViewColumn editColumn = dgCustomer.Columns["Edit"];
                DataGridViewColumn deleteColumn = dgCustomer.Columns["Delete"];

                if (editColumn != null && e.ColumnIndex == editColumn.Index)
                {
                    // Lấy mã khách hàng cần sửa
                    int customerID = (int)dgCustomer.Rows[e.RowIndex].Cells["MaKH"].Value;

                    // Gọi form sửa thông tin với MaKH đã biết
                    EditCustomer formEdit = new EditCustomer(customerID);
                    formEdit.ShowDialog();

                    // Load lại dữ liệu
                    dgCustomer.DataSource = cnn.ListCustomer();
                    dgCustomer.Refresh();
                }
                else if (deleteColumn != null && e.ColumnIndex == deleteColumn.Index)
                {
                    try
                    {
                        // Lấy mã khách hàng cần xóa
                        int customerID = (int)dgCustomer.Rows[e.RowIndex].Cells["MaKH"].Value;

                        // Hiển thị hộp thoại xác nhận
                        DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này không?",
                                                                    "Xác nhận xóa",
                                                                    MessageBoxButtons.YesNo,
                                                                    MessageBoxIcon.Question);

                        // Nếu người dùng nhấn Yes, tiến hành xóa
                        if (dialogResult == DialogResult.Yes)
                        {
                            cnn.DeleteCustomer(customerID);

                            MessageBox.Show("Xóa khách hàng thành công!",
                                            "Thông báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

                            dgCustomer.DataSource = cnn.ListCustomer();
                            dgCustomer.Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                                        "Lỗi",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            // Gọi form thêm thông tin khách hàng mới
            AddCustomer addCustomer = new AddCustomer();
            addCustomer.ShowDialog();

            // Load lại dữ liệu
            dgCustomer.DataSource = cnn.ListCustomer();
            dgCustomer.Refresh();
        }

        private void cbFilterCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterCustomer.SelectedValue != null && cbFilterCustomer.SelectedValue.ToString() == "V")
            {
                dgCustomer.DataSource = cnn.ListVIPCustomer();
                dgCustomer.Refresh();

                AddEditAndDeleteButtons();
            }
            else if (cbFilterCustomer.SelectedValue != null && cbFilterCustomer.SelectedValue.ToString() == "T")
            {
                dgCustomer.DataSource = cnn.ListNormalCustomer();
                dgCustomer.Refresh();

                AddEditAndDeleteButtons();
            }
        }
    }
}
