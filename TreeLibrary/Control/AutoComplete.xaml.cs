using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace ICMS.WPFControlsLibrary
{
	/// <summary>
	/// Autocomplete User control
	/// </summary>
	/// <summary>
	/// AutoComplete
	/// </summary>
	
	public partial class AutoComplete : ComboBox, IDisposable
	{
		/// <summary>
		/// Event for when pattern has been changed
		/// </summary>
		
		
		
		public event AutoComplete.AutoCompleteHandler PatternChanged;

		/// <summary>
		/// The timespan between keypress and pattern changed event
		/// </summary>
		
		
		
		[DefaultValue(200)]
		public int Delay
		{
			get
			{
				return this._delay;
			}
			set
			{
				this._delay = value;
			}
		}

		/// <summary>
		/// The maximum number of records to show in the drop down
		/// </summary>
		
		
		
		public int MaxRecords
		{
			get
			{
				return this._maxRecords;
			}
			set
			{
				this._maxRecords = value;
			}
		}

		/// <summary>
		/// Determines weather textbox does type ahead
		/// </summary>
		
		
		
		public bool TypeAhead
		{
			get
			{
				return this._typeAhead;
			}
			set
			{
				this._typeAhead = value;
			}
		}

		/// <summary>
		/// Gets the text box in charge of the editable portion of the combo box.
		/// </summary>
		
		
		public TextBox EditableTextBox
		{
			get
			{
				if (this.txtBox == null)
				{
					this.txtBox = (base.GetTemplateChild("PART_EditableTextBox") as TextBox);
				}
				return this.txtBox;
			}
		}

		
		
		
		public string NoticeString
		{
			get
			{
				return this.noticeString;
			}
			set
			{
				this.noticeString = value;
				if (this.NoticeLabel != null)
				{
					this.NoticeLabel.Content = value;
				}
			}
		}

		/// <summary>
		/// returns the selected items text representation
		/// </summary>
		
		
		public string SelectedText
		{
			get
			{
				string result;
				if (base.SelectedIndex == -1)
				{
					result = string.Empty;
				}
				else if (base.SelectedItem == null)
				{
					result = string.Empty;
				}
				else if (base.SelectedItem is ComboBoxItem)
				{
					result = Convert.ToString((base.SelectedItem as ComboBoxItem).Content);
				}
				else if (string.IsNullOrEmpty(base.DisplayMemberPath))
				{
					result = base.SelectedItem.ToString();
				}
				else
				{
					object value = base.SelectedItem.GetType().GetProperty(base.DisplayMemberPath).GetValue(base.SelectedItem, null);
					if (value == null)
					{
						result = string.Empty;
					}
					else
					{
						result = value.ToString();
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Specify weather or not to clear the datasource when the text is set to empty string
		/// </summary>
		/// <remarks>When this property is set to true, the patternchanged event will not be fired when the text is empty.</remarks>
		
		
		
		public bool ClearOnEmpty
		{
			get
			{
				return this._clearOnEmpty;
			}
			set
			{
				this._clearOnEmpty = true;
			}
		}

		
		
		
		public bool IsDropDownFromKeyEvent
		{
			get
			{
				return this._isDropDownFromKeyEvent;
			}
			set
			{
				this._isDropDownFromKeyEvent = value;
			}
		}

		
		public AutoComplete()
		{
			this.InitializeComponent();
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				this._interval = new Timer((double)this.Delay);
				this._interval.AutoReset = true;
				this._interval.Elapsed += new ElapsedEventHandler(this._interval_Elapsed);
			}
		}

		
		private void _interval_Elapsed(object sender, ElapsedEventArgs e)
		{
			this._interval.Stop();
			bool isKeyEvent = this.IsKeyEvent;
			this.IsKeyEvent = false;
			if (isKeyEvent)
			{
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					if (this.PatternChanged != null)
					{
						AutoComplete.AutoCompleteArgs autoCompleteArgs = new AutoComplete.AutoCompleteArgs(this.EditableTextBox.Text);
						this.PatternChanged(this, autoCompleteArgs);
						if (!autoCompleteArgs.CancelBinding)
						{
							base.ItemsSource = autoCompleteArgs.DataSource;
						}
					}
				}), DispatcherPriority.ApplicationIdle, new object[0]);
			}
		}

		
		private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
		{
			this.IsKeyEvent = false;
			base.IsDropDownOpen = false;
			if (base.SelectedIndex == -1)
			{
				base.Text = string.Empty;
			}
			else
			{
				base.Text = this.SelectedText;
			}
			this._interval.Close();
			try
			{
				this.EditableTextBox.CaretIndex = 0;
			}
			catch
			{
			}
		}

		
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsKeyEvent = false;
			this._interval.Stop();
		}

		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.EditableTextBox != null)
			{
				this.EditableTextBox.PreviewKeyDown += new KeyEventHandler(this.EditableTextBox_PreviewKeyDown);
				this.EditableTextBox.TextChanged += new TextChangedEventHandler(this.EditableTextBox_TextChanged);
				this.EditableTextBox.GotFocus += new RoutedEventHandler(this.EditableTextBox_GotFocus);
				this.EditableTextBox.LostFocus += new RoutedEventHandler(this.EditableTextBox_LostFocus);
			}
			this.NoticeLabel = (base.GetTemplateChild("LB_Notice") as Label);
			if (this.NoticeLabel != null)
			{
				this.NoticeLabel.Content = this.noticeString;
			}
		}

		
		private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.SelectedText))
			{
				this.NoticeLabel.Visibility = Visibility.Visible;
			}
		}

		
		private void EditableTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.NoticeLabel.Visibility = Visibility.Collapsed;
		}

		
		private void EditableTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Return && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl && e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.LeftShift && e.Key != Key.RightShift && e.Key != Key.Tab)
			{
				if (this.PatternChanged != null)
				{
					base.ItemsSource = null;
				}
				this._isDropDownFromKeyEvent = true;
				base.IsDropDownOpen = true;
				this._isDropDownFromKeyEvent = false;
				this.IsKeyEvent = true;
			}
		}

		
		private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.EditableTextBox != null)
			{
				if (this.ClearOnEmpty && string.IsNullOrEmpty(this.EditableTextBox.Text.Trim()))
				{
					base.ItemsSource = null;
					base.Items.Clear();
				}
				else if (this.IsKeyEvent)
				{
					this.ResetTimer();
				}
			}
		}

		
		protected void ResetTimer()
		{
			this._interval.Stop();
			this._interval.Start();
		}

		
		public void Dispose()
		{
			if (this.txtBox != null)
			{
				this.txtBox.PreviewKeyDown -= new KeyEventHandler(this.EditableTextBox_PreviewKeyDown);
				this.txtBox.TextChanged -= new TextChangedEventHandler(this.EditableTextBox_TextChanged);
			}
			if (this._interval != null)
			{
				this._interval.Elapsed -= new ElapsedEventHandler(this._interval_Elapsed);
				this._interval.Dispose();
			}
		}

		
		private const int DEFAULT_DELAY = 200;

		
		private Timer _interval;

		
		private int _maxRecords = 10;

		
		private bool _typeAhead = false;

		
		private bool IsKeyEvent = false;

		
		private bool _clearOnEmpty = true;

		
		private int _delay = 200;

		
		private TextBox txtBox;

		
		private Label NoticeLabel;

		
		private string noticeString;

		
		private bool _isDropDownFromKeyEvent = false;

		/// <summary>
		/// Used to pass arguments for autocomplete control
		/// </summary>
		
		public class AutoCompleteArgs : EventArgs
		{


			/// <summary>
			/// The current pattern in the auto complete
			/// </summary>
			
			
			public string Pattern
			{
				get
				{
					return this._pattern;
				}
			}

			/// <summary>
			/// Used to the the new datasource for the autocomplete
			/// </summary>
			
			
			
			public IEnumerable DataSource
			{
				get
				{
					return this._dataSource;
				}
				set
				{
					this._dataSource = value;
				}
			}

			/// <summary>
			/// Determines weather or not the datasource should be bounded to the autocomplete control
			/// </summary>
			
			
			
			public bool CancelBinding
			{
				get
				{
					return this._cancelBinding;
				}
				set
				{
					this._cancelBinding = value;
				}
			}

			
			public AutoCompleteArgs(string Pattern)
			{
				this._pattern = Pattern;
			}

			
			private string _pattern;

			
			private IEnumerable _dataSource;

			/// <summary>
			/// default false
			/// </summary>
			
			private bool _cancelBinding = false;
		}

		/// <summary>
		/// event handler for auto complete pattern changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		
		
		public delegate void AutoCompleteHandler(object sender, AutoComplete.AutoCompleteArgs args);
	}
}


