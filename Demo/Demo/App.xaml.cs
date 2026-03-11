using System.Configuration;
using System.Data;
using System.Windows;
using Demo.Model;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
    }

}
