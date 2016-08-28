using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Security.Principal;
using System.Security.Claims;

namespace Stateless1
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Stateless1 : StatelessService, ICalculator
    {
        public Stateless1(StatelessServiceContext context)
            : base(context)
        { }

        public Task<int> AddAsync(int x, int y)
        {
            IPrincipal principal = Thread.CurrentPrincipal;

            string message = "The IPrincipal was not passed on thread to remote service on cluster";
            if (principal is ClaimsPrincipal)
            {
                ClaimsPrincipal claimsPrincipal = principal as ClaimsPrincipal;
                Claim claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Value.Equals("TestClaimValue2"));

                if (claim != null)
                {
                    message = "Successfully passed the IPrincipal on the thread to the remote service on the cluster";
                }
            }
            ServiceEventSource.Current.ServiceMessage(this, message);

            int result = x + y;
            return Task.FromResult<int>(result);
        }

        public Task<int> SubtractAsync(int x, int y)
        {
            return Task.FromResult<int>(x - y);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context =>
                                this.CreateServiceRemotingListener(context)) };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
