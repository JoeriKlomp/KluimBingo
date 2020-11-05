using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KluimBingoGUI
{
    public partial class Form1 : Form
    {
        private BingoKaartGenerator.BingoKaartGenerator generator;
        private Color controlColor = Color.FromArgb(255, 240, 240, 240);

        public Form1()
        {
            InitializeComponent();
            generator = new BingoKaartGenerator.BingoKaartGenerator();
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
                label.BackColor = controlColor;
            else
                label.BackColor = Color.LightGreen;
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

            switch(bingoType)
            {
                case "1 rijtje":
                    checkSingleRowBingo(allNumbers, card);
                    break;
                case "2 rijtjes":
                    checkDoubleRowBingo(allNumbers, card);
                    break;
                case "Volle kaart":
                    checkFullCardBingo(allNumbers, card);
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

        private bool checkSingleRowBingo(string[] allNumbers, string card)
        {
            var cardNumbers = getSelectedCardNumbers(card, allNumbers);
            return true;
        }

        private bool checkDoubleRowBingo(string[] allNumbers, string card)
        {
            var cardNumbers = getSelectedCardNumbers(card, allNumbers);
            return true;
        }

        private bool checkFullCardBingo(string[] allNumbers, string card)
        {
            var cardNumbers = getSelectedCardNumbers(card, allNumbers);
            return true;
        }
    }
}
