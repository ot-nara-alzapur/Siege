﻿/*   Copyright 2009 - 2010 Marcus Bratton

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
*/

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Siege.ServiceLocation.Stores;

namespace Siege.ServiceLocation.HttpIntegration
{
    public abstract class SiegeHttpApplication : HttpApplication
    {
        private static IContextualServiceLocator locator;

        public IContextualServiceLocator ServiceLocator
        {
            get
            {
                return locator;
            }
        }

        protected abstract IServiceLocatorAdapter GetServiceLocatorAdapter();

        public virtual void RegisterRoutes(RouteCollection routes) { }

        protected virtual IContextStore GetContextStore()
        {
            return new HttpSessionStore();
        }

        protected virtual void OnApplicationStarted()
        {
        }

        protected virtual void OnApplicationStopped()
        {
        }

        protected virtual void OnApplicationError(Exception exception)
        {
        }

        public void Application_Start(object sender, EventArgs e)
        {
            lock (this)
            {
                locator = new SiegeContainer(GetServiceLocatorAdapter(), GetContextStore());
                locator
                    .Register(Given<RouteCollection>.Then(RouteTable.Routes))
                    .Register(Given<IContextStore>.Then(locator.ContextStore));


                RegisterRoutes(RouteTable.Routes);
                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new SiegeViewEngine());
                ViewEngines.Engines.Add(new WebFormViewEngine());

                ControllerBuilder.Current.SetControllerFactory(new SiegeControllerFactory(locator));

                OnApplicationStarted();
            }
        }

        public void Application_Stop(object sender, EventArgs e)
        {
            lock (this)
            {
                OnApplicationStopped();

                if (locator != null)
                {
                    locator.Dispose();
                    locator = null;
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }
    }
}