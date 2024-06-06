using Common;
using DataLibrary.QueueHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AdminTools
{
    class Program
    {
        static void Main(string[] args)
        {
            ISendAdminEmails proxy = ConnectToHMS();

            while (true) {
                Console.WriteLine("Unesite username:");
                string username = Console.ReadLine();

                if (username != "admin") continue;

                Console.WriteLine("Unesite lozinku:");
                string password = Console.ReadLine();

                if (password != "admin") continue;

                while (true)
                {
                    Console.WriteLine("Unesite email za oporavak ili Exit za izlazak!");
                    string email = Console.ReadLine();
                    if (email == "Exit") break;
                    if (!email.Contains('@')) Console.WriteLine("Pogresan email, pokusajte ponovo");
                    else
                    {
                        proxy.SendEmails(email);
                    } 
                }
            }
        }

        public static ISendAdminEmails ConnectToHMS()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<ISendAdminEmails> factory = new
            ChannelFactory<ISendAdminEmails>(binding, new
            EndpointAddress("net.tcp://localhost:6003/AdminEmailService"));
            return factory.CreateChannel();

            //Trace.WriteLine("Proxy: " + redditProxy.ToString());
        }
    }
}
