using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TestXMPP.Properties;
using ubiety;
using ubiety.common;
using ubiety.core;
using ubiety.registries;
using ErrorEventArgs = ubiety.ErrorEventArgs;

namespace TestXMPP
{
	public partial class Main : Form
	{
		private readonly XMPP _xmpp;

		public Main()
		{
			InitializeComponent();
			CompressionRegistry.AddCompression(Assembly.LoadFile(Path.Combine(Application.StartupPath, "ubiety.compression.sharpziplib.dll")));
			Errors.OnError += Errors_OnError;
			_xmpp = new XMPP(); 
			slVersion.Text = Resources.Version_Label + XMPP.Version;
		}

		private static void Errors_OnError(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message, Resources.Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void Button1Click(object sender, EventArgs e)
		{
			UbietySettings.AuthenticationTypes = MechanismType.Default | MechanismType.Plain;
			UbietySettings.Id = new JID(txtUsername.Text);
			UbietySettings.Password = txtPassword.Text;
			UbietySettings.SSL = btnSSL.Checked;
			_xmpp.Connect();
		}

		private void BtnExitClick(object sender, EventArgs e)
		{
			if (_xmpp != null && _xmpp.Connected)
				_xmpp.Disconnect();
			Application.Exit();
		}

		private void BtnSendClick(object sender, EventArgs e)
		{
			UbietyMessages.SendMessage(txtMessage.Text);
		}
	}
}