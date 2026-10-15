using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace RPA.ClaimStatements.Generator.Services
{
    // System.Net.Mail.SmtpClient only reads <system.net><mailSettings> on .NET Framework, so this section
    // (declared in App.config) is read explicitly to keep the existing per-environment SMTP configuration.
    public class SmtpSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("deliveryMethod", DefaultValue = SmtpDeliveryMethod.Network)]
        public SmtpDeliveryMethod DeliveryMethod
        {
            get { return (SmtpDeliveryMethod)this["deliveryMethod"]; }
        }

        [ConfigurationProperty("network")]
        public SmtpNetworkElement Network
        {
            get { return (SmtpNetworkElement)this["network"]; }
        }

        [ConfigurationProperty("specifiedPickupDirectory")]
        public SmtpPickupDirectoryElement SpecifiedPickupDirectory
        {
            get { return (SmtpPickupDirectoryElement)this["specifiedPickupDirectory"]; }
        }

        public static SmtpClient CreateClient()
        {
            var client = new SmtpClient();
            var section = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSettingsSection;

            if (section == null)
            {
                return client;
            }

            client.DeliveryMethod = section.DeliveryMethod;

            if (section.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
            {
                client.PickupDirectoryLocation = section.SpecifiedPickupDirectory.PickupDirectoryLocation;
            }
            else
            {
                var network = section.Network;

                client.Host = network.Host;
                client.Port = network.Port;
                client.EnableSsl = network.EnableSsl;
                client.UseDefaultCredentials = network.DefaultCredentials;

                if (!string.IsNullOrEmpty(network.UserName))
                {
                    client.Credentials = new NetworkCredential(network.UserName, network.Password);
                }
            }

            return client;
        }
    }

    public class SmtpNetworkElement : ConfigurationElement
    {
        [ConfigurationProperty("host")]
        public string Host
        {
            get { return (string)this["host"]; }
        }

        [ConfigurationProperty("port", DefaultValue = 25)]
        public int Port
        {
            get { return (int)this["port"]; }
        }

        [ConfigurationProperty("userName")]
        public string UserName
        {
            get { return (string)this["userName"]; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)this["password"]; }
        }

        [ConfigurationProperty("defaultCredentials", DefaultValue = false)]
        public bool DefaultCredentials
        {
            get { return (bool)this["defaultCredentials"]; }
        }

        [ConfigurationProperty("enableSsl", DefaultValue = false)]
        public bool EnableSsl
        {
            get { return (bool)this["enableSsl"]; }
        }
    }

    public class SmtpPickupDirectoryElement : ConfigurationElement
    {
        [ConfigurationProperty("pickupDirectoryLocation")]
        public string PickupDirectoryLocation
        {
            get { return (string)this["pickupDirectoryLocation"]; }
        }
    }
}
