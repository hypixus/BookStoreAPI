using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookStoreAPI;
using Newtonsoft.Json;

namespace BookStoreClient
{
    public partial class BookStoreForm : Form
    {
        private readonly string _email;

        private readonly string _password;

        //private HttpClient _httpClient = new();

        private BookStoreClass apiClient = new("https://localhost:7208/", new HttpClient());

        public BookStoreForm(string email, string password)
        {
            _email = email;
            _password = password;
            InitializeComponent();
        }

        private void BookStoreForm_Load(object sender, EventArgs e)
        {

        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var convertedPrice))
                    throw new Exception("Incorrect price format.");
                var result = await apiClient.BookCreateAsync(_email, _password, 1, NameTextBox.Text, AuthorTextBox.Text,
                    DescriptionTextBox.Text, ".", convertedPrice);
                if (result == null) throw new Exception("Received empty response.");
                textBox1.Text = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ReadButton_Click(object sender, EventArgs e)
        {
            try
            {
                var books = await apiClient.BooksGetAsync(_email, _password);
                if (books == null) throw new Exception("Received empty response.");
                textBox1.Text = JsonConvert.SerializeObject(books);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(IdTextBox.Text, out var id)) throw new Exception("Incorrect id format.");
                var bookToUpdate = await apiClient.BookDetailsGetAsync(id, _email, _password);
                if (bookToUpdate == null) throw new Exception("No book was found to update.");
                if (!double.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var convertedPrice))
                    throw new Exception("Incorrect price format.");
                var result = await apiClient.BookUpdateAsync(id, NameTextBox.Text, AuthorTextBox.Text,
                    DescriptionTextBox.Text, ".", convertedPrice, _email, _password);
                if (result == null) throw new Exception("Received empty response.");
                textBox1.Text = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(IdTextBox.Text, out var id)) throw new Exception("Incorrect id format.");
                var result = await apiClient.BookDeleteAsync(id, ".", ".", ".", ".", 0.0, _email, _password);
                if (result == null) throw new Exception("Received empty response.");
                textBox1.Text = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
