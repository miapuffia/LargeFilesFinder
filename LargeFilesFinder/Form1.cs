using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LargeFilesFinder {
	public partial class Form1 : Form {
		private struct BackgroundWorkData {
			public int fileCount;
			public int threshold;
			public string[] files;

			public BackgroundWorkData(int fileCount, int threshold, string[] files) {
				this.fileCount = fileCount;
				this.threshold = threshold;
				this.files = files;
			}
		}

		public Form1() {
			InitializeComponent();

			if(File.Exists(Directory.GetCurrentDirectory() + "/filePath.txt")) {
				textBox2.Text = File.ReadAllText(Directory.GetCurrentDirectory() + "/filePath.txt");
			}
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private void button1_Click(object sender, EventArgs e) {
			textProgressBar1.Value = 0;
			dataGridView1.Rows.Clear();

			label3.Visible = true;

			if(!Directory.Exists(textBox2.Text)) {
				dataGridView1.Rows.Add(new string[] { "Full path is not a valid directory", "" });
				return;
			}

			File.WriteAllText(Directory.GetCurrentDirectory() + "/filePath.txt", textBox2.Text);

			string[] files = Directory.GetFiles(textBox2.Text, "*", SearchOption.AllDirectories);

			int fileCount = files.Length;

			textProgressBar1.Maximum = fileCount;

			int threshold = textBox1.Text == "" ? 0 : int.Parse(textBox1.Text);

			if(threshold == 0) {
				threshold += radioButton2.Checked ? 1000 : 0;
				threshold += radioButton3.Checked ? 1000000 : 0;
				threshold += radioButton4.Checked ? 1000000000 : 0;
			}  else {
				threshold *= radioButton2.Checked ? 1000 : 1;
				threshold *= radioButton3.Checked ? 1000000 : 1;
				threshold *= radioButton4.Checked ? 1000000000 : 1;
			}

			backgroundWorker1.RunWorkerAsync(new BackgroundWorkData(fileCount, threshold, files));
		}

		static String BytesToString(long byteCount) {
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if(byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}

		private void panel1_Scroll(object sender, ScrollEventArgs e) {
			Console.WriteLine("hi");
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
			BackgroundWorkData data = (BackgroundWorkData) e.Argument;
			List<string[]> rowsToAdd = new List<string[]>();

			int i;

			for(i = 0; i < data.fileCount; i++) {
				FileInfo fileInfo = new FileInfo(data.files[i]);

				if(fileInfo.Length > data.threshold) {
					rowsToAdd.Add(new string[] { data.files[i], BytesToString(fileInfo.Length) });
				}

				backgroundWorker1.ReportProgress(i);
			}

			backgroundWorker1.ReportProgress(i, rowsToAdd);
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			textProgressBar1.Value = e.ProgressPercentage;

			if(e.UserState != null) {
				List<string[]> rowsToAdd = (List<string[]>) e.UserState;

				foreach(string[] rowToAdd in rowsToAdd) {
					dataGridView1.Rows.Add(rowToAdd);
				}
			}
		}

		private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
			label3.Visible = false;
		}
	}
}
