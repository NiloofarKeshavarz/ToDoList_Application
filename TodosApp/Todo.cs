using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodosApp
{
	public class Todo
	{
		//if you don't have the default constructor ===> Runtime Error
		public Todo() { } //EF always need a default constructor to instantiate my class Todo
		public Todo(string task, int difficulty, DateTime dueDate, StatusEnum status) 
		{ 
			Task = task;
			Difficulty = difficulty;
			DueDate = dueDate;
			Status = status;
			// Status = (StatusEnum)Enum.Parse(typeof(StatusEnum),status); based on the design of your ComboBox
		}

		[Key]
		public int Id { get; set; } // it should be public to be understood with the Entity Framework

		
		private string _task;

		[Required]
		[StringLength(100)]
		public string Task {
			get
			{
				return _task;
			}
			set
			{
				// Regex.IsMatch(task, @"^[a-zA-Z0-9 .,/;'"()+-*!\s])+$"
				if (value.Length <1 || value.Length > 100) {
					throw new ArgumentException("Task should be between 1 -100 characters");
				}
				_task = value;

			} 
		} // 1-100 characters, only letters, digits, space ./,;-+)(*! allowed
		  //"[a-zA-Z0-9 .,/;'"()+-*!]"


		private int _difficulty;

		[Required]
		public int Difficulty {
			get
			{
				return _difficulty;
			}
			set
			{
				if (value < 1|| value > 5)
				{
					throw new ArgumentException("Task difficulty must be between 1- 5");
				}
				_difficulty = value;
			}
		}  // 1-5 only front-end validation

		//[DisplayFormat(ApplyFormatInEditMode = true)] //, DataFormatString = "{yyyy/MM/0:dd}")]
		private DateTime _dueDate;
		[Required]
		[DataType(DataType.Date)]
		public DateTime DueDate {
			get
			{
				return _dueDate;
			}
			set
			{
				if(value.Year < 1900 || value.Year > 2099)
				{
					throw new ArgumentException("invalid year. Must be between 100-2099");
				}
				_dueDate = value;
			}
		}  // 1900-2099 year required, format in GUI is whatever the OS is configured to use

		public enum StatusEnum { Pending = 0, Done = 1, Delegated = 2}

		[Required]
		[EnumDataType(typeof(StatusEnum))]
		public StatusEnum Status { get; set; }

		public override string ToString()
		{
			return $"{Task}, {Difficulty}, {DueDate}, {Status}";
		}


	}
}
