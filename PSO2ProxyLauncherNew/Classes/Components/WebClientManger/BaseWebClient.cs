using System.Net;
using Leayal;

namespace PSO2ProxyLauncherNew.Classes.Components.WebClientManger
{
    class BaseWebClient : Leayal.Net.WebClient
    {
        protected override ICredentials GetCredentials(System.Uri address)
        {
            return this.GetCredentials(address, this.Credentials);
        }

        protected override ICredentials GetCredentials(System.Uri address, ICredentials defaultvalue)
        {
            if (this.AutoCredentials)
            {
                if (address.Host.IsEqual(Infos.DefaultValues.Arghlex.Web.DownloadHost, true))
                    return Infos.DefaultValues.Arghlex.Web.AccountArghlex;
                else
                    return defaultvalue;
            }
            else
                return defaultvalue;
        }

        protected override string GetUserAgent(System.Uri address)
        {
            if (this.AutoUserAgent)
            {
                if (address.Host.IsEqual(AIDA.ArksLayerHost, true))
                    return Infos.DefaultValues.AIDA.Web.UserAgent;
                else if (address.Host.IsEqual(Infos.DefaultValues.Kaze.Web.DownloadHost, true))
                    return Infos.DefaultValues.Kaze.Web.UserAgent;
                else if (address.Host.IsEqual(PSO2.DefaultValues.Web.DownloadHost, true))
                    return PSO2.DefaultValues.Web.UserAgent;
                else if (!string.IsNullOrEmpty(UserAgent))
                    return this.UserAgent;
                else
                    return null;
            }
            else if (!string.IsNullOrEmpty(UserAgent))
                return this.UserAgent;
            else
                return null;
        }
    }
}
