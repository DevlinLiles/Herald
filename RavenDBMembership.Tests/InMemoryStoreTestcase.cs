/*
 * Original code from http://blogs.taiga.nl/martijn/2010/11/25/raven-db-asp-net-membership-provider/
 * Modified from XUnit to MSTest by Tim Rayburn (http://TimRayburn.net)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Client;

namespace RavenDBMembership.Tests
{
	public abstract class InMemoryStoreTestcase
	{
		protected EmbeddableDocumentStore NewInMemoryStore()
		{
			var documentStore = new EmbeddableDocumentStore
			{
				RunInMemory = true
			};
			documentStore.Initialize();
			return documentStore;
		}
	}
}
