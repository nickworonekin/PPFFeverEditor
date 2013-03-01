using System;
using System.Drawing;
using System.Windows.Forms;
using wyDay.Controls;

namespace PPFFeverEditor
{
    public class About : Form
    {
        public About()
        {
            this.Text = "About";
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(400, 240);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            // Program Name
            Panel headerPanel = new Panel();
            headerPanel.Location = new Point(0, 0);
            headerPanel.Size = new Size(this.Width, 48);
            headerPanel.BackColor = SystemColors.Window;
            this.Controls.Add(headerPanel);

            PictureBox programIcon = new PictureBox();
            programIcon.Location = new Point(8, 8);
            programIcon.Size = new Size(32, 32);
            programIcon.Image = Resources.ProgramIcon.ToBitmap();
            headerPanel.Controls.Add(programIcon);

            Label programName = new Label();
            programName.Location = new Point(48, 0);
            programName.Size = new Size(this.Width - 48, 48);
            programName.TextAlign = ContentAlignment.MiddleLeft;
            programName.Font = new Font(SystemFonts.MessageBoxFont.FontFamily.Name, 18f, FontStyle.Bold);
            programName.Text = PPFFeverEditor.ProgramName;
            headerPanel.Controls.Add(programName);

            // Content Panel
            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(8, 56);
            contentPanel.Size = new Size(this.ClientSize.Width - 16, this.ClientSize.Height - 64);
            this.Controls.Add(contentPanel);

            // Version
            Label programVersion = new Label();
            programVersion.Location = new Point(0, 0);
            programVersion.Size = new Size(contentPanel.Width, 16);
            programVersion.TextAlign = ContentAlignment.MiddleLeft;
            programVersion.Text = "Version " + PPFFeverEditor.ProgramVersion;
            contentPanel.Controls.Add(programVersion);

            // Copyright
            Label copyrightLabel = new Label();
            copyrightLabel.Location = new Point(0, 20);
            copyrightLabel.Size = new Size(contentPanel.Width, 16);
            copyrightLabel.TextAlign = ContentAlignment.MiddleLeft;
            copyrightLabel.Text = PPFFeverEditor.ProgramCopyright;
            contentPanel.Controls.Add(copyrightLabel);

            // Open Source
            Label openSourceLabel = new Label();
            openSourceLabel.Location = new Point(0, 40);
            openSourceLabel.Size = new Size(contentPanel.Width, 16);
            openSourceLabel.TextAlign = ContentAlignment.MiddleLeft;
            openSourceLabel.Text = "This program is released as open source software.";
            contentPanel.Controls.Add(openSourceLabel);

            // Message
            Label messageLabel = new Label();
            messageLabel.Location = new Point(0, 80);
            messageLabel.Size = new Size(contentPanel.Width, contentPanel.Height - 16 - messageLabel.Location.Y);
            messageLabel.TextAlign = ContentAlignment.TopLeft;
            messageLabel.Text = "Thanks to Sega for actually releasing a Puyo game on PC. Now if only they could release their newer ones on PC as well.";
            contentPanel.Controls.Add(messageLabel);

            // Puyo Nexus Link
            LinkLabel2 pnLink = new LinkLabel2();
            pnLink.Location = new Point(0, contentPanel.Height - 16);
            pnLink.Size = new Size(contentPanel.Width, 16);
            pnLink.Text = "Puyo Nexus";
            pnLink.Click += delegate(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start("http://www.puyonexus.net");
            };
            contentPanel.Controls.Add(pnLink);

            this.ShowDialog();
        }
    }
}