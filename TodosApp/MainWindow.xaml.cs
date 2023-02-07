using Microsoft.Win32;
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

namespace TodosApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//private TodoDbContext dbContext; Before making Globals
		List<Todo> todos = new List<Todo>();
		
		public MainWindow()
		{
			InitializeComponent();

			try
			{
				//connect to the database
				Globals.dbContext = new TodoDbContext(); //exception
				LvToDos.ItemsSource = Globals.dbContext.Todos.ToList();// = SELECT * from people
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Connection failed" + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(1); // cutting the head off the chicken
				// OR Close();
			}

		}
		private void ComboStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void BtnUpdate_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Todo selectedTodo = LvToDos.SelectedItem as Todo;
				if (selectedTodo != null)
				{
					string updatingTask = TaskInput.Text;
					int updatingDifficuty = (int)DifficultySlider.Value;
					DateTime updatingDueDate = (DateTime)DueDate.SelectedDate;
					Todo.StatusEnum updatingStatus = (Todo.StatusEnum)StatusComboBox.SelectedIndex;
					using (var db = new TodoDbContext())
					{
						var item = db.Todos.Find(selectedTodo.Id);
						item.Task = updatingTask;
						item.Difficulty= updatingDifficuty;
						item.DueDate = updatingDueDate;
						item.Status = updatingStatus;
						db.SaveChanges();
					}
					selectedTodo.Task = updatingTask;
					selectedTodo.Difficulty= updatingDifficuty;
					selectedTodo.DueDate = updatingDueDate;
					selectedTodo.Status = updatingStatus;
					LvToDos.ItemsSource = Globals.dbContext.Todos.ToList();
					LvToDos.Items.Refresh();
				}
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Error updating from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			Delete();
			/* var r = from d in db.doctors
			 * where d.Id == this.updateing doc
			 * select d;*/
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{


			try
			{
				if(DueDate.SelectedDate == null)
				{
					throw new ArgumentException("Please select a due date");
				}

				string strTask = TaskInput.Text;
				int difficuty = (int)DifficultySlider.Value;
				DateTime dueDate = (DateTime)DueDate.SelectedDate;
				Todo.StatusEnum status = (Todo.StatusEnum)StatusComboBox.SelectedIndex;

				Todo newToDo = new Todo(strTask, difficuty, dueDate, status);
				Globals.dbContext.Todos.Add(newToDo);
				Globals.dbContext.SaveChanges();
				LvToDos.ItemsSource = Globals.dbContext.Todos.ToList();
				ResetFields();
				Console.WriteLine(newToDo);
			}
			catch(ArgumentException ex)
			{
				MessageBox.Show(this, ex.Message, "InputError", MessageBoxButton.OK, MessageBoxImage.Error);
			
			}
			catch (SystemException ex)
			{
				MessageBox.Show(this,"Error reading from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);

			}
		}
		private void ResetFields()
		{
			TaskInput.Text = "";
			DifficultySlider.Value = 1;
			DueDate.SelectedDate = DateTime.Today;
			StatusComboBox.SelectedIndex = 0;
			BtnDelete.IsEnabled = true;
			BtnUpdate.IsEnabled = true;
		}

		private void BtnExport_Click(object sender, RoutedEventArgs e)
		{
			try { 
			// prompts the user for a location to save the file
			SaveFileDialog saveFile = new SaveFileDialog();
			saveFile.Filter = "Text Files (*.txt) | *.txt";
				//CSV Files (*.csv)|*.csv"
				saveFile.FileName = "todo.txt";

			if(saveFile.ShowDialog() == true)
			{
				using(StreamWriter sr = new StreamWriter(saveFile.OpenFile()))
				{
					sr.WriteLine("Task,Difficulty,DueDate, Status");

					foreach(Todo todo in Globals.dbContext.Todos.ToList())
					{
						sr.WriteLine($"{todo.Task}, {todo.Difficulty}, {todo.DueDate}, {todo.Status}");
					}
				}
			}
			}
			catch(SystemException ex)
			{
				MessageBox.Show(this, "Error Exporting file\n" + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LvToDos_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Todo currentTodo = LvToDos.SelectedItem as Todo;
			if (currentTodo == null) {
				ResetFields();
			}
			else
			{
				TaskInput.Text = currentTodo.Task;
				DifficultySlider.Value = currentTodo.Difficulty;
				DueDate.SelectedDate = currentTodo.DueDate;
				StatusComboBox.SelectedIndex = 0;

			}
		}
		
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
			cm.PlacementTarget = sender as Button;
			cm.IsOpen = true;
			Delete();
		}
		public void Delete()
		{
			try
			{
				Todo selectedTodo = LvToDos.SelectedItem as Todo;
				if (selectedTodo != null)
				{
					using (var db = new TodoDbContext())
					{
						db.Todos.Remove(db.Todos.Find(selectedTodo.Id));
						db.SaveChanges();
					}
					todos.Remove(selectedTodo);
					LvToDos.ItemsSource = Globals.dbContext.Todos.ToList();
					LvToDos.Items.Refresh();

				}
			}
			catch (SystemException ex)
			{
				MessageBox.Show(this, "Error deleting from database\n" + ex.Message, "Databae Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		//private void BtnExit_Click(object sender, RoutedEventArgs e)
		//{

		//	// Close the main window
		//	this.Close();

		//	// Exit the application
		//	Application.Current.Shutdown();
		//}
		private void BtnExit_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to exit the application?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				Application.Current.Shutdown();
			}
		}
		private void SortByTask()
		{
			try
			{
				// Sort the data in the ListView by task
				var sortedTodos = Globals.dbContext.Todos.OrderBy(todo => todo.Task).ToList();
				LvToDos.ItemsSource = sortedTodos;
				LvToDos.Items.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error sorting data\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		//sort method by LINQ
		public List<Todo> SortByTask(List<Todo> todos)
		{
			var sortedTodos = todos.OrderBy(t => t.Task).ToList();
			return sortedTodos;
		}
		//public List<Todo> SortByDifficulty(List<Todo> todos)
		//{
		//	var sortedTodos = todos.OrderBy(t => t.Difficulty).ToList();
		//	return sortedTodos;
		//}

		private void BtnSort_Click_1(object sender, RoutedEventArgs e)
		{
			SortByTask();
		}
	}
}
