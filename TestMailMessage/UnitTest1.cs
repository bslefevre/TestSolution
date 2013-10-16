using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Office.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace TestMailMessage
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var mailMessage = new DCMailMessage();
            const string mailadres = "bjorn.le.fevre@carthago-ict.nl";
            const string mailAdres2 = "grotetest@test.nl";
            //mailMessage.To = mailadres;

            try
            {
                // Create the Outlook application.
                Outlook.Application oApp = new Outlook.Application();
                // Create a new mail item.
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                // Set HTMLBody. 
                //add the body of the email
                //oMsg.HTMLBody = "Hello, your message body will go here!! <img src=\"C:\\file Name.jpg\" />";
                //Add an attachment.
                //String sDisplayName = "MyAttachment";
                //int iPosition = 0; //(int)oMsg.Body.Length + 1;
                //int iAttachType = (int)Outlook.OlAttachmentType.olEmbeddeditem;
                //now attached the file
                //Outlook.Attachment oAttach = oMsg.Attachments.Add(@"C:\\fileName.jpg", iAttachType, iPosition, sDisplayName);
                //Subject line


                const string pattern = @"\bheight\b([\s\S]*?(\""\w{0,10}\""))";
                const string pattern2 = @"\bimg\b([\s\S]*?\w(\""))";
                var regex = new Regex(pattern2, RegexOptions.IgnoreCase);
                
                var htmlBody = "Hello, your message body will go here!! <img src=\"C:\\Users\\bjorn.le.fevre.Hyperion\\AppData\\Local\\Temp\\image0.jpg\" width=\"538\" height=\"303\" alt=\"\" style=\"border-width:0px;\" /><img src=\"C:\\Users\\bjorn.le.fevre.Hyperion\\AppData\\Local\\Temp\\image1.jpg\" width=\"538\" height=\"303\" />";

                //htmlBody = regex.Replace(htmlBody, AddPxToHeightTag);

                var geefExtensionsInHtmlBody = GeefExtensionsInHtmlBody(htmlBody);

                var matchCollection = regex.Matches(htmlBody);
                oMsg.HTMLBody =
                    htmlBody;


                oMsg.Subject = "Your Subject will go here.";
                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                // Change the recipient in the next line if necessary.
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(mailadres);
                Outlook.Recipient oRecip2 = (Outlook.Recipient)oRecips.Add("Eddy B[edwin.vdbelt@innolan.nl]");
                Outlook.Recipient oRecip3 = (Outlook.Recipient)oRecips.Add(mailadres);

                oRecip.Resolve();

                oRecip2.Type = (int) Outlook.OlMailRecipientType.olBCC;
                oRecip2.Resolve();
                oRecip3.Type = (int)Outlook.OlMailRecipientType.olCC;
                oRecip3.Resolve();
                // Send.
                oMsg.Display();
                // Clean up.
                //oRecip = null;
                oRecips = null;
                oMsg = null;
                oApp = null;
            }//end of try block
            catch (Exception ex)
            {
            }//end of catch
        }

        private Dictionary<int, string> GeefExtensionsInHtmlBody(string htmlBody)
        {
            var extensionDictionary = new Dictionary<int, string>();
            const string pattern2 = @"\bimg\b([\s\S]*?\w(\""))";
                var regex = new Regex(pattern2, RegexOptions.IgnoreCase);
            var matchCollection = regex.Matches(htmlBody);
            var extensionNumber = 0;
            foreach (Match match in matchCollection)
            {
                var value = match.Value;
                var strings = value.Replace("\"", "").Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                var stringArray = strings.ToArray();
                var extension = stringArray.Last();
                extensionDictionary.Add(extensionNumber, extension);
                extensionNumber++;
            }

            return extensionDictionary;
        }

        private string Evaluator(Match match)
        {
            var value = match.Value;

            var imgRemoved = value.Replace("img src=\"", "");

            var strings = value.Replace("\"", "").Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var stringArray = strings.ToArray();

            var extension = stringArray.Last();

            return value;
        }

        private string AddPxToHeightTag(Match match)
        {
            var value = match.Value;

            var heightRemoved = value.Replace("height=\"", "");

            var backslashAddedWithPx = heightRemoved.Replace("\"", "px\"");

            var heightAdded = "height=\"" + backslashAddedWithPx;

            return heightAdded;
        }
    }

    //public class DCMailMessage : MailMessage
    //{
    //    public new string To { get; set; }
    //}

    //public class DCMailItem : Outlook.MailItem
    //{
    //    void Outlook._MailItem.Close(Outlook.OlInspectorClose SaveMode)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object Copy()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Delete()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Display(object Modal)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object Move(Outlook.MAPIFolder DestFldr)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void PrintOut()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Save()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SaveAs(string Path, object Type)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ClearConversationIndex()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    Outlook.MailItem Outlook._MailItem.Forward()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    Outlook.MailItem Outlook._MailItem.Reply()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    Outlook.MailItem Outlook._MailItem.ReplyAll()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    void Outlook._MailItem.Send()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ShowCategoriesDialog()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddBusinessCard(Outlook.ContactItem contact)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void MarkAsTask(Outlook.OlMarkInterval MarkInterval)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ClearTaskFlag()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Outlook.Conversation GetConversation()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Outlook.Application Application { get; private set; }
    //    public Outlook.OlObjectClass Class { get; private set; }
    //    public Outlook.NameSpace Session { get; private set; }
    //    public object Parent { get; private set; }
    //    public Outlook.Actions Actions { get; private set; }
    //    public Outlook.Attachments Attachments { get; private set; }
    //    public string BillingInformation { get; set; }
    //    public string Body { get; set; }
    //    public string Categories { get; set; }
    //    public string Companies { get; set; }
    //    public string ConversationIndex { get; private set; }
    //    public string ConversationTopic { get; private set; }
    //    public DateTime CreationTime { get; private set; }
    //    public string EntryID { get; private set; }
    //    public Outlook.FormDescription FormDescription { get; private set; }
    //    public Outlook.Inspector GetInspector { get; private set; }
    //    public Outlook.OlImportance Importance { get; set; }
    //    public DateTime LastModificationTime { get; private set; }
    //    public object MAPIOBJECT { get; private set; }
    //    public string MessageClass { get; set; }
    //    public string Mileage { get; set; }
    //    public bool NoAging { get; set; }
    //    public int OutlookInternalVersion { get; private set; }
    //    public string OutlookVersion { get; private set; }
    //    public bool Saved { get; private set; }
    //    public Outlook.OlSensitivity Sensitivity { get; set; }
    //    public int Size { get; private set; }
    //    public string Subject { get; set; }
    //    public bool UnRead { get; set; }
    //    public Outlook.UserProperties UserProperties { get; private set; }
    //    public bool AlternateRecipientAllowed { get; set; }
    //    public bool AutoForwarded { get; set; }
    //    public string BCC { get; set; }
    //    public string CC { get; set; }
    //    public DateTime DeferredDeliveryTime { get; set; }
    //    public bool DeleteAfterSubmit { get; set; }
    //    public DateTime ExpiryTime { get; set; }
    //    public DateTime FlagDueBy { get; set; }
    //    public string FlagRequest { get; set; }
    //    public Outlook.OlFlagStatus FlagStatus { get; set; }
    //    public string HTMLBody { get; set; }
    //    public bool OriginatorDeliveryReportRequested { get; set; }
    //    public bool ReadReceiptRequested { get; set; }
    //    public string ReceivedByEntryID { get; private set; }
    //    public string ReceivedByName { get; private set; }
    //    public string ReceivedOnBehalfOfEntryID { get; private set; }
    //    public string ReceivedOnBehalfOfName { get; private set; }
    //    public DateTime ReceivedTime { get; private set; }
    //    public bool RecipientReassignmentProhibited { get; set; }
    //    public Outlook.Recipients Recipients { get; private set; }
    //    public bool ReminderOverrideDefault { get; set; }
    //    public bool ReminderPlaySound { get; set; }
    //    public bool ReminderSet { get; set; }
    //    public string ReminderSoundFile { get; set; }
    //    public DateTime ReminderTime { get; set; }
    //    public Outlook.OlRemoteStatus RemoteStatus { get; set; }
    //    public string ReplyRecipientNames { get; private set; }
    //    public Outlook.Recipients ReplyRecipients { get; private set; }
    //    public Outlook.MAPIFolder SaveSentMessageFolder { get; set; }
    //    public string SenderName { get; private set; }
    //    public bool Sent { get; private set; }
    //    public DateTime SentOn { get; private set; }
    //    public string SentOnBehalfOfName { get; set; }
    //    public bool Submitted { get; private set; }
    //    public string To { get; set; }
    //    public string VotingOptions { get; set; }
    //    public string VotingResponse { get; set; }
    //    public Outlook.Links Links { get; private set; }
    //    public Outlook.ItemProperties ItemProperties { get; private set; }
    //    public Outlook.OlBodyFormat BodyFormat { get; set; }
    //    public Outlook.OlDownloadState DownloadState { get; private set; }
    //    public int InternetCodepage { get; set; }
    //    public Outlook.OlRemoteStatus MarkForDownload { get; set; }
    //    public bool IsConflict { get; private set; }
    //    public bool IsIPFax { get; set; }
    //    public Outlook.OlFlagIcon FlagIcon { get; set; }
    //    public bool HasCoverSheet { get; set; }
    //    public bool AutoResolvedWinner { get; private set; }
    //    public Outlook.Conflicts Conflicts { get; private set; }
    //    public string SenderEmailAddress { get; private set; }
    //    public string SenderEmailType { get; private set; }
    //    public bool EnableSharedAttachments { get; set; }
    //    public Outlook.OlPermission Permission { get; set; }
    //    public Outlook.OlPermissionService PermissionService { get; set; }
    //    public Outlook.PropertyAccessor PropertyAccessor { get; private set; }
    //    public Outlook.Account SendUsingAccount { get; set; }
    //    public string TaskSubject { get; set; }
    //    public DateTime TaskDueDate { get; set; }
    //    public DateTime TaskStartDate { get; set; }
    //    public DateTime TaskCompletedDate { get; set; }
    //    public DateTime ToDoTaskOrdinal { get; set; }
    //    public bool IsMarkedAsTask { get; private set; }
    //    public string ConversationID { get; private set; }
    //    public Outlook.AddressEntry Sender { get; set; }
    //    public string PermissionTemplateGuid { get; set; }
    //    public object RTFBody { get; set; }
    //    public string RetentionPolicyName { get; private set; }
    //    public DateTime RetentionExpirationDate { get; private set; }
    //    public event Outlook.ItemEvents_10_OpenEventHandler Open;
    //    public event Outlook.ItemEvents_10_CustomActionEventHandler CustomAction;
    //    public event Outlook.ItemEvents_10_CustomPropertyChangeEventHandler CustomPropertyChange;
    //    public event Outlook.ItemEvents_10_ForwardEventHandler Forward;
    //    public event Outlook.ItemEvents_10_CloseEventHandler Close;
    //    public event Outlook.ItemEvents_10_PropertyChangeEventHandler PropertyChange;
    //    public event Outlook.ItemEvents_10_ReadEventHandler Read;
    //    public event Outlook.ItemEvents_10_ReplyEventHandler Reply;
    //    public event Outlook.ItemEvents_10_ReplyAllEventHandler ReplyAll;
    //    public event Outlook.ItemEvents_10_SendEventHandler Send;
    //    public event Outlook.ItemEvents_10_WriteEventHandler Write;
    //    public event Outlook.ItemEvents_10_BeforeCheckNamesEventHandler BeforeCheckNames;
    //    public event Outlook.ItemEvents_10_AttachmentAddEventHandler AttachmentAdd;
    //    public event Outlook.ItemEvents_10_AttachmentReadEventHandler AttachmentRead;
    //    public event Outlook.ItemEvents_10_BeforeAttachmentSaveEventHandler BeforeAttachmentSave;
    //    public event Outlook.ItemEvents_10_BeforeDeleteEventHandler BeforeDelete;
    //    public event Outlook.ItemEvents_10_AttachmentRemoveEventHandler AttachmentRemove;
    //    public event Outlook.ItemEvents_10_BeforeAttachmentAddEventHandler BeforeAttachmentAdd;
    //    public event Outlook.ItemEvents_10_BeforeAttachmentPreviewEventHandler BeforeAttachmentPreview;
    //    public event Outlook.ItemEvents_10_BeforeAttachmentReadEventHandler BeforeAttachmentRead;
    //    public event Outlook.ItemEvents_10_BeforeAttachmentWriteToTempFileEventHandler BeforeAttachmentWriteToTempFile;
    //    public event Outlook.ItemEvents_10_UnloadEventHandler Unload;
    //    public event Outlook.ItemEvents_10_BeforeAutoSaveEventHandler BeforeAutoSave;
    //    public event Outlook.ItemEvents_10_BeforeReadEventHandler BeforeRead;
    //    public event Outlook.ItemEvents_10_AfterWriteEventHandler AfterWrite;
    //}
}
