/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Banned IP Page.
    /// </summary>
    public partial class bannedip : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_BANNEDIP", "TITLE"));

            this.BindData();
        }

        /// <summary>
        /// Adds text to the Add Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Add_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var addButton = (Button)sender;

            addButton.Text = addButton.ToolTip = this.GetText("ADMIN_BANNEDIP", "ADD_IP");
        }

        /// <summary>
        /// Adds text to the Import Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Import_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var importButton = (Button)sender;

            importButton.Text = importButton.ToolTip = this.GetText("ADMIN_BANNEDIP", "IMPORT_IPS");
        }

        /// <summary>
        /// The list_ item command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void List_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "import":
                    YafBuildLink.Redirect(ForumPages.admin_bannedip_import);
                    break;
                case "add":
                    YafBuildLink.Redirect(ForumPages.admin_bannedip_edit);
                    break;
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_bannedip_edit, "i={0}", e.CommandArgument);
                    break;
                case "delete":
                    {
                        var ip = this.GetIPFromID(e.CommandArgument);
                        this.GetRepository<BannedIP>().DeleteByID(e.CommandArgument.ToType<int>());
                        this.BindData();
                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_BANNEDIP", "MSG_REMOVEBAN"));

                        if (YafContext.Current.Get<YafBoardSettings>().LogBannedIP)
                        {
                            this.Get<ILogger>()
                                .Log(
                                    this.PageContext.PageUserID,
                                    " YAF.Pages.Admin.bannedip",
                                    "IP or mask {0} was deleted by {1}.".FormatWith(
                                        ip,
                                        this.Get<YafBoardSettings>().EnableDisplayName
                                            ? this.PageContext.CurrentUserData.DisplayName
                                            : this.PageContext.CurrentUserData.UserName),
                                    EventLogTypes.IpBanLifted);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Helper to get mask from ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>
        /// Returns the IP
        /// </returns>
        private string GetIPFromID(object id)
        {
            return (from RepeaterItem ri in this.list.Items
                    let chid = ((Label)ri.FindControl("MaskBox")).Text
                    let fid = ((HiddenField)ri.FindControl("fID")).Value
                    where id.ToString() == fid
                    select chid).FirstOrDefault();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;

            var bannedList = this.GetRepository<BannedIP>().List(null, this.PagerTop.CurrentPageIndex, this.PagerTop.PageSize);
            this.list.DataSource = bannedList;

            this.PagerTop.Count = bannedList != null && bannedList.Rows.Count > 0
                                      ? bannedList.AsEnumerable().First().Field<int>("TotalRows")
                                      : 0;

            this.DataBind();
        }

        #endregion
    }
}