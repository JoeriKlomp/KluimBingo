using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KluimBingoGUI
{
    public partial class KluimBingo : Form
    {
        private BingoKaartGenerator.BingoKaartGenerator generator;
        private Color controlColor = Color.FromArgb(255, 240, 240, 240);
        private List<string> drawnNumbers;

        public KluimBingo()
        {
            InitializeComponent();
            generator = new BingoKaartGenerator.BingoKaartGenerator();
            drawnNumbers = new List<string>();
            checkPresentPdfs();
            setBingoNumbers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            generator.GenerateAll();
            MessageBox.Show("Alle pdfs gegenereerd!");
        }

        private void setBingoNumbers()
        {
            for (int i = 75; i >= 1; i--)
            {
                int value = 76 - i;
                int j = i - 1;
                var label = table1.Controls[j] as Label;
                label.Text = value.ToString();
                label.Click += label_click;
            }
        }

        private void label_click(object sender, EventArgs e)
        {
            var label = sender as Label;

            if (label.BackColor == Color.LightGreen)
            { 
                label.BackColor = controlColor;
                drawnNumbers.Remove(label.Text);
            }
            else
            { 
                label.BackColor = Color.LightGreen;
                drawnNumbers.Add(label.Text);
            }
        }

        private void checkPresentPdfs()
        {
            var pdfs = Directory.GetFiles("PDFs");
            if (pdfs.Length != 0)
            {
                button1.Enabled = false;
                foreach (var item in pdfs)
                    importedPdfs.Items.Add(item.Replace("PDFs\\", ""));
            }
        }

        private void checkBingoButton_Click(object sender, EventArgs e)
        {
            string card = getSelectedCard();
            string bingoType = getSelectedBingoType();
            string selectedPdf = importedPdfs.SelectedItem as string;

            if (string.IsNullOrEmpty(card) || string.IsNullOrEmpty(bingoType) || string.IsNullOrEmpty(selectedPdf))
            {
                MessageBox.Show("Kies eerst de persoon, het soort kaartje en type bingo!");
                return;
            }

            var person = selectedPdf.Replace(".pdf", "");
            string[] allNumbers = File.ReadAllLines($"Texts\\{person}.txt");

            bool result;
            switch (bingoType)
            {
                case "1 rijtje":
                    result = checkSingleRowBingo(allNumbers, card);
                    MessageBox.Show(result.ToString());
                    break;
                case "2 rijtjes":
                    result = checkDoubleRowBingo(allNumbers, card);
                    MessageBox.Show(result.ToString());
                    break;
                case "Volle kaart":
                    result = checkFullCardBingo(allNumbers, card);
                    MessageBox.Show(result.ToString());
                    break;
            }
        }

        private string getSelectedCard()
        {
            for (int i = 0; i < groupBox1.Controls.Count; i++)
            {
                var radio = groupBox1.Controls[i] as RadioButton;
                if (radio.Checked)
                    return groupBox1.Controls[i].Text;
            }
            
            return string.Empty;
        }

        private string getSelectedBingoType()
        {
            for (int i = 0; i < groupBox2.Controls.Count; i++)
            {
                var radio = groupBox2.Controls[i] as RadioButton;
                if (radio.Checked)
                    return groupBox2.Controls[i].Text;
            }

            return string.Empty;
        }

        private string[] getSelectedCardNumbers(string cardNumber, string[] allNumbers)
        {
            switch(cardNumber)
            {
                case "1e kaartje":
                    return allNumbers.Take(25).ToArray();
                case "2e kaartje":
                    return allNumbers.Skip(25).Take(25).ToArray();
                case "3e kaartje":
                    return allNumbers.Skip(50).Take(25).ToArray();
                case "4e kaartje":
                    return allNumbers.Skip(75).Take(25).ToArray();
                default:
                    return null;
            }
        }

        private int countBingoRows(string[] cardNumbers) 
        {
            int counter = 0;

            //Check alle kolommen
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (!drawnNumbers.Contains(cardNumbers[5 * i + j]))
                        break;
                    if (j == 4)
                        counter++;
                }
            }

            //Check alle rijen
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (!drawnNumbers.Contains(cardNumbers[5 * j + i]))
                        break;
                    if (j == 4)
                        counter++;
                }
            }

            return counter;
        }

        private bool checkSingleRowBingo(string[] allNumbers, string card)
        {
            string[] cardNumbers = getSelectedCardNumbers(card, allNumbers);
            return countBingoRows(cardNumbers) >= 1;
        }

        private bool checkDoubleRowBingo(string[] allNumbers, string card)
        {
            var cardNumbers = getSelectedCardNumbers(card, allNumbers);
            return countBingoRows(cardNumbers) >= 2;
        }

        private bool checkFullCardBingo(string[] allNumbers, string card)
        {
            var cardNumbers = getSelectedCardNumbers(card, allNumbers);
            for(int i = 0; i < cardNumbers.Length; i++)
            {
                if (!drawnNumbers.Contains(cardNumbers[i]))
                    return false;
            }
            return true;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            drawnNumbers = new List<string>();
            for (int i = 0; i < 75; i++)
                table1.Controls[i].BackColor = controlColor;
        }
    }
}
