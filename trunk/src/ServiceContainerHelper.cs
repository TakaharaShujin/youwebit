//Copyright (c) 2007 Maxence Dislaire

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.
using System.ComponentModel.Design;
using System.Diagnostics;

namespace YouWebIt
{
    /// <summary>
    /// This class is a generic wrapper around the .net ServiceContainer
    /// </summary>
    [DebuggerStepThrough()]
    public class ServiceContainerHelper : IServiceContainerHelper
    {
        private ServiceContainer m_serviceContainer;

        public ServiceContainerHelper()
        {
            m_serviceContainer = new ServiceContainer();
        }

        public TService AddService<TService>(TService serviceInstance) where TService : class
        {
            if (m_serviceContainer.GetService(typeof(TService)) == null)
            {
                m_serviceContainer.AddService(typeof(TService), serviceInstance);
            }
            return serviceInstance;
        }

        public TService GetService<TService>() where TService : class
        {
            return (TService)m_serviceContainer.GetService(typeof(TService));
        }

        public void RemoveService<TService>(TService serviceInstance) where TService : class
        {
            m_serviceContainer.RemoveService(serviceInstance.GetType());
        }
    }
}