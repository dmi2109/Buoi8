using binding.Models;
using binding.SchoolDBDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace binding
{
    public partial class Form1 : Form
    {
        private SchoolContext context;
        private int currentIndex = -1;
        public Form1()
        {
            InitializeComponent();
        }
       

        private void LoadMajors()
        {
            var majors = new HashSet<string>();
            foreach (DataGridViewRow row in dgvStudent.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    majors.Add(row.Cells[3].Value.ToString());
                }
            }

            cmbMajor.Items.Clear();
            foreach (var major in majors)
            {
                cmbMajor.Items.Add(major);
            }
        }


        private void UpdateStudentInfo(int index)
        {
            if (index >= 0 && index < dgvStudent.Rows.Count)
            {
                var row = dgvStudent.Rows[index];

                // Kiểm tra giá trị ô trước khi truy cập
                txtFullName.Text = row.Cells[1].Value?.ToString() ?? string.Empty;
                txtAge.Text = row.Cells[2].Value?.ToString() ?? string.Empty;
                cmbMajor.SelectedItem = row.Cells[3].Value?.ToString() ?? null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'schoolDBDataSet.Students' table. You can move, or remove it, as needed.
            this.studentsTableAdapter.Fill(this.schoolDBDataSet.Students);

            LoadMajors();

            if (dgvStudent.Rows.Count > 0)
            {
                currentIndex = 0;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Tạo một hàng mới trong DataTable
            var newRow = schoolDBDataSet.Students.NewRow();
            newRow["FullName"] = txtFullName.Text;
            newRow["Age"] = int.Parse(txtAge.Text);
            newRow["Major"] = cmbMajor.SelectedItem.ToString();


            schoolDBDataSet.Students.Rows.Add(newRow);

            studentsTableAdapter.Update(schoolDBDataSet.Students);


            this.studentsTableAdapter.Fill(this.schoolDBDataSet.Students);
        }

        private void LoadCurrentStudent()
        {
            var row = dgvStudent.Rows[currentIndex];
            txtFullName.Text = row.Cells["FullName"].Value.ToString();
            txtAge.Text = row.Cells["Age"].Value.ToString();
            cmbMajor.SelectedItem = row.Cells["Major"].Value.ToString();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow != null)
            {
                var row = (DataRowView)dgvStudent.CurrentRow.DataBoundItem;
                row.Row.Delete();

                studentsTableAdapter.Update(schoolDBDataSet.Students);

                this.studentsTableAdapter.Fill(this.schoolDBDataSet.Students);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow != null)
            {
                var row = dgvStudent.CurrentRow.DataBoundItem as Students;

                if (row != null)
                {
                    row.FullName = txtFullName.Text;
                    row.Age = int.Parse(txtAge.Text);
                    row.Major = cmbMajor.SelectedItem?.ToString();

                    using (var context = new SchoolContext())
                    {
                        context.Entry(row).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    dgvStudent.Refresh();
                    var index = dgvStudent.CurrentRow.Index;
                    dgvStudent.Rows[index].Cells[1].Value = row.FullName;
                    dgvStudent.Rows[index].Cells[2].Value = row.Age;
                    dgvStudent.Rows[index].Cells[3].Value = row.Major;
                }
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentIndex = e.RowIndex;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentIndex < dgvStudent.Rows.Count - 1)
            {
                currentIndex++;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateStudentInfo(currentIndex);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
