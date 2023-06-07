using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.IdentityModel.Tokens;
using Avalonia.Controls;
using MessageBox.Avalonia;

namespace Majorsilence.BearerTokenGenerator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
        
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string key = GenerateKey();
                outputTextBlock.Text = key;
            }
            catch (Exception ex)
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message);
                messageBox.Show();
            }
        }

        private string GenerateKey()
        {
            string issuer = issuerTextBox.Text;
            string audience = audienceTextBox.Text;
            int expires = (int)expiresNumericUpDown.Value;
            string secret = secretTextBox.Text;
            DateTime? notBeforeDate = this.notBefore.SelectedDate?.DateTime;
            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires:   DateTime.Now.AddTicks(TimeSpan.FromMinutes(expires).Ticks),
                notBefore: notBeforeDate,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            string key = new JwtSecurityTokenHandler().WriteToken(token);
            return key;
        }
    }
}