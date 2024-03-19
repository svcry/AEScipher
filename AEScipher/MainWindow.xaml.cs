using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AEScipher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                string encryptedText = EncryptText(inputText, password);
                OutputTextBlock.Text = "Encrypted Text: " + encryptedText;
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = "Encryption failed: " + ex.Message;
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string encryptedText = InputTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                string decryptedText = DecryptText(encryptedText, password);
                OutputTextBlock.Text = "Decrypted Text: " + decryptedText;
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = "Decryption failed: " + ex.Message;
            }
        }

        private void OutputTextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                // Создаем и отображаем контекстное меню
                ContextMenu contextMenu = new ContextMenu();

                // Добавляем элемент меню для копирования текста
                MenuItem copyMenuItem = new MenuItem();
                copyMenuItem.Header = "Copy";
                copyMenuItem.Click += CopyMenuItem_Click;
                contextMenu.Items.Add(copyMenuItem);

                // Определяем позицию, где был отпущена правая кнопка мыши
                Point point = e.GetPosition(OutputTextBlock);

                // Показываем контекстное меню в указанной позиции
                contextMenu.IsOpen = true;
                OutputTextBlock.ContextMenu = contextMenu;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
                contextMenu.PlacementTarget = OutputTextBlock;
                contextMenu.HorizontalOffset = point.X;
                contextMenu.VerticalOffset = point.Y;
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputTextBlock.Text);
        }
        private string EncryptText(string inputText, string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("This is a salt.");
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private string DecryptText(string encryptedText, string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("This is a salt.");
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}

