using CSharpFunctionalExtensions;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Model;
using CustomerManagement.Logic.Utils;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace CustomerManagement.Api.Controllers
{
    public class CustomerController : BaseConterller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IEmailGateway _emailGateway;
        private readonly IndustryRepository _industryRepository;

        public CustomerController(UnitOfWork unitOfWork, IEmailGateway emailGateway)
            : base(unitOfWork)
        {

            _customerRepository = new CustomerRepository(unitOfWork);
            _industryRepository = new IndustryRepository(unitOfWork);
            _emailGateway = emailGateway;
        }

        [HttpPost]
        [Route("customers")]
        public HttpResponseMessage Create(CreateCustomerModel model)
        {
            var customerName = CustomerName.Create(model.Name);
            var email = Email.Create(model.PrimaryEmail);
            Result<Maybe<Email>> secondaryEmail = GetSecondaryEmail(model.SecondaryEmail);
            Result<Industry> industry = Industry.Get(model.Industry);

            var result = Result.Combine(customerName, email, secondaryEmail, industry);

            if (result.IsFailure)
                Error(industry.Error);

            var customer = new Customer(
                customerName.Value, email.Value,
                secondaryEmail.Value,
                industry.Value);

            _customerRepository.Save(customer);

            return Ok();
        }

        private Result<Maybe<Email>> GetSecondaryEmail(string secondaryEmail)
        {
            if (secondaryEmail == null)
                return Result.Success<Maybe<Email>>(null);

            return Email.Create(secondaryEmail).Map(email => (Maybe<Email>)email);

            //var email = Email.Create(secondaryEmail);
            //if (email.IsSuccess)
            //    return Result.Success<Maybe<Email>>(email.Value);

            //return Result.Failure<Maybe<Email>>(email.Error);
        }

        [HttpPut]
        [Route("customers/{id}")]
        public HttpResponseMessage Update(UpdateCustomerModel model)
        {
            var customerResult = _customerRepository.GetById(model.Id)
                .ToResult("Customer with such Id is not found: " + model.Id);
            Result<Industry> industryResult = Industry.Get(model.Industry);

            return Result.Combine(customerResult, industryResult)
                .OnSuccess(() => customerResult.Value.UpdateIndustry(industryResult.Value))
                .OnBoth(r => r.IsSuccess ? Ok() : Error(r.Error));

            //Maybe<Customer> customerOrNothing = _customerRepository.GetById(model.Id);
            //if (customerOrNothing.HasNoValue)
            //    Error("Customer with such Id is not found: " + model.Id);

            //var customer = customerOrNothing.Value;

            //Result<Industry> industry = Industry.Get(model.Industry);

            //if (industry.IsFailure)
            //    Error(industry.Error);

            //customer.UpdateIndustry(industry.Value);
            //return Ok();
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public HttpResponseMessage DisableEmailing(long id)
        {
            Maybe<Customer> customerOrNothing = _customerRepository.GetById(id);
            if (customerOrNothing.HasNoValue)
                Error("Customer with such Id is not found: " + id);

            customerOrNothing.Value.DisableEmailing();

            return Ok();
        }

        [HttpGet]
        [Route("customers/{id}")]
        public HttpResponseMessage Get(long id)
        {
            Maybe<Customer> customerOrNothing = _customerRepository.GetById(id);
            if (customerOrNothing.HasNoValue)
                Error("Customer with such Id is not found: " + id);

            var customer = customerOrNothing.Value;

            var model = new
            {
                customer.Id,
                Name = customer.Name.Value,
                PrimaryEmail = customer.PrimaryEmail.Value,
                SecondaryEmail = customer.SecondaryEmail.HasValue ?
                                            customer.SecondaryEmail.Value.Value : null,
                Industry = customer.EmailingSettings.Industry.Name,
                customer.EmailingSettings.EmailCampaign,
                customer.Status
            };

            return Ok(model);
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public HttpResponseMessage Promote(long id)
        {
            return _customerRepository.GetById(id)
                 .ToResult("Customer with such Id is not found: " + id)
                 .Ensure(c => c.CanBePromoted(), "The customer has the highest status possible")
                 .OnSuccess(c => c.Promote())
                 .Map(c => _emailGateway.SendPromotionNotification(c.PrimaryEmail, c.Status))
                 .OnBoth(r => r.IsSuccess ? Ok() : Error(r.Error));


            //Maybe<Customer> customerOrNothing = _customerRepository.GetById(id);
            //if (customerOrNothing.HasNoValue)
            //    Error("Customer with such Id is not found: " + id);

            //var customer = customerOrNothing.Value;

            //if (!customer.CanBePromoted())
            //    Error("The customer has the highest status possible");

            //customer.Promote();
            //var result = _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);

            //if (result.IsFailure)
            //    return Error(result.Error);

            //return Ok();
        }
    }
}
