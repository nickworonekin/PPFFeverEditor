using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace PPFFeverEditor
{
    public static class ImportExport
    {
        public static void Import(Chain[][][][] chains, int set, int chain)
        {
            byte[] data = null;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Fever Data|*.dat|All Files|*.*";
                ofd.Title = "Import Fever Data";
                ofd.AddExtension = true;
                ofd.CheckFileExists = true;
                ofd.RestoreDirectory = true;
                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Make sure filesize is correct
                    long size = new FileInfo(ofd.FileName).Length;
                    if (size != 2706 && size != 8781 && size != 11706 && size != 31206)
                    {
                        MessageBox.Show("This is not a valid fever data file.\n(File Size is incorrect.)", "Import Unsuccessful");
                        return;
                    }

                    // Let's read the data in now
                    data = File.ReadAllBytes(ofd.FileName);
                    if (!Compare(data, new byte[] { 0x50, 0x50, 0x46, 0x46, 0x45 }, 0) || data[5] < 0x01 || data[5] > 0x04)
                    {
                        MessageBox.Show("This is not a valid fever data file.\n(Header is incorrect.)", "Import Unsuccessful");
                        return;
                    }

                    int pos = 6; // Set start position (which is always 6)

                    // Ok, figure out what we want to do
                    if (data[5] == 0x01) // Chains in set only
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 13; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    for (int y = 0; y < 12; y++)
                                    {
                                        for (int x = 0; x < 6; x++)
                                        {
                                            chains[set][i][j][k].Grid[x, y] = (Puyo)data[pos++];
                                        }
                                    }

                                    chains[set][i][j][k].TriggerColor = (Puyo)data[pos++];
                                    chains[set][i][j][k].ArrowPosition.X = data[pos++];
                                    chains[set][i][j][k].ArrowPosition.Y = data[pos++];
                                }
                            }
                        }
                    }

                    else if (data[5] == 0x02) // n Chains Only
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    for (int y = 0; y < 12; y++)
                                    {
                                        for (int x = 0; x < 6; x++)
                                        {
                                            chains[i][j][chain][k].Grid[x, y] = (Puyo)data[pos++];
                                        }
                                    }

                                    chains[i][j][chain][k].TriggerColor = (Puyo)data[pos++];
                                    chains[i][j][chain][k].ArrowPosition.X = data[pos++];
                                    chains[i][j][chain][k].ArrowPosition.Y = data[pos++];
                                }
                            }
                        }
                    }
                    else if (data[5] == 0x03) // Group #
                    {
                        // Display a dialog so we can select the correct group
                        Form form = new Form();
                        form.Text = "Select a Group";
                        form.MinimizeBox = false;
                        form.MaximizeBox = false;
                        form.ClientSize = new Size(400, 70);
                        form.StartPosition = FormStartPosition.CenterScreen;
                        form.FormBorderStyle = FormBorderStyle.FixedDialog;
                        form.ShowIcon = false;
                        form.ShowInTaskbar = false;

                        Label messageLabel = new Label();
                        messageLabel.Location = new Point(10, 10);
                        messageLabel.Size = new Size(form.ClientSize.Width - 20, 16);
                        messageLabel.TextAlign = ContentAlignment.MiddleLeft;
                        messageLabel.Text = "Select the group you want to replace.";
                        form.Controls.Add(messageLabel);

                        ComboBox groupSelector = new ComboBox();
                        groupSelector.DropDownStyle = ComboBoxStyle.DropDownList;
                        groupSelector.Location = new Point(10, 38);
                        groupSelector.Size = new Size(((form.ClientSize.Width - 20) / 2) - 10, 24);
                        groupSelector.Items.AddRange(new string[] { "Group #1", "Group #2", "Group #3", "Group #4" });
                        groupSelector.MaxDropDownItems = 4;
                        groupSelector.SelectedIndex = 0;
                        form.Controls.Add(groupSelector);

                        Button confirmButton = new Button();
                        confirmButton.UseVisualStyleBackColor = true;
                        confirmButton.Text = "Select";
                        confirmButton.Location = new Point(((form.ClientSize.Width - 20) / 2) + 10, 36);
                        confirmButton.Size = new Size(((form.ClientSize.Width - 20) / 2) - 10, 24);
                        confirmButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                        confirmButton.Click += delegate(object sender, EventArgs e)
                        {
                            form.Close();

                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int k = 0; k < 13; k++)
                                    {
                                        for (int y = 0; y < 12; y++)
                                        {
                                            for (int x = 0; x < 6; x++)
                                            {
                                                chains[i][j][k][groupSelector.SelectedIndex].Grid[x, y] = (Puyo)data[pos++];
                                            }
                                        }

                                        chains[i][j][k][groupSelector.SelectedIndex].TriggerColor = (Puyo)data[pos++];
                                        chains[i][j][k][groupSelector.SelectedIndex].ArrowPosition.X = data[pos++];
                                        chains[i][j][k][groupSelector.SelectedIndex].ArrowPosition.Y = data[pos++];
                                    }
                                }
                            }
                        };
                        form.Controls.Add(confirmButton);

                        form.ShowDialog();
                    }
                    else if (data[5] == 0x04) // All Data
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (i == 2 && j == 2) // RunRun 5 colors does not exist
                                    break;

                                for (int k = 0; k < 13; k++)
                                {
                                    for (int m = 0; m < 4; m++)
                                    {
                                        for (int y = 0; y < 12; y++)
                                        {
                                            for (int x = 0; x < 6; x++)
                                                chains[i][j][k][m].Grid[x, y] = (Puyo)data[pos++];
                                        }

                                        chains[i][j][k][m].TriggerColor = (Puyo)data[pos++];
                                        chains[i][j][k][m].ArrowPosition.X = data[pos++];
                                        chains[i][j][k][m].ArrowPosition.Y = data[pos++];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Export data (this version only exports complete voice pattern data)
        public static void Export(Chain[][][][] chains, int set, int chain)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Fever Data - Chains in Set Only|*.dat|Fever Data - " + (chain + 3) + " Chains Only|*.dat|Fever Data - Group #1|*.dat|Fever Data - Group #2|*.dat|Fever Data - Group #3|*.dat|Fever Data - Group #4|*.dat|Fever Data - All Data|*.dat";
                sfd.Title = "Export Fever Data";
                sfd.AddExtension = true;
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = true;

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream outstream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                        {
                            byte[] header;
                            switch (sfd.FilterIndex)
                            {
                                case 2: header = new byte[] { 0x50, 0x50, 0x46, 0x46, 0x45, 0x02 }; break; // n Chains Only
                                case 3: case 4: case 5: case 6:
                                    header = new byte[] { 0x50, 0x50, 0x46, 0x46, 0x45, 0x03 }; break; // Group #
                                case 7: header = new byte[] { 0x50, 0x50, 0x46, 0x46, 0x45, 0x04 }; break; // All Data
                                default: header = new byte[] { 0x50, 0x50, 0x46, 0x46, 0x45, 0x01 }; break; // Chains in set only
                            }

                            outstream.Write(header, 0, header.Length);

                            if (sfd.FilterIndex == 1) // Chains in set only
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 13; j++)
                                    {
                                        for (int k = 0; k < 4; k++)
                                        {
                                            for (int y = 0; y < 12; y++)
                                            {
                                                for (int x = 0; x < 6; x++)
                                                {
                                                    outstream.WriteByte((byte)chains[set][i][j][k].Grid[x, y]);
                                                }
                                            }

                                            outstream.WriteByte((byte)chains[set][i][j][k].TriggerColor);
                                            outstream.WriteByte((byte)chains[set][i][j][k].ArrowPosition.X);
                                            outstream.WriteByte((byte)chains[set][i][j][k].ArrowPosition.Y);
                                        }
                                    }
                                }
                            }
                            else if (sfd.FilterIndex == 2) // n Chains Only
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        for (int k = 0; k < 4; k++)
                                        {
                                            for (int y = 0; y < 12; y++)
                                            {
                                                for (int x = 0; x < 6; x++)
                                                {
                                                    outstream.WriteByte((byte)chains[i][j][chain][k].Grid[x, y]);
                                                }
                                            }

                                            outstream.WriteByte((byte)chains[i][j][chain][k].TriggerColor);
                                            outstream.WriteByte((byte)chains[i][j][chain][k].ArrowPosition.X);
                                            outstream.WriteByte((byte)chains[i][j][chain][k].ArrowPosition.Y);
                                        }
                                    }
                                }
                            }
                            else if (sfd.FilterIndex == 3 || sfd.FilterIndex == 4 || sfd.FilterIndex == 5 || sfd.FilterIndex == 6) // Group #
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        for (int k = 0; k < 13; k++)
                                        {
                                            for (int y = 0; y < 12; y++)
                                            {
                                                for (int x = 0; x < 6; x++)
                                                {
                                                    outstream.WriteByte((byte)chains[i][j][k][(sfd.FilterIndex - 3)].Grid[x, y]);
                                                }
                                            }

                                            outstream.WriteByte((byte)chains[i][j][k][(sfd.FilterIndex - 3)].TriggerColor);
                                            outstream.WriteByte((byte)chains[i][j][k][(sfd.FilterIndex - 3)].ArrowPosition.X);
                                            outstream.WriteByte((byte)chains[i][j][k][(sfd.FilterIndex - 3)].ArrowPosition.Y);
                                        }
                                    }
                                }
                            }
                            else if (sfd.FilterIndex == 7) // All Data
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        if (i == 2 && j == 2) // RunRun 5 colors does not exist
                                            break;

                                        for (int k = 0; k < 13; k++)
                                        {
                                            for (int m = 0; m < 4; m++)
                                            {
                                                for (int y = 0; y < 12; y++)
                                                {
                                                    for (int x = 0; x < 6; x++)
                                                        outstream.WriteByte((byte)chains[i][j][k][m].Grid[x, y]);
                                                }

                                                outstream.WriteByte((byte)chains[i][j][k][m].TriggerColor);
                                                outstream.WriteByte((byte)chains[i][j][k][m].ArrowPosition.X);
                                                outstream.WriteByte((byte)chains[i][j][k][m].ArrowPosition.Y);
                                            }
                                        }
                                    }
                                }
                            }

                            outstream.Close();
                        }

                        MessageBox.Show("Fever data exported successfully.", "Export Successful");
                    }
                    catch
                    {
                        MessageBox.Show("An error occured when writing the fever data.", "Export Unsuccessful");
                    }
                }
            }
        }

        // Compares an array to another array
        private static bool Compare(byte[] a1, byte[] a2, int startIndex)
        {
            for (int i = 0; i < a2.Length; i++)
            {
                if (a1[startIndex + i] != a2[i])
                    return false;
            }

            return true;
        }
    }
}