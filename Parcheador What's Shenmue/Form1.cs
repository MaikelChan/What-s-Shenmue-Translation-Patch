using System;
using System.Reflection;
using System.Windows.Forms;
using Windows7.DesktopIntegration;

namespace Parcheador_Whats_Shenmue
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}

		bool Patching = false;

		private void Form1_Load(object sender, EventArgs e)
		{
			comboBox1.SelectedIndex = 0;
			comboBox2.SelectedIndex = 0;

			toolStripStatusLabel1.Text = Localization.StatusWaiting;
			toolStripProgressBar1.Maximum = PatchInfo.ProgressBarMaxValue;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog ODlg = new OpenFileDialog();
			ODlg.FileName = "";
			ODlg.Filter = Localization.MainFormODlgFilter;
			if (ODlg.ShowDialog() != DialogResult.OK) return;

			textBox1.Text = ODlg.FileName;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			EnableControls(false);
			Patching = true;

			Globals.InitGlobals();

			string[] GDIFiles = { textBox1.Text };
			Patcher _Patcher = new Patcher(GDIFiles, GameInfo.NumberOfDiscs, (Patcher.ImageFormat)comboBox2.SelectedIndex);
			_Patcher.StatusChanged += ChangeStatus;
			_Patcher.ProcessFinished += FinishProcess;
			_Patcher.OutputMessage += ShowMessage;

			_Patcher.Start();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			About AboutForm = new About();
			AboutForm.ShowDialog();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Localization.LoadLanguage((Localization.Languages)comboBox1.SelectedIndex);
			UpdateFormTexts();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Patching)
			{
				MessageBox.Show(Localization.MsgBoxBusy, Localization.MsgBoxWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Cancel = true;
				return;
			}

#if !DEBUG
			if (Directory.Exists(Globals.TempPath)) Directory.Delete(Globals.TempPath, true);
#endif
		}

		void FinishProcess(bool ProcessFinishedSuccessfully)
		{
			EnableControls(true);
			Patching = false;
			ChangeStatus(Localization.StatusWaiting, PatchInfo.ProgressBarMaxValue);
			if (ProcessFinishedSuccessfully)
			{
				MessageBox.Show(Localization.MsgBoxSuccess, Localization.MsgBoxInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
				MessageBox.Show(Localization.MsgBoxFinalInfo, Localization.MsgBoxWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		void ShowMessage(string Message, Patcher.MessageType MessageType)
		{
			switch (MessageType)
			{
				case Patcher.MessageType.Information:
					MessageBox.Show(Message, Localization.MsgBoxInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;
				case Patcher.MessageType.Warning:
					MessageBox.Show(Message, Localization.MsgBoxWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					break;
				case Patcher.MessageType.Error:
					MessageBox.Show(Message, Localization.MsgBoxError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
			}
		}

		void ChangeStatus(string Status, int ProgressValue)
		{
			statusStrip1.Invoke(new Action(() =>
				{
					toolStripProgressBar1.Value = ProgressValue;
					toolStripStatusLabel1.Text = Status;
				}));

			if (Utils.IsWindows7OrSuperior())
			{
				this.Invoke(new Action(() =>
					{
						if (ProgressValue > 0)
							Windows7Taskbar.SetProgressState(Handle, Windows7Taskbar.ThumbnailProgressState.Normal);
						else
							Windows7Taskbar.SetProgressState(Handle, Windows7Taskbar.ThumbnailProgressState.NoProgress);

						Windows7Taskbar.SetProgressValue(Handle, (ulong)ProgressValue, (ulong)PatchInfo.ProgressBarMaxValue);
					}));
			}
		}

		void EnableControls(bool e)
		{
			textBox1.Invoke(new Action(() => { textBox1.Enabled = e; }));
			button1.Invoke(new Action(() => { button1.Enabled = e; }));
			button2.Invoke(new Action(() => { button2.Enabled = e; }));
			comboBox1.Invoke(new Action(() => { comboBox1.Enabled = e; }));
			comboBox2.Invoke(new Action(() => { comboBox2.Enabled = e; }));
		}

		void UpdateFormTexts()
		{
			Version Vers = Assembly.GetEntryAssembly().GetName().Version;
			Text = String.Format(Localization.MainFormTitle, GameInfo.GameName, Vers.Major, Vers.Minor, Vers.Build);
			button2.Text = Localization.MainFormBrowse;
			button1.Text = Localization.MainFormPatch;
			button3.Text = Localization.MainFormAbout;
			toolStripStatusLabel1.Text = Localization.StatusWaiting;

			groupBox1.Text = Localization.MainFormOptions;
			label2.Text = Localization.MainFormLanguage;
			label3.Text = Localization.MainFormOutputFormat;

			int SelectedIndex = comboBox2.SelectedIndex;
			comboBox2.Items.Clear();
			comboBox2.Items.Add(Localization.MainFormOutputFormatGDI);
			comboBox2.Items.Add(Localization.MainFormOutputFormatCDI);
			comboBox2.SelectedIndex = SelectedIndex;
		}
	}
}