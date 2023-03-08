using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using Microsoft.Win32;


namespace lab4fourdatabasekeyidstudymegapuperlab4
{
    public partial class MainWindow : Window
    {
        private string _dataBase_name;
        SQLiteConnection _sqlConnection;

        private const string Table1Name = "stud", Table2Name = "marks", Physics = "physics", Math = "math", Name = "name", ID = "id";

        private List<Student> _students = new List<Student>();

        public MainWindow()
        {
            InitializeComponent();
            ConnectDataBase();
            UpdateList();
        }

        private void ConnectDataBase()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Data Base File (*.db)|*.db|All files (*.*)|*.*";
            dlg.ShowDialog();
            _dataBase_name = dlg.FileName;
            _sqlConnection = new SQLiteConnection("Data Source=" + _dataBase_name);
            _sqlConnection.Open();
        }

        private void tb1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Delete_click(object sender, RoutedEventArgs e)
        {
            string sql = $"DELETE FROM {Table1Name} WHERE id = {list.SelectedIndex + 1}";
            SQLiteCommand command = new SQLiteCommand(sql, _sqlConnection);
            command.ExecuteNonQuery();

            sql = $"DELETE FROM {Table2Name} WHERE id = {_students[list.SelectedIndex].ID}";
            command = new SQLiteCommand(sql, _sqlConnection);
            command.ExecuteNonQuery();

            UpdateList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name;
            int math, physics;

            name = tb1.Text;
            math = int.Parse(((ComboBoxItem)cb1.SelectedItem).Content.ToString());
            physics = int.Parse(((ComboBoxItem)cb2.SelectedItem).Content.ToString());

            int index = 0;
            bool isNew = true;
            for (int i = 0; i < _students.Count; i++)
            {
                if (_students[i].Name == tb1.Text)
                {
                    index = list.SelectedIndex;
                    isNew = false;
                    break;
                }
            }

            string sql;
            SQLiteCommand command;

            if (isNew)
            {
                sql = $"INSERT INTO {Table1Name} ({Name}) VALUES ('" + name + "')";
                command = new SQLiteCommand(sql, _sqlConnection);
                command.ExecuteNonQuery();

                sql = $"INSERT INTO {Table2Name} ({Math}, {Physics}) VALUES ('" + math + "', '" + physics + "')";
                command = new SQLiteCommand(sql, _sqlConnection);
                command.ExecuteNonQuery();
            }
            else
            {
                sql = $"UPDATE {Table2Name} SET {Physics} = {physics}, {Math} = {math} WHERE {Table2Name}.id = {_students[list.SelectedIndex].ID}";
                command = new SQLiteCommand(sql, _sqlConnection);
                command.ExecuteNonQuery();
            }

            UpdateList();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex == -1) return;
            tb1.Text = _students[list.SelectedIndex].Name;
            cb1.Text = _students[list.SelectedIndex].Math.ToString();
            cb2.Text = _students[list.SelectedIndex].Physics.ToString();
        }

        private void UpdateList()
        {
            string sql = $"SELECT * FROM {Table1Name}, {Table2Name} WHERE {Table1Name}.id = {Table2Name}.id";

            SQLiteCommand command = new SQLiteCommand(sql, _sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            list.Items.Clear();
            _students.Clear();

            while (reader.Read())
            {
                list.Items.Add($"{reader[Name]} : Математика: {reader[Math]}, Физика: {reader[Physics]}");
                _students.Add(new Student(reader[Name].ToString(), int.Parse(reader[Math].ToString()),
                    int.Parse(reader[Physics].ToString()), int.Parse(reader[ID].ToString())));
            }
        }

        public class Student
        {
            public string Name { get; private set; }
            public int Math { get; private set; }
            public int Physics { get; private set; }
            public int ID { get; private set; }

            public Student(string name, int math, int physics, int id)
            {
                Name = name;
                Math = math;
                Physics = physics;
                ID = id;
            }
        }
    }
}
