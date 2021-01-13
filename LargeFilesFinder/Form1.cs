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
		public Form1() {
			InitializeComponent();

			if(File.Exists(Directory.GetCurrentDirectory() + "/filePath.txt")) {
				textBox2.Text = File.ReadAllText(Directory.GetCurrentDirectory() + "/filePath.txt");
			}
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private async void button1_Click(object sender, EventArgs e) {
			dataGridView1.Rows.Clear();

			if(!Directory.Exists(textBox2.Text)) {
				dataGridView1.Rows.Add(new string[] { "Full path is not a valid directory", "" });
				return;
			}

			File.WriteAllText(Directory.GetCurrentDirectory() + "/filePath.txt", textBox2.Text);

			string[] allFiles = Directory.GetFiles(textBox2.Text, "*", SearchOption.AllDirectories);

			int fileCount = allFiles.Length;

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

			List<string[]> rowsToAdd = new List<string[]>();

			await Task.Run(() => {
				for(int i = 0; i < fileCount; i++) {
					FileInfo fileInfo = new FileInfo(allFiles[i]);

					if(fileInfo.Length > threshold) {
						rowsToAdd.Add(new string[] { allFiles[i], BytesToString(fileInfo.Length) });
					}
				}
			});

			foreach(string[] rowToAdd in rowsToAdd) {
				dataGridView1.Rows.Add(rowToAdd);
			}
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
	}
}
