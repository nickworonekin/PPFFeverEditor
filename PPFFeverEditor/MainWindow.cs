using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace PPFFeverEditor
{
    public class MainWindow : Form
    {
        // Variables related to layout.
        TextBox[,] chainBox = new TextBox[3, 4];
        ComboBox[,] triggerColorBox = new ComboBox[3, 4];
        ComboBox setSelector, chainSelector;

        // Game
        GameFile gameFile;

        // Chains
        Chain[][][][] chains; // [3][3][13][4]

        // Field Display
        Panel fieldPanel;
        FieldDisplay fieldDisplay;

        public MainWindow()
        {
            this.ClientSize = new Size(680, 472);
            this.MinimumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = PPFFeverEditor.ProgramName + " v" + PPFFeverEditor.ProgramVersion;
            this.Icon = Resources.ProgramIcon;
            this.Show();
            this.Enabled = false;

            // Display the disclaimer
            Disclaimer.Display();

            // Before we do anything else, we need to load the game file
            gameFile = new GameFile();

            // Create the top header which contains the type selector & chain selector
            Panel topPanel = new Panel();
            topPanel.BackColor = SystemColors.Window;
            topPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(this.ClientSize.Width, 40);
            this.Controls.Add(topPanel);

            setSelector = new ComboBox();
            setSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            setSelector.Location = new Point(10, 10);
            setSelector.Size = new Size(100, 20);
            setSelector.Items.AddRange(new string[] { "Endless", "Normal", "RunRun" });
            setSelector.MaxDropDownItems = 3;
            setSelector.SelectedIndex = 1;
            topPanel.Controls.Add(setSelector);

            chainSelector = new ComboBox();
            chainSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            chainSelector.Location = new Point(120, 10);
            chainSelector.Size = new Size(100, 20);
            chainSelector.Items.AddRange(new string[] {
                "3 Chain", "4 Chain", "5 Chain", "6 Chain", "7 Chain", "8 Chain", "9 Chain",
                "10 Chain", "11 Chain", "12 Chain", "13 Chain", "14 Chain", "15 Chain" });
            chainSelector.MaxDropDownItems = 13;
            chainSelector.SelectedIndex = 0;
            topPanel.Controls.Add(chainSelector);

            setSelector.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                // Enable/Disable the 5 color set if we need to
                if (setSelector.SelectedIndex == 2 && chainBox[2, 0].Enabled)
                {
                    chainBox[2, 0].Enabled = false; triggerColorBox[2, 0].Enabled = false;
                    chainBox[2, 1].Enabled = false; triggerColorBox[2, 1].Enabled = false;
                    chainBox[2, 2].Enabled = false; triggerColorBox[2, 2].Enabled = false;
                    chainBox[2, 3].Enabled = false; triggerColorBox[2, 3].Enabled = false;

                }

                GetChainData(setSelector.SelectedIndex, chainSelector.SelectedIndex);

                // Enable/Disable the 5 color set if we need to
                if (setSelector.SelectedIndex == 2 && !chainBox[2, 0].Enabled)
                {
                    chainBox[2, 0].Text = String.Empty; triggerColorBox[2, 0].SelectedIndex = 0;
                    chainBox[2, 1].Text = String.Empty; triggerColorBox[2, 1].SelectedIndex = 0;
                    chainBox[2, 2].Text = String.Empty; triggerColorBox[2, 2].SelectedIndex = 0;
                    chainBox[2, 3].Text = String.Empty; triggerColorBox[2, 3].SelectedIndex = 0;
                }
                if (setSelector.SelectedIndex != 2 && !chainBox[2, 0].Enabled)
                {
                    chainBox[2, 0].Enabled = true; triggerColorBox[2, 0].Enabled = true;
                    chainBox[2, 1].Enabled = true; triggerColorBox[2, 1].Enabled = true;
                    chainBox[2, 2].Enabled = true; triggerColorBox[2, 2].Enabled = true;
                    chainBox[2, 3].Enabled = true; triggerColorBox[2, 3].Enabled = true;
                }

                fieldDisplay.SetChain(null);
            };
            chainSelector.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                GetChainData(setSelector.SelectedIndex, chainSelector.SelectedIndex);
                fieldDisplay.SetChain(null);
            };

            Button aboutButton = new Button();
            aboutButton.UseVisualStyleBackColor = true;
            aboutButton.Text = "About";
            aboutButton.Location = new Point(this.ClientSize.Width - 74, 8);
            aboutButton.Size = new Size(64, 24);
            aboutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            aboutButton.Click += delegate(object sender, EventArgs e)
            {
                new About();
            };
            topPanel.Controls.Add(aboutButton);

            // Add the content panel
            Panel contentPanel = new Panel();
            contentPanel.BackColor = SystemColors.Window;
            contentPanel.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Location = new Point(8, 48);
            contentPanel.Size = new Size(this.ClientSize.Width - 216, 384);
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(contentPanel);

            // Add the field panel
            fieldPanel = new Panel();
            fieldPanel.Location = new Point(this.ClientSize.Width - 200, 48);
            fieldPanel.Size = new Size(192, 384);
            fieldPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            fieldPanel.BackgroundImage = Resources.fieldBG;
            fieldPanel.BackgroundImageLayout = ImageLayout.Tile;
            this.Controls.Add(fieldPanel);

            fieldDisplay = new FieldDisplay(fieldPanel);

            // Add the chain boxes
            for (int i = 0; i < 3; i++)
            {
                Label chainLabel = new Label();
                chainLabel.Location = new Point(8, 8 + (i * 118));
                chainLabel.Size = new Size(contentPanel.Width - 88, 16);
                chainLabel.Font = new Font(chainLabel.Font, FontStyle.Bold);
                chainLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                chainLabel.Text = (i + 3) + " Colors";
                chainLabel.TextAlign = ContentAlignment.BottomLeft;
                contentPanel.Controls.Add(chainLabel);

                for (int j = 0; j < 4; j++)
                {
                    chainBox[i, j] = new TextBox();
                    chainBox[i, j].Location = new Point(8, 8 + (i * 118) + ((j + 1) * 22));
                    chainBox[i, j].Size = new Size(contentPanel.Width - 88, 16);
                    chainBox[i, j].Name = i + "," + j;
                    chainBox[i, j].Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    chainBox[i, j].GotFocus += ChainBoxGotFocus;
                    chainBox[i, j].TextChanged += ChainBoxTextChanged;
                    contentPanel.Controls.Add(chainBox[i, j]);

                    triggerColorBox[i, j] = new ComboBox();
                    triggerColorBox[i, j].DropDownStyle = ComboBoxStyle.DropDownList;
                    triggerColorBox[i, j].Location = new Point(contentPanel.Width - 72, 8 + (i * 118) + ((j + 1) * 22));
                    triggerColorBox[i, j].Size = new Size(64, 16);
                    triggerColorBox[i, j].Name = i + "," + j;
                    triggerColorBox[i, j].Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    triggerColorBox[i, j].Items.AddRange(new string[] { "Red", "Green", "Blue", "Yellow", "Purple" });
                    triggerColorBox[i, j].MaxDropDownItems = 5;
                    triggerColorBox[i, j].SelectedIndexChanged += TriggerColorBoxSelectedIndexChanged;
                    contentPanel.Controls.Add(triggerColorBox[i, j]);
                }
            }

            // Bottom Panel
            Panel bottomPanel = new Panel();
            bottomPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            bottomPanel.Location = new Point((this.ClientSize.Width / 2) - 140, 440);
            bottomPanel.Size = new Size(280, 24);
            bottomPanel.SizeChanged += delegate(object sender, EventArgs e)
            {
                bottomPanel.Left = (this.ClientSize.Width / 2) - 140;
            };
            this.Controls.Add(bottomPanel);

            // Let's add the buttons at the bottom.
            Button importButton = new Button();
            importButton.Text = "Import";
            importButton.Location = new Point(0, 0);
            importButton.Size = new Size(64, 24);
            bottomPanel.Controls.Add(importButton);
            importButton.Click += ImportData;

            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Location = new Point(72, 0);
            saveButton.Size = new Size(64, 24);
            saveButton.Click += delegate(object sender, EventArgs e)
            {
                SetChainData(setSelector.SelectedIndex, chainSelector.SelectedIndex);
                gameFile.Save();
                MessageBox.Show("Fever data saved successfully.", "Saved");
            };
            bottomPanel.Controls.Add(saveButton);

            Button saveAllButton = new Button();
            saveAllButton.Text = "Save All";
            saveAllButton.Location = new Point(144, 0);
            saveAllButton.Size = new Size(64, 24);
            saveAllButton.Click += delegate(object sender, EventArgs e)
            {
                SetAllChainData();
                gameFile.Save();
                MessageBox.Show("All fever data saved successfully.", "Saved");
            };
            bottomPanel.Controls.Add(saveAllButton);

            Button exportButton = new Button();
            exportButton.Text = "Export";
            exportButton.Location = new Point(216, 0);
            exportButton.Size = new Size(64, 24);
            bottomPanel.Controls.Add(exportButton);
            exportButton.Click += ExportData;

            GetAllChainData();
            GetChainData(setSelector.SelectedIndex, chainSelector.SelectedIndex);

            this.Enabled = true;
        }

        // Gets all the chain data
        private void GetAllChainData()
        {
            chains = new Chain[3][][][]; // # of sets
            for (int i = 0; i < 3; i++)
            {
                chains[i] = new Chain[3][][]; // # of colors
                for (int j = 0; j < 3; j++)
                {
                    chains[i][j] = new Chain[13][]; // # of chains
                    for (int k = 0; k < 13; k++)
                    {
                        chains[i][j][k] = new Chain[4]; // # of types
                        for (int m = 0; m < 4; m++)
                        {
                            chains[i][j][k][m] = new Chain();
                            chains[i][j][k][m].Grid = new Puyo[6, 12];

                            int offset = gameFile.OffsetStart + 0x08 + (i * 0x3A80) + (j * 0x1380) + (k * 0x180) + (m * 0x60);
                            for (int y = 0; y < 12; y++)
                            {
                                for (int x = 0; x < 6; x++)
                                    chains[i][j][k][m].Grid[x, y] = (Puyo)gameFile.Data[offset + (y * 6) + x];

                                chains[i][j][k][m].TriggerColor = (Puyo)gameFile.Data[offset + 72];
                                chains[i][j][k][m].ArrowPosition = new Point(gameFile.Data[offset + 73], gameFile.Data[offset + 74]);
                            }
                        }
                    }
                }
            }
        }

        // Gets the chain data
        private void GetChainData(int set, int chain)
        {
            for (int i = 0; i < 3; i++)
            {
                if (set == 2 && i == 2) // RunRun chains don't have 5 colors
                    break;

                for (int j = 0; j < 4; j++)
                {
                    triggerColorBox[i, j].SelectedIndex = (int)chains[set][i][chain][j].TriggerColor - 1;

                    chainBox[i, j].Text = String.Empty;
                    bool addToString = false;
                    for (int y = 0; y < 12; y++)
                    {
                        for (int x = 0; x < 6; x++)
                        {
                            if (!addToString)
                            {
                                if (chains[set][i][chain][j].Grid[x, y] == Puyo.None)
                                    continue;
                                else
                                    addToString = true;
                            }

                            switch (chains[set][i][chain][j].Grid[x, y])
                            {
                                case Puyo.Red: chainBox[i, j].Text += '4'; break;
                                case Puyo.Green: chainBox[i, j].Text += '7'; break;
                                case Puyo.Blue: chainBox[i, j].Text += '5'; break;
                                case Puyo.Yellow: chainBox[i, j].Text += '6'; break;
                                case Puyo.Purple: chainBox[i, j].Text += '8'; break;
                                case Puyo.Nuisance: chainBox[i, j].Text += '1'; break;
                                default: chainBox[i, j].Text += '0'; break;
                            }
                        }
                    }
                }
            }
        }

        // Sets the chain data
        private void SetChainData(int set, int chain)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int offset = gameFile.OffsetStart + 0x08 + (set * 0x3A80) + (i * 0x1380) + (chain * 0x180) + (j * 0x60);

                    // Set the column heights
                    for (int x = 0; x < 6; x++)
                    {
                        for (int y = 0; y < 12; y++)
                        {
                            if (chains[set][i][chain][j].Grid[x, y] == Puyo.None)
                            {
                                if (y < 11)
                                    continue;
                                else
                                {
                                    gameFile.Data[offset - 0x08 + x] = 0;
                                    break;
                                }
                            }
                            else
                            {
                                gameFile.Data[offset - 0x08 + x] = (byte)(12 - y);
                                break;
                            }
                        }
                    }

                    // Copy the chain data into the game data
                    for (int y = 0; y < 12; y++)
                    {
                        for (int x = 0; x < 6; x++)
                            gameFile.Data[offset + (y * 6) + x] = (byte)chains[set][i][chain][j].Grid[x, y];
                    }

                    gameFile.Data[offset + 72] = (byte)chains[set][i][chain][j].TriggerColor;
                    gameFile.Data[offset + 73] = (byte)chains[set][i][chain][j].ArrowPosition.X;
                    gameFile.Data[offset + 74] = (byte)chains[set][i][chain][j].ArrowPosition.Y;
                }
            }
        }

        // Set all chain data
        private void SetAllChainData()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 13; j++)
                    SetChainData(i, j);
            }
        }

        // Update Chain Data
        private void UpdateChainData(int set, int color, int chain, int type, string str)
        {
            int pos = str.Length - 1;
            for (int y = 11; y >= 0; y--)
            {
                for (int x = 5; x >= 0; x--)
                {
                    if (pos < 0)
                        chains[set][color][chain][type].Grid[x, y] = Puyo.None;
                    else
                    {
                        switch (str[pos])
                        {
                            case '4': chains[set][color][chain][type].Grid[x, y] = Puyo.Red; break;
                            case '7': chains[set][color][chain][type].Grid[x, y] = Puyo.Green; break;
                            case '5': chains[set][color][chain][type].Grid[x, y] = Puyo.Blue; break;
                            case '6': chains[set][color][chain][type].Grid[x, y] = Puyo.Yellow; break;
                            case '8': chains[set][color][chain][type].Grid[x, y] = Puyo.Purple; break;
                            case '1': chains[set][color][chain][type].Grid[x, y] = Puyo.Nuisance; break;
                            default: chains[set][color][chain][type].Grid[x, y] = Puyo.None; break;
                        }

                        pos--;
                    }
                }
            }
        }

        private void ChainBoxGotFocus(object sender, EventArgs e)
        {
            string[] name = (sender as TextBox).Name.Split(',');

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                fieldDisplay.SetChain(chains[setSelector.SelectedIndex][int.Parse(name[0])][chainSelector.SelectedIndex][int.Parse(name[1])]);
            };
            bw.RunWorkerAsync();
        }
        private void ChainBoxTextChanged(object sender, EventArgs e)
        {
            if (!(sender as TextBox).Focused)
                return;

            string[] name = (sender as TextBox).Name.Split(',');
            int colors = int.Parse(name[0]), set = int.Parse(name[1]);
            UpdateChainData(setSelector.SelectedIndex, colors, chainSelector.SelectedIndex, set, (sender as TextBox).Text);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                fieldDisplay.SetChain(chains[setSelector.SelectedIndex][colors][chainSelector.SelectedIndex][set]);
            };
            bw.RunWorkerAsync();
        }

        private void TriggerColorBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name = (sender as ComboBox).Name.Split(',');
            chains[setSelector.SelectedIndex][int.Parse(name[0])][chainSelector.SelectedIndex][int.Parse(name[1])].TriggerColor = (Puyo)((sender as ComboBox).SelectedIndex + 1);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                fieldPanel.Refresh();
            };
            bw.RunWorkerAsync();
        }

        private void ImportData(object sender, EventArgs e)
        {
            // Just send over the entire chain array to the Import Data function
            ImportExport.Import(chains, setSelector.SelectedIndex, chainSelector.SelectedIndex);

            GetChainData(setSelector.SelectedIndex, chainSelector.SelectedIndex);
            fieldDisplay.SetChain(null);
        }

        private void ExportData(object sender, EventArgs e)
        {
            // Just send over the entire chain array to the Export Data function
            ImportExport.Export(chains, setSelector.SelectedIndex, chainSelector.SelectedIndex);
        }
    }
}