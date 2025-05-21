using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace lost_and_found_system
{
    public partial class AddUser : Form
    {
        private string connectionString =
            "server=localhost;user id=root;password=jerald;database=lostandfoundsystem;";

        public AddUser()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadDataUsers();

            // live‐search (if you prefer typing)
            btnSearch.TextChanged += (s, e) => DoSearch(btnSearch.Text.Trim());
            // or manual button

        }

        private void InitializeDataGridView()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        // ─── Add User ─────────────────────────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            // validation omitted for brevity...

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand("sp_AddUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_name", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("p_email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("p_password", txtPassword.Text);
                cmd.Parameters.AddWithValue("p_phone", txtPhone.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("User added successfully.");
                LoadDataUsers();
                ClearInputs();
            }
        }

        // ─── Update User ──────────────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int userId))
            {
                MessageBox.Show("Select a valid user first.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand("sp_UpdateUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_user_id", userId);
                cmd.Parameters.AddWithValue("p_name", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("p_email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("p_phone", txtPhone.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("User updated successfully.");
                LoadDataUsers();
                ClearInputs();
            }
        }

        // ─── Delete User ──────────────────────────────────────────────────────
        private void button3_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int userId))
            {
                MessageBox.Show("Select a valid user first.");
                return;
            }
            if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand("sp_DeleteUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_user_id", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("User deleted successfully.");
                LoadDataUsers();
                ClearInputs();
            }
        }

        // ─── Reload All Users ─────────────────────────────────────────────────
        private void button4_Click(object sender, EventArgs e)
            => LoadDataUsers();

        private void LoadDataUsers()
        {
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand("sp_GetAllUsers", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // ─── Populate Fields On Row‐Click ─────────────────────────────────────
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var id = dataGridView1.Rows[e.RowIndex].Cells["user_id"].Value;
            if (!int.TryParse(id?.ToString(), out int userId)) return;

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand("sp_GetUserById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_user_id", userId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtId.Text = reader["user_id"].ToString();
                        txtFullName.Text = reader["name"].ToString();
                        txtEmail.Text = reader["email"].ToString();
                        txtPhone.Text = reader["phone"].ToString();
                    }
                }
            }
        }

        // ─── Search (inline) ───────────────────────────────────────────────────
        private void DoSearch(string keyword)
        {
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(
                "SELECT * FROM users WHERE name LIKE @keyword OR email LIKE @keyword", conn))
            {
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }


     

        // ─── Navigate Back ────────────────────────────────────────────────────
        private void button5_Click(object sender, EventArgs e)
        {
            new DashboardForm().Show();
            Hide();
        }

        // ─── Export To Excel ──────────────────────────────────────────────────
        private void button10_Click_1(object sender, EventArgs e)
        {
            // (your ClosedXML export code remains unchanged)
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtPhone.Clear();
        }


        private void textSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch(textSearch.Text.Trim());
        }

        // optional stubs left out for brevity...
    }
}
