using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ConsoleMonitoringSystem
{
    class Program
    {
        static string Conn = "data source=DESKTOP-NQGDDL6;integrated security=true; TrustServerCertificate=true; initial catalog=Monitoring";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter a command (Showlist to display data, Clear to clear the screen, Send email to send the email, Exit to quit): ");
                string command = Console.ReadLine();

                if (command.ToLower() == "showlist")
                {
                    DisplayData();
                }
                else if (command.ToLower() == "clear")
                {
                    Console.Clear();
                }
                else if (command.ToLower() == "send email")
                {
                    SendEmail();
                }
                else if (command.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid command. Please try again.");
                }
            }
        }

        static void DisplayData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Conn))
                {
                    connection.Open();

             
                    string query = "SELECT * FROM VW_Overview";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            Console.WriteLine("ApplicationTypeName\tApplicationName\tIP\tIP2\tLoginDate\tLoginSuccesfully\tUsername\tPassword");

                      
                            while (reader.Read())
                            {
                                string applicationTypeName = reader["ApplicationTypeName"].ToString();
                                string applicationName = reader["ApplicationName"].ToString();
                                string ip = reader["IP"].ToString();
                                string ip2 = reader["IP2"].ToString();
                                DateTime loginDate = (DateTime)reader["LoginDate"];
                                int loginSuccessfully = Convert.ToInt32(reader["LoginSuccesfully"]);
                                string username = reader["Username"].ToString();
                                string password = reader["Password"].ToString();

                                Console.WriteLine($"{applicationTypeName}\t{applicationName}\t{ip}\t{ip2}\t{loginDate}\t{loginSuccessfully}\t{username}\t{password}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void SendEmail()
        {
            try
            {
 
                string smtpHost = "smtp03.hostnet.nl";
                int smtpPort = 587;
                string smtpUsername = "jeffrey.doornbos@navigator-eu.com";
                string smtpPassword = "Zoetermeer1!";

                using (SqlConnection connection = new SqlConnection(Conn))
                {
                    connection.Open();


                    string query = "SELECT * FROM VW_Overview";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
       
                            StringBuilder reportBuilder = new StringBuilder();


                            reportBuilder.AppendLine("ApplicationTypeName,ApplicationName,IP,IP2,LoginDate,LoginSuccessfully,Username,Password");


                            while (reader.Read())
                            {
                                string applicationTypeName = reader["ApplicationTypeName"].ToString();
                                string applicationName = reader["ApplicationName"].ToString();
                                string ip = reader["IP"].ToString();
                                string ip2 = reader["IP2"].ToString();
                                DateTime loginDate = (DateTime)reader["LoginDate"];
                                int loginSuccessfully = Convert.ToInt32(reader["LoginSuccesfully"]);
                                string username = reader["Username"].ToString();
                                string password = reader["Password"].ToString();


                                reportBuilder.AppendLine($"{applicationTypeName},{applicationName},{ip},{ip2},{loginDate},{loginSuccessfully},{username},{password}");
                            }


                            string csvReport = reportBuilder.ToString();

                            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(csvReport)))
                            {
                                // Create the attachment for the email
                                var attachment = new Attachment(memoryStream, "OverviewReport.csv", "text/csv");

                                // Create the email message
                                using (var message = new MailMessage("jeffrey.doornbos@navigator-eu.com", "jeffrey.jubal@gmail.com", "Overview Report", "Please find the Overview report attached."))
                                {
                                    // Attach the CSV report to the email
                                    message.Attachments.Add(attachment);

                                    // Configure the SMTP client and send the email
                                    using (var client = new SmtpClient(smtpHost, smtpPort))
                                    {
                                        client.EnableSsl = true;
                                        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                                        client.Send(message);
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Email sent successfully to jeffrey.jubal@gmail.com");
            }
            catch (Exception ex)
            {
                // handle any errors here
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
