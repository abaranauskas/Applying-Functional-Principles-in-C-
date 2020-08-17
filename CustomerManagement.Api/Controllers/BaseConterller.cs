using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CustomerManagement.Api.Controllers
{
    public class BaseConterller: ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public BaseConterller(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage Error(string errorMessage)
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, Envelope.Error(errorMessage));
        }
        
        protected new HttpResponseMessage Ok()
        {
            _unitOfWork.Commit();
            return Request.CreateResponse(HttpStatusCode.OK, Envelope.Ok());
        }
        protected new HttpResponseMessage Ok<T>(T result)
        {
            _unitOfWork.Commit();
            return Request.CreateResponse(HttpStatusCode.OK, Envelope.Ok(result));
        }
    }
}