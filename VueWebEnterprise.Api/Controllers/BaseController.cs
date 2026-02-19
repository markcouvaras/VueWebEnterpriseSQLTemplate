using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VueWebEnterprise.Api.Controllers
{
    /// <summary>
    /// Base controller for all versioned API controllers.
    /// Provides a shared MediatR instance via the Mediator property.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator? _mediator;

        /// <summary>
        /// Lazy-resolved MediatR instance so derived controllers don't need
        /// to inject it through their own constructors.
        /// </summary>
        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    }
}
