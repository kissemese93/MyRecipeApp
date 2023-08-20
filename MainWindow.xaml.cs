using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MyRecipeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string rNameDefault = "name";
        string rDescriptionDefault = "description";

        // Keep track of whether the text boxes have been edited by the user
        private bool rNameEdited = false;
        private bool rDescEdited = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTextBox(rNameBox, rNameDefault);
            InitializeTextBox(rDescBox, rDescriptionDefault);
            lBox.SelectionChanged += ListBox_SelectionChanged;

            LoadRecipesFromJson("C:\\Users\\kisse\\Documents\\recipes.json"); // Load recipes from JSON file
        }

        public void LoadRecipesFromJson(string filePath)
        {
            string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullFilePath = System.IO.Path.Combine(documentsFolderPath, "recipes.json");

            if (File.Exists(fullFilePath))
            {
                try
                {
                    string json = File.ReadAllText(fullFilePath);
                    List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(json);

                    if (recipes != null)
                    {
                        foreach (Recipe recipe in recipes)
                        {
                            lBox.Items.Add(recipe);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading recipes from JSON: " + ex.Message);
                }
            }
            else
            {
                CreateInitialJsonFile(fullFilePath);
            }
        }

        public void CreateInitialJsonFile(string filePath)
        {
            List<Recipe> initialRecipes = new List<Recipe>();
            string initialJson = JsonConvert.SerializeObject(initialRecipes, Formatting.Indented);

            try
            {
                File.WriteAllText(filePath, initialJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating the initial JSON file: " + ex.Message);
            }
        }

        public void Add_Click(object sender, RoutedEventArgs e)
        {
            string name = rNameBox.Text;
            string desc = rDescBox.Text;
            Recipe recipe = new Recipe(name, desc);
            lBox.Items.Add(recipe);

        }
        public void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (lBox.SelectedItem != null)
            {
                Recipe selectedRecipe = (Recipe)lBox.SelectedItem;
                string newName = rNameBox.Text;
                string newDesc = rDescBox.Text;

                // Update the selected item's properties with new values
                selectedRecipe.Name = newName;
                selectedRecipe.Description = newDesc;

                // Refresh the ListBox to reflect the changes
                lBox.Items.Refresh();
            }
        }
        public void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lBox.SelectedItem != null)
            {
                lBox.Items.Remove(lBox.SelectedItem);
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (lBox.SelectedItem != null)
            {
                // Cast the selected item to the Recipe class (assuming Recipe is your item type)
                Recipe selectedRecipe = (Recipe)lBox.SelectedItem;

                // Set the TextBoxes with the name and description of the selected item
                rNameBox.Text = selectedRecipe.Name;
                rDescBox.Text = selectedRecipe.Description;
            }
        }
        public void SaveRecipesToJson(string filePath)
        {
            List<Recipe> recipes = new List<Recipe>();

            // Add all recipes from the ListBox to the list
            foreach (Recipe recipe in lBox.Items)
            {
                recipes.Add(recipe);
            }

            // Serialize the list of recipes to JSON and write to the file
            string json = JsonConvert.SerializeObject(recipes, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveRecipesToJson(saveFileDialog.FileName);
            }
        }
        //Textbox behaviour
        private void InitializeTextBox(TextBox textBox, string defaultText)
        {
            textBox.Text = defaultText;
            textBox.Foreground = Brushes.Gray;
            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!IsTextBoxEdited(textBox))
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!IsTextBoxEdited(textBox) && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = GetDefaultText(textBox);
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox == rNameBox)
            {
                rNameEdited = true;
            }
            else if (textBox == rDescBox)
            {
                rDescEdited = true;
            }
        }

        private bool IsTextBoxEdited(TextBox textBox)
        {
            if (textBox == rNameBox)
            {
                return rNameEdited;
            }
            else if (textBox == rDescBox)
            {
                return rDescEdited;
            }

            return false;
        }

        private string GetDefaultText(TextBox textBox)
        {
            if (textBox == rNameBox)
            {
                return rNameDefault;
            }
            else if (textBox == rDescBox)
            {
                return rDescriptionDefault;
            }

            return "";
        }

    }
}
