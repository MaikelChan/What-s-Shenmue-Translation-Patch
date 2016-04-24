using System;
using System.Drawing;
using System.Windows.Forms;
using WSPatcher.Properties;

namespace WSPatcher
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void About_Load(object sender, EventArgs e)
		{
			switch (Localization.SelectedLanguage)
			{
				case Localization.Languages.English:
					BackgroundImage = Resources.credits_en;
					ClientSize = Resources.credits_en.Size;
					break;
				case Localization.Languages.Spanish:
					BackgroundImage = Resources.credits_es;
					ClientSize = Resources.credits_es.Size;
					break;
				case Localization.Languages.German:
					BackgroundImage = Resources.credits_de;
					ClientSize = Resources.credits_de.Size;
					break;
				case Localization.Languages.Portuguese:
					BackgroundImage = Resources.credits_ptBR;
					ClientSize = Resources.credits_ptBR.Size;
					break;
			}

			button1.Text = Localization.AboutFormOK;
			Text = Localization.MainFormAbout;
		}
	}
}