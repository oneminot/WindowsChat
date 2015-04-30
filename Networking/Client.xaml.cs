using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq; //gives LINQ access to XML
using System.IO; //file IO

using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


    }
}
/*         static XDocument course()
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8","yes"),
                new XComment("Course Info"),
                new XElement("Course", 
                    new XAttribute("CourseID", "321"),
                    new XElement("Description", 
                        new XAttribute("Credits", "4"),
                        new XAttribute("System", "4.0"),
                        new XElement("Students",
                            new XElement("Student1", 
                                new XElement("First", "Joel"),
                                new XElement("Last", "Collins"),
                                new XElement("Grade", "B")),
                            new XElement("Student2", 
                                new XElement("First", "Mitch"),
                                new XElement("Last", "Steinman"),
                                new XElement("Grade", "d")),
                            new XElement("Student3", 
                                new XElement("First", "Matt"),
                                new XElement("Last", "Hargrove"),
                                new XElement("Grade", "F")),
                            new XElement("Student4", 
                                new XElement("First", "Andrew"),
                                new XElement("Last", "Clink"),
                                new XElement("Grade", "C---")),
                            new XElement("Student5", 
                                new XElement("First", "Chloe"),
                                new XElement("Last", "Ondracek"),
                                new XElement("Grade", "C")),
                            new XElement("Student6", 
                                new XElement("First", "Miles"),
                                new XElement("Last", "Knudsvig"),
                                new XElement("Grade", "A")),
                            new XElement("Student7", 
                                new XElement("First", "Sean"),
                                new XElement("Last", "Downes"),
                                new XElement("Grade", "A")))))
                );
            return doc;
        }

        public void SaveData(string filename)
        {
            XDocument doc = course();
            doc.Save(filename);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveData("ClassList");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            XDocument doc = course();
            var data = from item in doc.Descendants("Students")
                       select new { First = item.Element("First").Value };

        }
*/