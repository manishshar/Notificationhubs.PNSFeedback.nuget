using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notificationhubs.PNSFeedback;

namespace NugetConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PNSFeedback pNS = new PNSFeedback("Endpoint=sb://{namespace}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DeUDBkVDp7uPzVIktmIh4ddanBah+x792fBIPYKPkes=", "{notificationhubname}");
            var FolderDate= pNS.GetNotificationHubPNSFeedbackDates();
            var status = pNS.GetNotificationHubPNSFeedbackExporttoCSVByDate(FolderDate[0], "C://", "testfilename");

        }
    }
}
