﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Dialogue.Logic.Constants;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Dialogue.Logic.Routes
{
    using System.ComponentModel;
    using Umbraco.Web.Routing;

    public class DialogueMemberRouteHandler : UmbracoVirtualNodeByIdRouteHandler
    {

       private struct UrlNames
        {
            public int NodeId { get; set; }
            public string SearchUrlName { get; set; }
            public string SearchPageName { get; set; }
        }

        private readonly List<UrlNames> _urlNames = new List<UrlNames>();

        
        public DialogueMemberRouteHandler(UrlProvider umbracoUrlProvider, IEnumerable<IPublishedContent> itemsForRoute)
            : base(umbracoUrlProvider, itemsForRoute)
        {
            foreach (var node in itemsForRoute)
            {
                _urlNames.Add(new UrlNames
                {
                    NodeId = node.Id,
                    SearchUrlName = node.GetPropertyValue<string>(AppConstants.PropMemberUrlName),
                    //SearchPageName = node.GetPropertyValue<string>("searchPageName")
                    SearchPageName = string.Format("Member Page ({0})", node.Id)
                });
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DialogueMemberRouteHandler(int realNodeId,
            string searchUrlName,
            string searchPageName)
            : base(realNodeId)
        {
            _urlNames.Add(new UrlNames
            {
                NodeId = realNodeId,
                SearchPageName = searchPageName,
                SearchUrlName = searchUrlName
            });
        }

        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext, IPublishedContent baseContent)
        {
            var urlNames = _urlNames.Single(x => x.NodeId == baseContent.Id);

            var controllerName = requestContext.RouteData.GetRequiredString("controller");
            var rootUrl = baseContent.Url;

            return new DialogueVirtualPage(
                baseContent,
                urlNames.SearchPageName,
                controllerName,
                rootUrl.EnsureEndsWith('/') + urlNames.SearchUrlName);
        }


    }
}