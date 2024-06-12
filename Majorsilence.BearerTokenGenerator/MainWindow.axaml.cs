using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
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
            
            foreach (var field in typeof(SecurityAlgorithms).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly)
                {
                    var algorithm = field.GetValue(null).ToString();
                    if (algorithm.StartsWith("HS"))
                    {
                        SecurityAlgorithmsList.Add(new KeyValuePair<string, string>(field.Name, algorithm));
                    }
                }
            }
            // Set the default selected algorithm
            SelectedAlgorithm = SecurityAlgorithmsList.Find(pair => pair.Key == "HmacSha256");
            
            DataContext = this;
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

        public List<KeyValuePair<string, string>> SecurityAlgorithmsList { get; } = new List<KeyValuePair<string, string>>();
        public KeyValuePair<string, string> SelectedAlgorithm { get; set; } 
        
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
                signingCredentials: new SigningCredentials(authSigningKey, SelectedAlgorithm.Value)
            );
            string key = new JwtSecurityTokenHandler().WriteToken(token);
            return key;
        }
    }
}