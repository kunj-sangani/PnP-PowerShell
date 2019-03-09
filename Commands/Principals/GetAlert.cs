﻿#if !SP2013 && !SP2016
using Microsoft.SharePoint.Client;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Get, "PnPAlert")]
    [CmdletHelp("Returns registered alerts for a user.",
        Category = CmdletHelpCategory.Principals,
        SupportedPlatform = CmdletSupportedPlatform.SP2019 | CmdletSupportedPlatform.Online)]
    [CmdletExample(
        Code = @"PS:> Get-PnPAlert",
        Remarks = @"Returns all registered alerts for the current user",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-PnPAlert -List ""Demo List""",
        Remarks = @"Returns all alerts registered on the given list for the current user.",
        SortOrder = 2)]
    [CmdletExample(
        Code = @"PS:> Get-PnPAlert -List ""Demo List"" -User ""i:0#.f|membership|Alice@contoso.onmicrosoft.com""",
        Remarks = @"Returns all alerts registered on the given list for the specified user.",
        SortOrder = 3)]
    public class GetAlert : PnPWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID, Title or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = false, HelpMessage = "User to retrieve the alerts for (User ID, login name or actual User object). Skip this parameter to retrieve the alerts for the current user. Note: Only site owners can retrieve alerts for other users.")]
        public UserPipeBind User;

        protected override void ExecuteCmdlet()
        {
            List list = null;
            if (List != null)
            {
                list = List.GetList(SelectedWeb);
            }

            var alert = new AlertCreationInformation();

            User user;
            if (null != User)
            {
                user = User.GetUser(ClientContext);
                if (user == null)
                {
                    throw new ArgumentException("Unable to find user", "Identity");
                }
            }
            else
            {
                user = SelectedWeb.CurrentUser;
            }

            user.EnsureProperty(u => u.Alerts.IncludeWithDefaultProperties(a => a.ListID));
            if (list != null)
            {
                WriteObject(user.Alerts.Where(l => l.ListID == list.Id), true);
            }
            else
            {
                WriteObject(user.Alerts, true);
            }
        }
    }
}
#endif