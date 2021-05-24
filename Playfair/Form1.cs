using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Playfair
{
    public partial class Form1 : Form
    {
        bool check = true;
        Random rnd = new Random();
        List<double> frequency_letter = new List<double>()
        { 8.2, 1.5, 2.8, 4.3, 12.7, 2.2, 2.0, 6.1, 7.0, 0.2, 0.8, 4.0, 2.4,
          6.7, 7.5, 1.9, 0.1, 6.0, 6.3, 9.1, 2.8, 1.0, 2.4, 0.2, 2.0, 0.1};
        string alfabet = "abcdefghijklmnopqrstuvwxyz";
        string[] trigramm = { "the", "and", "ing", "her", "tha", "ere",
          "hat", "eth", "ent", "nth", "for", "his",
          "thi", "ter", "int", "dth", "you", "all",
          "hes", "ion", "ith", "oth", "est", "tth",
          "oft", "ver", "sth", "ers", "fth", "rea" };
        char[,] table_key = new char[5, 5];
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            textBox7.Enabled = false;
        }

        private string Exceptions(string alfabet, string text)
        {
            text = Regex.Replace(text, @"[\.!,\s:;?\d\W\/а-я]", "").ToLower();
            if (check)
            {
                if (text.Length != 26)
                    MessageBox.Show("Длина ключа не соответствует длине английского алфавита!");
                else
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (!text.Contains(alfabet[i].ToString()))
                            MessageBox.Show("Ключ должен содержать все буквы английского алфавита и быть равен его длине");
                    }
            }
            if (!check)
            {

            }
            return text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string text = "";
                if (textBox2.Text.Length == 0) MessageBox.Show("Неверный формат данных");
                else
                    text = File.ReadAllText(openFileDialog1.FileName);
                textBox2.Text = text.ToLower();
            }
        }

        private void button2_Click(object sender, EventArgs e)//кнопка вызывающая метод шифрования
        {
            try
            {
                textBox1.Text = Exceptions(alfabet, textBox1.Text);
                if(check)
                    textBox3.Text = Cipher(textBox1.Text, textBox2.Text, alfabet);
                if(!check)
                    textBox3.Text = Cipher(KeyP(textBox7.Text), textBox2.Text, alfabet);
                textBox5.Text = textBox3.Text;
            }
            catch (System.FormatException) { MessageBox.Show("Неверный формат данных!"); }
        }

        string Decipher(string key, string text, string alfabet)//метод дешифрования
        {
            string res = "";
            if (check)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    int count = key.IndexOf(text[j]);
                    if (count > 25) count %= 26;
                    res += alfabet[count];
                }
            }
            if (!check)
            {
                string ABC = alfabet.Replace("j", "i");
                foreach (var symbol in text)
                    res += ABC[key.IndexOf(symbol)];
            }
            return res;
        }



        string Cipher(string key, string text, string alfabet)//метод шифрования
        {
            string res = "";
            if (check)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    int count = alfabet.IndexOf(text[j]);
                    if (count > 25) count %= 26;//если вышел за размер алфавита
                    res += key[count];//берем соответствующитеие буквы из ключа
                }
            }
            if (!check)
            {
                textBox7.Text = KeyP(textBox7.Text);
                string result = "";
                if (text.Length % 2 == 1) 
                    text += "x";
                text = text.Replace("j", "i");
                List<string> bigramms = new List<string>();
                int k = 0;
                for(int i = 0; i < text.Length; i += 2)
                {
                    if (text[i] == text[i + 1])
                        bigramms.Add(String.Concat(text[i], "x"));
                    bigramms.Add(String.Concat(text[i], text[i + 1]));
                    k++;
                }
                foreach (var a in bigramms)
                {
                    /* Определим индексы элементов биграммы в матрице ключа размером 5 на 5
                     * Наш ключ сейчас представлен в виде обычной строки, поэтому индексы находим
                     * следующим образом: Допустим индекс символа в нашем ключе равен 12. Это значит,
                     * что его индекс в матричном виде был бы [3,2]. Чтобы получить из 12 число 3, а
                     * это в нашем случае номер строки, нужно взять целое от деления 17 на 5.
                     * Чтобы получить 2 - номер столбца, нужно взять остаток от деления 17 на 5
                     * То есть к индексу нужно сначала прибавить 5*/

                    List<int> indexAtKey = new List<int> { key.IndexOf(a[0]) + 1, key.IndexOf(a[1]) + 1 };
                    int lineA0 = (indexAtKey[0] + 5) / 5; // Номер строки
                    int lineA1 = (indexAtKey[1] + 5) / 5;
                    int columnA0 = (indexAtKey[0] + 5) % 5; // Номер столбца
                    int columnA1 = (indexAtKey[1] + 5) % 5;

                    if (columnA0 == 0)
                    {
                        columnA0 = 5;
                        lineA0--;
                    }
                    if (columnA1 == 0)
                    {
                        columnA1 = 5;
                        lineA1--;
                    }

                    // Сущ-ет различные вариации замены эелментов биграмм
                    if (lineA0 != lineA1)
                    {
                        if (columnA0 != columnA1)// Если получается прямоугольник из символов в матрице
                        {
                            // Обратный порядок для замены элементов биграммы
                            result += key[lineA0 * 5 + columnA1 - 5 - 1];
                            result += key[lineA1 * 5 + columnA0 - 5 - 1];
                        }
                        else // в одном столбце
                        {
                            result += key[lineA1 * 5 + columnA1 - 5 - 1];
                            result += key[lineA0 * 5 + columnA0 - 5 - 1];// lineA0+2 убрал пока
                        }
                    }
                    else
                    {
                        if (columnA0 != columnA1)// в одной строке
                        {
                            result += key[lineA1 * 5 + columnA1 - 5 - 1];
                            result += key[lineA0 * 5 + columnA0 + 2 - 5 - 1];
                        }
                        //else// Если совпали символы то второй заменим на х
                        //{
                        //    result += key[lineA1 * 5 + columnA1 - 5 - 1];
                        //    //result += key[key.IndexOf('x')];
                        //}
                    }
                }
                res = result;
            }
            return res;
        }

        private void button3_Click(object sender, EventArgs e)//рефреш баттон
        {
            textBox3.Text = "";
            //textBox5.Text = Decipher(textBox1.Text, textBox3.Text, alfabet);
            
        }

        private void button4_Click(object sender, EventArgs e)//кнопка сохранения файла
        {
            if (textBox3.Text.Length != 0)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                string filename = saveFileDialog1.FileName;
                File.WriteAllText(filename, textBox3.Text);
            }
            else MessageBox.Show("Неверный формат данных");
        }

        private void button5_Click(object sender, EventArgs e)//кнопка открытия файла
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string text = "";
                if (textBox5.Text.Length == 0) MessageBox.Show("Неверный формат данных");
                else
                    text = File.ReadAllText(openFileDialog1.FileName);
                textBox5.Text = text.ToLower();
            }
        }

        private void button7_Click(object sender, EventArgs e)//очищаем используемые поля
        {
            textBox4.Text = "";
            textBox6.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox6.Text.Length != 0)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                string filename = saveFileDialog1.FileName;
                File.WriteAllText(filename, textBox6.Text);
            }
            else MessageBox.Show("Неверный формат данных");
        }

        string Key()//string result)//генерим ключ
        {
            string pkey = alfabet;
            string p_key = "";
            while (p_key.Length < 26)
            {
                int r = rnd.Next(pkey.Length);
                p_key += pkey[r];//рандомный ключ
                pkey = pkey.Replace(pkey[r].ToString(), "");
            }
            return p_key;
        }

        string KeyP(string key)
        {
            textBox8.Text = "";
            string abc = alfabet.Replace("j", "");
            for (int i = 0; i < key.Length; i++)
            {
                if (abc.Contains(key[i].ToString()))
                {
                    abc = abc.Replace(key[i].ToString(), "");
                }
            }
            key += abc;
            int k = 0;
            for(int i = 0; i < 5; i++)
                for(int j = 0; j < 5; j++)
                {
                    table_key[i, j] = key[k];
                    k++;
                    textBox8.Text += table_key[i, j] + " ";
                    if (j == 4) textBox8.Text += Environment.NewLine;
                }
            return key;
        }

        private void button6_Click(object sender, EventArgs e)//кнопка подбора ключа
        {//получение преполагаемого текста
            if (check)
            {
                string pkey = Key();
                string result = Decipher(pkey, textBox5.Text, alfabet);
                string modif_key = pkey;
                string modif_result;
                double max_x = Frecuency(trigramm, result, frequency_letter, alfabet);
                for (int i = 0; i < 700000; i++)
                {
                    int i1 = rnd.Next(alfabet.Length);
                    int i2 = rnd.Next(alfabet.Length);
                    modif_key = Change_key(modif_key, i1, i2);
                    modif_result = Decipher(modif_key, textBox5.Text, alfabet);
                    if (Frecuency(trigramm, modif_result, frequency_letter, alfabet) > max_x)
                    {
                        pkey = modif_key;
                        max_x = Frecuency(trigramm, modif_result, frequency_letter, alfabet);
                        break;
                    }
                }
                textBox4.Text = pkey.ToString();
                textBox6.Text = Decipher(pkey, textBox5.Text, alfabet);
            }
            if (!check)
            {
                string pkey = alfabet;
                string result = Decipher(pkey, textBox5.Text, alfabet);
                string modif_key = pkey;
                string modif_result;
                double max_x = Frecuency(trigramm, result, frequency_letter, alfabet);
                for (int i = 0; i < 10000; i++)
                {
                    int i1 = rnd.Next(alfabet.Length);
                    int i2 = rnd.Next(alfabet.Length);
                    modif_key = Change_key(modif_key, i1, i2);
                    modif_result = Decipher(modif_key, textBox5.Text, alfabet);
                    if (Frecuency(trigramm, modif_result, frequency_letter, alfabet) > max_x)
                    {
                        pkey = modif_key;
                        max_x = Frecuency(trigramm, modif_result, frequency_letter, alfabet);
                        break;
                    }
                }
                textBox4.Text = pkey.ToString();
                textBox6.Text = Decipher(pkey, textBox5.Text, alfabet);
            }
        }
        //метод, считающий частоту встречаемости буквы
        double Frecuency(string[] trigramm, string result, List<double> frequency_letter, string alfabet)
        {
            int count = 1;
            foreach (string s in trigramm)
            {
                if (s.Contains(result))
                    count++;
            }
            double x = 1;
            if (count > 2)
            {
                for (int i = 0; i < alfabet.Length; i++)//высчитываем x
                {
                    int counter = 0;
                    for (int j = 0; j < result.Length; j++)
                    {
                        if (alfabet[i] == result[j]) counter++;
                    }
                    double frequency = counter * 100 / alfabet.Length;
                    x += frequency * frequency_letter[i];
                    x *= count;
                }//
            }
            return x;
        }
        string Change_key(string key, int i1, int i2)//метод, переставляющий 2 буквы в ключе
        {
            char[] letters = key.ToCharArray();
            char bubble;
            bubble = letters[i1];
            letters[i1] = letters[i2];
            letters[i2] = bubble;
            key = String.Concat(letters);
            return key;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            check = true;
            textBox7.Enabled = false;
            textBox1.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            check = false;
            textBox1.Enabled = false;
            textBox7.Enabled = true;
        }
    }
}
