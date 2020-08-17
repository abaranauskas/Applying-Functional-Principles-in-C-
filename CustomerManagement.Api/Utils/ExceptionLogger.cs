using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace CustomerManagement.Api.Utils
{
    public class ExceptionLogger: IExceptionLogger
    {
        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
