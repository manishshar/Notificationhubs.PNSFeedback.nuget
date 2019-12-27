using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Notificationhubs.PNSFeedback
{
    public class PNSFeedback
    {
       

        private static List<string> _ListBlobs;
        private static string _connectionString;
        private static string _notificationHubPath;

        private static DateTime FeedbackTime;
        private static string NotificationSystemError;
        private static string Platform;
        private static string PnsHandle;
        private static string RegistrationId;
        private static string NotificationId;

       

        /// <summary>
        /// Create connection to azure notification hub
        /// </summary>
        /// <param name="connectionString">connection string for azure notification hub namespace</param>
        /// <param name="notificationHubPath">notification hub name</param>
        public PNSFeedback(string connectionString, string notificationHubPath)
        {
            _connectionString = connectionString;
            _notificationHubPath = notificationHubPath;
            _ListBlobs = new List<string>();
        }
        /// <summary>
        /// Get Dates from notification hub for all PNS feedbacks available in storage container. These dates are acctually folders names created in datewise
        /// </summary>
        /// <returns>List of dates</returns>
        public List<string> GetNotificationHubPNSFeedbackDates()
        {
            
            NotificationHubClient Hub = NotificationHubClient.CreateClientFromConnectionString(_connectionString, _notificationHubPath);

            var feedbackuri = Hub.GetFeedbackContainerUriAsync().GetAwaiter().GetResult();
            CloudBlobContainer cbc = new CloudBlobContainer(feedbackuri);

            foreach (IListBlobItem blobItem in cbc.ListBlobs())
            {
                _ListBlobs.Add(((Microsoft.Azure.Storage.Blob.CloudBlobDirectory)blobItem).Prefix);
                //Console.WriteLine(blobItem.Uri);
                //CloudAppendBlob cloudAppendBlob = cbc.GetAppendBlobReference(((Microsoft.Azure.Storage.Blob.CloudBlob)blobItem).Name);
                //var txt = cloudAppendBlob.DownloadTextAsync().GetAwaiter().GetResult();
                //var pnsXmlSplit = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var pnsFeedbackXmlString in pnsXmlSplit)
                //{
                //    var xdoc = XDocument.Parse(pnsFeedbackXmlString);
                //    XNamespace ns = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";

                //    //FeedbackTime = DateTime.Parse(xdoc.Root.Element(ns + "FeedbackTime").Value);
                //    //NotificationSystemError = Convert.ToString(xdoc.Root.Element(ns + "NotificationSystemError").Value);
                //    //Platform = Convert.ToString(xdoc.Root.Element(ns + "Platform").Value);
                //    //PnsHandle = Convert.ToString(xdoc.Root.Element(ns + "PnsHandle").Value);
                //    //RegistrationId = Convert.ToString(xdoc.Root.Element(ns + "RegistrationId").Value);
                //    //NotificationId = Convert.ToString(xdoc.Root.Element(ns + "NotificationId").Value);

                //}
            }
            return _ListBlobs;
        }



        /// <summary>
        /// Get Notification hub PNS feedback in export to CSV by passing date "YYYYMMDD/"
        /// </summary>
        /// <param name="PNSFeedbackDate">Pass date for which PNS feedback to be fetched</param>
        /// <param name="FilePathtoSave">CSV file path to save</param>
        /// <param name="Filename">CSV file name to save</param>
        /// <returns>returns success if operation is succeeded otherwise failure for operation failure</returns>
        public string GetNotificationHubPNSFeedbackExporttoCSVByDate(string PNSFeedbackDate, string FilePathtoSave, string Filename)
        {
            try
            {

                NotificationHubClient Hub = NotificationHubClient.CreateClientFromConnectionString(_connectionString, _notificationHubPath);

                var feedbackuri = Hub.GetFeedbackContainerUriAsync().GetAwaiter().GetResult();
                CloudBlobContainer cbc = new CloudBlobContainer(feedbackuri);
                var csv = new StringBuilder();
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", "FeedbackTime", "Platform", "PnsHandle", "InstallationId", "NotificationId", "NotificationSystemError");
                csv.AppendLine(newLine);
                foreach (IListBlobItem blobItem in cbc.ListBlobs(prefix: PNSFeedbackDate))
                {

                    CloudAppendBlob cloudAppendBlob = cbc.GetAppendBlobReference(((Microsoft.Azure.Storage.Blob.CloudBlob)blobItem).Name);
                    var txt = cloudAppendBlob.DownloadTextAsync().GetAwaiter().GetResult();
                    var pnsXmlSplit = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var pnsFeedbackXmlString in pnsXmlSplit)
                    {
                        var xdoc = XDocument.Parse(pnsFeedbackXmlString);
                        XNamespace ns = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";

                        FeedbackTime = DateTime.Parse(xdoc.Root.Element(ns + "FeedbackTime").Value);
                        NotificationSystemError = Convert.ToString(xdoc.Root.Element(ns + "NotificationSystemError").Value);
                        Platform = Convert.ToString(xdoc.Root.Element(ns + "Platform").Value);
                        PnsHandle = Convert.ToString(xdoc.Root.Element(ns + "PnsHandle").Value);
                        RegistrationId = Convert.ToString(xdoc.Root.Element(ns + "RegistrationId").Value);
                        NotificationId = Convert.ToString(xdoc.Root.Element(ns + "NotificationId").Value);

                        csv.AppendLine(string.Format("{0},{1},{2},{3},{4}", FeedbackTime, Platform, PnsHandle, NotificationId, NotificationSystemError));
                    }
                }

                File.WriteAllText(string.Concat(FilePathtoSave, Filename, ".csv"), csv.ToString());
                return "Success";
            }
            catch (Exception ex)
            {
                return string.Concat("Failure:", ex.ToString());

            }
        }
    }
}
